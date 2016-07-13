'use strict';
/*global _global, _messages, dontBlock:true, floatThead*/

function getMessageDetail(requestType, fromDate, toDate, fromTime, toTime, createddate, fromShift, toShift,
    totalDays, totalMins, overtimeType, isOvertimeHours) {
    var result = '';
    dontBlock = true;
    $.ajax({
        url: _global.root + "request/GetMessageDetailRequest",
        async: false,
        type: 'post',
        data: {
            "requesttype": requestType, "fromdate": fromDate, "todate": toDate, "requestedDate": fromDate,
            "fromtime": fromTime, "totime": toTime, "createddate": createddate, "fromshiftCode": fromShift,
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

            return "<div class='tooltip'>" +
                "<table class='table-tooltip checkinout'>" +
                "<tr class='hasborder'>" +
                "<td class='col1'>Details</td>" +
                "<td class='col2'>" + result +
                "</td></tr><tr class='hasborder'><td class='col1'>Created Date</td><td class='col2'>" +
                object.attr('data-createddate') +
                "</td></tr><tr><td class='col1'>Remark</td><td class='col2'>" +
                object.attr('data-remark') +
                "</td></tr></table></div>";
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
        url: _global.root + 'report/TimesheetDetails',
        type: 'POST',
        dataType: 'html',
        data: $('.form-filter').serialize(),
        success: function (response) {
            $('#timesheetDetails').html(response);
            initTooltip();
            floatThead('table#data');
        },
        error: function () {
            alert(_messages.errorTryAgain);
        }
    });
}

$(document).ready(function () {
    $('#monthPickr').monPickr({
        showDateRange: false,
        today: new Date(),
        minDate: '2010/1/1',
        maxDate: $('#maxDate').val(),
        sortDesc: true,
        afterChangeMonth: function () {
            var Month = $('#monthPickr').monPickr("getMonth", "MM");
            var Year = $('#monthPickr').monPickr("getMonth", "yyyy");
            $('#Month').val(Month);
            $('#Year').val(Year);
        }
    });

    query();

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();
        query();
    });
});