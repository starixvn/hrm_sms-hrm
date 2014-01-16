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
});
