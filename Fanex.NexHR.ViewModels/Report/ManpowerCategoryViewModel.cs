namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class ManpowerCategoryViewModel
    {
        public int Year { get; set; }

        public IList<ManpowerCategoryModel> Manpowers { get; set; }
    }
}