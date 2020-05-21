namespace Fanex.NexHR.ViewModels.Unit
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class EditUnitViewModel
    {
        [Required]
        public int UnitId { get; set; }

        [AllowHtml]
        [Required]
        public string Code { get; set; }

        [AllowHtml]
        [Required]
        public string Name { get; set; }

        //[Required]
        //public int ManagerId { get; set; }

        //public string Manager { get; set; }

        //public int? ParentId { get; set; }

        public bool IsActive { get; set; }

        //public IList<Department> ParentDepartments { get; set; }      
    }
}
