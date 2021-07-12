namespace HRM.UserControl.Client.Core.Configurations
{
    public interface IUserControlSettings
    {
        int SiteId { get; }

        string SiteName { get; }

        string AccessToken { get; }

        string ServiceUrl { get; }
    }
}