namespace HRM.UserControl.Models.UserMapping
{
    using System;

    [Serializable]
    public class UserMapping
    {
        public short SiteId { get; set; }

        public int RefId { get; set; }

        public int NewUserId { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}