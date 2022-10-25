import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import '../../date';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import {
    axiosError, blockUI, floatThead, unblockUI, confirm, alert, autoIndex, initDatepicker,
    resolveClientUrl, downloadFile
} from '../../shared/common.functions';

(function () {
    var $disciplineTableBody, $dialogAddOtherDiscipline, $dialogViewOtherDiscipline, $btnAddOtherDiscipline;

    function showSuccessMsg($dialog, msg) {
        $dialog.find('.validation-summary-valid')
            .show().addClass('success').find('ul:first').append('<li>' + msg + '</li>');
    }

    function showErrorMsg($dialog, msg) {
        $dialog.find('.validation-summary-valid')
            .show().addClass('validation-summary-errors').find('ul:first').append(msg);
    }

    function removeMsg($dialog) {
        $dialog.find('.validation-summary-valid')
            .hide().removeClass('success').removeClass('validation-summary-errors').find('ul:first').html("");
    }

    function validateAddOtherDiscipline() {
        var isValid = true,
            errorString = '',
            empId = $dialogAddOtherDiscipline.find('#EmployeeId').val(),
            workDate = $dialogAddOtherDiscipline.find('#WorkDate').datepicker('getDate'),
            disciplineTypeId = $dialogAddOtherDiscipline.find('#DisciplineTypeId').val(),
            score = $dialogAddOtherDiscipline.find('#MinusScore').val(),
            comment = $dialogAddOtherDiscipline.find('#Comment').val();

        if (empId && parseInt(empId, 10) <= 0) {
            errorString += '<li>The Employee field is required.</li>';
        }

        if (workDate === null || workDate === '') {
            errorString += '<li>Discipline Date field is required.</li>';
        }

        if (disciplineTypeId && parseInt(disciplineTypeId, 10) === 0) {
            errorString += '<li>Discipline Type field is required.</li>';
        }

        if (score.trim().length === 0 || parseFloat(score) <= 0) {
            errorString += '<li>Score must be greater than 0.</li>';
        }

        if (comment.trim().length === 0) {
            errorString += '<li>Comment field is required.</li>';
        }

        if (errorString.length > 0) {
            showErrorMsg($dialogAddOtherDiscipline, errorString);
            isValid = false;
        }

        return isValid;
    }

    function initAutocomplete($dialog) {
        var $selectorName = $dialog.find('#Employee'),
            $selectorId = $dialog.find('#EmployeeId');
        $selectorName.autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: resolveClientUrl('discipline/GetMemberToAddOtherDiscipline'),
                    data: {
                        keyword: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.NickName + ' (' + item.FullName + ')',
                                value: item.EmployeeId
                            };
                        }));
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                $selectorName.val(ui.item.label);
                $selectorId.val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                $selectorName.val(ui.item.label);
                $selectorId.val(ui.item.value);
            },
            change: function (event, ui) {
                event.preventDefault();
                if (ui && ui.item) {
                    $selectorName.val(ui.item.label);
                    $selectorId.val(ui.item.value);
                } else {
                    if ($selectorName.val() === '') {
                        $selectorId.val('');
                    } else {
                        $selectorId.val('0');
                    }
                }
            }
        }).autocomplete('widget').addClass('search-employee-autocomplete');
    }

    function initEventForOtherDisciplineDropdown() {
        $dialogAddOtherDiscipline.find('select#DisciplineTypeId').on('change', function () {
            var score = $('option:selected', this).data('score');
            $dialogAddOtherDiscipline.find('#MinusScore').val(score);
        });
    }

    function initAddOtherDisciplineDialog() {
        initAutocomplete($dialogAddOtherDiscipline);
        initDatepicker();
        initEventForOtherDisciplineDropdown();
        $dialogAddOtherDiscipline.find('select#DisciplineTypeId').change();

        $dialogAddOtherDiscipline.dialog({
            modal: true,
            dialogClass: 'other-discipline-dialog',
            title: 'Add Other Discipline',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function () {
                        removeMsg($dialogAddOtherDiscipline);
                        if (validateAddOtherDiscipline()) {
                            $.ajax({
                                type: 'POST',
                                url: resolveClientUrl('discipline/AddOtherDiscipline'),
                                data: $('#frmAddOtherDiscipline').serialize(),
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        showSuccessMsg($dialogAddOtherDiscipline, 'Add other discipline successfully!');
                                        $('#frmAddOtherDiscipline').find('#IsSuccessful').prop('checked', true);
                                    } else {
                                        showErrorMsg($dialogAddOtherDiscipline, '<li>' + result.Message + '</li>');
                                    }
                                }
                            });
                        }
                    }
                },
                {
                    text: 'Cancel',
                    class: 'btn btn-default',
                    click: function () {
                        $(this).dialog('close');
                    }
                }
            ],
            close: function () {
                if ($('#frmAddOtherDiscipline').find('#IsSuccessful').prop('checked')) {
                    query();
                }
            },
            minWidth: 460
        });
    }

    function initEventViewOtherDiscipline() {
        $disciplineTableBody.find('a.other-discipline').on('click', function () {
            var empId = parseInt($(this).data('empid'), 10),
                nickname = $(this).data('nickname');

            if (empId > 0) {
                $.ajax({
                    type: 'GET',
                    url: resolveClientUrl('discipline/ViewOtherDiscipline'),
                    dataType: 'html',
                    data: {
                        employeeId: empId,
                        fromDate: $('form.form-filter #FromDate').datepicker({ dateFormat: 'dd M, yy' }).val(),
                        toDate: $('form.form-filter #ToDate').datepicker({ dateFormat: 'dd M, yy' }).val()
                    },
                    success: function (data) {
                        $dialogViewOtherDiscipline.html(data);
                        initEventForDeleteOtherDiscipline();
                        $dialogViewOtherDiscipline.dialog({
                            modal: true,
                            title: 'Other Discipline - ' + nickname,
                            close: function () {
                                if ($dialogViewOtherDiscipline.find('#IsSuccessful').prop('checked')) {
                                    query();
                                }
                            },
                            minWidth: 500
                        });
                    }
                });
            }
        });
    }

    function initEventForViewDisciplineDetail() {
        $disciplineTableBody.find('a.viewDisciplineDetail').on('click', function () {
            var empId = $(this).data('empid'),
                employee = $(this).data('employee'),
                teamId = $('form.form-filter #TeamId').val(),
                isLocal = $('form.form-filter #IsLocal').val(),
                reasonId = $('form.form-filter #ReasonId').val(),
                fromDate = $('form.form-filter #FromDate').datepicker({ dateFormat: 'dd M, yy' }).val(),
                toDate = $('form.form-filter #ToDate').datepicker({ dateFormat: 'dd M, yy' }).val();

            window.open(resolveClientUrl('discipline/DisciplineDetail?fromDate=' + fromDate + '&toDate=' + toDate
                + '&employeeId=' + empId + '&employee=' + employee + '&teamId=' + teamId
                + '&isLocal=' + isLocal + '&reasonId=' + reasonId + '&fromSummaryPage=true'), '_blank');
        });
    }

    function noInformation(col) {
        return '<tr><td colspan="' + col + '"></td></tr><tr><td colspan="' + col + '">No information available</td></tr>';
    }

    function initEventForDeleteOtherDiscipline() {
        $dialogViewOtherDiscipline.find('a.deleteOtherDiscipline').on('click', function () {
            var empId = $(this).data('empid'),
                workdate = $(this).data('workdate'),
                disciplineTypeId = $(this).data('disciplinetype'),
                rowIndex = $(this).data('row-index');

            confirm('Are you sure you want to delete this other discipline?', function () {
                $.post(resolveClientUrl('discipline/DeleteOtherDiscipline'), {
                    employeeId: empId,
                    workDate: workdate,
                    disciplineTypeId: disciplineTypeId
                }, function () {
                    $dialogViewOtherDiscipline.find('#IsSuccessful').prop('checked', true);
                    $('#row_' + rowIndex).remove();
                    autoIndex();

                    if ($dialogViewOtherDiscipline.find('table.table tbody > tr').length === 0) {
                        $(noInformation(6)).appendTo('#dialogViewOtherDiscipline table.table');
                    }
                });
            });
        });
    }

    function initEventForDateRangeDropdown() {
        $('form.form-filter #DateRange').on('change', function () {
            var dateRange = $('form.form-filter #DateRange').val();

            if (parseInt(dateRange) !== 0) {
                var quarter = dateRange.charAt(0);
                var year = dateRange.substring(1, dateRange.length);

                var fromDate = new Date($('form.form-filter #FromDate').val());
                var toDate = new Date($('form.form-filter #ToDate').val());
                switch (parseInt(quarter)) {
                    case 1: // First Half Year 1H
                        fromDate.setFullYear(parseInt(year) - 1, 11, 1);
                        toDate.setFullYear(parseInt(year), 5, 0);
                        break;

                    case 2: // Second Half Year 2H
                        fromDate.setFullYear(parseInt(year), 5, 1);
                        toDate.setFullYear(parseInt(year), 11, 0);
                        break;

                    default:
                        fromDate.setFullYear(parseInt(year), 0, 1);
                        toDate.setFullYear(parseInt(year), 11, 31);
                        break;
                }

                $('form.form-filter #FromDate').val(formatDate(fromDate));
                $('form.form-filter #ToDate').val(formatDate(toDate));
            }
        });
    }

    function initEventAddOtherDiscipline() {
        $btnAddOtherDiscipline.on('click', function (event) {
            event.preventDefault();

            $.ajax({
                type: 'GET',
                url: resolveClientUrl('discipline/PartialAddOtherDiscipline'),
                dataType: 'html',
                success: function (data) {
                    $dialogAddOtherDiscipline.html(data);
                    initAddOtherDisciplineDialog();
                }
            });
        });
    }

    function formatDate(date) {
        var monthNames = [
            "Jan", "Feb", "Mar",
            "Apr", "May", "Jun", "Jul",
            "Aug", "Sep", "Oct",
            "Nov", "Dec"
        ];

        var day = date.getDate();
        var monthIndex = date.getMonth();
        var year = date.getFullYear();

        return (day < 10 ? "0" + day : day) + ' ' + monthNames[monthIndex] + ', ' + year;
    }

    function query() {
        blockUI();
        $.ajax({
            url: resolveClientUrl('discipline/getDisciplineSummaryGrid'),
            data: $('form.form-filter').serialize(),
            success: function (data) {
                $disciplineTableBody.html(data);
                $('.tooltip-tipsy').tipsy({ html: true, gravity: 'w' });
                initEventViewOtherDiscipline();
                initEventForViewDisciplineDetail();
                unblockUI();
            }
        });
    }

    $(document).ready(function () {
        $disciplineTableBody = $('#disciplineTableBody');
        $dialogAddOtherDiscipline = $('#dialogAddOtherDiscipline');
        $dialogViewOtherDiscipline = $('#dialogViewOtherDiscipline');
        $btnAddOtherDiscipline = $('#btnAddOtherDiscipline');
        initDatepicker();
        initEventForDateRangeDropdown();
        $.ajax({
            url: resolveClientUrl('Section/GetOrgChart'),
            success: function (data) {
                $('#Team').multiSelectTreeView({
                    data: data,
                    isSelectedAll: true,
                    hasSearchBox: true
                });
                $('#TeamId').val($('#Team').multiSelectTreeView('getSelectedItems'));

                query();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });

        $('form.form-filter #FromDate, form.form-filter #ToDate').on('change', function () {
            $('form.form-filter #DateRange').val('0');
        });

        $('form.form-filter #submit').on('click', function (event) {
            $('#TeamId').val($('#Team').multiSelectTreeView('getSelectedItems'));
            query();
        });

        $('form.form-filter #btnExcel').on('click', function (event) {
            event.preventDefault();

            blockUI();
            axios({
                url: resolveClientUrl('ExportDiscipline/ExportDisciplineSummaryReport'),
                method: 'GET',
                params: parse($('form.form-filter').serialize()),
                responseType: 'blob'
            }).then(response => {
                const fileName = response.headers['content-disposition'].match(/filename=(.+)/)[1];
                downloadFile(response.data, fileName);
            }).catch(axiosError).then(() => unblockUI());
        });

        floatThead("table#data");
        initEventAddOtherDiscipline();
    });
})();