namespace HRM.UserControl.Client.Core.Session
{
    public interface IUserControlSession
    {
        int UserId { get; set; }

        string JwtToken { get; set; }

        void ClearAll();
    }
}