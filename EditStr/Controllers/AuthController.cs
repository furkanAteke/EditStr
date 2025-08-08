using Core.Abstract.Service;
using Core.DTO;
using EjoProduction.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace EditStr.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IStringLocalizer<AuthController> _localizer;
        private readonly string key = "HashPasswordKey@1234!";
        public AuthController(IServiceProvider serviceProvider, IHttpContextAccessor contextAccessor, IStringLocalizer<AuthController> localizer) : base(serviceProvider)
        {
            _userService = serviceProvider.GetRequiredService<IUserService>();
            _contextAccessor = contextAccessor;
            _localizer = localizer;
        }
        [AllowAnonymous]
        public IActionResult Login() => View();
        public IActionResult AccessDenied() => View();

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromForm] UserLoginDto user)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = ResultView.Failure(_localizer["Validation"]).JSOutput;
            }
            else
            {
                if (user.Email != null && user.Password != null)
                {
                    var hasher = new PasswordHasher<string>();
                    var model = _userService.CanLogin(user);
                    if (model.Value is not null)
                    {
                        var login = model.Value;
                        if (hasher.VerifyHashedPassword(key, login.Password, user.Password) == PasswordVerificationResult.Success)
                        {
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
                                IsPersistent = user.RememberMe,
                                IssuedUtc = DateTime.Now.ToUniversalTime(),
                            };

                            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                            CookieOptions option = new CookieOptions();
                            option.Expires = DateTime.Now.AddDays(1);
                            return Redirect("/Home/Index");
                        }
                        else
                        {
                            TempData["Message"] = _localizer["UserPassEr"];
                        }
                    }
                    else
                    {
                        TempData["Message"] = _localizer["UserNotFound"];
                    }
                }
                else
                {
                    TempData["Message"] = _localizer["UserValue"];
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword([FromForm] UserPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = ResultView.Failure(_localizer["Validation"]).JSOutput;
            }

            var hasher = new PasswordHasher<string>();
            int userId = int.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var employee = _userService.Get(userId);
            if (employee is not null)
            {
                if (dto.Password == dto.PasswordAgain)
                {
                    employee.Password = hasher.HashPassword(key, dto.Password);
                    _userService.Update(employee);
                    return Ok(_localizer["PassChange"].ToString());
                }
                else
                {
                    return BadRequest(_localizer["PassMatch"].ToString());
                }
            }
            else
            {
                return Ok(_localizer["UserValue"].ToString());
            }
        }

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return LocalRedirect("/Auth/Login");
        }
    }
}