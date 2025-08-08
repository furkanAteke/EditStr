using Core.Abstract.Service;
using Core.DTO;
using Core.Helpers;
using EditStr.Models;
using EjoProduction.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Security.Claims;

namespace EditStr.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly IEditService _editService;
        private readonly IStringLocalizer<HomeController> _localizer;
        private string folderPath;
        public HomeController(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IStringLocalizer<HomeController> localizer) : base(serviceProvider)
        {
            _httpContext = httpContextAccessor;
            _environment = webHostEnvironment;
            _localizer = localizer;
            _userService = serviceProvider.GetRequiredService<IUserService>();
            _categoryService = serviceProvider.GetRequiredService<ICategoryService>();
            _editService = serviceProvider.GetRequiredService<IEditService>();
            folderPath = Path.Combine(_environment.WebRootPath, "Uploads");
        }
        public IActionResult Index()
        {
            CategoryEditViewModel model = new CategoryEditViewModel();
            model.Category = _categoryService.Get();
            model.Edits = _editService.Get();
            return View(model);
        }

        public JsonResult EditDelete(int Id)
        {
            var video = _editService.Get(Id);
            if (_editService.Delete(Id).IsSuccess)
            {
                ImageHelper.RemoveFile(Path.Combine(folderPath, "Videos", video.EditFull ?? ""));
                return new JsonResult("success");
            }
            else
                return new JsonResult("error");
        }

        [HttpPost]
        public IActionResult CategoryAdd([FromForm] CategoryDto category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResultView.Failure(_localizer["Validation"]).JSOutput);
            }
            else
            {
                var result = _categoryService.Create(category);
                if (result.IsSuccess)
                {
                    return Ok(ResultView.Success(_localizer["CategoryAdd"]).JSOutput);
                }
                else
                {
                    return BadRequest(ResultView.Failure("Error : " + result.Error).JSOutput);
                }
            }
        }

        public JsonResult CategoryDelete(int Id)
        {
            var edits = _editService.Get().Where(e => e.CategoryId == Id).ToList();

            foreach (var edit in edits)
            {
                _editService.Delete(edit.Id);
                ImageHelper.RemoveFile(Path.Combine(folderPath, "Videos", edit.EditFull ?? ""));
            }
            var result = _categoryService.Delete(Id);

            if (result.IsSuccess)
                return new JsonResult("success");
            else
                return new JsonResult("error");
        }

        [Authorize(Roles = "Admin,Yönetici,Editör,Editor")]
        public IActionResult EditAdd()
        {
            CategoryEditViewModel model = new CategoryEditViewModel();
            model.Category = _categoryService.Get();
            return View(model);
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public IActionResult EditAdd([FromForm] CategoryEditViewModel editDto) 
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = ResultView.Failure(_localizer["Validation"]).JSOutput;

            }
            else
            {
                if (editDto is not null)
                {
                    if (ImageHelper.IsVideoFile(editDto.Edit.EditPath.FileName))
                    {
                        ImageHelper.RemoveFile(Path.Combine(folderPath, "Videos", editDto.Edit.EditFull ?? ""));

                        using var stream = editDto.Edit.EditPath.OpenReadStream();
                        editDto.Edit.EditFull = ImageHelper.SaveVideo(stream, editDto.Edit.EditPath.FileName, Path.Combine(folderPath, "Videos"));
                        var result = _editService.Create(editDto.Edit);
                        if (result.IsSuccess)
                        {
                            TempData["Message"] = ResultView.Success(_localizer["VideoAdd"]).JSOutput;
                        }
                        else
                        {
                            ImageHelper.RemoveFile(Path.Combine(folderPath, "Videos", editDto.Edit.EditFull ?? ""));
                            TempData["Message"] = ResultView.Failure("Error : " + result.Error).JSOutput;
                        }
                    }
                    else
                    {
                        TempData["Message"] = ResultView.Failure(_localizer["ImageText"]).JSOutput;
                    }
                }
                else
                {
                    TempData["Message"] = ResultView.Failure(_localizer["ImageResult"]).JSOutput;
                }
            }
            return Redirect("EditAdd/");
        }

        [HttpPost]
        public IActionResult EditProfile([FromForm] UserEditDto user)
        {
            var userId = int.Parse(_httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var model = _userService.GetUserForEdit(userId);
            if (model is null)
            {
                return BadRequest(_localizer["UserFound"].ToString());
            }
            var userProfile = model.Value;
            userProfile.Id = userId;
            userProfile.FullName = user.FullName;
            if (user.ProfileImage is not null)
            {
                if (!ImageHelper.IsValidImage(user.ProfileImage.FileName))
                {
                    return BadRequest(_localizer["ImageText"].ToString());
                }

                ImageHelper.RemoveFile(Path.Combine(folderPath, "Users", userProfile.Image ?? ""));

                using var stream = user.ProfileImage.OpenReadStream();
                userProfile.Image = ImageHelper.SaveImage(stream, user.ProfileImage.FileName, Path.Combine(folderPath, "Users"), 300, 300);
            }
            _userService.Update(userProfile);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userProfile.Id.ToString()),
                new Claim(ClaimTypes.Name, userProfile.FullName),
                new Claim(ClaimTypes.Email, userProfile.Email),
                new Claim("Image", userProfile.Image ?? ""),
                new Claim(ClaimTypes.Role, userProfile.UserRole.Name),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                IsPersistent = true,
                IssuedUtc = DateTime.Now.ToUniversalTime(),
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return Ok(_localizer["Info"].ToString());
        }
    }
}