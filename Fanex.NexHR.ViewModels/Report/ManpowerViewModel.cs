namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class ManpowerViewModel
    {
        public int Year { get; set; }

        public IList<ManpowerModel> Manpowers { get; set; }
    }
}