// --------------------------------------------------------------------------
// <copyright file="IShiftService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;

    public interface IShiftService
    {
        IList<Shift> GetAll(int statusId = 1, bool isShowOff = false);

        IList<Shift> GetByQuery(string query, bool isShowOff);

        IList<SelectListItem> GetSelectList();

        Shift GetByDate(int employeeId, DateTime date);

        Shift GetById(int shiftId);

        int Insert(Shift model, out int errorCode, out string message);

        bool Update(Shift model, out int errorCode, out string message);

        bool Delete(int shiftId, out int errorCode, out string message);
    }
}
