$(document).ready(function () {
    $("#Name").attr("maxlength", "20");
    if($("#IsUsed").val() == "True")
    {
        $("#Name").attr("disabled", "disabled");
        $("#HourTimeIn").attr("disabled", "disabled");
        $("#MuniteTimeIn").attr("disabled", "disabled");

        $("#HourTimeOut").attr("disabled", "disabled");
        $("#MuniteTimeOut").attr("disabled", "disabled");
        $("#IsInAfterDay").attr("disabled", "disabled");
        $("#IsOutAfterDay").attr("disabled", "disabled");
    }

    $('form').bind('submit', function () {
        $(this).find(':input').removeAttr('disabled');
    });

    var successed = $("#Successed").val();
    if (successed == "True") {
        confirm('Successfull!\nPress OK to continue', function () {
            document.location = _global.root + "shift/index";
        });
    }

});