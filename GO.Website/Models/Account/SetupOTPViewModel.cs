namespace GO.Website.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    public class SetupOTPViewModel
    {
        public string TextCode { get; set; }

        [Required]
        public string Otp { get; set; }

        public string Email { get; set; }

        public string AppName { get; set; }

        public string QrCodeText => $"otpauth://totp/{AppName}:{Email}?secret={TextCode}&issuer={AppName}";
    }
}
