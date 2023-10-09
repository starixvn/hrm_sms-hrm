namespace GO.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using GO.Website.Criteria.Account;
    using GO.Website.Filters;
    using GO.Website.Models;
    using GO.Website.Models.Account;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;

    public class AccountController : Controller
    {
        private readonly IUserControlSettings userControlSettings;
        private readonly IUserControlSession userControlSession;
        private readonly IUserOtpService userOtpService;
        private readonly OtpConfig otpConfig;
        private readonly ITwoFactorAuthenticatorService twoFactorAuthService;
        private readonly IUserService userService;

        public AccountController(
            IUserControlSettings userControlSettings,
            IUserControlSession userControlSession,
            ITwoFactorAuthenticatorService twoFactorAuthService,
            IUserOtpService userOtpService,
            IUserService userService,
            OtpConfig otpConfig)
        {
            this.userControlSettings = userControlSettings;
            this.userControlSession = userControlSession;
            this.twoFactorAuthService = twoFactorAuthService;
            this.userOtpService = userOtpService;
            this.userService = userService;
            this.otpConfig = otpConfig;
        }

        [GOAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Setup2Fa()
        {
            return View("~/Views/Account/Setup2FA.cshtml");
        }

        public async Task<IActionResult> VerifyOTP()
        {
            var userOtp = await userOtpService.GetUserOTPByIdAsync(userControlSession.UserId, userControlSession.SiteId);
            if (!otpConfig.Enable || userOtp is not { Enabled: true })
            {
                await DoLoginAsync(userControlSession.UserId);
            }

            return otpConfig.Enable && userOtp is { Enabled: true } ? View("~/Views/Account/VerifyOTP.cshtml") : RedirectToAction("index", "Home");
        }

        [GOAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAccountOTP()
        {
            var userOtp = await userOtpService.GetUserOTPByIdAsync(userControlSession.UserId, userControlSession.SiteId);
            var user = await userService.GetByIdAsync(userControlSession.UserId);
            if (userOtp != null && !string.IsNullOrWhiteSpace(userOtp.SecretKey))
            {
                userOtp.RecoveryCode = twoFactorAuthService.GenerateSetupCode(user.Email, userOtp.SecretKey);
            }

            return Json(userOtp);
        }

        [GOAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SetupOtp()
        {
            var user = await userService.GetByIdAsync(userControlSession.UserId);
            var model = new SetupOTP
            {
                TextCode = await GetOtpSetupCode(user.Email),
                AppName = otpConfig.AppName,
                Email = user.Email
            };
            return Json(model);
        }

        [HttpPost]
        [GOAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> VerifySetupOTP([FromBody] SetupOTP model)
        {
            var userOtp = await userOtpService.GetUserOTPByIdAsync(userControlSession.UserId, userControlSession.SiteId);
            var isOTPValid = twoFactorAuthService.ValidateOTPCode(userOtp.SecretKey, model.Otp);

            if (isOTPValid)
            {
                await userOtpService.UpdateOTPStatusAsync(new UpdateStatusUserOTPCommand
                {
                    UserId = userControlSession.UserId,
                    SiteId = userControlSession.SiteId,
                    IsRegistered = true,
                    Enabled = true
                });
            }

            return Json(isOTPValid);
        }

        [GOAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ResetSetupOtp([FromBody] UserOtp userOtp)
        {
            var result = await userOtpService.UpdateOTPStatusAsync(new UpdateStatusUserOTPCommand
            {
                UserId = userControlSession.UserId,
                SiteId = userControlSession.SiteId,
                IsRegistered = !userOtp.IsRegistered,
                Enabled = !userOtp.Enabled
            });
            return Json(result);
        }

        [GOAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateStatusOTP([FromBody] UserOtp userOtp)
        {
            var result = await userOtpService.UpdateOTPStatusAsync(new UpdateStatusUserOTPCommand
            {
                UserId = userControlSession.UserId,
                SiteId = userControlSession.SiteId,
                IsRegistered = userOtp.IsRegistered,
                Enabled = !userOtp.Enabled
            });
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP([FromBody] SetupOTP model)
        {
            var userOtp = await userOtpService.GetUserOTPByIdAsync(userControlSession.UserId, userControlSession.SiteId);
            var isOTPValid = twoFactorAuthService.ValidateOTPCode(userOtp.SecretKey, model.Otp);

            if (isOTPValid)
            {
                await DoLoginAsync(userControlSession.UserId);
            }

            return Json(isOTPValid);
        }

        protected internal virtual async Task DoLoginAsync(int userId)
        {
            userControlSession.MenuItems = await userService.GetMenuItemAsync();
            userControlSession.Functions = await userService.GetFunctionPermissionAsync(userControlSettings.SiteId, userControlSession.UserId);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }

        private async Task<string> GetOtpSetupCode(string email)
        {
            var secretKey = Guid.NewGuid().ToString("N");
            await userOtpService.UpdateSecretKeyAsync(userControlSession.UserId, userControlSession.SiteId, secretKey);

            return twoFactorAuthService.GenerateSetupCode(email, secretKey);
        }
    }
}