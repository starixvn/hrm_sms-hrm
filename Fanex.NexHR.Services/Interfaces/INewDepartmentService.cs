namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface INewDepartmentService
    {
        IList<NewDepartment> GetAll(bool? isActive);
    }
}
