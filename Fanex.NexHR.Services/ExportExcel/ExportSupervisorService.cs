namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using OfficeOpenXml;

    public interface IExportSupervisorService
    {
        byte[] ExportEmployeeSupervisor(IList<SupervisorReportModel> supervisors);

        byte[] ExportSectionSupervisor(IList<SectionSupervisorReportModel> supervisors);
    }

#pragma warning disable S109 // Magic numbers should not be used
    public class ExportSupervisorService : IExportSupervisorService
    {
        private const byte HeaderRowIndex = 2;
        private const byte BodyRowIndex = 3;
        private const byte OddRowIndex = 3;
        private const byte EvenRowIndex = 4;
        private const byte MinimumWidth = 10;
        private const byte MaximumWidth = 50;

        public byte[] ExportEmployeeSupervisor(IList<SupervisorReportModel> supervisors)
        {
            string templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/EmployeeSupervisor.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    const byte TotalColumns = 10;
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add(Labels.EmployeeSupervisor);
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title & header
                    templateWorksheet.Cells[rowIndex, colIndex, HeaderRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, HeaderRowIndex, TotalColumns]);

                    // data
                    rowIndex = BodyRowIndex;
                    var recordIndex = 1;
                    foreach (SupervisorReportModel supervisor in supervisors)
                    {
                        colIndex = 1;
                        if (recordIndex % 2 != 0)
                        {
                            templateWorksheet.Cells[OddRowIndex, 1, OddRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                        }
                        else
                        {
                            templateWorksheet.Cells[EvenRowIndex, 1, EvenRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                        }

                        worksheet.Cells[rowIndex, colIndex++].Value = recordIndex++;
                        worksheet.Cells[rowIndex, colIndex++].Value = supervisor.NickName;
                        worksheet.Cells[rowIndex, colIndex++].Value = supervisor.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = supervisor.Section.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = supervisor.Roles2String;
                        worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.Verifier);
                        worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.Approver);
                        worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.ShiftArrangement);
                        worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.OnBehalfRequest);
                        worksheet.Cells[rowIndex, colIndex].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.DisciplineManager);

                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
                    worksheet.View.FreezePanes(BodyRowIndex, 1);

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        public byte[] ExportSectionSupervisor(IList<SectionSupervisorReportModel> supervisors)
        {
            string templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/SectionSupervisor.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    const byte TotalColumns = 7;
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add(Labels.SectionSupervisor);
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title & header
                    templateWorksheet.Cells[rowIndex, colIndex, HeaderRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, HeaderRowIndex, TotalColumns]);

                    // data
                    rowIndex = BodyRowIndex;
                    var recordIndex = 1;
                    foreach (SectionSupervisorReportModel supervisor in supervisors)
                    {
                        colIndex = 1;
                        if (recordIndex % 2 != 0)
                        {
                            templateWorksheet.Cells[OddRowIndex, 1, OddRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                        }
                        else
                        {
                            templateWorksheet.Cells[EvenRowIndex, 1, EvenRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                        }

                        worksheet.Cells[rowIndex, colIndex++].Value = recordIndex++;
                        worksheet.Cells[rowIndex, colIndex++].Value = supervisor.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.Verifier);
                        worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.Approver);
                        worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.ShiftArrangement);
                        worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.OnBehalfRequest);
                        worksheet.Cells[rowIndex, colIndex].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.DisciplineManager);

                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
                    worksheet.View.FreezePanes(BodyRowIndex, 1);

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        private static string GetSupervisorByActionType(IList<SupervisorModel> supervisors, SupervisorActionType actionType)
        {
            return string.Join(", ", supervisors.Where(x => x.ActionType == (byte)actionType).Select(x => x.NickName));
        }
    }
}