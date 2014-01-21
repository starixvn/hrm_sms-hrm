$(document).ready(function () {

    //show or hide Defaul Calendar tag
    if ($('#IsDefaultSchedule').is(':checked')) {
        $("#dfsCalendar").show();
    } else {
        $("#dfsCalendar").hide();
    }
    $('#IsDefaultSchedule').click(function () {
        $("#dfsCalendar").toggle();
    });

    $("#generate").on("click", function () {
        $.ajax({
            url: _global.root + 'api/employee/GetNewEmployeeCode',
            success: function (data) {
                $("#Code").val("X" + data);
            }
        });
    });
});
