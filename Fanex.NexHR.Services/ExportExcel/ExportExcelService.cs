namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelService : IExportExcelService
    {
        private const string DefaultSheetName = "sheet1";
        private const int _RowStartBodyLeaveSummary = 4;
        private const int _RowStartBodyWorkingTimeRequest = 3;
        private const int _RowStartBodyWorkingCalendar = 5;

        private const string _LeaveSummaryFilePathExcelTemplate = "~\\ExcelTemplates\\LeaveSummaryTemplate.xls";
        private const string _WorkingTimeRequestFilePathExcelTemplate = "~\\ExcelTemplates\\WorkingTimeRequestTemplate.xls";

        public HSSFWorkbook ExportLeaveSummary(IList<LeaveSummary> leaveSummaryList, int year)
        {
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_LeaveSummaryFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet(DefaultSheetName);
            var sheet = hssfworkbook.CloneSheet(DefaultSheetName, "LeaveSummary");

            //// render hearder
            var rowTakeLeave = sheet.GetRow(2);
            var cellTakeLeave = rowTakeLeave.GetCell(8);
            cellTakeLeave.SetCellValue("Leave in " + year);

            var rowAnnualBalance = sheet.GetRow(2);
            var cellAnnualBalance = rowAnnualBalance.GetCell(20);
            cellAnnualBalance.SetCellValue("Balance Of AL " + year);

            var cellKIVBalance = rowAnnualBalance.GetCell(21);
            cellKIVBalance.SetCellValue("Balance Of KIV " + year);

            //// render body
            var rowIndex = _RowStartBodyLeaveSummary;
            var height = sheet.GetRow(_RowStartBodyLeaveSummary).Height;
            var index = 0;

            foreach (LeaveSummary item in leaveSummaryList)
            {
                var rowCopy = _RowStartBodyWorkingCalendar + (index % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;

                row.GetCell(0).SetCellValue(index + 1);
                row.GetCell(1).SetCellValue(item.Department);
                row.GetCell(2).SetCellValue(item.Section);
                row.GetCell(3).SetCellValue(item.Team);
                row.GetCell(4).SetCellValue(item.FullName);
                row.GetCell(5).SetCellValue(item.Nickname);
                row.GetCell(6).SetCellValue(item.CreditKIV);
                row.GetCell(7).SetCellValue(item.AnnualLeaveCurrentYear);
                row.GetCell(8).SetCellValue(item.Jan);
                row.GetCell(9).SetCellValue(item.Feb);
                row.GetCell(10).SetCellValue(item.Mar);
                row.GetCell(11).SetCellValue(item.Apr);
                row.GetCell(12).SetCellValue(item.May);
                row.GetCell(13).SetCellValue(item.Jun);
                row.GetCell(14).SetCellValue(item.Jul);
                row.GetCell(15).SetCellValue(item.Aug);
                row.GetCell(16).SetCellValue(item.Sep);
                row.GetCell(17).SetCellValue(item.Oct);
                row.GetCell(18).SetCellValue(item.Nov);
                row.GetCell(19).SetCellValue(item.Dec);
                row.GetCell(20).SetCellValue(item.LeaveBalance);
                row.GetCell(21).SetCellValue(item.KIVBalance);

                index++;
                rowIndex++;
            }
            //// end render body

            //// remove sheet & row template
            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        public HSSFWorkbook ExportWorkingTimeRequest(IList<RequestEntity> requestList, string title)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_WorkingTimeRequestFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet(DefaultSheetName);
            var sheet = hssfworkbook.CloneSheet(DefaultSheetName, title);
            var rowIndex = _RowStartBodyWorkingTimeRequest;
            var height = sheet.GetRow(_RowStartBodyWorkingTimeRequest).Height;

            var rowTitle = sheet.GetRow(0);
            var cellTitle = rowTitle.GetCell(0);
            cellTitle.SetCellValue(title);
            foreach (var item in requestList)
            {
                var rowCopy = _RowStartBodyWorkingTimeRequest + (rowIndex % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;
                RenderBodyWorkingTimeRequest(rowIndex, item, row);
                rowIndex++;
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        private static void RenderBodyWorkingTimeRequest(
            int rowIndex,
            RequestEntity item,
            IRow row)
        {
            ICell cellNo = null;
            ICell cellDepartment = null;
            ICell cellSection = null;
            ICell cellTeam = null;
            ICell cellName = null;
            ICell cellNick = null;
            ICell cellRequest = null;
            ICell cellDetails = null;
            ICell cellRemarks = null;
            ICell cellCreateDate = null;
            ICell cellStatus = null;

            cellNo = row.GetCell(0);
            cellDepartment = row.GetCell(1);
            cellSection = row.GetCell(2);
            cellTeam = row.GetCell(3);
            cellName = row.GetCell(4);
            cellNick = row.GetCell(5);
            cellRequest = row.GetCell(6);
            cellDetails = row.GetCell(7);
            cellRemarks = row.GetCell(8);
            cellCreateDate = row.GetCell(9);
            cellStatus = row.GetCell(10);

            cellNo.SetCellValue(rowIndex - _RowStartBodyLeaveSummary + 2);
            cellDepartment.SetCellValue(item.Requester.Team.Section.Department.Code);
            cellSection.SetCellValue(item.Requester.Team.Section.Code);
            cellTeam.SetCellValue(item.Requester.Team.Code);
            cellNick.SetCellValue(item.Requester.NickName);
            cellName.SetCellValue(item.Requester.FullName);
            cellRequest.SetCellValue(item.RequestType.Id == (byte)RequestType.Leave ? item.LeaveType.Name : item.RequestType.Name);
            cellDetails.SetCellValue(RequestService.ToRequestMessage(item, isNoFormat: true));
            cellRemarks.SetCellValue(item.Remark);
            cellCreateDate.SetCellValue(item.CreatedDate);
            cellStatus.SetCellValue(item.Status.Name);
        }

        public static void WriteRowExcelReport(ExcelTemplateWriter writer, int rowIndex, int rowStartBody, object record)
        {
            var rowStart = (rowIndex % 2 != 0) ? rowStartBody : rowStartBody + 1;
            writer.WriteRow(rowStart, record);
        }     
    }
}