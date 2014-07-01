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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Shift Service.
    /// </summary>
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepository;

        public ShiftService(IShiftRepository shiftRepository = null)
        {
            this._shiftRepository = shiftRepository ?? new ShiftRepository();
        }

        public ShiftService()
            : this(null)
        { 
        }

        /// <summary>
        /// Function to get all shift.
        /// </summary>
        /// <param name="statusId">Status Id.</param>
        /// <returns>Shift List.</returns>
        public IList<Shift> GetAll(int statusId = 1)
        {
            var allShifts = this._shiftRepository.GetAll();

            if (statusId == (int)ShiftStatus.Active)
            {
                allShifts = allShifts.Where(t => t.IsDisabled == false).ToList();
            }
            else if (statusId == (int)ShiftStatus.Disabled)
            {
                allShifts = allShifts.Where(t => t.IsDisabled == true).ToList();
            }

            return allShifts;
        }

        /// <summary>
        /// Function to get shifts by query.
        /// </summary>
        /// <param name="query">Condition query.</param>
        /// <returns>Shift List.</returns>
        public IList<Shift> GetByQuery(string query)
        {
            IList<Shift> shifts = new List<Shift>();

            var actualShifts = this.GetAll();

            foreach (var shift in actualShifts)
            {
                shifts.Add(shift);
            }

            var offDay = new Shift
            {
                Code = "Off",
                Id = -1
            };

            shifts.Insert(0, offDay);
            query = query.ToUpper();

            if (shifts != null && shifts.Any())
            {
                shifts = shifts.Where(t => t.Code.ToUpper().StartsWith(query.ToUpper())).ToList();
            }

            return shifts;
        }

        /// <summary>
        /// Function to get select list shift items.
        /// </summary>
        /// <returns>SelectListItem List.</returns>
        public IList<SelectListItem> GetSelectList()
        {
            IList<SelectListItem> list = new List<SelectListItem>();
            var shifts = this.GetAll();

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

        public Shift GetByDate(int employeeId, DateTime date)
        {
            if (employeeId > 0)
            {
                return this._shiftRepository.GetByDate(employeeId, date);
            }

            return null;
        }

        public Shift GetById(int shiftId)
        {
            if (shiftId > 0)
            {
                return this._shiftRepository.GetById(shiftId);
            }

            return null;
        }

        public int Insert(Shift model, out int errorCode, out string message)
        {
            return this._shiftRepository.Insert(model, out errorCode, out message);
        }

        public bool Update(Shift model, out int errorCode, out string message)
        {
            return this._shiftRepository.Update(model, out errorCode, out message);
        }
    }
}
