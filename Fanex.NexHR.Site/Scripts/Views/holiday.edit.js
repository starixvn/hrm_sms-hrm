$(document).ready(function () {
    $("#ActualFromDate, #ActualToDate, #FromDate, #ToDate").datepicker({
        changeMonth: true,
        changeYear: true
    });
    var holidayName = $("#HolidayName").val();
    var numday = $("#numDay").val();
    var fromDate = $("#FromDate").val();
    var toDate = $("#ToDate").val();
    var actualFromDate = $("#ActualFromDate").val();

    $("#numDay").change(function () {
        setTheDate();
    });

    $("#numDay").click(function () {
        setTheDate();
    });

    function setTheDate() {
        var numday = $("#numDay").val();
        var fromDate = $("#FromDate").val();
        var dat = Date.parse(fromDate);
        var theDate = new Date(dat);
        theDate.setDate(theDate.getDate() + parseInt(numday) - 1);
        $("#ToDate, #ActualToDate").val(theDate.format("dd mmm, yyyy"));
    }

    $("#FromDate, #ToDate").change(function () {
        var fromDate = $("#FromDate").datepicker('getDate');
        var toDate = $("#ToDate").datepicker('getDate');
        var dayDiff = (toDate - fromDate) / 1000 / 60 / 60 / 24;
        $("#numDay").val(dayDiff);
    });

    $("#btnClear").click(function () {
        $("#HolidayName").val(holidayName);
        $("#ActualFromDate").val(actualFromDate);
        $("#numDay").val(numday);
        $("#FromDate").val(fromDate);
        $("#ToDate").val(toDate);
    });
});