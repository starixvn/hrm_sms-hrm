namespace GO.Website.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Core.Services;
    using Criteria.Account;
    using GO.Website.Models;
    using GO.Website.Models.Account;
    using GO.Website.Services.Interfaces;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    public class AccountController : BaseController
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
        public IActionResult Setup2Fa()
        {
            return View();
        }

        public async Task<IActionResult> SetupOtp()
        {
            var user = await userService.GetByIdAsync(userControlSession.UserId);

            var setupOtpViewModel = new SetupOTPViewModel
            {
                TextCode = await GetOtpSetupCode("brian.nguyen@starixsoft.com"),
                AppName = otpConfig.AppName,
                Email = "brian.nguyen@starixsoft.com"
            };
            return Json(setupOtpViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> VerifySetupOTP([FromBody] SetupOTPViewModel viewModel)
        {
            ModelState.Clear();

            var userOtp = await baseService.GetDataEntityAsync<UserOtp>(new GetUserOTPByIdCriteria{UserId = userControlSession.UserId, SiteId = userControlSession.SiteId});
            var isOTPValid = twoFactorAuthService.ValidateOTPCode(otpConfig, userOtp.SecretKey, viewModel.Otp);

            if (!TryValidateModel(viewModel) || !isOTPValid)
            {
                return Forbid();
            }

            await userOtpService.UpdateOTPStatusAsync(userOtp.UserId, userOtp.SiteId, true);
            return new OkResult();
        }

        public async Task<IActionResult> ResetOtpDevice()
        {
            var result =  await userOtpService.UpdateOTPStatusAsync(userControlSession.UserId, userControlSession.SiteId, false);
            return Json(result);
        }

        private async Task<string> GetOtpSetupCode(string email)
        {
            var secretKey = Guid.NewGuid().ToString("N")[..12];
            await userOtpService.UpdateSecretKeyAsync(userControlSession.UserId, userControlSession.SiteId, secretKey);

            return twoFactorAuthService.SetupGoogleAuthInfo(otpConfig, email, secretKey);
        }
    }
}