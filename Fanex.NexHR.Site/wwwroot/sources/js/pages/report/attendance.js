import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import '../../../libs/monthPicker/monPickr-0.0.3';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import { axiosError, blockUI, floatThead, unblockUI, initFromDateOnChange, confirm, alert, initDatepicker } from '../../shared/common.functions';
import '../../date';

function getMessageDetail(requestType, fromDate, toDate, fromTime, toTime,
    createdDate, fromShift, toShift, totalDays, totalMins, overtimeType, isOvertimeHours, actualLateInEarlyOut) {
    var result = '';
    $.ajax({
        url: _global.root + "request/GetMessageDetailRequest",
        async: false,
        type: 'post',
        data: {
            "requesttype": requestType, "fromdate": fromDate, "todate": toDate, "requestedDate": fromDate,
            "fromtime": fromTime, "totime": toTime,
            "createddate": createdDate, "fromshiftCode": fromShift,
            "toshiftcode": toShift, "totaldays": totalDays, "totalmins": totalMins,
            "overtimetype": overtimeType, "isOvertimehours": isOvertimeHours, "actualLateInEarlyOut": actualLateInEarlyOut,
        },
        success: function (data) {
            result = data;
        }
    });
    return result;
}

function initTooltip() {
    $('.tooltip-request').tipsy({
        html: true,
        gravity: 'n',
        fade: true,
        className: 'table-tipsy',
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
            var status = "";
            switch (parseInt(object.attr('data-currentStatus'))) {
                case 1:
                    status = "Pending";
                    break;
                case 2:
                    status = "Verified";
                    break;
                case 4:
                    status = "Approved";
                    break;
                case 6:
                    status = "Processed";
                    break;
                default:
                    status = "Approved";
            }
            return "<div>" + "<table class='table-tooltip checkinout'>" +
                "<tr class='hasborder'>" +
                "<td class='col1'>Details</td>" +
                "<td class='col2'>" + result +
                "</td></tr><tr class='hasborder'><td class='col1'>Created Date</td><td class='col2'>" +
                object.attr('data-createddate') +
                "</td></tr><tr  class='hasborder'><td class='col1'>Remark</td><td class='col2'>" +
                object.attr('data-remark') + "</td></tr>" +
                "</td></tr><tr><td class='col1'>Status</td><td class='col2'>" + status + "</td></table ></div>";
        }
    });

    $('.tooltip-overtime').tipsy({
        html: true,
        gravity: 'n',
        fade: true,
        className: 'table-tipsy',
        title: function () {
            var object = $(this);

            return "<div>" + "<div class='overtime-tooltip'>" +
                object.attr('data-fromtime') + "-" + object.attr('data-totime') + "</div></div>";
        }
    });

    $('.tooltip-tipsy').tipsy({
        html: true,
        gravity: 'w'
    });
}

function query() {
    blockUI();
    $.ajax({
        url: _global.root + 'report/AttendanceDetails',
        type: 'POST',
        dataType: 'html',
        data: $('.filter').serialize(),
        success: function (response) {
            $('#attendanceDetails').html(response);
            initTooltip();
            floatThead('table#data');
            unblockUI();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            unblockUI();
            alert(textStatus + ': ' + errorThrown);
        }
    });
}

$(document).ready(function () {
    $('#ExcludeDeletedUsers').on('change', function (event) {
        $(this).val(event.target.checked);
    });

    $.ajax({
        url: _global.root + 'Department/GetHierarchy',
        success: function (data) {
            $('#Section').multiSelectTreeView({
                data: data,
                selectedItems: $('#GroupIds').val()
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        }
    });

    initFromDateOnChange($('#FromDate'), $('#ToDate'));
    initDatepicker();

    $('form.filter #category').on('change', function () {
        $('form.filter #IsLocal').val($(this).val());
    });

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();
        $('#GroupIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $("#btnExcel").click(function () {
        $('#GroupIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        location.href = _global.root + "AttendanceReport/ExportExcel?" + $('.filter').serialize();
    });

    query();
});