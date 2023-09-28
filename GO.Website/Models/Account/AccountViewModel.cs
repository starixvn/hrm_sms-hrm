namespace GO.Website.Models.Account
{
    using Manpower;

    public class AccountViewModel : RijndaelData
    {
        public SetupOTPViewModel SetupOtpViewModel { get; set; }

        public EmployeeInfo EmployeeInfo { get; set; }
    }
}
