// --------------------------------------------------------------------------
// <copyright file="ReminderAttendanceService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.10">Created</change>
// </history>

namespace Fanex.NexHR.ReminderService.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.ReminderService.Criteria;
    using Fanex.NexHR.ReminderService.Models;
    using Fanex.NexHR.ReminderService.Repositories;
    using Fanex.WindowsService;

    public class ReminderAttendanceService : IService
    {
        private const string _SubjectMissingCheckIn = "{0} - Missing Check In at Date: {1}";
        private const string _SubjectMissingCheckOut = "{0} - Missing Check Out at Date: {1}";
        private const string _SubjectLateIn = "{0} - Late In at Date: {1}";
        private const string _SubjectEarlyOut = "{0} - Early Out at Date: {1}";
        private const string _SubjectAbsent = "{0} - Absent at Date: {1}";
        private const string _SubjectAboveOffDay = "{0} - Double check your working schedule at week: {1} to {2}";

        private const string _TplEmailMissingCheckIn = "emails\\MissingCheckIn.cshtml";
        private const string _TplEmailMissingCheckOut = "emails\\MissingCheckOut.cshtml";
        private const string _TplEmailLateIn = "emails\\LateIn.cshtml";
        private const string _TplEmailEarlyOut = "emails\\EarlyOut.cshtml";
        private const string _TplEmailAbsent = "emails\\Absent.cshtml";
        private const string _TplEmailAboveOffDay = "emails\\OffDay.cshtml";

        private readonly IEmployeeRepository _repository;

        public ReminderAttendanceService()
            : this(null)
        { 
        }

        public ReminderAttendanceService(IEmployeeRepository repository)
        {
            _repository = repository ?? new EmployeeRepository();
        }

        public void ExcuteJob()
        {
            //RemindAttendance();
            var date = DateTime.Now;
            var x = FormatTime(date);
            Console.WriteLine("I am working");
            System.Threading.Thread.Sleep(2000);
        }

        public void RemindAttendance()
        {
            RemindForMissingCheckInOut();

            RemindForLateInEarlyOut();

            RemindForAbsent();

            RemindForAboveOffDay();
        }

        public void RemindForMissingCheckInOut()
        {
            IList<EmployeeMissingCheckInOutModel> employees = GetEmployeeMissingCheckInOut();
            foreach (EmployeeMissingCheckInOutModel employee in employees)
            {
                if (employee.Type == 1)
                {
                    RemindForMissingCheckIn(employee);
                }
                else
                {
                    RemindForMissingCheckOut(employee);
                }
            }
        }

        private void RemindForMissingCheckIn(EmployeeMissingCheckInOutModel employee)
        {
            string emailSubject = string.Format(_SubjectMissingCheckIn, employee.NickName, FormatDate(employee.WorkDate));
            var bodyTemplate = EmailService.ReadTextFile(_TplEmailMissingCheckIn);
            var body = string.Format(bodyTemplate, employee.NickName, FormatDate(employee.WorkDate), employee.ShiftCode, FormatTime(employee.CheckOut));

            EmailService.SendEmail(employee.Email, string.Empty, emailSubject, body);
        }

        private void RemindForMissingCheckOut(EmployeeMissingCheckInOutModel employee)
        {
            string emailSubject = string.Format(_SubjectMissingCheckOut, employee.NickName, FormatDate(employee.WorkDate), employee.ShiftCode, FormatTime(employee.CheckIn));
            var bodyTemplate = EmailService.ReadTextFile(_TplEmailMissingCheckOut);
            var body = string.Format(bodyTemplate, employee.NickName, FormatDate(employee.WorkDate), employee.ShiftCode);

            EmailService.SendEmail(employee.Email, string.Empty, emailSubject, body);
        }

        public void RemindForLateInEarlyOut()
        {
            IList<EmployeeLateInEarlyOutModel> employees = GetEmployeeLateInEarlyOut();
            foreach (EmployeeLateInEarlyOutModel employee in employees)
            {
                if (employee.Type == 3)
                {
                    RemindForLateIn(employee);
                }
                else
                {
                    RemindForEarlyOut(employee);
                }
            }
        }

        private void RemindForLateIn(EmployeeLateInEarlyOutModel employee)
        {
            string emailSubject = string.Format(_SubjectLateIn, employee.NickName, FormatDate(employee.WorkDate));
            var bodyTemplate = EmailService.ReadTextFile(_TplEmailLateIn);
            var body = string.Format(
                bodyTemplate,
                employee.NickName,
                FormatDate(employee.WorkDate),
                employee.ShiftCode,
                FormatTime(employee.CheckIn),
                FormatTime(employee.CheckOut),
                employee.LateIn);

            EmailService.SendEmail(employee.Email, string.Empty, emailSubject, body);
        }

        private void RemindForEarlyOut(EmployeeLateInEarlyOutModel employee)
        {
            string emailSubject = string.Format(_SubjectEarlyOut, employee.NickName, FormatDate(employee.WorkDate));
            var bodyTemplate = EmailService.ReadTextFile(_TplEmailEarlyOut);
            var body = string.Format(
                bodyTemplate, 
                employee.NickName, 
                FormatDate(employee.WorkDate), 
                employee.ShiftCode,
                FormatTime(employee.CheckIn),
                FormatTime(employee.CheckOut),
                employee.EarlyOut);

            EmailService.SendEmail(employee.Email, string.Empty, emailSubject, body);
        }

        private void RemindForAbsent()
        {
            IList<EmployeeAbsentModel> employees = GetEmployeeAbsent();
            foreach (EmployeeAbsentModel employee in employees)
            {
                RemindForAbsent(employee);
            }
        }

        private void RemindForAbsent(EmployeeAbsentModel employee)
        {
            string emailSubject = string.Format(_SubjectAbsent, employee.NickName, FormatDate(employee.WorkDate));
            var bodyTemplate = EmailService.ReadTextFile(_TplEmailAbsent);
            var body = string.Format(bodyTemplate, employee.NickName, FormatDate(employee.WorkDate));

            EmailService.SendEmail(employee.Email, string.Empty, emailSubject, body);
        }

        private void RemindForAboveOffDay()
        {
            IList<EmployeeOffDayModel> employees = GetEmployeeAboveOffDay();
            foreach (EmployeeOffDayModel employee in employees)
            {
                RemindForAboveOffDay(employee);
            }
        }

        private void RemindForAboveOffDay(EmployeeOffDayModel employee)
        {
            string emailSubject = string.Format(_SubjectAboveOffDay, employee.NickName, FormatDate(employee.FromDate), FormatDate(employee.ToDate));
            var bodyTemplate = EmailService.ReadTextFile(_TplEmailAboveOffDay);
            var body = string.Format(bodyTemplate, employee.NickName, FormatDate(employee.FromDate), FormatDate(employee.ToDate));

            EmailService.SendEmail(employee.Email, string.Empty, emailSubject, body);
        }

        public IList<EmployeeMissingCheckInOutModel> GetEmployeeMissingCheckInOut()
        {
            var criteria = new GetEmployeeMissingCheckInOutCriteria();
            return _repository.GetEmployeeMissingCheckInOut(criteria);
        }

        public IList<EmployeeAbsentModel> GetEmployeeAbsent()
        {
            var criteria = new GetEmployeeAbsentCriteria();
            return _repository.GetEmployeeAbsent(criteria);
        }

        public IList<EmployeeLateInEarlyOutModel> GetEmployeeLateInEarlyOut()
        {
            var criteria = new GetEmployeeLateInEarlyOutCriteria();
            return _repository.GetEmployeeLateInEarlyOut(criteria);
        }

        public IList<EmployeeOffDayModel> GetEmployeeAboveOffDay()
        {
            var criteria = new GetEmployeeOffDayCriteria();
            return _repository.GetEmployeeAboveOffDay(criteria);
        }

        private string FormatDate(DateTime date)
        {
            return date.ToString("dd MMM yyyy");
        }

        private string FormatTime(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("HH:mm") : string.Empty;
        }
    }
}
