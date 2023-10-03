namespace GO.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Core.Services;
    using Criteria.Account;
    using Filters;
    using GO.Website.Models;
    using GO.Website.Models.Account;
    using GO.Website.Services.Interfaces;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;

    public class AccountController : Controller
    {
        private readonly IUserControlSession userControlSession;
        private readonly IUserOtpService userOtpService;
        private readonly OtpConfig otpConfig;
        private readonly ITwoFactorAuthenticatorService twoFactorAuthService;
        private readonly IUserService userService;
        private readonly IBaseService baseService;
        public AccountController(
            IUserControlSession userControlSession,
            ITwoFactorAuthenticatorService twoFactorAuthService,
            IUserOtpService userOtpService,
            IUserService userService,
            IBaseService baseService,
            OtpConfig otpConfig)
        {
            this.userControlSession = userControlSession;
            this.twoFactorAuthService = twoFactorAuthService;
            this.userOtpService = userOtpService;
            this.userService = userService;
            this.baseService = baseService;
            this.otpConfig = otpConfig;
        }

        [GOAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Setup2Fa()
        {
            return View();
        }        
        
        public async Task<IActionResult> VerifyOTP()
        {
            var userOtp = await baseService.GetDataEntityAsync<UserOtp>(new GetUserOTPByIdCriteria { UserId = userControlSession.UserId, SiteId = userControlSession.SiteId });
            if (userOtp is not { Enabled: true })
            {
                await DoLoginAsync(userControlSession.UserId);
            }
            return userControlSession.UserId > 0 && userControlSession.SiteId > 0 && userOtp is { Enabled: true } ? View() : RedirectToAction("index", "Home");
        }

        [GOAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAccountOTP()
        {
            var userOtp = await baseService.GetDataEntityAsync<UserOtp>(new GetUserOTPByIdCriteria { UserId = userControlSession.UserId, SiteId = userControlSession.SiteId });
            var user = await userService.GetByIdAsync(userControlSession.UserId);
            if (userOtp != null && !string.IsNullOrWhiteSpace(userOtp.SecretKey))
            {
                userOtp.RecoveryCode = twoFactorAuthService.SetupGoogleAuthInfo(otpConfig, user.Email, userOtp.SecretKey);
            }
            return Json(userOtp);
        }

        [GOAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SetupOtp()
        {
            var user = await userService.GetByIdAsync(userControlSession.UserId);

            var setupOtpViewModel = new SetupOTPViewModel
            {
                TextCode = await GetOtpSetupCode(user.Email),
                AppName = otpConfig.AppName,
                Email = user.Email
            };
            return Json(setupOtpViewModel);
        }

        [HttpPost]
        [GOAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> VerifySetupOTP([FromBody] SetupOTPViewModel viewModel)
        {
            var userOtp = await baseService.GetDataEntityAsync<UserOtp>(new GetUserOTPByIdCriteria{UserId = userControlSession.UserId, SiteId = userControlSession.SiteId});
            var isOTPValid = twoFactorAuthService.ValidateOTPCode(otpConfig, userOtp.SecretKey, viewModel.Otp);

            if (TryValidateModel(viewModel) && isOTPValid)
            {
                await userOtpService.UpdateOTPStatusAsync(new UpdateStatusUserOTPCommand
                {
                    UserId = userControlSession.UserId,
                    SiteId = userControlSession.SiteId,
                    IsOTPRegistered = true,
                    Enabled = true
                });
            }

            return Json(isOTPValid);
        }

        [GOAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ResetSetupOtp([FromBody] UserOtp userOtp)
        {
            var result =  await userOtpService.UpdateOTPStatusAsync(new UpdateStatusUserOTPCommand
            {
                UserId = userControlSession.UserId,
                SiteId = userControlSession.SiteId,
                IsOTPRegistered = !userOtp.IsOTPRegistered,
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
                IsOTPRegistered = userOtp.IsOTPRegistered,
                Enabled = !userOtp.Enabled
            });
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP([FromBody] SetupOTPViewModel viewModel)
        {
            var userOtp = await baseService.GetDataEntityAsync<UserOtp>(new GetUserOTPByIdCriteria { UserId = userControlSession.UserId, SiteId = userControlSession.SiteId });
            var isOTPValid = twoFactorAuthService.ValidateOTPCode(otpConfig, userOtp.SecretKey, viewModel.Otp);

            if (TryValidateModel(viewModel) && isOTPValid)
            {
                await DoLoginAsync(userControlSession.UserId);
            }

            return Json(isOTPValid);
        }

        protected internal virtual async Task DoLoginAsync(int userId)
        {
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

            return twoFactorAuthService.SetupGoogleAuthInfo(otpConfig, email, secretKey);
        }
    }
}