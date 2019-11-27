import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import '../../../libs/monthPicker/monPickr-0.0.3';
import axios from 'axios';
import { parse } from 'qs';

import { axiosError, blockUI, floatThead, unblockUI, resolveClientUrl } from '../../shared/common.functions';
import '../../date';

const timesheetModule = (function ($) {
    function initTooltip() {
        $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });

        $('.tooltip-request').tipsy({
            html: true,
            gravity: 'n',
            fade: true,
            className: 'tipsy-request',
            title: function () {
                const object = $(this);
                let title = '';
                $.ajax({
                    url: resolveClientUrl('request/GetMessageDetailRequest'),
                    async: false,
                    type: 'post',
                    data: {
                        'requesttype': object.attr('data-requesttype'),
                        'fromdate': object.attr('data-fromdate'),
                        'todate': object.attr('data-todate'),
                        'requestedDate': object.attr('data-fromdate'),
                        'fromtime': object.attr('data-fromtime'),
                        'totime': object.attr('data-totime'),
                        'createddate': object.attr('data-createddate'),
                        'fromshiftCode': object.attr('data-fromshift'),
                        'toshiftcode': object.attr('data-toshift'),
                        'totaldays': object.attr('data-totaldays'),
                        'totalmins': object.attr('data-totalmins'),
                        'overtimetype': object.attr('data-ottype'),
                        'isOvertimehours': object.attr('data-isothours'),
                        'actualLateInEarlyOut': object.attr('data-actualLateInEarlyOut'),
                        'keepInViewType': object.attr('data-keepinviewtypeid')
                    },
                    success: function (data) {
                        title = "<table class='table-tooltip'>" +
                            "<tr class='hasborder'>" +
                            "<td class='col1'>Details</td>" +
                            "<td class='col2'>" + data +
                            "</td></tr><tr class='hasborder'><td class='col1'>Created Date</td><td class='col2'>" +
                            object.attr('data-createddate') +
                            "</td></tr><tr><td class='col1'>Remark</td><td class='col2'>" +
                            object.attr('data-remark') +
                            "</td></tr></table>";
                    }
                });

                return title;
            }
        });

        $('.tooltip-overtime').tipsy({
            html: true,
            gravity: 'n',
            fade: true,
            className: 'tipsy-tooltip',
            title: function () {
                const object = $(this);
                return "<div><div class='overtime-tooltip'>" + object.attr('data-fromtime') + "-" + object.attr('data-totime') + "</div></div>";
            }
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
            $('#timesheetDetails').html(response.data);
            initTooltip();
            floatThead('table#data');
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