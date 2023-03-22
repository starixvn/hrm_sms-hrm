namespace GO.Models.UserMapping
{
    using System;

    [Serializable]
    public class UserMapping
    {
        public int UserId { get; set; }

        public int RefId { get; set; }

        public int? NewUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}