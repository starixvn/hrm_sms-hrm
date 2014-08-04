// --------------------------------------------------------------------------
// <copyright file="ShiftService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Shift Service.
    /// </summary>
    public class ShiftService : BaseService, IShiftService
    {
        /// <summary>
        /// Function to get all shift.
        /// </summary>
        /// <param name="statusId">Status Id.</param>
        /// <param name="isShowOff">Is Show Off.</param>
        /// <returns>Shift List.</returns>
        public IList<Shift> GetAll(int statusId = 1, bool isShowOff = false)
        {
            var allShifts = GetData<Shift>(new GetAllShiftCriteria()).ToList();
            if (statusId == (int)ShiftStatus.Active)
            {
                allShifts = allShifts.Where(t => t.IsDisabled == false).ToList();
            }
            else if (statusId == (int)ShiftStatus.Disabled)
            {
                allShifts = allShifts.Where(t => t.IsDisabled == true).ToList();
            }

            if (isShowOff)
            {
                var offDay = new Shift
                {
                    Code = "Off",
                    Id = -1
                };

                allShifts.Insert(0, offDay);
            }

            return allShifts;
        }

        /// <summary>
        /// Function to get shifts by query.
        /// </summary>
        /// <param name="query">Condition query.</param>
        /// <param name="isShowOff">Is Show Off.</param>
        /// <returns>Shift List.</returns>
        public IList<Shift> GetByQuery(string query, bool isShowOff)
        {
            IList<Shift> shifts = this.GetAll(isShowOff: isShowOff);
            query = query.ToUpper();

            if (shifts != null && shifts.Any())
            {
                shifts = shifts.Where(t => t.Code.ToUpper().StartsWith(query.ToUpper(), System.StringComparison.CurrentCulture)).ToList();
            }

            return shifts;
        }

        /// <summary>
        /// Function to get select list shift items.
        /// </summary>
        /// <param name="statusId">Status Id.</param>
        /// <param name="isShowOff">Is Show Off.</param>
        /// <returns>SelectListItem List.</returns>
        public IList<SelectListItem> GetSelectList(int statusId = 1, bool isShowOff = false)
        {
            IList<SelectListItem> list = new List<SelectListItem>();
            var shifts = this.GetAll(statusId, isShowOff);

            foreach (Shift item in shifts)
            {
                var listItem = new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                };

                list.Add(listItem);
            }

            return list;
        }

        public Shift GetByDate(GetEmployeeShiftByDateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                return GetDataEntity<Shift>(criteria);
            }

            return null;
        }

        public Shift GetById(GetShiftByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                return GetDataEntity<Shift>(criteria);
            }

            return null;
        }

        public CommandResult Insert(InsertShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = null;
            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult Update(UpdateShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = null;
            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult Delete(DeleteShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = null;
            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }
    }
}
