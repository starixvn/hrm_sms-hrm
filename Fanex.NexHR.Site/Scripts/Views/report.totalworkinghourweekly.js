$(document).ready(function () {
    $("#TeamId").multiselect();
    $("#StartDate, #EndDate").attr('readonly', true);

    $("#btnSubmit").click(function (e) {
        e.preventDefault();
        submitForm(false);
    }).focus();

    $("#btnExcel").click(function (e) {
        e.preventDefault();
        submitForm(true);
    });
    
    function submitForm(isExcel) {
        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var employeeId = $("#EmployeeId").val();
        var isLocal = $("#IsLocal").val();
        var startDate = $("#StartDate").val();
        var endDate = $("#EndDate").val();
        var employee = $("#Employee").val();

        var url = _global.root + "report/TotalWorkingHourWeekly?startDate=" + startDate
		+ "&endDate=" + endDate
		+ "&Employee=" + employee
		+ "&TeamIds=" + teams
		+ "&EmployeeId=" + employeeId
		+ "&isLocal=" + isLocal
		+ "&requestData=" + true
		+ "&isExcel=" + isExcel;

        location.href = url;
    }
});