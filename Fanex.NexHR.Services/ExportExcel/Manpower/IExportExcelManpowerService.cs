namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IExportExcelManpowerService
    {
        void Export(IList<ManpowerModel> manpowers, int year);
    }
}