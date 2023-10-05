namespace GO.Website.Models.Account
{
    public class SetupOTP
    {
        public string TextCode { get; set; }

        public string Otp { get; set; }

        public string Email { get; set; }

        public string AppName { get; set; }

        public string QrCodeText => $"otpauth://totp/{AppName}:{Email}?secret={TextCode}&issuer={AppName}";
    }
}