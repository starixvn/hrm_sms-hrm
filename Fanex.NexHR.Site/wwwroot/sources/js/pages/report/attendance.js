import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import tippy from 'tippy.js';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import '../../date';
import '../request/date.filter';
import {
    blockUI, unblockUI, floatThead, initFromDateOnChange, initDatepicker,
    resolveClientUrl, downloadFile, axiosError
} from '../../shared/common.functions';
import { CurrentRequestStatus } from '../../shared/constants';

var sorts = [];

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
        url: resolveClientUrl('report/AttendanceDetails'),
        method: 'POST',
        params: parse($('form.filter').serialize() + '&sort=' + JSON.stringify(sorts)),
        responseType: 'text'
    }).then(response => {
        $('#attendanceDetails').html(response.data);
        floatThead('table#data');
        initTooltip();
    }).catch(axiosError).then(() => unblockUI());
}

function eventSorting() {
    $('th.col-sorter').on('click', function () {
        var $this = $(this).find('div.fa'),
            property = $this.attr('data-property'),
            direction, index;

        index = sorts.map(function (e) { return e.Property; }).indexOf(property);

        if ($this.hasClass('fa-sort-desc')) {
            $this.removeClass('fa-sort-desc').addClass('fa-sort-asc');
            direction = 'asc';
        } else if ($this.hasClass('fa-sort-asc')) {
            $this.removeClass('fa-sort-asc').addClass('fa-sort-desc');
            direction = 'desc';
        } else {
            refreshSorting();
            $this.addClass('fa-sort-desc');
            direction = 'desc';
        }

        if (index !== -1) {
            sorts[index].Direction = direction;
        } else {
            sorts.shift();
            sorts.push({ Property: property, Direction: direction });
        }

        query(false);

    });
}

function refreshSorting() {
    $('th.col-sorter').each(function () {
        $(this).find('div.fa').removeClass('fa-sort-asc').removeClass('fa-sort-desc');
    });
}

$(document).ready(function () {

    eventSorting();

    $('#ExcludeDeletedUsers').on('change', function (event) {
        $(this).val(event.target.checked);
    });

    axios(resolveClientUrl('Department/GetHierarchy'))
        .then(response => {
            $('#Section').multiSelectTreeView({
                data: response.data,
                selectedItems: $('#GroupIds').val()
            });

            query();
        }).catch(axiosError);

    initFromDateOnChange($('#FromDate'), $('#ToDate'));
    initDatepicker();

    $('form.filter #category').on('change', function () {
        $('form.filter #IsLocal').val($(this).val());
    });

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();
        refreshSorting();
        $('#GroupIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $("#btnExcel").click(function () {
        $('#GroupIds').val($('#Section').multiSelectTreeView('getSelectedItems'));

        blockUI();
        axios({
            url: resolveClientUrl('AttendanceReport/ExportExcel'),
            method: 'GET',
            params: parse($('form.filter').serialize() + '&sort=' + JSON.stringify(sorts)),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename=(.+)/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});