
namespace HRM.UserControl.Admin.Website.Models.Menu
{
    using System;
    using System.Collections.Generic;

    public class Menu
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public string URL { get; set; }
        public int? ParentId { get; set; }
        public string FunctionName { get; set; }
        public int DisplayOrder { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public IList<Menu> Childs { get; set; } = new List<Menu>();
    }
}
