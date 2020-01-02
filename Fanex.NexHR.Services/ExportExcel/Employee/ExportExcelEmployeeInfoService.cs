namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeInformation;
    using Fanex.NexHR.Resources;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelEmployeeInfoService : IExportExcelEmployeeInfoService
    {
        private const string EmployeeBasicInfoTemplatePath = @"~\ExcelTemplates\EmployeeBasicInfoTemplate.xls";
        private const string EmployeeDetailsInfoTemplatePath = @"~\ExcelTemplates\EmployeeDetailsInfoTemplate.xls";
        private const string EmployeeFullInfoTemplatePath = @"~\ExcelTemplates\EmployeeFullProfileInfoTemplate.xls";

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
            var record = new
            {
                Count = rowIndex,
                Code = employee.Code,
                FullName = employee.FullName,
                NickName = employee.NickName,
                Section = employee.Team.Name,
                GroupDate = employee.JoinedDate,
                CompanyDate = employee.CompanyDate,
                ConfirmedDate = employee.ConfirmedDate,
                ResignedDate = employee.ResignedDate,
                DoB = employee.DateOfBirth,
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

        #region Employee Full Profile Information

        public void ExportEmployeeFullProfile(IList<EmployeeFullProfile> profiles)
        {
            int maxRelationship = profiles.Max(p => p.Relationships.Count());

            string templatePath = HttpContext.Current.Server.MapPath(EmployeeFullInfoTemplatePath);
            string fileName = "EmployeeFullInfo_" + DateTime.Now.ToString("yyyyMMdd");
            const string sheetName = "Employee Full Information";
            var writer = new ExcelTemplateWriter(templatePath, fileName, sheetName);

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows(2, 1);
            FillEmployeeFullProfileHeader(writer, maxRelationship);

            WriteFullProfileBody(writer, profiles, maxRelationship);
            writer.SyncColumnsWidth(21 + maxRelationship);
            writer.CreateFreezePane(0, 3);

            writer.Render();
        }

        private static void WriteFullProfileBody(ExcelTemplateWriter writer,
            IList<EmployeeFullProfile> profiles,
            int maxLengthRelationship)
        {
            var currentRow = 3;
            foreach (var p in profiles)
            {
                var writeRowNumber = (currentRow % 2 == 0) ? 4 : 3;

                var record = CreateRecordEployeeFullProfile(p, currentRow);
                if (maxLengthRelationship > 0)
                {
                    var baseColumn = 21;
                    for (int i = 0; i < maxLengthRelationship; i++)
                    {
                        writer.CopyCellStyle(writeRowNumber, 1, currentRow, baseColumn + i);
                        baseColumn++;
                        writer.CopyCellStyle(writeRowNumber, 1, currentRow, baseColumn + i);
                        baseColumn++;
                        writer.CopyCellStyle(writeRowNumber, 1, currentRow, baseColumn + i);
                    }
                }
                writer.WriteRow(writeRowNumber, record);
                WriteRelationship(writer, p.Relationships, currentRow);

                currentRow++;
            }
        }

        private static void WriteRelationship(ExcelTemplateWriter writer, IEnumerable<EmployeeRelationship> relationships, int currentRow)
        {
            if (relationships != null && relationships.Any())
            {
                int baseColumn = 21;
                foreach (var r in relationships)
                {
                    writer.WriteCellValue(currentRow, baseColumn, r.FullName);
                    baseColumn++;
                    writer.WriteCellValue(currentRow, baseColumn, r.DateOfBirth.ToString("dd/MM/yyyy"));
                    baseColumn++;
                    writer.WriteCellValue(currentRow, baseColumn, r.Gender == 0 ? "Female" : "Male");
                    baseColumn++;
                }
            }
        }

        private static object CreateRecordEployeeFullProfile(EmployeeFullProfile p, int currentRow)
        {
            var gender = "unknown";
            if (p.Gender.HasValue)
            {
                gender = p.Gender == 0 ? "Female" : "Male";
            }

            var record = new
            {
                Index = currentRow - 2,
                TeamName = p.Team.Name,
                p.Code,
                p.FullName,
                p.NickName,
                Gender = gender,
                DOB = p.DateOfBirth,
                p.MarriageName,
                p.Address,
                p.CurrentAddress,
                p.CardNo,
                DateOfIDCard = p.DateOfIdCard,
                p.PlaceOfIdCard,
                PlaceRegKCB = string.Empty,
                p.Phone,
                p.HomePhone,
                p.Email,
                p.Qualification,
                p.Major,
                Emergency = p.Emergency != null ? p.Emergency.Phone : string.Empty,
                EmergencyFullName = p.Emergency != null ? p.Emergency.FullName : string.Empty
            };

            return record;
        }

        private static void FillEmployeeFullProfileHeader(ExcelTemplateWriter writer, int maxRelationship)
        {
            const int headerRow = 2;
            var currentColumn = 21;

            if (maxRelationship > 0)
            {
                for (int i = 0; i < maxRelationship; i++)
                {
                    var index = i + 1;
                    writer.CopyCellStyle(headerRow, 1, desRow: headerRow, desCol: currentColumn);
                    writer.WriteCellValue(headerRow, currentColumn, "Con " + index);
                    currentColumn++;
                    writer.CopyCellStyle(headerRow, 1, desRow: headerRow, desCol: currentColumn);
                    writer.WriteCellValue(headerRow, currentColumn, "Ngày sinh " + index);
                    currentColumn++;
                    writer.CopyCellStyle(headerRow, 1, desRow: headerRow, desCol: currentColumn);
                    writer.WriteCellValue(headerRow, currentColumn, "Giới tính " + index);
                    currentColumn++;
                }
            }
        }

        #endregion Employee Full Profile Information
    }
}