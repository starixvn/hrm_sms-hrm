namespace HRM.UserControl.Client.Core.Session
{
    public interface IUCSession
    {
        int UserId { get; set; }

        string JwtToken { get; set; }

        void ClearAll();
    }
}