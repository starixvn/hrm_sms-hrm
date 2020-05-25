namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface INewDepartmentService
    {
        IList<NewDepartment> GetByActive(bool? isActive);

        NewDepartment GetById(int departmentId);

        CommandResult Delete(int id);
    }
}
