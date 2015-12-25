// --------------------------------------------------------------------------
// <copyright file="DepartmentService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.12.21">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class DepartmentService : BaseService, IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService()
            : this(null)
        {
        }

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            this._departmentRepository = departmentRepository ?? new DepartmentRepository();
        }

        public Department GetById(GetDepartmentByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Department department = new Department();

            if (criteria.IsValid())
            {
                department = _departmentRepository.GetById(criteria);
            }

            return department;
        }

        public IList<Department> GetDepartment(GetDepartmentByParentIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Department> departments = null;

            if (criteria.IsValid())
            {
                departments = _departmentRepository.GetDepartment(criteria);
            }

            return departments;
        }

        public IList<Department> GetParentDepartment(GetParentByDepartmentCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Department> departments = null;

            if (criteria.IsValid())
            {
                departments = _departmentRepository.GetDepartment(criteria);
            }

            return departments;
        }

        public CommandResult Add(InsertDepartmentCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult Edit(UpdateDepartmentCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult Delete(DeleteDepartmentCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }
    }
}