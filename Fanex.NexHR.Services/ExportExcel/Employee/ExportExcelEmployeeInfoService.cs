namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeInformation;
    using Fanex.NexHR.Resources;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelEmployeeInfoService : IExportExcelEmployeeInfoService
    {
        private const string EmployeeBasicInfoTemplatePath = @"~\ExcelTemplates\EmployeeBasicInfoTemplate.xls";
        private const string EmployeeDetailsInfoTemplatePath = @"~\ExcelTemplates\EmployeeDetailsInfoTemplate.xls";
        private const string EmployeeFullInfoTemplatePath = @"~\ExcelTemplates\EmployeeFullProfileInfoTemplate.xls";
        private readonly IEmployeeService employeeService;

        public ExportExcelEmployeeInfoService(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        private enum BasicInfo
        {
            RowStartHeader = 2,
            RowStartBody = 3,
            TotalColumns = 21
        }

        public void ExportEmployeeBasicInfo(IList<Employee> employees)
        {
            string templatePath = HttpContext.Current.Server.MapPath(EmployeeBasicInfoTemplatePath);
            string fileName = "EmployeeBasicInfo_" + DateTime.Now.ToString("yyyyMMdd");
            const string sheetName = "Employee Basic Info";
            var writer = new ExcelTemplateWriter(templatePath, fileName, sheetName);

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows((int)BasicInfo.RowStartHeader, 1);

            FillContentRowEmployeeBasicInfoReport(employees, ref writer);
            writer.SyncColumnsWidth((int)BasicInfo.TotalColumns);
            writer.CreateFreezePane(0, (int)BasicInfo.RowStartBody);

            writer.Render();
        }

        public void ExportEmployeeDetailsInfo(IList<Employee> employees)
        {
            string templatePath = HttpContext.Current.Server.MapPath(EmployeeDetailsInfoTemplatePath);
            string fileName = "EmployeeDetailsInfo_" + DateTime.Now.ToString("yyyyMMdd");
            const string sheetName = "Employee Details Info";
            var writer = new ExcelTemplateWriter(templatePath, fileName, sheetName);

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows(2, 1);

            FillContentRowEmployeeDetailsInfoReport(employees, ref writer);
            writer.SyncColumnsWidth(43);
            writer.CreateFreezePane(0, 3);

            writer.Render();
        }

        #region Employee Basic Info

        private void FillContentRowEmployeeBasicInfoReport(IList<Employee> employees, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (Employee employee in employees)
            {
                FillRowEmployeeBasicInfoReport(employee, ref writer, ref rowIndex);
            }
        }

        private void FillRowEmployeeBasicInfoReport(Employee employee, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordEmployeeBasicInfoReport(employee, rowIndex);

            var writeRowNumber = (rowIndex % 2 == 0) ? 4 : 3;

            writer.WriteRow(writeRowNumber, record);

            rowIndex++;
        }

        private object CreateRecordEmployeeBasicInfoReport(Employee employee, int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                employee.Unit,
                Department = employee.Team.Section.Department.Code,
                Section = employee.Team.Section.Code,
                Team = employee.Team.Name,
                employee.Code,
                employee.FullName,
                employee.NickName,
                GroupDate = employee.JoinedDate,
                employee.CompanyDate,
                employee.ConfirmedDate,
                employee.ResignedDate,
                employee.ResignedReason,
                LengthOfService = employeeService.CaculateLengthOfService(employee.JoinedDate, employee.MilitaryFromDate, employee.MilitaryToDate),
                DoB = employee.DateOfBirth.ToDateMonthYearFormatedDate(),
                Gender = employee.Gender == 0 ? "F" : "M",
                CellPhone = employee.Phone,
                employee.Email,
                employee.Address,
                employee.CurrentAddress,
                Category = employee.IsLocal ? Labels.Local : Labels.Expat
            };

            return record;
        }

        #endregion Employee Basic Info

        #region Employee Details Info

        private void FillContentRowEmployeeDetailsInfoReport(IList<Employee> employees, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (Employee employee in employees)
            {
                FillRowEmployeeDetailsInfoReport(employee, ref writer, ref rowIndex);
            }
        }

        private void FillRowEmployeeDetailsInfoReport(Employee employee, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordEmployeeDetailsInfoReport(employee, rowIndex);
            var writeRowNumber = (rowIndex % 2 == 0) ? 4 : 3;

            writer.WriteRow(writeRowNumber, record);

            rowIndex++;
        }

        private object CreateRecordEmployeeDetailsInfoReport(Employee employee, int rowIndex)
        {
            string dateOfInCard = employee.DateOfIdCard.HasValue ? employee.DateOfIdCard.Value.ToDateMonthYearFormatedDate() : string.Empty;
            string passportExp = employee.PassportExp.HasValue ? employee.PassportExp.Value.ToDateMonthYearFormatedDate() : string.Empty;
            string groupDate = employee.JoinedDate.HasValue ? employee.JoinedDate.Value.ToDateMonthYearFormatedDate() : string.Empty;
            string companyDate = employee.CompanyDate.HasValue ? employee.CompanyDate.Value.ToDateMonthYearFormatedDate() : string.Empty;
            string confirmedDate = employee.ConfirmedDate.HasValue ? employee.ConfirmedDate.Value.ToDateMonthYearFormatedDate() : string.Empty;
            string resignedDate = employee.ResignedDate.HasValue ? employee.ResignedDate.Value.ToDateMonthYearFormatedDate() : string.Empty;

            var record = new
            {
                Count = rowIndex,
                Code = employee.Code,
                EmployeeCard = employee.EmployeeCard,
                DomainUsername = employee.DomainUsername,
                FullName = employee.FullName,
                NickName = employee.NickName,
                Team = employee.Team.Name,
                DoB = employee.DateOfBirth,
                PlaceOfBirth = employee.PlaceOfBirth,
                Ethnic = employee.Ethnic,
                Native = employee.Native,
                Religion = employee.Religion,
                Gender = employee.Gender == 0 ? "F" : "M",
                MarriageName = employee.MarriageName,
                CountryName = employee.CountryName,
                CardNo = employee.CardNo,
                DateOfIdCard = dateOfInCard,
                PlaceOfIdCard = employee.PlaceOfIdCard,
                PassportNo = employee.PassportNo,
                PassportExp = passportExp,
                Qualification = employee.Qualification,
                Major = employee.Major,
                Phone = employee.Phone,
                HomePhone = employee.HomePhone,
                Email = employee.Email,
                Address = employee.Address,
                CurrentAddress = employee.CurrentAddress,
                Skype = employee.Skype,
                Unit = employee.Unit,
                JobTitle = employee.JobTitle,
                GroupDate = groupDate,
                CompanyDate = companyDate,
                ConfirmedDate = confirmedDate,
                ResignedDate = resignedDate,
                ResignReason = employee.ResignedReason,
                WorkingDays = employee.WorkingDays,
                WorkingHoursbyWeek = employee.WorkingHoursbyWeek,
                WorkingHours = employee.WorkingHours,
                OffDaysbyWeek = employee.OffDaysbyWeek,
                AnnualLeaveBalance = employee.AnnualLeaveBalance,
                Category = employee.IsLocal ? Labels.Local : Labels.Expat,
                BankAccount = employee.BankAccount,
                TaxCode = employee.TaxCode,
                SocialInsuranceNo = employee.SocialInsuranceNo,
                RegisteredHospital = employee.RegisteredHospital
            };

            return record;
        }

        #endregion Employee Details Info

        #region Employee Full Profile Information

        public void ExportEmployeeFullProfile(IList<EmployeeFullProfile> profiles)
        {
            string templatePath = HttpContext.Current.Server.MapPath(EmployeeFullInfoTemplatePath);
            string fileName = "EmployeeFullInfo_" + DateTime.Now.ToString("yyyyMMdd");
            const string sheetName = "Employee Full Information";
            var writer = new ExcelTemplateWriter(templatePath, fileName, sheetName);

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows(2, 1);

            WriteFullProfileBody(writer, profiles);
            writer.SyncColumnsWidth(56);
            writer.CreateFreezePane(0, 3);

            writer.Render();
        }

        private void WriteFullProfileBody(ExcelTemplateWriter writer,
            IList<EmployeeFullProfile> profiles)
        {
            var currentRow = 3;
            foreach (var p in profiles)
            {
                var writeRowNumber = (currentRow % 2 == 0) ? 4 : 3;

                var record = CreateRecordEployeeFullProfile(p, currentRow);
                writer.WriteRow(writeRowNumber, record);
                currentRow++;
            }
        }

        private object CreateRecordEployeeFullProfile(EmployeeFullProfile p, int currentRow)
        {
            var gender = "unknown";
            if (p.Gender.HasValue)
            {
                gender = p.Gender == 0 ? "Female" : "Male";
            }

            var record = new
            {
                Index = currentRow - 2,
                p.Unit,
                Department = p.DepartmentCode,
                Section = p.SectionCode,
                p.TeamName,
                p.Code,
                p.EmployeeCard,
                p.FullName,
                p.NickName,
                Gender = gender,
                GroupDate = p.JoinedDate?.ToString("dd/MM/yyyy"),
                CompanyDate = p.CompanyDate?.ToString("dd/MM/yyyy"),
                ConfirmedDate = p.ConfirmedDate?.ToString("dd/MM/yyyy"),
                ResignedDate = p.ResignedDate?.ToString("dd/MM/yyyy"),
                p.ResignedReason,
                LengthOfService = employeeService.CaculateLengthOfService(p.JoinedDate, p.MilitaryFromDate, p.MilitaryToDate),
                p.JobTitle,
                p.JobLevel,
                TeamDate = p.TeamDate?.ToString("dd/MM/yyyy"),
                DOB = p.DateOfBirth.ToString("dd/MM/yyyy"),
                p.PlaceOfBirth,
                p.Ethnic,
                p.Native,
                p.Religion,
                p.MarriageName,
                p.Nationality,
                p.CardNo,
                DateOfIDCard = p.DateOfIdCard?.ToString("dd/MM/yyyy"),
                p.PlaceOfIdCard,
                p.PassportNo,
                PassportExp = p.PassportExp?.ToString("dd/MM/yyyy"),
                p.Qualification,
                p.Major,
                p.Address,
                p.CurrentAddress,
                p.Phone,
                p.HomePhone,
                p.Email,
                p.PickupRemark,
                p.Skype,
                p.LetsTalk,
                CanOT = p.CanOT ? Labels.Yes : Labels.No,
                p.SpecialOT,
                p.WorkingDays,
                p.WorkingHours,
                p.OffDaysbyWeek,
                p.AnnualLeaveBalance,
                p.KeepInViewBalance,
                Category = p.IsLocal ? Labels.Local : Labels.Expat,
                p.Roles2String,
                SpecialUser = p.IsTopManager ? Labels.Yes : Labels.No,
                VerifiedBy = employeeService.GetSupervisors(p.Supervisor, (short)RoleSystem.Verifier),
                ApprovedBy = employeeService.GetSupervisors(p.Supervisor, (short)RoleSystem.Approver),
                DisciplineManager = employeeService.GetSupervisors(p.Supervisor, (short)RoleSystem.DisciplineManager),
                ShiftArrangement = employeeService.GetSupervisors(p.Supervisor, (short)RoleSystem.ShiftManager),
                OnBehalfRequest = employeeService.GetSupervisors(p.Supervisor, (short)RoleSystem.OnBehalfRequest),
            };

            return record;
        }

        #endregion Employee Full Profile Information
    }
}