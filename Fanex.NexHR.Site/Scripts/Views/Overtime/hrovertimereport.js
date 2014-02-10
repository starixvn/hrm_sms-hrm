$(document).ready(function () {

    //checkForNormalOrFiscalCalendar();
    GetHRMonthlyOvertimeReportDetails();

    $("#fromDate").attr('readonly', 'readonly');
    $("#toDate").attr('readonly', 'readonly');

    $("#calendar").change(function () {
        $('form').submit();
    });

    $("#submitMonthlyReport").on("click", function () {
        GetHRMonthlyOvertimeReportDetails();
    });

    $("#exportExcel").on("click", function () {
        exportOvertimeReport();
    });

});

function exportOvertimeReport() {
    var calendar = $("#calendar").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    var url = null;

    url = _global.root + "export/EmployeeOvertimeRecord?calendar=" + calendar + "&fromDate=" + fromDate + "&toDate=" + toDate;
    location.href = url;
}

function GetHRMonthlyOvertimeReportDetails() {
    var calendar = $("#calendar").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    var HRMonthlyReportDetails = $("#HRMonthlyReportDetails");

    var params = {
        calendar: calendar,
        fromDate: fromDate,
        toDate: toDate
    };

    $.ajax({
        url: _global.root + "overtime/HRMonthlyReportDetails",
        data: params,
        async: false
    }).done(function (response) {
        HRMonthlyReportDetails.empty();
        HRMonthlyReportDetails.html(response);
    });

    $("#hrMonthlyReport").dataTable({
        "sScrollY": "500px",
        "sScrollX": "100%",
        "sScrollXInner": "100%",
        "bScrollCollapse": true,
        "bFilter": true,
        "bSort": false,
        "bPaginate": false,
        "bInfo": true,
        "bJQueryUI": true,
        "fnFooterCallback": function (nRow, aaData, iStart, iEnd, aiDisplay) {
            /* Calculate Total Normal OT Hour */
            var iNormalDay = 0;
            for (var i = iStart; i < iEnd; i++) {
                iNormalDay += aaData[aiDisplay[i]][3] * 1;
            }
            //Off Days
            var iOffDay = 0;
            for (var i = iStart; i < iEnd; i++) {
                iOffDay += aaData[aiDisplay[i]][4] * 1;
            }
            //Public Holiday
            var iPublicHoliday = 0;
            for (var i = iStart; i < iEnd; i++) {
                iPublicHoliday += aaData[aiDisplay[i]][5] * 1;
            }
            /* Modify the footer row to match what we want */
            var nCells = nRow.getElementsByTagName('th');
            nCells[3].innerHTML = parseFloat(iNormalDay).toFixed(2);
            nCells[4].innerHTML = parseFloat(iOffDay).toFixed(2);
            nCells[5].innerHTML = parseFloat(iPublicHoliday).toFixed(2);
        }
    });
}