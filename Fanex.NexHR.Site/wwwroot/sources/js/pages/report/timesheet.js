import $ from 'jquery/dist/jquery';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import '../../../libs/monthPicker/monPickr-0.0.3';
import { axiosError, blockUI, floatThead, unblockUI, initFromDateOnChange, confirm, alert } from '../../shared/common.functions';
import '../../date';

function getMessageDetail(requestType, fromDate, toDate, fromTime, toTime, createddate, fromShift, toShift,
    totalDays, totalMins, overtimeType, isOvertimeHours, actualLateInEarlyOut) {
    var result = '';
    blockUI();
    $.ajax({
        url: _global.root + "request/GetMessageDetailRequest",
        async: false,
        type: 'post',
        data: {
            "requesttype": requestType, "fromdate": fromDate, "todate": toDate, "requestedDate": fromDate,
            "fromtime": fromTime, "totime": toTime, "createddate": createddate, "fromshiftCode": fromShift,
            "toshiftcode": toShift, "totaldays": totalDays, "totalmins": totalMins,
            "overtimetype": overtimeType, "isOvertimehours": isOvertimeHours, "actualLateInEarlyOut": actualLateInEarlyOut
        },
        success: function (data) {
            result = data;
        }
    });
    unblockUI();
    return result;
}

function initTooltip() {
    $(".tooltip-request").tipsy({
        html: true,
        gravity: 'n',
        fade: true,
        className: 'tipsy-tooltip',
        title: function () {
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
                object.attr('data-isothours'),
                object.attr('data-actualLateInEarlyOut'));

            return "<div class='tipsy-tooltip'>" +
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

    $('.tooltip-overtime').tipsy({
        html: true,
        gravity: 'n',
        fade: true,
        className: 'tipsy-tooltip',
        title: function () {
            var object = $(this);

            return "<div>" + "<div class='overtime-tooltip'>" +
                object.attr('data-fromtime') + "-" + object.attr('data-totime') + "</div></div>";
        }
    });
}

function query() {
    $.ajax({
        url: _global.root + 'report/TimesheetDetails',
        type: 'POST',
        dataType: 'html',
        data: $('.filter').serialize(),
        success: function (response) {
            $('#timesheetDetails').html(response);
            $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });
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