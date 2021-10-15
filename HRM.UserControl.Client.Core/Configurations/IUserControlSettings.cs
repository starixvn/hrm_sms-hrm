namespace HRM.UserControl.Client.Core.Configurations
{
    public interface IUserControlSettings
    {
        short SiteId { get; }

        string AccessToken { get; }

        string ServiceUrl { get; }
    }
}