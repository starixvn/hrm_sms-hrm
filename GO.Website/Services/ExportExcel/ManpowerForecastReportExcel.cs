namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Report.Manpower;
    using OfficeOpenXml;

    public class ManpowerForecastReportExcel : ExcelBase
    {
        private readonly IEnumerable<AnnualManpowerForecastRequest> requests;
        private readonly int year;
        private const byte RowHeaderIndex = 2;
        private const byte RowBodyIndex = 5;
        private const byte RowTotalUnitIndex = 7;
        private const byte ColBodyIndex = 5;
        private const byte ColUnitIndex = 2;
        private const byte ColDepartmentIndex = 3;
        private const byte ColSectionIndex = 4;
        private const byte ColGrandTotalIndex = 8;
        private const byte IndexUnitType = 2;
        private const byte NumColCompany = 3;

        public ManpowerForecastReportExcel(
               string templatePath,
               IEnumerable<AnnualManpowerForecastRequest> requests, int year) : base(templatePath, 0, "ManpowerForecast")
        {
            this.requests = requests;
            this.year = year;
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[1, 1].Copy(worksheet.Cells[1, 1]);
        }

        protected internal override void ProcessUI(ExcelWorksheet worksheet)
        {
            worksheet.Cells.AutoFitColumns();
            worksheet.View.FreezePanes(RowBodyIndex, 1);
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            int UnitColIndex = 5;
            var companies = GetCompanies();
            templateWorksheet.Cells[RowHeaderIndex, 1, RowBodyIndex - 1, UnitColIndex - 1]
                .Copy(worksheet.Cells[RowHeaderIndex, 1, RowBodyIndex - 1, UnitColIndex - 1]);

            RenderHeadcount(templateWorksheet, worksheet, companies, UnitColIndex, (year - 1) + " Manpower Headcount (Current)");
            UnitColIndex = UnitColIndex + NumColCompany * companies.Count + 1;
            RenderHeadcount(templateWorksheet, worksheet, companies, UnitColIndex, (year - 1) + " Approved Forecast Headcount");
            UnitColIndex = UnitColIndex + NumColCompany * companies.Count + 1;
            RenderHeadcount(templateWorksheet, worksheet, companies, UnitColIndex, "Additional Headcount");
            UnitColIndex = UnitColIndex + NumColCompany * companies.Count + 1;
            RenderHeadcount(templateWorksheet, worksheet, companies, UnitColIndex, year + " Manpower Headcount (Forecast)");
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            var companies = GetCompanies();
            var unitTypes = GetUnitTypes();
            int rowIndex = RowBodyIndex;
            foreach (var unitType in unitTypes)
            {
                var numRowUnitType = GetNumRowMergeUnitType(unitType);
                templateWorksheet.Cells[ColBodyIndex, 1, ColBodyIndex, ColUnitIndex]
                       .Copy(worksheet.Cells[rowIndex, 1, rowIndex + numRowUnitType - 1, 1]);
                var excelAddress = $"{worksheet.Cells[rowIndex, 1, rowIndex + numRowUnitType - 1, 1].Address}";
                worksheet.Cells[excelAddress].Merge = true;
                worksheet.Cells[excelAddress].Value = unitType;
                RenderUnit(templateWorksheet, worksheet, ref rowIndex, unitType, companies);
                RenderTotalUnitHeadcount(templateWorksheet, worksheet, ref rowIndex, unitType, companies);
            }
        }

        private void RenderTotalUnitHeadcount(
            ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            string unitType,
            List<string> companies)
        {
            int colIndex = ColSectionIndex;
            templateWorksheet.Cells[RowTotalUnitIndex, 1, RowTotalUnitIndex, ColUnitIndex]
                       .Copy(worksheet.Cells[rowIndex, 1, rowIndex, colIndex]);
            var excelAddress = $"{worksheet.Cells[rowIndex, 1, rowIndex, colIndex++].Address}";
            worksheet.Cells[excelAddress].Merge = true;
            worksheet.Cells[excelAddress].Value = "Total Headcount (" + unitType + ")";
            RenderTotalUnitCurrentHeadcount(templateWorksheet, worksheet, ref rowIndex, ref colIndex, companies, unitType);
            RenderTotalUnitApprovedForecastHeadcount(templateWorksheet, worksheet, ref rowIndex, ref colIndex, companies, unitType);
            RenderTotalUnitAdditionalHeadcount(templateWorksheet, worksheet, ref rowIndex, ref colIndex, companies, unitType);
            RenderTotalUnitForecastHeadcount(templateWorksheet, worksheet, ref rowIndex, ref colIndex, companies, unitType);
            rowIndex++;
        }

        private void RenderTotalUnitCurrentHeadcount(
           ExcelWorksheet templateWorksheet,
           ExcelWorksheet worksheet,
           ref int rowIndex,
           ref int colIndex,
           List<string> companies,
           string unitType
           )
        {
            int grandtotal = 0;
            foreach (var company in companies)
            {
                var (local, expat) = CalculaTotalCurrentHeadcountUnitType(company, unitType);
                templateWorksheet.Cells[RowTotalUnitIndex, 1, RowTotalUnitIndex, NumColCompany]
                       .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(local);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(expat);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(local + expat);
                grandtotal += local + expat;
            }
            templateWorksheet.Cells[RowTotalUnitIndex, 1]
                       .Copy(worksheet.Cells[rowIndex, colIndex ]);
            worksheet.Cells[rowIndex, colIndex++].Value = grandtotal;
        }

        private (int,int) CalculaTotalCurrentHeadcountUnitType(string company,
           string unitType)
        {
            int local = 0;
            int expat = 0;
            var requestsUnitType = requests.Where(x => x.UnitName.Substring(0, IndexUnitType).Contains(unitType)).ToList();
            foreach (var request in requestsUnitType)
            {
                foreach(var detail in request.Details)
                {
                    if(company == detail.CompanyName)
                    {
                        local += detail.LocalCurrentHeadcount;
                        expat += detail.ExpatCurrentHeadcount;
                    }
                }
            }
            return (local, expat);
        }

        private void RenderTotalUnitApprovedForecastHeadcount(
          ExcelWorksheet templateWorksheet,
          ExcelWorksheet worksheet,
          ref int rowIndex,
          ref int colIndex,
          List<string> companies,
           string unitType
          )
        {
            int grandtotal = 0;
            foreach (var company in companies)
            {
                var (local, expat) = CalculaTotalApprovedForecastHeadcountUnitType(company, unitType);
                templateWorksheet.Cells[RowTotalUnitIndex, 1, RowTotalUnitIndex, NumColCompany]
                       .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(local);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(expat);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(local + expat);
                grandtotal += local + expat;
            }
            templateWorksheet.Cells[RowTotalUnitIndex, 1]
                      .Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = grandtotal;
        }

        private (int, int) CalculaTotalApprovedForecastHeadcountUnitType(string company,
          string unitType)
        {
            int local = 0;
            int expat = 0;
            var requestsUnitType = requests.Where(x => x.UnitName.Substring(0, IndexUnitType).Contains(unitType)).ToList();
            foreach (var request in requestsUnitType)
            {
                foreach (var detail in request.Details)
                {
                    if (company == detail.CompanyName)
                    {
                        local += detail.LocalApprovedHeadcount;
                        expat += detail.ExpatApprovedHeadcount;
                    }
                }
            }
            return (local, expat);
        }

        private void RenderTotalUnitAdditionalHeadcount(
          ExcelWorksheet templateWorksheet,
          ExcelWorksheet worksheet,
          ref int rowIndex,
          ref int colIndex,
          List<string> companies,
           string unitType
          )
        {
            int grandtotal = 0;
            foreach (var company in companies)
            {
                var (local, expat) = CalculaTotalAdditionalHeadcountUnitType(company, unitType);
                templateWorksheet.Cells[RowTotalUnitIndex, 1, RowTotalUnitIndex, NumColCompany]
                       .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(local);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(expat);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(local + expat);
                grandtotal += local + expat;
            }
            templateWorksheet.Cells[RowTotalUnitIndex, 1]
                      .Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = grandtotal;
        }

        private (int, int) CalculaTotalAdditionalHeadcountUnitType(string company,
         string unitType)
        {
            int local = 0;
            int expat = 0;
            var requestsUnitType = requests.Where(x => x.UnitName.Substring(0, IndexUnitType).Contains(unitType)).ToList();
            foreach (var request in requestsUnitType)
            {
                foreach (var detail in request.Details)
                {
                    if (company == detail.CompanyName)
                    {
                        local += detail.LocalAddition;
                        expat += detail.TotalAddition;
                    }
                }
            }
            return (local, expat);
        }

        private void RenderTotalUnitForecastHeadcount(
          ExcelWorksheet templateWorksheet,
          ExcelWorksheet worksheet,
          ref int rowIndex,
          ref int colIndex,
          List<string> companies,
           string unitType
          )
        {
            int grandtotal = 0;
            foreach (var company in companies)
            {
                var (local, expat) = CalculaTotalForecastHeadcountUnitType(company, unitType);
                templateWorksheet.Cells[RowTotalUnitIndex, 1, RowTotalUnitIndex, NumColCompany]
                       .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(local);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(expat);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(local + expat);
                grandtotal += local + expat;
            }
            templateWorksheet.Cells[RowTotalUnitIndex, 1]
                      .Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = grandtotal;
        }
        private (int, int) CalculaTotalForecastHeadcountUnitType(string company,
        string unitType)
        {
            int local = 0;
            int expat = 0;
            var requestsUnitType = requests.Where(x => x.UnitName.Substring(0, IndexUnitType).Contains(unitType)).ToList();
            foreach (var request in requestsUnitType)
            {
                foreach (var detail in request.Details)
                {
                    if (company == detail.CompanyName)
                    {
                        local += detail.LocalApprovedHeadcount + detail.LocalAddition;
                        expat += detail.ExpatApprovedHeadcount + detail.TotalAddition;
                    }
                }
            }
            return (local, expat);
        }

        private void RenderUnit(
           ExcelWorksheet templateWorksheet,
           ExcelWorksheet worksheet,
           ref int rowIndex,
           string unitType,
           List<string> companies)
        {
            foreach (var request in requests)
            {
                if (request.UnitName.Substring(0, IndexUnitType).Contains(unitType))
                {
                    var departments = GetDepartments(request.Details);
                    var numRowUnit = GetNumRowMergeUnit(departments);
                    templateWorksheet.Cells[ColBodyIndex, 1, ColBodyIndex, ColUnitIndex]
                        .Copy(worksheet.Cells[rowIndex, ColUnitIndex, rowIndex + numRowUnit - 1, ColUnitIndex]);
                    var excelAddress = $"{worksheet.Cells[rowIndex, ColUnitIndex, rowIndex + numRowUnit - 1, ColUnitIndex].Address}";
                    worksheet.Cells[excelAddress].Merge = true;
                    worksheet.Cells[excelAddress].Value = request.UnitName;
                    RenderDepartment(templateWorksheet, worksheet, departments,ref rowIndex, request, companies);
                }
            }
        }

        private void RenderDepartment(
          ExcelWorksheet templateWorksheet,
          ExcelWorksheet worksheet,
          List<Department> departments,
          ref int rowIndex,
          AnnualManpowerForecastRequest request,
          List<string> companies)
        {
            foreach (var department in departments)
            {
                templateWorksheet.Cells[ColBodyIndex, 1, ColBodyIndex, ColUnitIndex]
                       .Copy(worksheet.Cells[rowIndex, ColDepartmentIndex, rowIndex + department.Sections.Count, ColDepartmentIndex]);
                var excelAddress = $"{worksheet.Cells[rowIndex, ColDepartmentIndex, rowIndex + department.Sections.Count, ColDepartmentIndex].Address}";
                worksheet.Cells[excelAddress].Merge = true;
                worksheet.Cells[excelAddress].Value = department.DepartmentName;
                RenderSection(templateWorksheet, worksheet, department, ref rowIndex, request, companies);
            }
        }

        private void RenderSection(
          ExcelWorksheet templateWorksheet,
         ExcelWorksheet worksheet,
         Department department,
         ref int rowIndex,
         AnnualManpowerForecastRequest request,
         List<string> companies)
        {
            //Section
            foreach (var section in department.Sections)
            {
                int colIndex = ColSectionIndex;
                templateWorksheet.Cells[ColBodyIndex, ColSectionIndex]
                       .Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex++].Value = section;
                RenderCurrentHeadcount(templateWorksheet, worksheet, request, department, section, ref rowIndex, ref colIndex, companies);
                RenderApprovedForecastHeadcount(templateWorksheet, worksheet, request, department, section, ref rowIndex, ref colIndex, companies);
                RenderAdditionalHeadcount(templateWorksheet, worksheet, request, department, section, ref rowIndex, ref colIndex, companies);
                RenderForecastHeadcount(templateWorksheet, worksheet, request, department, section, ref rowIndex, ref colIndex, companies);
                rowIndex++;
            }
            RenderTotalDepartmentHeadcount(templateWorksheet, worksheet, request, department, ref rowIndex, companies);
        }

        private void RenderTotalDepartmentHeadcount(
           ExcelWorksheet templateWorksheet,
           ExcelWorksheet worksheet,
           AnnualManpowerForecastRequest request,
           Department department,
           ref int rowIndex,
           List<string> companies)
        {
            int colIndex = ColSectionIndex;
            templateWorksheet.Cells[ColBodyIndex + 1, ColSectionIndex]
                      .Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = "Total Headcount";
            RenderTotalDepartmentCurrentHeadcount(templateWorksheet, worksheet, request, department, ref rowIndex, ref colIndex, companies);
            RenderTotalDepartmentApprovedForecastHeadcount(templateWorksheet, worksheet, request, department, ref rowIndex, ref colIndex, companies);
            RenderTotalDepartmentAdditionalHeadcount(templateWorksheet, worksheet, request, department, ref rowIndex, ref colIndex, companies);
            RenderTotalDepartmentForecastHeadcount(templateWorksheet, worksheet, request, department, ref rowIndex, ref colIndex, companies);
            rowIndex++;
        }


        private static void RenderTotalDepartmentCurrentHeadcount(
           ExcelWorksheet templateWorksheet,
           ExcelWorksheet worksheet,
           AnnualManpowerForecastRequest request,
           Department department,
           ref int rowIndex,
           ref int colIndex,
           List<string> companies
           )
        {
            int grandtotal = 0;
            foreach (var company in companies)
            {
                var localCurrentHeadcount = 0;
                var expatCurrentHeadcount = 0;
                foreach (var detail in request.Details)
                {
                    if (detail.UnitName == request.UnitName && detail.DepartmentName.ToLowerInvariant() == department.DepartmentName.ToLowerInvariant()
                        && company == detail.CompanyName)
                    {
                        localCurrentHeadcount += detail.LocalCurrentHeadcount;
                        expatCurrentHeadcount += detail.ExpatCurrentHeadcount;
                    }
                }
                templateWorksheet.Cells[ColBodyIndex + 1, ColSectionIndex + 1, ColBodyIndex + 1, ColSectionIndex + NumColCompany]
                      .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(localCurrentHeadcount);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(expatCurrentHeadcount);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(localCurrentHeadcount + expatCurrentHeadcount);
                grandtotal += localCurrentHeadcount + expatCurrentHeadcount;
            }
            templateWorksheet.Cells[ColBodyIndex + 1, ColSectionIndex + 1]
                      .Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = grandtotal;
        }

        private static void RenderTotalDepartmentApprovedForecastHeadcount(
           ExcelWorksheet templateWorksheet,
           ExcelWorksheet worksheet,
           AnnualManpowerForecastRequest request,
           Department department,
           ref int rowIndex,
           ref int colIndex,
           List<string> companies
           )
        {
            int grandtotal = 0;
            foreach (var company in companies)
            {
                var localApprovedHeadcount = 0;
                var expatApprovedHeadcount = 0;
                foreach (var detail in request.Details)
                {
                    if (detail.UnitName == request.UnitName && detail.DepartmentName.ToLowerInvariant() == department.DepartmentName.ToLowerInvariant()
                        && company == detail.CompanyName)
                    {
                        localApprovedHeadcount += detail.LocalApprovedHeadcount;
                        expatApprovedHeadcount += detail.ExpatApprovedHeadcount;
                    }
                }
                templateWorksheet.Cells[ColBodyIndex + 1, ColSectionIndex + 1, ColBodyIndex + 1, ColSectionIndex + NumColCompany]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(localApprovedHeadcount);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(expatApprovedHeadcount);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(localApprovedHeadcount + expatApprovedHeadcount);
                grandtotal += localApprovedHeadcount + expatApprovedHeadcount;
                
            }
            templateWorksheet.Cells[ColBodyIndex + 1, ColSectionIndex + 1]
                      .Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = grandtotal;
        }

        private static void RenderTotalDepartmentAdditionalHeadcount(
           ExcelWorksheet templateWorksheet,
          ExcelWorksheet worksheet,
          AnnualManpowerForecastRequest request,
          Department department,
          ref int rowIndex,
           ref int colIndex,
           List<string> companies
          )
        {
            int grandtotal = 0;
            foreach (var company in companies)
            {
                var localAddition = 0;
                var expatAddition = 0;
                foreach (var detail in request.Details)
                {
                    if (detail.UnitName == request.UnitName && detail.DepartmentName.ToLowerInvariant() == department.DepartmentName.ToLowerInvariant()
                        && company == detail.CompanyName)
                    {
                        localAddition += detail.LocalAddition;
                        expatAddition += detail.ExpatAddition;
                    }
                }
                templateWorksheet.Cells[ColBodyIndex + 1, ColSectionIndex + 1, ColBodyIndex + 1, ColSectionIndex + NumColCompany]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(localAddition);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(expatAddition);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(localAddition + expatAddition);
                grandtotal += localAddition + expatAddition;
            }
            templateWorksheet.Cells[ColBodyIndex + 1, ColSectionIndex + 1]
                      .Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = grandtotal;
        }

        private static void RenderTotalDepartmentForecastHeadcount(
           ExcelWorksheet templateWorksheet,
           ExcelWorksheet worksheet,
           AnnualManpowerForecastRequest request,
           Department department,
           ref int rowIndex,
           ref int colIndex,
           List<string> companies
          )
        {
            int grandtotal = 0;
            foreach (var company in companies)
            {
                var localForecast = 0;
                var expatForecast = 0;
                foreach (var detail in request.Details)
                {
                    if (detail.UnitName == request.UnitName && detail.DepartmentName.ToLowerInvariant() == department.DepartmentName.ToLowerInvariant()
                        && company == detail.CompanyName)
                    {
                        localForecast +=  detail.LocalApprovedHeadcount + detail.LocalAddition;
                        expatForecast += detail.ExpatApprovedHeadcount + detail.ExpatAddition;
                    }
                }
                templateWorksheet.Cells[ColBodyIndex + 1, ColSectionIndex + 1, ColBodyIndex + 1, ColSectionIndex + NumColCompany]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(localForecast);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(expatForecast);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(localForecast + expatForecast);
                grandtotal += localForecast + expatForecast;
            }
            templateWorksheet.Cells[ColBodyIndex + 1, ColSectionIndex + 1]
                     .Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = grandtotal;
        }


        private static void RenderCurrentHeadcount(
           ExcelWorksheet templateWorksheet,
           ExcelWorksheet worksheet,
           AnnualManpowerForecastRequest request,
           Department department,
           string section,
           ref int rowIndex,
           ref int colIndex,
           List<string> companies
           )
        {
            var grandtotal = 0;
            foreach(var company in companies)
            {
                var checkNone = 0;
                foreach (var detail in request.Details)
                {
                    if(detail.UnitName == request.UnitName && detail.DepartmentName.ToLowerInvariant() == department.DepartmentName.ToLowerInvariant()
                        && detail.SectionName.ToLowerInvariant() == section.ToLowerInvariant() && company== detail.CompanyName)
                    {
                        grandtotal += detail.TotalCurrentHeadcount;
                        checkNone++;
                        templateWorksheet.Cells[ColBodyIndex, ColSectionIndex + 1 , ColBodyIndex, ColSectionIndex + NumColCompany]
                            .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                        worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(detail.LocalCurrentHeadcount);
                        worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(detail.ExpatCurrentHeadcount);
                        worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(detail.TotalCurrentHeadcount);
                    }
                }
                if(checkNone == 0)
                {
                    templateWorksheet.Cells[ColBodyIndex, ColSectionIndex + 1, ColBodyIndex, ColSectionIndex + NumColCompany]
                           .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                    colIndex += NumColCompany;
                }
            }
            templateWorksheet.Cells[ColBodyIndex, ColSectionIndex + 1]
                           .Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = grandtotal;
        }

        private static void RenderApprovedForecastHeadcount(
          ExcelWorksheet templateWorksheet,
           ExcelWorksheet worksheet,
           AnnualManpowerForecastRequest request,
           Department department,
           string section,
           ref int rowIndex,
           ref int colIndex,
           List<string> companies
           )
        {
            var grandtotal = 0;
            foreach (var company in companies)
            {
                var checkNone = 0;
                foreach (var detail in request.Details)
                {
                    if (detail.UnitName == request.UnitName && detail.DepartmentName.ToLowerInvariant() == department.DepartmentName.ToLowerInvariant()
                        && detail.SectionName.ToLowerInvariant() == section.ToLowerInvariant() && company == detail.CompanyName)
                    {
                        grandtotal += detail.TotalCurrentHeadcount;
                        checkNone++;
                        templateWorksheet.Cells[ColBodyIndex, ColSectionIndex + 1, ColBodyIndex, ColSectionIndex + NumColCompany]
                           .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                        worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(detail.LocalApprovedHeadcount);
                        worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(detail.ExpatApprovedHeadcount);
                        worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(detail.TotalApprovedHeadcount);
                    }
                }
                if (checkNone == 0)
                {
                    templateWorksheet.Cells[ColBodyIndex, ColSectionIndex + 1, ColBodyIndex, ColSectionIndex + NumColCompany]
                           .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                    colIndex += NumColCompany;
                }
            }
            templateWorksheet.Cells[ColBodyIndex, ColSectionIndex + 1]
                           .Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = grandtotal;
        }

        private static void RenderAdditionalHeadcount(
          ExcelWorksheet templateWorksheet,
          ExcelWorksheet worksheet,
          AnnualManpowerForecastRequest request,
          Department department,
          string section,
          ref int rowIndex,
           ref int colIndex,
           List<string> companies
          )
        {
            var grandtotal = 0;
            foreach (var company in companies)
            {
                var checkNone = 0;
                foreach (var detail in request.Details)
                {
                    if (detail.UnitName == request.UnitName && detail.DepartmentName.ToLowerInvariant() == department.DepartmentName.ToLowerInvariant()
                        && detail.SectionName.ToLowerInvariant() == section.ToLowerInvariant() && company == detail.CompanyName)
                    {
                        grandtotal += detail.TotalCurrentHeadcount;
                        checkNone++;
                        templateWorksheet.Cells[ColBodyIndex, ColSectionIndex + 1, ColBodyIndex, ColSectionIndex + NumColCompany]
                           .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                        worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(detail.LocalAddition);
                        worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(detail.ExpatAddition);
                        worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(detail.TotalAddition);
                    }
                }
                if (checkNone == 0)
                {
                    templateWorksheet.Cells[ColBodyIndex, ColSectionIndex + 1, ColBodyIndex, ColSectionIndex + NumColCompany]
                           .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                    colIndex += NumColCompany;
                }
            }
            templateWorksheet.Cells[ColBodyIndex, ColSectionIndex + 1]
                           .Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = grandtotal;
        }

        private static void RenderForecastHeadcount(
          ExcelWorksheet templateWorksheet,
          ExcelWorksheet worksheet,
          AnnualManpowerForecastRequest request,
          Department department,
          string section,
          ref int rowIndex,
           ref int colIndex,
           List<string> companies
          )
        {
            var grandtotal = 0;
            foreach (var company in companies)
            {
                var checkNone = 0;
                foreach (var detail in request.Details)
                {
                    if (detail.UnitName == request.UnitName && detail.DepartmentName.ToLowerInvariant() == department.DepartmentName.ToLowerInvariant()
                        && detail.SectionName.ToLowerInvariant() == section.ToLowerInvariant() && company == detail.CompanyName)
                    {
                        grandtotal += detail.TotalCurrentHeadcount;
                        checkNone++;
                        templateWorksheet.Cells[ColBodyIndex, ColSectionIndex + 1, ColBodyIndex, ColSectionIndex + NumColCompany]
                           .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                        worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(detail.LocalApprovedHeadcount + detail.LocalAddition);
                        worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(detail.ExpatApprovedHeadcount + detail.ExpatAddition);
                        worksheet.Cells[rowIndex, colIndex++].Value = RenderNumberOfHeadcount(detail.TotalApprovedHeadcount + detail.TotalAddition);
                    }
                }
                if (checkNone == 0)
                {
                    templateWorksheet.Cells[ColBodyIndex, ColSectionIndex + 1, ColBodyIndex, ColSectionIndex + NumColCompany]
                           .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + NumColCompany - 1]);
                    colIndex += NumColCompany;
                }
            }
            templateWorksheet.Cells[ColBodyIndex, ColSectionIndex + 1]
                          .Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = grandtotal;
        }


        private static int GetNumRowMergeUnit(List<Department> departments)
        {
            int numRow = 0;
            foreach (var department in departments)
            {
                numRow += department.Sections.Count + 1;               
            }
            return numRow;
        }

        private int GetNumRowMergeUnitType(string unitType)
        {
            int numRow = 0;
            foreach (var request in requests)
            {
                if (request.UnitName.Substring(0, IndexUnitType).Contains(unitType))
                {
                    var departments = GetDepartments(request.Details);
                    foreach(var department in departments)
                    {
                        numRow += department.Sections.Count + 1;
                    }
                }
            }
            return numRow;
        }

        private static List<Department> GetDepartments(IList<AnnualManpowerForecastRequestDetail> details)
        {
            var departments = new List<Department>();
            foreach(var detail in details)
            {
                if(!departments.Any(x => x.DepartmentName.ToLowerInvariant().Contains(detail.DepartmentName.ToLowerInvariant())))
                {
                    departments.Add(new Department
                    {
                        DepartmentName = detail.DepartmentName,
                        Sections = GetSections(details, detail.DepartmentName)
                    });
                }
            }
            return departments;
        }

        private static List<string> GetSections(IList<AnnualManpowerForecastRequestDetail> details, string departmentName)
        {
            var sections = new List<string>();
            foreach( var detail in details)
            {
                if(!sections.Any(x => x.ToLowerInvariant().Contains(detail.SectionName.ToLowerInvariant())) &&
                    detail.DepartmentName.ToLowerInvariant().Contains(departmentName.ToLowerInvariant()))
                {
                    sections.Add(detail.SectionName);
                }
            }
            return sections;
        }

        private static void RenderHeadcount(
            ExcelWorksheet templateWorksheet, 
            ExcelWorksheet worksheet, 
            List<string> companies, 
            int unitColIndex, 
            string name)
        {
            templateWorksheet.Cells[RowHeaderIndex, ColBodyIndex, RowHeaderIndex, ColBodyIndex]
                .Copy(worksheet.Cells[RowHeaderIndex, unitColIndex, RowHeaderIndex, unitColIndex + NumColCompany * companies.Count]);
            var excelAddress = $"{worksheet.Cells[RowHeaderIndex, unitColIndex, RowHeaderIndex, unitColIndex + NumColCompany * companies.Count].Address}";
            worksheet.Cells[excelAddress].Merge = true;
            worksheet.Cells[excelAddress].Value = name;

            foreach(var company in companies)
            {
                templateWorksheet.Cells[RowHeaderIndex + 1, ColBodyIndex, RowHeaderIndex + 1, ColBodyIndex]
                    .Copy(worksheet.Cells[RowHeaderIndex + 1, unitColIndex, RowHeaderIndex + 1, unitColIndex + NumColCompany]);
                excelAddress = $"{worksheet.Cells[RowHeaderIndex + 1, unitColIndex, RowHeaderIndex + 1, unitColIndex + NumColCompany - 1].Address}";
                worksheet.Cells[excelAddress].Merge = true;
                worksheet.Cells[excelAddress].Value = company;

                templateWorksheet.Cells[ColBodyIndex - 1, ColBodyIndex, ColBodyIndex - 1, ColBodyIndex + NumColCompany]
                    .Copy(worksheet.Cells[ColBodyIndex - 1, unitColIndex, ColBodyIndex - 1, unitColIndex + NumColCompany]);
                worksheet.Cells[ColBodyIndex - 1, unitColIndex].Value = "Local";
                worksheet.Cells[ColBodyIndex - 1, unitColIndex + 1].Value = "Expat";
                worksheet.Cells[ColBodyIndex - 1, unitColIndex + NumColCompany - 1].Value = "Total";
                unitColIndex += NumColCompany;
            }
            templateWorksheet.Cells[RowHeaderIndex + 1, ColGrandTotalIndex, ColBodyIndex - 1, ColGrandTotalIndex]
                    .Copy(worksheet.Cells[RowHeaderIndex + 1, unitColIndex, ColBodyIndex - 1, unitColIndex]);
        }       

        private List<string> GetCompanies()
        {
            var companies = new List<string>();
            foreach (var item in requests)
            {
                foreach (var detail in item.Details)
                {
                    if (!companies.Any(x => x.Contains(detail.CompanyName)))
                    {
                        companies.Add(detail.CompanyName);
                    }
                }
            }
            return companies;
        }

        private List<string> GetUnitTypes()
        {
            var unitTypes = new List<string>();
            foreach (var item in requests)
            {
                if (!unitTypes.Any(x => x.Contains(item.UnitName.Substring(0, IndexUnitType))))
                {
                    unitTypes.Add(item.UnitName.Substring(0, IndexUnitType));
                }
            }
            return unitTypes;
        }

        private static string RenderNumberOfHeadcount(int number) => number != 0 ? number.ToString() : string.Empty;
    }
}