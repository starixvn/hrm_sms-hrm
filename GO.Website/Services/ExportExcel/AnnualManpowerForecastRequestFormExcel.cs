namespace GO.Website.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GO.Website.Models.Manpower;
    using OfficeOpenXml;

    public class AnnualManpowerForecastRequestFormExcel : ExcelBase
    {
        private const byte HeaderRowIndex = 3;
        private const byte BodyRowIndex = 6;
        private const byte TotalSectionRowIndex = 7;
        private const byte TotalDeptRowIndex = 8;
        private const byte TotalColPerCompany = 12;
        private const byte DeptColIndex = 2;
        private const byte SectionColIndex = 3;
        private const byte TotalFixedCols = 6;

        private readonly Dictionary<string, (short firstCol, short lastCol)> companyColIndex = new()
        {
            { "CTI", (7, 18) },
            { "SMS", (20, 31) },
            { "Total", (33, 44) }
        };

        private readonly AnnualManpowerForecastRequest request;
        private readonly IList<string> companyNames;
        private readonly Dictionary<string, List<string>> departments;
        private readonly IList<string> jobLevels;

        public AnnualManpowerForecastRequestFormExcel(string templatePath, AnnualManpowerForecastRequest request)
            : base(templatePath, 0, "AMPF")
        {
            this.request = request;

            companyNames = request.Details.Select(item => item.CompanyName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            departments = request.Details.Select(item => item.DepartmentName).Distinct(StringComparer.OrdinalIgnoreCase)
                .ToDictionary(item => item, item => request.Details.Where(sec => item.Equals(sec.DepartmentName, StringComparison.OrdinalIgnoreCase))
                    .Select(sec => sec.SectionName).Distinct(StringComparer.OrdinalIgnoreCase).ToList());
            jobLevels = request.Details.Select(item => item.JobLevel).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            const byte TotalCols = 18;
            templateWorksheet.Cells[1, 1, HeaderRowIndex - 1, TotalCols]
                .Copy(worksheet.Cells[1, 1, HeaderRowIndex - 1, TotalCols]);
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[HeaderRowIndex, 1, BodyRowIndex - 1, companyColIndex.First().Value.firstCol - 1]
                .Copy(worksheet.Cells[HeaderRowIndex, 1, BodyRowIndex - 1, companyColIndex.First().Value.firstCol - 1]);
            short colIndex = companyColIndex.First().Value.firstCol;
            foreach (var company in companyNames)
            {
                var companyTemplate = companyColIndex.First(item => item.Key.Equals(company, StringComparison.OrdinalIgnoreCase));

                templateWorksheet.Cells[HeaderRowIndex, companyTemplate.Value.firstCol, BodyRowIndex - 1, companyTemplate.Value.lastCol]
                    .Copy(worksheet.Cells[HeaderRowIndex, colIndex, BodyRowIndex - 1, colIndex + TotalColPerCompany]);

                FillCompanyHeader(worksheet, ref colIndex);

                colIndex++;
            }

            if (companyNames.Count >= 2)
            {
                templateWorksheet.Cells[HeaderRowIndex, companyColIndex.Last().Value.firstCol, BodyRowIndex - 1, companyColIndex.Last().Value.lastCol]
                    .Copy(worksheet.Cells[HeaderRowIndex, colIndex, BodyRowIndex - 1, colIndex + TotalColPerCompany]);

                FillCompanyHeader(worksheet, ref colIndex);
            }
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            int rowIndex = BodyRowIndex;
            string excelAddress;
            foreach (var department in departments)
            {
                int startDeptRowIndex = rowIndex;
                foreach (var section in department.Value)
                {
                    int startSectionRowIndex = rowIndex;
                    foreach (var jobLevel in jobLevels)
                    {
                        short colIndex = 1;
                        templateWorksheet.Cells[BodyRowIndex, 1, BodyRowIndex, TotalFixedCols]
                            .Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalFixedCols]);

                        worksheet.Cells[rowIndex, colIndex++].Value = request.UnitName;
                        worksheet.Cells[rowIndex, colIndex++].Value = department.Key;
                        worksheet.Cells[rowIndex, colIndex++].Value = section;
                        colIndex++;
                        worksheet.Cells[rowIndex, colIndex++].Value = jobLevel;
                        colIndex++;

                        FillCompanyRow(templateWorksheet, worksheet, department.Key, section, jobLevel, rowIndex, colIndex);

                        rowIndex++;
                    }

                    FillTotalSectionRow(templateWorksheet, worksheet, department.Key, section, rowIndex, companyColIndex.First().Value.firstCol);

                    excelAddress = $"{worksheet.Cells[startSectionRowIndex, SectionColIndex, rowIndex, SectionColIndex].Address}";
                    worksheet.Cells[excelAddress].Merge = true;

                    rowIndex++;
                }

                FillTotalDepartmentRow(templateWorksheet, worksheet, department.Key, rowIndex, companyColIndex.First().Value.firstCol);

                excelAddress = $"{worksheet.Cells[startDeptRowIndex, DeptColIndex, rowIndex, DeptColIndex].Address}";
                worksheet.Cells[excelAddress].Merge = true;

                rowIndex++;
            }

            excelAddress = $"{worksheet.Cells[BodyRowIndex, 1, rowIndex - 1, 1].Address}";
            worksheet.Cells[excelAddress].Merge = true;
        }

        protected internal override void ProcessUI(ExcelWorksheet worksheet)
        {
            const byte MinimumWidth = 5;
            const byte MaximumWidth = 20;
            worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);

            worksheet.Column(1).Width = MaximumWidth;
            worksheet.Column(DeptColIndex).Width = MaximumWidth;
            worksheet.Column(SectionColIndex).Width = MaximumWidth;

            worksheet.View.FreezePanes(BodyRowIndex, TotalFixedCols);
        }

        private void FillCompanyHeader(ExcelWorksheet worksheet, ref short colIndex)
        {
            const byte ColStep = 3;
            worksheet.Cells[HeaderRowIndex + 1, colIndex].Value = $"Current Headcount {request.Period.Year - 1}";
            colIndex += ColStep;
            worksheet.Cells[HeaderRowIndex + 1, colIndex].Value = $"Approved Forecast Headcount {request.Period.Year - 1}";
            colIndex += ColStep;
            worksheet.Cells[HeaderRowIndex + 1, colIndex].Value = "Additional";
            colIndex += ColStep;
            worksheet.Cells[HeaderRowIndex + 1, colIndex].Value = $"Forecast Headcount {request.Period.Year}";
            colIndex += ColStep;
        }

        private void FillCompanyRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet,
            string department, string section, string jobLevel,
            int rowIndex, short colIndex)
        {
            foreach (var company in companyNames)
            {
                var companyTemplate = companyColIndex.First(item => item.Key.Equals(company, StringComparison.OrdinalIgnoreCase));

                templateWorksheet.Cells[BodyRowIndex, companyTemplate.Value.firstCol, BodyRowIndex, companyTemplate.Value.lastCol]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + TotalColPerCompany]);

                var detail = request.Details.FirstOrDefault(item => item.CompanyName.Equals(company, StringComparison.OrdinalIgnoreCase)
                    && item.DepartmentName.Equals(department, StringComparison.OrdinalIgnoreCase)
                    && item.SectionName.Equals(section, StringComparison.OrdinalIgnoreCase)
                    && item.JobLevel.Equals(jobLevel, StringComparison.OrdinalIgnoreCase));

                if(detail != null)
                {
                    SetCellValue(worksheet.Cells[rowIndex, colIndex++], detail.ExpatCurrentHeadcount);
                    SetCellValue(worksheet.Cells[rowIndex, colIndex++], detail.LocalCurrentHeadcount);
                    SetCellValue(worksheet.Cells[rowIndex, colIndex++], detail.TotalCurrentHeadcount);

                    SetCellValue(worksheet.Cells[rowIndex, colIndex++], detail.ExpatApprovedHeadcount);
                    SetCellValue(worksheet.Cells[rowIndex, colIndex++], detail.LocalApprovedHeadcount);
                    SetCellValue(worksheet.Cells[rowIndex, colIndex++], detail.TotalApprovedHeadcount);

                    SetCellValue(worksheet.Cells[rowIndex, colIndex++], detail.ExpatAddition);
                    SetCellValue(worksheet.Cells[rowIndex, colIndex++], detail.LocalAddition);
                    SetCellValue(worksheet.Cells[rowIndex, colIndex++], detail.TotalAddition);

                    SetCellValue(worksheet.Cells[rowIndex, colIndex++], detail.ExpatApprovedHeadcount + detail.ExpatAddition);
                    SetCellValue(worksheet.Cells[rowIndex, colIndex++], detail.LocalApprovedHeadcount + detail.LocalAddition);
                    SetCellValue(worksheet.Cells[rowIndex, colIndex++], detail.TotalApprovedHeadcount + detail.TotalAddition);
                }
                else
                {
                    colIndex += TotalColPerCompany;
                }

                colIndex++;
            }

            if (companyNames.Count >= 2)
            {
                templateWorksheet.Cells[BodyRowIndex, companyColIndex.Last().Value.firstCol, BodyRowIndex, companyColIndex.Last().Value.lastCol]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + TotalColPerCompany]);

                var details = request.Details.Where(item => item.DepartmentName.Equals(department, StringComparison.OrdinalIgnoreCase)
                    && item.SectionName.Equals(section, StringComparison.OrdinalIgnoreCase)
                    && item.JobLevel.Equals(jobLevel, StringComparison.OrdinalIgnoreCase)).ToList();

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatCurrentHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalCurrentHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalCurrentHeadcount));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatApprovedHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalApprovedHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalApprovedHeadcount));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalAddition));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatApprovedHeadcount + item.ExpatAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalApprovedHeadcount + item.LocalAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex], details.Sum(item => item.TotalApprovedHeadcount + item.TotalAddition));
            }
        }

        private void FillTotalSectionRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet,
            string department, string section,
            int rowIndex, short colIndex)
        {
            templateWorksheet.Cells[TotalSectionRowIndex, 1, TotalSectionRowIndex, TotalFixedCols]
                .Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalFixedCols]);

            foreach (var company in companyNames)
            {
                var companyTemplate = companyColIndex.First(item => item.Key.Equals(company, StringComparison.OrdinalIgnoreCase));

                templateWorksheet.Cells[TotalSectionRowIndex, companyTemplate.Value.firstCol, TotalSectionRowIndex, companyTemplate.Value.lastCol]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + TotalColPerCompany]);

                var details = request.Details.Where(item => item.CompanyName.Equals(company, StringComparison.OrdinalIgnoreCase)
                    && item.DepartmentName.Equals(department, StringComparison.OrdinalIgnoreCase)
                    && item.SectionName.Equals(section, StringComparison.OrdinalIgnoreCase));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatCurrentHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalCurrentHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalCurrentHeadcount));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatApprovedHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalApprovedHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalApprovedHeadcount));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalAddition));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatApprovedHeadcount + item.ExpatAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalApprovedHeadcount + item.LocalAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalApprovedHeadcount + item.TotalAddition));

                colIndex++;
            }

            if (companyNames.Count >= 2)
            {
                templateWorksheet.Cells[TotalSectionRowIndex, companyColIndex.Last().Value.firstCol, TotalSectionRowIndex, companyColIndex.Last().Value.lastCol]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + TotalColPerCompany]);

                var details = request.Details.Where(item => item.DepartmentName.Equals(department, StringComparison.OrdinalIgnoreCase)
                    && item.SectionName.Equals(section, StringComparison.OrdinalIgnoreCase)).ToList();

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatCurrentHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalCurrentHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalCurrentHeadcount));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatApprovedHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalApprovedHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalApprovedHeadcount));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalAddition));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatApprovedHeadcount + item.ExpatAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalApprovedHeadcount + item.LocalAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex], details.Sum(item => item.TotalApprovedHeadcount + item.TotalAddition));
            }
        }

        private void FillTotalDepartmentRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet,
            string department, int rowIndex, short colIndex)
        {
            templateWorksheet.Cells[TotalDeptRowIndex, 1, TotalDeptRowIndex, TotalFixedCols]
                .Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalFixedCols]);

            foreach (var company in companyNames)
            {
                var companyTemplate = companyColIndex.First(item => item.Key.Equals(company, StringComparison.OrdinalIgnoreCase));

                templateWorksheet.Cells[TotalDeptRowIndex, companyTemplate.Value.firstCol, TotalDeptRowIndex, companyTemplate.Value.lastCol]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + TotalColPerCompany]);

                var details = request.Details.Where(item => item.CompanyName.Equals(company, StringComparison.OrdinalIgnoreCase)
                    && item.DepartmentName.Equals(department, StringComparison.OrdinalIgnoreCase));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatCurrentHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalCurrentHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalCurrentHeadcount));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatApprovedHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalApprovedHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalApprovedHeadcount));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalAddition));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatApprovedHeadcount + item.ExpatAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalApprovedHeadcount + item.LocalAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalApprovedHeadcount + item.TotalAddition));

                colIndex++;
            }

            if (companyNames.Count >= 2)
            {
                templateWorksheet.Cells[TotalDeptRowIndex, companyColIndex.Last().Value.firstCol, TotalDeptRowIndex, companyColIndex.Last().Value.lastCol]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + TotalColPerCompany]);

                var details = request.Details.Where(item => item.DepartmentName.Equals(department, StringComparison.OrdinalIgnoreCase)).ToList();

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatCurrentHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalCurrentHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalCurrentHeadcount));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatApprovedHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalApprovedHeadcount));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalApprovedHeadcount));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.TotalAddition));

                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.ExpatApprovedHeadcount + item.ExpatAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex++], details.Sum(item => item.LocalApprovedHeadcount + item.LocalAddition));
                SetCellValue(worksheet.Cells[rowIndex, colIndex], details.Sum(item => item.TotalApprovedHeadcount + item.TotalAddition));
            }
        }

        private static void SetCellValue(ExcelRange cell, int value)
        {
            if(value > 0)
            {
                cell.Value = value;
            }
        }
    }
}