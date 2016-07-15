namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IExportExcelManpowerCategoryService
    {
        void Export(IList<ManpowerCategoryModel> manpowers, int year);
    }
}