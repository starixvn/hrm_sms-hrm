namespace Fanex.NexHR.Models.SyncData
{
    public class SyncByApiSetting
    {
        public string BaseUrl { get; set; } = "http://localhost:51384/api";

        public string SyncShiftUrl { get; set; } = "/SyncData/GetShift";

        public string AuthenticateUrl { get; set; } = "/Token/GetToken";

        public string AppCredentialId { get; set; } = "M8dgUChV3RoYdl4fXVA7OTRm9SaO6D9c";

        public string AppCredentialPassword { get; set; } = "l4fXVA7OTRm9SaO6D9cM8dgUChV3RoYd";
    }
}
