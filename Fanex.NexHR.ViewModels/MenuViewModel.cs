namespace Fanex.NexHR.ViewModels
{
    using Fanex.NexHR.Models;

    public class MenuViewModel
    {
        public int MenuId { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string Link { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public FunctionModel Function { get; set; }

        public MenuModel ParentMenu { get; set; }
    }
}