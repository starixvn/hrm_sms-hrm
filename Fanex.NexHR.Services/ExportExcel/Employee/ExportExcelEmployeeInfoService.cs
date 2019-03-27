namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    public class ExportExcelEmployeeInfoService : IExportExcelEmployeeInfoService
    {
        private const string EmployeeBasicInfoTemplatePath = @"~\ExcelTemplates\EmployeeBasicInfoTemplate.xls";
        private const string EmployeeDetailsInfoTemplatePath = @"~\ExcelTemplates\EmployeeDetailsInfoTemplate.xls";

        private enum BasicInfo
        {
            RowStartHeader = 2,
            RowStartBody = 3,
            TotalColumns = 16
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
            writer.SyncColumnsWidth(42);
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
            string groupDate = employee.JoinedDate.HasValue ? employee.JoinedDate.Value.ToDateMonthYearFormatedDate() : string.Empty;
            string companyDate = employee.CompanyDate.HasValue ? employee.CompanyDate.Value.ToDateMonthYearFormatedDate() : string.Empty;
            string confirmedDate = employee.ConfirmedDate.HasValue ? employee.ConfirmedDate.Value.ToDateMonthYearFormatedDate() : string.Empty;
            string resignedDate = employee.ResignedDate.HasValue ? employee.ResignedDate.Value.ToDateMonthYearFormatedDate() : string.Empty;

            var record = new
            {
                Count = rowIndex,
                Code = employee.Code,
                FullName = employee.FullName,
                NickName = employee.NickName,
                Section = employee.Team.Name,
                GroupDate = groupDate,
                CompanyDate = companyDate,
                ConfirmedDate = confirmedDate,
                ResignedDate = resignedDate,
                DoB = employee.DateOfBirth.ToDateMonthYearFormatedDate(),
                Gender = employee.Gender == 0 ? "F" : "M",
                CellPhone = employee.Phone,
                Email = employee.Email,
                CurrentAddress = employee.CurrentAddress,
                Unit = employee.Unit,
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
                Section = employee.Team.Name,
                DoB = employee.DateOfBirth.ToDateMonthYearFormatedDate(),
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
                WorkingDays = employee.WorkingDays,
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
    }
}