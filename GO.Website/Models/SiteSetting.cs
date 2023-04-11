namespace GO.Website.Models
{
    using System;

    [Serializable]
    public class SiteSetting
    {
        public short SiteId { get; set; }
        public bool IsHrmSystem { get; set; }

        public string BaseUrl { get; set; }
        public string TokenUrl { get; set; }
        public string TodolistUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}