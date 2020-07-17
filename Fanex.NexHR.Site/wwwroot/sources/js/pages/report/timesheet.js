import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import '../../../libs/monthPicker/monPickr-0.0.3';
import tippy from 'tippy.js';
import axios from 'axios';
import { parse } from 'qs';

import { axiosError, blockUI, floatThead, unblockUI, resolveClientUrl } from '../../shared/common.functions';
import '../../date';
import { CurrentRequestStatus } from '../../shared/constants';

const timesheetModule = (function ($) {
    function initTooltip() {
        tippy('.tooltip-request', {
            maxWidth: 'none',
            animation: 'fade',
            arrow: true,
            placement: 'auto',
            allowHTML: true,
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
        axios({
            url: resolveClientUrl('report/TimesheetDetails'),
            method: 'POST',
            params: parse($('.filter').serialize()),
            responseType: 'text'
        }).then(response => {
            $('#data').html(response.data);
            floatThead('table#data');
            initTooltip();
        }).catch(axiosError).then(() => unblockUI());
    }

    function initModule() {
        $('#monthPickr').monPickr({
            showDateRange: false,
            today: new Date(),
            minDate: '2010/1/1',
            maxDate: $('#maxDate').val(),
            sortDesc: true,
            afterChangeMonth: function () {
                const month = $('#monthPickr').monPickr("getMonth", "MM");
                const year = $('#monthPickr').monPickr("getMonth", "yyyy");
                $('#Month').val(month);
                $('#Year').val(year);
            }
        });

        $('#btnSubmit').click(function (evt) {
            evt.preventDefault();
            query();
        });

        query();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    timesheetModule.initModule();
});