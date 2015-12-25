// --------------------------------------------------------------------------
// <copyright file="GlobalSuppression.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.12.25">Created</change>
// </history>

using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member",
    Target = "Fanex.NexHR.ReminderService.Services.ReminderAttendanceService.#GetEmployeeAboveOffDay()", Justification = "Reviewed. Suppression is OK here.")]
[module: SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member",
    Target = "Fanex.NexHR.ReminderService.Services.ReminderAttendanceService.#GetEmployeeAbsent()", Justification = "Reviewed. Suppression is OK here.")]
[module: SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member",
    Target = "Fanex.NexHR.ReminderService.Services.ReminderAttendanceService.#GetEmployeeLateInEarlyOut()", Justification = "Reviewed. Suppression is OK here.")]
[module: SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member",
    Target = "Fanex.NexHR.ReminderService.Services.ReminderAttendanceService.#GetEmployeeMissingCheckInOut()", Justification = "Reviewed. Suppression is OK here.")]