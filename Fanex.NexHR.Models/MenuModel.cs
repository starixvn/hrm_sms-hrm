namespace Fanex.NexHR.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public class MenuModel
    {
        public int MenuId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Text { get; set; }

        public string Link { get; set; }

        public int? ParentId { get; set; }

        public int? FunctionId { get; set; }

        public int? DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}