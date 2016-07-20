'use strict';
/*global _global, floatThead, _messages, dontBlock:true*/

function getMessageDetail(requestType, fromDate, toDate, fromTime, toTime,
    createdDate, fromShift, toShift, totalDays, totalMins, overtimeType, isOvertimeHours) {
    var result = '';
    dontBlock = true;
    $.ajax({
        url: _global.root + "request/GetMessageDetailRequest",
        async: false,
        type: 'post',
        data: {
            "requesttype": requestType, "fromdate": fromDate, "todate": toDate, "requestedDate": fromDate,
            "fromtime": fromTime, "totime": toTime,
            "createddate": createdDate, "fromshiftCode": fromShift,
            "toshiftcode": toShift, "totaldays": totalDays, "totalmins": totalMins,
            "overtimetype": overtimeType, "isOvertimehours": isOvertimeHours
        },
        success: function (data) {
            result = data;
        }
    });
    dontBlock = false;
    return result;
}

function initTooltip() {
    $('.tooltip-request').tooltip({
        track: true,
        delay: 0,
        showURL: false,
        bodyHandler: function () {
            var object = $(this);
            var result = getMessageDetail(
            object.attr('data-requesttype'),
            object.attr('data-fromdate'),
            object.attr('data-todate'),
            object.attr('data-fromtime'),
            object.attr('data-totime'),
            object.attr('data-createddate'),
            object.attr('data-fromshift'),
            object.attr('data-toshift'),
            object.attr('data-totaldays'),
            object.attr('data-totalmins'),
            object.attr('data-ottype'),
            object.attr('data-isothours'));
            return "<div class='tooltip'>" + "<table class='table-tooltip checkinout'>" +
                "<tr class='hasborder'>" +
                "<td class='col1'>Details</td>" +
                "<td class='col2'>" + result +
                "</td></tr><tr class='hasborder'><td class='col1'>Created Date</td><td class='col2'>" +
                object.attr('data-createddate') +
                "</td></tr><tr><td class='col1'>Remark</td><td class='col2'>" +
                object.attr('data-remark') + "</td></tr></table></div>";
        }
    });

    $('.tooltip-overtime').tooltip({
        track: true,
        delay: 0,
        showURL: false,
        bodyHandler: function () {
            var object = $(this);

            return "<div class='tooltip '>" + "<div class='overtime-tooltip'>" +
                object.attr('data-fromtime') + "-" + object.attr('data-totime') + "</div></div>";
        }
    });
}

function query() {
    $.ajax({
        url: _global.root + 'report/AttendanceDetails',
        type: 'POST',
        dataType: 'html',
        data: $('.form-filter').serialize(),
        success: function (response) {
            $('#attendanceDetails').html(response);
            initTooltip();
            floatThead('table#data');
        },
        error: function () {
            alert(_messages.errorTryAgain);
        }
    });
}

$(document).ready(function () {
    $('form.form-filter #group').multiselect();
    $('#FromDate, #ToDate').attr('readonly', 'readonly');

    $('form.form-filter #group').on('change', function () {
        var sectionIds = $(this).val() !== null ? $(this).val().join(',') : '';
        $('#GroupIds').val(sectionIds);
    });

    $('form.form-filter #category').on('change', function () {
        $('form.form-filter #IsLocal').val($(this).val());
    });

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();
        query();
    });

    $("#btnExcel").click(function () {
        location.href = _global.root + "export/Attendance?" + $('.form-filter').serialize();
    });

    query();
});