using Core.Abstract.Service;
using Core.DTO;
using Core.Helpers;
using EditStr.Models;
using EjoProduction.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace EditStr.Controllers
{
    [Authorize(Roles = "Admin,Yönetici")]
    public class AdminController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IUserRoleService _userRoleService;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<AdminController> _localizer;
        private readonly string key = "HashPasswordKey@1234!";
        private string folderPath;
        public AdminController(IServiceProvider serviceProvider, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IStringLocalizer<AdminController> localizer) : base(serviceProvider)
        {
            _configuration = configuration;
            _environment = webHostEnvironment;
            _localizer = localizer;
            _userRoleService = serviceProvider.GetRequiredService<IUserRoleService>();
            _userService = serviceProvider.GetRequiredService<IUserService>();
            key = _configuration.GetSection("DefaultVariables").GetValue<string>("HasherVal") ?? "HashPasswordKey@1234!";
            folderPath = Path.Combine(_environment.WebRootPath, "Uploads");
        }
        public IActionResult RoleAdd()
        {
            RoleViewModel model = new RoleViewModel();
            model.List = _userRoleService.Get();
            return View(model);
        }

        [HttpPost]
        public IActionResult RoleAdd([FromForm] UserRoleDto userRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResultView.Failure(_localizer["Validation"]).JSOutput);
            }
            else
            {
                var result = _userRoleService.Create(userRoleDto);
                if (result.IsSuccess)
                {
                    return Ok(ResultView.Success(_localizer["RoleAdd"]).JSOutput);
                }
                else
                {
                    return BadRequest(ResultView.Failure("Error : " + result.Error).JSOutput);
                }
            }
        }

        public IActionResult RoleEdit(int Id)
        {
            var model = _userRoleService.Get(Id);
            return Json(model);
        }

        [HttpPost]
        public IActionResult RoleEdit([FromForm] UserRoleDto userRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResultView.Failure(_localizer["Validation"]).JSOutput);
            }
            else
            {
                var result = _userRoleService.Update(userRoleDto);
                if (result.IsSuccess)
                {
                    return Ok(ResultView.Success(_localizer["RoleUpdate"]).JSOutput);
                }
                else
                {
                    return BadRequest(ResultView.Failure("Error : " + result.Error).JSOutput);
                }
            }
        }

        public JsonResult RoleDelete(int Id)
        {
            var video = _userRoleService.Get(Id);
            if (_userRoleService.Delete(Id).IsSuccess)
            {
                return new JsonResult("success");
            }
            else
                return new JsonResult("error");
        }

        public IActionResult UserAdd()
        {
            GenericViewModel<UserRoleDto> model = new GenericViewModel<UserRoleDto>();
            model.ParentList = _userRoleService.Get()
                 .Select(l => new SelectListItem { Text = l.Name, Value = l.Id.ToString() })
                 .ToList();
            return View(model);
        }

        public IActionResult UserList()
        {
            GenericViewModel<IReadOnlyList<UserDto>> model = new GenericViewModel<IReadOnlyList<UserDto>>();
            model.Response = _userService.Get();
            model.ParentList = _userRoleService.Get()
                 .Select(l => new SelectListItem { Text = l.Name, Value = l.Id.ToString() })
                 .ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult UserAdd([FromForm] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = ResultView.Failure(_localizer["Validation"]).JSOutput;

            }
            else
            {
                if (userDto.ProfileImage is not null)
                {
                    if (!ImageHelper.IsValidImage(userDto.ProfileImage.FileName))
                    {
                        TempData["Message"] = ResultView.Failure(_localizer["ImageText"]).JSOutput;
                    }

                    ImageHelper.RemoveFile(Path.Combine(folderPath, "Users", userDto.Image ?? ""));

                    using var stream = userDto.ProfileImage.OpenReadStream();
                    userDto.Image = ImageHelper.SaveImage(stream, userDto.ProfileImage.FileName, Path.Combine(folderPath, "Users"), 300, 300);
                }
                if (userDto.Password != null && userDto.PasswordAgain != null)
                {
                    if (userDto.Password == userDto.PasswordAgain)
                    {
                        var hasher = new PasswordHasher<string>();
                        userDto.Password = hasher.HashPassword(key, userDto.Password);
                        var result = _userService.Create(userDto);
                        if (result.IsSuccess)
                        {
                            TempData["Message"] = ResultView.Success(_localizer["UserAdd"]).JSOutput;

                        }
                        else
                        {
                            TempData["Message"] = ResultView.Failure("Error : " + result.Error).JSOutput;
                        }
                    }
                    else
                    {
                        TempData["Message"] = ResultView.Failure(_localizer["UserMatch"]).JSOutput;
                    }
                }
                else
                {
                    TempData["Message"] = ResultView.Failure(_localizer["UserEmpty"]).JSOutput;
                }
            }
            return Redirect("UserAdd/");
        }

        public IActionResult UserEdit(int Id)
        {
            var model = _userService.Get(Id);
            return Json(model);
        }

        [HttpPost]
        public IActionResult UserEdit([FromForm] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResultView.Failure(_localizer["Validation"]).JSOutput);
            }
            var model = _userService.CanLoginForAdmin(userDto);
            if (userDto.ProfileImage is not null)
            {
                if (!ImageHelper.IsValidImage(userDto.ProfileImage.FileName))
                {
                    return BadRequest(ResultView.Failure(_localizer["ImageText"]).JSOutput);
                }

                ImageHelper.RemoveFile(Path.Combine(folderPath, "Users", userDto.Image ?? ""));

                using var stream = userDto.ProfileImage.OpenReadStream();
                userDto.Image = ImageHelper.SaveImage(stream, userDto.ProfileImage.FileName, Path.Combine(folderPath, "Users"), 300, 300);
            }
            if (userDto.Password != null)
            {
                var hasher = new PasswordHasher<string>();
                userDto.Password = hasher.HashPassword("key", userDto.Password);
            }
            else
            {
                userDto.Password = model.Value.Password;
            }
            var login = model.Value;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, login.Id.ToString()),
                new Claim(ClaimTypes.Name, login.FullName),
                new Claim(ClaimTypes.Email, login.Email),
                new Claim("Image", login.Image ?? ""),
                new Claim(ClaimTypes.Role, login.UserRole.Name),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                IsPersistent = true,
                IssuedUtc = DateTime.Now.ToUniversalTime(),
            };

            var result = _userService.Update(userDto);
            if (result.IsSuccess)
            {
                return Ok(ResultView.Success(_localizer["UserUpdate"]).JSOutput);
            }
            else
            {
                return BadRequest(ResultView.Failure("Error : " + result.Error).JSOutput);
            }
        }

        [HttpPost]
        public IActionResult UserImageRemove([FromForm] int id)
        {
            var user = _userService.Get(id);
            if (user is not null)
            {
                if (user.Image is not null)
                {
                    ImageHelper.RemoveFile(Path.Combine(folderPath, "Users", user.Image));
                    user.Image = null;
                    _userService.Update(user);
                }
            }
            else
            {
                return BadRequest(_localizer["ImageFound"].ToString());
            }

            return Ok();
        }

        public async Task<JsonResult> UserDelete(int Id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var video = _userService.Get(Id);

            if (_userService.Delete(Id).IsSuccess)
            {
                ImageHelper.RemoveFile(Path.Combine(folderPath, "Users", video.Image ?? ""));
                if (currentUserId == Id)
                {
                    await HttpContext.SignOutAsync();
                    return new JsonResult("selfDeleted");
                }

                return new JsonResult("success");
            }
            else
            {
                return new JsonResult("error");
            }
        }
    }
}
