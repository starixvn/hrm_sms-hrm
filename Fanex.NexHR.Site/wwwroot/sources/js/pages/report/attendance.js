import 'jquery-ui-bundle/jquery-ui';
import tippy from 'tippy.js';
import 'floatthead/dist/jquery.floatThead';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import { blockUI, floatThead, unblockUI, initFromDateOnChange, alert, initDatepicker, resolveClientUrl } from '../../shared/common.functions';
import { CurrentRequestStatus } from '../../shared/constants';
import '../../date';
import '../request/date.filter';

function initTooltip() {
    tippy('.tooltip-request', {
        maxWidth: 'none',
        animation: 'fade',
        arrow: true,
        placement: 'auto',
        allowHTML: true,
        //delay: [null, 200000],
        appendTo: document.getElementById('tippy-request'),
        content: (reference) => {
            const object = $(reference),
                result = $('<div/>').html(object.data('requestdetail')).text(),
                leavetype = object.data('leavetype');
            let status = '';
            switch (parseInt(object.attr('data-currentStatus'))) {
                case CurrentRequestStatus.Pending:
                    status = "Pending";
                    break;
                case CurrentRequestStatus.Verified:
                    status = "Verified";
                    break;
                case CurrentRequestStatus.Approved:
                    status = "Approved";
                    break;
                case CurrentRequestStatus.Processed:
                    status = "Processed";
                    break;
                default:
                    status = "Approved";
            }

            return "<div>" + "<table class='table-tooltip checkinout'>" +
                (leavetype ? "<tr class='hasborder'><td class='col1'>Leave Type</td><td class='col2'>" + leavetype + "</td></tr>" : '') +
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

    tippy('.tooltip-tippy', {
        animation: 'fade',
        arrow: true,
        placement: 'auto'
    });
}

function query() {
    blockUI();
    $.ajax({
        url: resolveClientUrl('report/AttendanceDetails'),
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
        url: resolveClientUrl('Department/GetHierarchy'),
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
        location.href = resolveClientUrl("AttendanceReport/ExportExcel?" + $('.filter').serialize());
    });

    query();
});