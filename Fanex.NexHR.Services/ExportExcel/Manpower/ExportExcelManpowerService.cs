namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
    public class ExportExcelManpowerService : IExportExcelManpowerService
    {
        public const int RowStartHeader = 2;
        public const int RowStartBody = 4;
        public const int RowFooter = 6;
        public const int TotalColumns = 54;
        private const string ManpowerTemplatePath = @"~\ExcelTemplates\ManpowerReportTemplate.xls";

        public void Export(IList<ManpowerModel> manpowers, int year)
        {
            string templatePath = HttpContext.Current.Server.MapPath(ManpowerTemplatePath);
            string fileName = "ManpowerReport_" + DateTime.Now.ToString("yyyyMMdd");
            string reportTitle = Labels.ManpowerReportTitle + " " + year;

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Manpower Report");

            /// Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            /// Fill report header.
            writer.CopyRows(RowStartHeader, 2);

            FillContentRowReport(manpowers, year, ref writer);
            FillContentRowFooterReport(manpowers, year, ref writer);
            writer.SyncColumnsWidth(TotalColumns);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRowReport(IList<ManpowerModel> manpowers, int year, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (ManpowerModel manpower in manpowers)
            {
                FillRowReport(manpower, year, ref writer, ref rowIndex);
            }
        }

        private void FillRowReport(ManpowerModel manpower, int year, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordReport(manpower, year, rowIndex);
            if (rowIndex % 2 != 0)
            {
                writer.WriteRow(RowStartBody, record);
            }
            else
            {
                writer.WriteRow(RowStartBody + 1, record);
            }

            rowIndex++;
        }

        private dynamic GetValueForCell(int value, int year, int month)
        {
            bool isShowValue = false;
            dynamic result;

            if ((DateTime.Now.Year > year) || (DateTime.Now.Year == year && DateTime.Now.Month >= month))
            {
                isShowValue = true;
            }

            if (isShowValue)
            {
                result = value;
            }
            else
            {
                result = "-";
            }

            return result;
        }

        private object CreateRecordReport(ManpowerModel manpower, int year, int rowIndex)
        {
            int month = 1;
            var record = new
            {
                Count = rowIndex,
                GroupCode = manpower.GroupCode,
                ProbationResigned = manpower.ProbationResigned,
                ConfirmedResigned = manpower.ConfirmedResigned,
                NewHire = manpower.NewHire,
                ActiveUser = manpower.ActiveUser,
                ProbationResignedJan = GetValueForCell(manpower.ProbationResignedJan, year, month),
                ConfirmedResignedJan = GetValueForCell(manpower.ConfirmedResignedJan, year, month),
                NewHireJan = GetValueForCell(manpower.NewHireJan, year, month),
                ActiveUserJan = GetValueForCell(manpower.ActiveUserJan, year, month),
                ProbationResignedFeb = GetValueForCell(manpower.ProbationResignedFeb, year, ++month),
                ConfirmedResignedFeb = GetValueForCell(manpower.ConfirmedResignedFeb, year, month),
                NewHireFeb = GetValueForCell(manpower.NewHireFeb, year, month),
                ActiveUserFeb = GetValueForCell(manpower.ActiveUserFeb, year, month),
                ProbationResignedMar = GetValueForCell(manpower.ProbationResignedMar, year, ++month),
                ConfirmedResignedMar = GetValueForCell(manpower.ConfirmedResignedMar, year, month),
                NewHireMar = GetValueForCell(manpower.NewHireMar, year, month),
                ActiveUserMar = GetValueForCell(manpower.ActiveUserMar, year, month),
                ProbationResignedApr = GetValueForCell(manpower.ProbationResignedApr, year, ++month),
                ConfirmedResignedApr = GetValueForCell(manpower.ConfirmedResignedApr, year, month),
                NewHireApr = GetValueForCell(manpower.NewHireApr, year, month),
                ActiveUserApr = GetValueForCell(manpower.ActiveUserApr, year, month),
                ProbationResignedMay = GetValueForCell(manpower.ProbationResignedMay, year, ++month),
                ConfirmedResignedMay = GetValueForCell(manpower.ConfirmedResignedMay, year, month),
                NewHireMay = GetValueForCell(manpower.NewHireMay, year, month),
                ActiveUserMay = GetValueForCell(manpower.ActiveUserMay, year, month),
                ProbationResignedJun = GetValueForCell(manpower.ProbationResignedJun, year, ++month),
                ConfirmedResignedJun = GetValueForCell(manpower.ConfirmedResignedJun, year, month),
                NewHireJun = GetValueForCell(manpower.NewHireJun, year, month),
                ActiveUserJun = GetValueForCell(manpower.ActiveUserJun, year, month),
                ProbationResignedJul = GetValueForCell(manpower.ProbationResignedJul, year, ++month),
                ConfirmedResignedJul = GetValueForCell(manpower.ConfirmedResignedJul, year, month),
                NewHireJul = GetValueForCell(manpower.NewHireJul, year, month),
                ActiveUserJul = GetValueForCell(manpower.ActiveUserJul, year, month),
                ProbationResignedAug = GetValueForCell(manpower.ProbationResignedAug, year, ++month),
                ConfirmedResignedAug = GetValueForCell(manpower.ConfirmedResignedAug, year, month),
                NewHireAug = GetValueForCell(manpower.NewHireAug, year, month),
                ActiveUserAug = GetValueForCell(manpower.ActiveUserAug, year, month),
                ProbationResignedSep = GetValueForCell(manpower.ProbationResignedSep, year, ++month),
                ConfirmedResignedSep = GetValueForCell(manpower.ConfirmedResignedSep, year, month),
                NewHireSep = GetValueForCell(manpower.NewHireSep, year, month),
                ActiveUserSep = GetValueForCell(manpower.ActiveUserSep, year, month),
                ProbationResignedOct = GetValueForCell(manpower.ProbationResignedOct, year, ++month),
                ConfirmedResignedOct = GetValueForCell(manpower.ConfirmedResignedOct, year, month),
                NewHireOct = GetValueForCell(manpower.NewHireOct, year, month),
                ActiveUserOct = GetValueForCell(manpower.ActiveUserOct, year, month),
                ProbationResignedNov = GetValueForCell(manpower.ProbationResignedNov, year, ++month),
                ConfirmedResignedNov = GetValueForCell(manpower.ConfirmedResignedNov, year, month),
                NewHireNov = GetValueForCell(manpower.NewHireNov, year, month),
                ActiveUserNov = GetValueForCell(manpower.ActiveUserNov, year, month),
                ProbationResignedDec = GetValueForCell(manpower.ProbationResignedDec, year, ++month),
                ConfirmedResignedDec = GetValueForCell(manpower.ConfirmedResignedDec, year, month),
                NewHireDec = GetValueForCell(manpower.NewHireDec, year, month),
                ActiveUserDec = GetValueForCell(manpower.ActiveUserDec, year, month)
            };

            return record;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Ok.")]
        private void FillContentRowFooterReport(IList<ManpowerModel> manpowers, int year, ref ExcelTemplateWriter writer)
        {
            int month = 1;
            var record = new
            {
                Count = Labels.Total,
                GroupCode = string.Empty,
                ProbationResigned = manpowers.Sum(t => t.ProbationResigned),
                ConfirmedResigned = manpowers.Sum(t => t.ConfirmedResigned),
                NewHire = manpowers.Sum(t => t.NewHire),
                ActiveUser = manpowers.Sum(t => t.ActiveUser),
                ProbationResignedJan = GetValueForCell(manpowers.Sum(t => t.ProbationResignedJan), year, month),
                ConfirmedResignedJan = GetValueForCell(manpowers.Sum(t => t.ConfirmedResignedJan), year, month),
                NewHireJan = GetValueForCell(manpowers.Sum(t => t.NewHireJan), year, month),
                ActiveUserJan = GetValueForCell(manpowers.Sum(t => t.ActiveUserJan), year, month),
                ProbationResignedFeb = GetValueForCell(manpowers.Sum(t => t.ProbationResignedFeb), year, ++month),
                ConfirmedResignedFeb = GetValueForCell(manpowers.Sum(t => t.ConfirmedResignedFeb), year, month),
                NewHireFeb = GetValueForCell(manpowers.Sum(t => t.NewHireFeb), year, month),
                ActiveUserFeb = GetValueForCell(manpowers.Sum(t => t.ActiveUserFeb), year, month),
                ProbationResignedMar = GetValueForCell(manpowers.Sum(t => t.ProbationResignedMar), year, ++month),
                ConfirmedResignedMar = GetValueForCell(manpowers.Sum(t => t.ConfirmedResignedMar), year, month),
                NewHireMar = GetValueForCell(manpowers.Sum(t => t.NewHireMar), year, month),
                ActiveUserMar = GetValueForCell(manpowers.Sum(t => t.ActiveUserMar), year, month),
                ProbationResignedApr = GetValueForCell(manpowers.Sum(t => t.ProbationResignedApr), year, ++month),
                ConfirmedResignedApr = GetValueForCell(manpowers.Sum(t => t.ConfirmedResignedApr), year, month),
                NewHireApr = GetValueForCell(manpowers.Sum(t => t.NewHireApr), year, month),
                ActiveUserApr = GetValueForCell(manpowers.Sum(t => t.ActiveUserApr), year, month),
                ProbationResignedMay = GetValueForCell(manpowers.Sum(t => t.ProbationResignedMay), year, ++month),
                ConfirmedResignedMay = GetValueForCell(manpowers.Sum(t => t.ConfirmedResignedMay), year, month),
                NewHireMay = GetValueForCell(manpowers.Sum(t => t.NewHireMay), year, month),
                ActiveUserMay = GetValueForCell(manpowers.Sum(t => t.ActiveUserMay), year, month),
                ProbationResignedJun = GetValueForCell(manpowers.Sum(t => t.ProbationResignedJun), year, ++month),
                ConfirmedResignedJun = GetValueForCell(manpowers.Sum(t => t.ConfirmedResignedJun), year, month),
                NewHireJun = GetValueForCell(manpowers.Sum(t => t.NewHireJun), year, month),
                ActiveUserJun = GetValueForCell(manpowers.Sum(t => t.ActiveUserJun), year, month),
                ProbationResignedJul = GetValueForCell(manpowers.Sum(t => t.ProbationResignedJul), year, ++month),
                ConfirmedResignedJul = GetValueForCell(manpowers.Sum(t => t.ConfirmedResignedJul), year, month),
                NewHireJul = GetValueForCell(manpowers.Sum(t => t.NewHireJul), year, month),
                ActiveUserJul = GetValueForCell(manpowers.Sum(t => t.ActiveUserJul), year, month),
                ProbationResignedAug = GetValueForCell(manpowers.Sum(t => t.ProbationResignedAug), year, ++month),
                ConfirmedResignedAug = GetValueForCell(manpowers.Sum(t => t.ConfirmedResignedAug), year, month),
                NewHireAug = GetValueForCell(manpowers.Sum(t => t.NewHireAug), year, month),
                ActiveUserAug = GetValueForCell(manpowers.Sum(t => t.ActiveUserAug), year, month),
                ProbationResignedSep = GetValueForCell(manpowers.Sum(t => t.ProbationResignedSep), year, ++month),
                ConfirmedResignedSep = GetValueForCell(manpowers.Sum(t => t.ConfirmedResignedSep), year, month),
                NewHireSep = GetValueForCell(manpowers.Sum(t => t.NewHireSep), year, month),
                ActiveUserSep = GetValueForCell(manpowers.Sum(t => t.ActiveUserSep), year, month),
                ProbationResignedOct = GetValueForCell(manpowers.Sum(t => t.ProbationResignedOct), year, ++month),
                ConfirmedResignedOct = GetValueForCell(manpowers.Sum(t => t.ConfirmedResignedOct), year, month),
                NewHireOct = GetValueForCell(manpowers.Sum(t => t.NewHireOct), year, month),
                ActiveUserOct = GetValueForCell(manpowers.Sum(t => t.ActiveUserOct), year, month),
                ProbationResignedNov = GetValueForCell(manpowers.Sum(t => t.ProbationResignedNov), year, ++month),
                ConfirmedResignedNov = GetValueForCell(manpowers.Sum(t => t.ConfirmedResignedNov), year, month),
                NewHireNov = GetValueForCell(manpowers.Sum(t => t.NewHireNov), year, month),
                ActiveUserNov = GetValueForCell(manpowers.Sum(t => t.ActiveUserNov), year, month),
                ProbationResignedDec = GetValueForCell(manpowers.Sum(t => t.ProbationResignedDec), year, ++month),
                ConfirmedResignedDec = GetValueForCell(manpowers.Sum(t => t.ConfirmedResignedDec), year, month),
                NewHireDec = GetValueForCell(manpowers.Sum(t => t.NewHireDec), year, month),
                ActiveUserDec = GetValueForCell(manpowers.Sum(t => t.ActiveUserDec), year, month)
            };

            writer.WriteRow(RowFooter, record);
        }
    }
}