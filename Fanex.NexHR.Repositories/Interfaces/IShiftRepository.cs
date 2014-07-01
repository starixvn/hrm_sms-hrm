// --------------------------------------------------------------------------
// <copyright file="IShiftRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.NexHR.Models;

    public interface IShiftRepository
    {
        IList<Shift> GetAll();

        Shift GetByDate(int employeeId, DateTime date);

        Shift GetById(int shiftId);

        int Insert(Shift model, out int errorCode, out string message);

        bool Update(Shift model, out int errorCode, out string message);
    }
}
