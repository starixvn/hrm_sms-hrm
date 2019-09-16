'use strict';
/*global _global*/

function exportExcel() {
    var from = new Date($('#displayFrom').text()),
        to = new Date($('#displayTo').text()),
        fromDate = $.datepicker.formatDate('d M, yy', from),
        toDate = $.datepicker.formatDate('d M, yy', to);

    var url = _global.root + 'ExportCalendar/EmployeeWorkingCalendar?fromDate=' + fromDate
        + '&toDate=' + toDate
        + '&EmployeeId=' + $('#EmployeeId').val()
        + '&SectionIds=' + $('#Section').multiSelectTreeView('getSelectedItems')
        + '&JobTitleIds=' + $('#JobTitleIds').val()
        + '&IsLocal=' + $('#IsLocal').val();
    location.href = url;
}

function getWorkingCalendarDetails(fromDate, toDate) {
    var workingCalendarContainer = $("#workingCalendarDetails");

    $.get(_global.root + 'calendar/WorkingCalendarDetails',
        {
            'EmployeeId': $('#EmployeeId').val(),
            'fromDate': fromDate,
            'toDate': toDate,
            'SectionIds': $('#Section').multiSelectTreeView('getSelectedItems'),
            'JobTitleIds': $('#JobTitleIds').val(),
            'IsLocal': $('#IsLocal').val()
        })
        .done(function (content) {
            workingCalendarContainer.empty();
            workingCalendarContainer.html(content).dataTable({
                'sScrollY': '520px',
                'sScrollX': '100%',
                'sScrollXInner': '100%',
                'bScrollCollapse': true,
                'bFilter': true,
                'bSort': false,
                'bPaginate': true,
                'sPaginationType': 'full_numbers',
                'bInfo': true,
                'bJQueryUI': true,
                'bLengthChange': false,
                'iDisplayLength': 50,
                'aoColumnDefs': [{ 'bSortable': false, 'aTargets': [2, 3, 4, 5, 6, 7, 8] }]
            });
        });
}

$(document).ready(function () {
    var fromDate = $('#OriginalFromDate').val(),
        toDate = $('#OriginalToDate').val();

    $('#displayFrom, #StartDate').text($.datepicker.formatDate('d M, yy', new Date(fromDate)));
    $('#displayTo, #EndDate').text($.datepicker.formatDate('d M, yy', new Date(toDate)));
    $('#StartDate, #EndDate').attr('readonly', 'readonly');

    $.ajax({
        url: _global.root + 'Department/GetHierarchy',
        success: function (data) {
            $('#Section').multiSelectTreeView({
                data: data,
                selectedItems: $('#SectionIds').val()
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        }
    });

    getWorkingCalendarDetails(fromDate, toDate);

    $('#title').multiselect({
        noneSelectedText: 'Select positions',
        selectedText: '# selected',
        selectedList: 0
    });

    $('form.form-filter #title').on('change', function () {
        var jobTitleIds = $(this).val() !== null ? $(this).val().join(',') : '';
        $('#JobTitleIds').val(jobTitleIds);
    });

    $('form.form-filter #category').on('change', function () {
        $('form.form-filter #IsLocal').val($(this).val());
    });

    //Click next link
    $('#next').click(function () {
        destroyDatatable();
        var fromNextDate = $('#FromNextDate').val(),
            toNextDate = $('#ToNextDate').val();

        prepareDataToQuery(fromNextDate, toNextDate);
        getWorkingCalendarDetails(fromNextDate, toNextDate);
    });

    //Click previous link
    $('#prev').click(function () {
        destroyDatatable();
        var fromPrevDate = $('#FromPreviousDate').val(),
            toPrevDate = $('#ToPreviousDate').val();

        prepareDataToQuery(fromPrevDate, toPrevDate);
        getWorkingCalendarDetails(fromPrevDate, toPrevDate);
    });

    //Click submit button
    $('#submit').click(function () {
        destroyDatatable();
        var fromDate = $('#StartDate').val(),
            toDate = $('#EndDate').val();

        prepareDataToQuery(fromDate, toDate);
        getWorkingCalendarDetails(fromDate, toDate);
    });

    function prepareDataToQuery(fromPrevOrNexDate, toPrevOrNexDate) {
        var displayDateFrom = $.datepicker.formatDate('d M, yy', new Date(fromPrevOrNexDate)),
            displayDateTo = $.datepicker.formatDate('d M, yy', new Date(toPrevOrNexDate));

        $('#displayFrom').text(displayDateFrom);
        $('#displayTo').text(displayDateTo);
        $('#StartDate').val(displayDateFrom);
        $('#EndDate').val(displayDateTo);
    }

    function destroyDatatable() {
        $('#workingCalendarDetails').dataTable().fnDestroy();
    }

    $('#exportExcel').click(function () {
        exportExcel();
    });
});