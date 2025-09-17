using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Foodly.Web.Models.Account;
using System.Security.Claims;

namespace Foodly.Web.Controllers
{
    [AllowAnonymous]
    [Route("auth")]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signIn;
        private readonly UserManager<IdentityUser> _users;

        public AccountController(SignInManager<IdentityUser> signIn, UserManager<IdentityUser> users)
        {
            _signIn = signIn;
            _users  = users;
        }

        // GET /auth/login
        [HttpGet("login")]
        public IActionResult Login(string? returnUrl = null)
            => View(new LoginVm { ReturnUrl = returnUrl });

        // POST /auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _users.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials");
                return View(vm);
            }

            var res = await _signIn.PasswordSignInAsync(user, vm.Password, vm.RememberMe, lockoutOnFailure: false);
            if (!res.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials");
                return View(vm);
            }

            return LocalRedirect(string.IsNullOrWhiteSpace(vm.ReturnUrl) ? "/app" : vm.ReturnUrl!);
        }

        // GET /auth/register
        [HttpGet("register")]
        public IActionResult Register(string? returnUrl = null)
            => View(new RegisterVm { ReturnUrl = returnUrl });

        // POST /auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = new IdentityUser { Email = vm.Email, UserName = vm.Email };
            var create = await _users.CreateAsync(user, vm.Password);

            if (!create.Succeeded)
            {
                foreach (var e in create.Errors) ModelState.AddModelError(string.Empty, e.Description);
                return View(vm);
            }

            // сохраним полное имя в клейме, чтобы не усложнять модель пользователя
            await _users.AddClaimAsync(user, new Claim("full_name", vm.FullName));

            await _signIn.SignInAsync(user, isPersistent: true);
            return LocalRedirect(string.IsNullOrWhiteSpace(vm.ReturnUrl) ? "/app" : vm.ReturnUrl!);
        }

        // GET /auth/forgot
        [HttpGet("forgot")]
        public IActionResult Forgot() => View(new ForgotPasswordVm());

        // POST /auth/forgot
        [HttpPost("forgot")]
        public async Task<IActionResult> Forgot(ForgotPasswordVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _users.FindByEmailAsync(vm.Email);
            // мы не отдаем, существует ли юзер — показываем одинаковый ответ (без реальной отправки почты для демо)
            vm.Sent = true;
            return View(vm);
        }

        // GET /auth/logout
        [Authorize, HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return Redirect("/auth/login");
        }
    }
}
