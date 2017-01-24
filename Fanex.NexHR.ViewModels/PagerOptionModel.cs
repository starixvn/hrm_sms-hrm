namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;

    public class PagerOptionModel
    {
        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalItem { get; set; }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Ok.")]
        public IList<SelectListItem> ItemsPerPageArray
        {
            get
            {
                var itemsPerPages = new List<SelectListItem>
                {
                    new SelectListItem { Text = "50", Value = "50" },
                    new SelectListItem { Text = "100", Value = "100" },
                    new SelectListItem { Text = "200", Value = "200" }
                };

                return itemsPerPages;
            }
        }
    }
}