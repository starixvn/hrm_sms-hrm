namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;

    public interface IDepartmentService
    {
        Department GetById(int id);

        IList<Department> GetDepartment(int? parentId = null, bool? isActive = true);

        IList<Department> GetParentDepartment(int departmentId);

        CommandResult Delete(int actorId, int id);
    }
}