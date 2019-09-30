import $ from 'jquery/dist/jquery';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'jquery-contextmenu/dist/jquery.contextMenu';
import toastr from 'toastr';
import axios from 'axios';
import { stringify } from 'qs';

import '../../date';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import './date.filter';
import { axiosError, blockUI, unblockUI, floatingHeader, confirm, initDatepicker, initFromDateOnChange, toastOptions } from '../../shared/common.functions';
import { RequestCommand, RequestType } from '../../shared/constants';

$(function () {
    function processRequest(requestId, requestTypeId, roleId, command, comment, disciplines) {
        blockUI();
        const model = {
            requestId: requestId,
            roleId: roleId,
            command: command,
            comment: comment,
            disciplines: disciplines
        };
        let requestUrl = _global.root;

        switch (requestTypeId) {
            case RequestType.LateIn:
            case RequestType.EarlyOut:
                requestUrl += 'LateInEarlyOut/ProcessRequest';
                break;
            case RequestType.ChangeShift:
                requestUrl += 'ChangeShift/ProcessRequest';
                break;
            default:
                requestUrl += 'request/ProcessRequest';
                break;
        }

        axios({
            url: requestUrl,
            method: 'POST',
            data: stringify({ model }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            unblockUI();
            const data = response.data;
            if (data.IsSuccessful) {
                window.location.reload();
            } else {
                toastr.error(data.Message, '', toastOptions());
            }
        }).catch(axiosError);
    }

    function renderProcessRequestDialog(requestId, requestTypeId, roleId, employeeId, command, title) {
        $('#dialogProcessRequest').dialog({
            title: title,
            height: 'auto',
            width: 'auto',
            modal: true,
            buttons: [{
                text: 'Confirm',
                class: 'btn btn-primary',
                click: function () {
                    let ignoredDisciplines = [],
                        comment = $(this).find('textarea#Comment').val(),
                        isValid = true;

                    $(this).find('table.table tbody tr').each(function () {
                        let $tr = $(this),
                            chkIgnore = $tr.find('.chk-ignore'),
                            chkRecord = $tr.find('.chk-record'),
                            chkWeekendLeaveIgnore = $tr.find('.chk-weekend-ignore'),
                            chkWeekendLeaveRecord = $tr.find('.chk-weekend-record');

                        if (chkIgnore.length === 1) {
                            if (!chkIgnore.prop('checked') && !chkRecord.prop('checked')) {
                                isValid = false;
                            } else {
                                ignoredDisciplines.push({
                                    WorkDate: chkIgnore.data('workdate'),
                                    EmployeeId: employeeId,
                                    DisciplineTypeId: chkIgnore.data('disciplinetypeid'),
                                    IsIgnored: chkIgnore.prop('checked')
                                });
                            }
                        }

                        if (chkWeekendLeaveIgnore.length === 1) {
                            if (!chkWeekendLeaveIgnore.prop('checked') && !chkWeekendLeaveRecord.prop('checked')) {
                                isValid = false;
                            } else {
                                ignoredDisciplines.push({
                                    WorkDate: chkWeekendLeaveIgnore.data('workdate'),
                                    EmployeeId: employeeId,
                                    DisciplineTypeId: 10,
                                    IsIgnored: chkWeekendLeaveIgnore.prop('checked')
                                });
                            }
                        }
                    });

                    if (!isValid) {
                        $(this).find('.validation-summary-valid').show()
                            .html('You must select <b>Violation Ignored</b> or <b>Violation Recorded</b> on each type of request.');
                    } else {
                        $(this).find('.validation-summary-valid').hide();
                        processRequest(requestId, requestTypeId, roleId, command, comment, ignoredDisciplines);
                        $(this).dialog('close');
                    }
                }
            },
            {
                text: 'Cancel',
                class: 'btn btn-default',
                click: function () {
                    $(this).dialog('close');
                }
            }]
        });
    }

    function getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, command, title) {
        const model = {
            RequestId: requestId,
            RoleId: roleId,
            RequestTypeId: requestTypeId,
            EmployeeId: employeeId,
            Command: command
        };

        $.ajax({
            type: 'GET',
            url: _global.root + 'request/PartialViewProcessRequest',
            data: model,
            dataType: 'html',
            success: function (data) {
                $('#dialogProcessRequest').html(data);

                renderProcessRequestDialog(requestId, requestTypeId, roleId, employeeId, command, title);

                let chkIgnoreAll = $('.chk-ignoreAll'),
                    chkRecordAll = $('.chk-recordAll'),
                    chkWeekendLeaveIgnoreAll = $('.chk-weekend-ignoreAll'),
                    chkWeekendLeaveRecordAll = $('.chk-weekend-recordAll'),
                    chkIgnores = $('.chk-ignore'),
                    chkRecords = $('.chk-record'),
                    chkWeekendLeaveIgnores = $('.chk-weekend-ignore'),
                    chkWeekendLeaveRecords = $('.chk-weekend-record');

                chkIgnoreAll.on('click', function () {
                    let checked = $(this).prop('checked');
                    chkRecordAll.prop('checked', !checked);
                    chkIgnores.prop('checked', checked);
                    chkRecords.prop('checked', !checked);
                });

                chkRecordAll.on('click', function () {
                    let checked = $(this).prop('checked');
                    chkIgnoreAll.prop('checked', !checked);
                    chkRecords.prop('checked', checked);
                    chkIgnores.prop('checked', !checked);
                });

                chkWeekendLeaveIgnoreAll.on('click', function () {
                    let checked = $(this).prop('checked');
                    chkWeekendLeaveRecordAll.prop('checked', !checked);
                    chkWeekendLeaveIgnores.prop('checked', checked);
                    chkWeekendLeaveRecords.prop('checked', !checked);
                });

                chkWeekendLeaveRecordAll.on('click', function () {
                    let checked = $(this).prop('checked');
                    chkWeekendLeaveIgnoreAll.prop('checked', !checked);
                    chkWeekendLeaveRecords.prop('checked', checked);
                    chkWeekendLeaveIgnores.prop('checked', !checked);
                });

                $('#dialogProcessRequest').find('table.table tbody tr').each(function () {
                    let chkIgnore = $(this).find('.chk-ignore'),
                        chkRecord = $(this).find('.chk-record'),
                        chkWeekendLeaveIgnore = $(this).find('.chk-weekend-ignore'),
                        chkWeekendLeaveRecord = $(this).find('.chk-weekend-record');

                    chkIgnore.on('click', function () {
                        if ($(this).prop('checked')) {
                            chkRecord.prop('checked', false);
                        }
                    });

                    chkRecord.on('click', function () {
                        if ($(this).prop('checked')) {
                            chkIgnore.prop('checked', false);
                        }
                    });

                    chkWeekendLeaveIgnore.on('click', function () {
                        if ($(this).prop('checked')) {
                            chkWeekendLeaveRecord.prop('checked', false);
                        }
                    });

                    chkWeekendLeaveRecord.on('click', function () {
                        if ($(this).prop('checked')) {
                            chkWeekendLeaveIgnore.prop('checked', false);
                        }
                    });
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    }

    function processCommand(command, options) {
        const eventElement = options.$trigger;
        const requestTypeId = parseInt(eventElement.attr('requestTypeId'), 10);
        const requestId = parseInt(eventElement.attr('requestId'), 10);
        const roleId = parseInt(eventElement.attr('roleId'), 10);
        const employeeId = parseInt(eventElement.attr('employeeId'), 10);
        let title;
        switch (command) {
            case 'Accept':
                title = 'Are you sure you want to verify?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Accept, title);
                break;
            case 'NotAccept':
                title = 'Are you sure you don\'t want to verify?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.NotAccept, title);
                break;
            case 'Approve':
                title = 'Are you sure you want to approve?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Approve, title);
                break;
            case 'Reject':
                title = 'Are you sure you want to reject?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Reject, title);
                break;
            case 'Process':
                title = 'Are you sure you want to process?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Process, title);
                break;
            case 'Void':
                title = 'Are you sure you want to void?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Void, title);
                break;
            case 'VoidBySA':
                title = 'Are you sure you want to void?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.VoidBySA, title);
                break;
            case 'Reset':
                confirm('Are you sure you want to reset?', function () {
                    processRequest(requestId, requestTypeId, roleId, RequestCommand.Reset);
                });
                break;
        }
    }

    function renderIconOk(opt, $itemElement, itemKey, item) {
        $itemElement.html('<span class="processed"><i class="fa fa-check"></i>' + item.name + '</span>');
        return '';
    }

    function renderIconRemove(opt, $itemElement, itemKey, item) {
        $itemElement.html('<span class="void"><i class="fa fa-times"></i>' + item.name + '</span>');
        return '';
    }

    function bindMenu(element, menu) {
        let listItem = {};
        switch (menu) {
            case 'hrMenu':
                listItem = {
                    'Process': { name: 'Process', icon: renderIconOk },
                    'Void': { name: 'Void', icon: renderIconRemove }
                };
                break;
            case 'verifierMenu':
                listItem = {
                    'Accept': { name: 'Verified', icon: renderIconOk },
                    'NotAccept': { name: 'Not Verified', icon: renderIconRemove }
                };
                break;
            case 'approverMenu':
                listItem = {
                    'Approve': { name: 'Approve', icon: renderIconOk },
                    'Reject': { name: 'Reject', icon: renderIconRemove }
                };
                break;
            case 'resetMenu':
                listItem = {
                    'Reset': { name: 'Reset', icon: renderIconRemove }
                };
                break;
            case 'voidMenu':
                listItem = {
                    'VoidBySA': { name: 'Void', icon: renderIconRemove }
                };
                break;
        }

        $.contextMenu({
            selector: element,
            trigger: 'left',
            callback: processCommand,
            items: listItem
        });
    }

    function initEventExportExcel() {
        $('#btnExcel').click(function () {
            const action = $('#ActionName').val() + 'WorkingTimeRequest';
            const url = _global.root + 'ExportRequest/' + action
                + '?DateOption=' + $('#Date').val()
                + '&TeamIds=' + $('#Section').multiSelectTreeView('getSelectedItems')
                + '&Employee=' + $('#Employee').val()
                + '&EmployeeId=' + $('#EmployeeId').val()
                + '&FromDate=' + $('#FromDate').val()
                + '&ToDate=' + $('#ToDate').val()
                + '&StatusId=' + $('#statusId').val()
                + '&RequestType=' + $("input[type='radio'][name='RequestType']:checked").val()
                + '&IsLocal=' + $('#IsLocal').val()
                + '&IsFilterByAffectedDate=' + $('#IsFilterByAffectedDate').val();
            location.href = url;
        });
    }

    $(document).ready(function () {
        initDatepicker();
        $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });

        axios(_global.root + 'Department/GetHierarchy')
            .then(response => {
                $('#Section').multiSelectTreeView({
                    data: response.data,
                    selectedItems: $('#TeamIds').val()
                });
            }).catch(axiosError);

        bindMenu('.hr-actions', 'hrMenu');
        bindMenu('.verifier-actions', 'verifierMenu');
        bindMenu('.approver-actions', 'approverMenu');
        bindMenu('.verifier-actions-reset', 'resetMenu');
        bindMenu('.approver-actions-reset', 'resetMenu');
        bindMenu('.hr-actions-reset', 'resetMenu');
        bindMenu('.sa-void', 'voidMenu');

        $('.viewhistory').click(function () {
            const url = $(this).attr('id');
            $.get(url, function (data) {
                $('#dialoghistorycontent').html(data);
                $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });
                $('#dialoghistorycontent').dialog({
                    modal: true,
                    height: 'auto',
                    width: 1100,
                    title: 'Request History'
                });
            });
        });

        initFromDateOnChange($('#FromDate'), $('#ToDate'));

        $('form').submit(function (e) {
            e.preventDefault();
            const from = new Date($('#FromDate').val());
            const to = new Date($('#ToDate').val());
            if (from > to) {
                alert('From Date must be less than To Date');
                $('#submit').removeClass('disabled');
                $('#submit').val('Submit');
                e.preventDefault();
            } else {
                const url = _global.root + 'request/' + $('#ActionName').val()
                    + '?DateOption=' + $('#Date').val()
                    + '&TeamIds=' + $('#Section').multiSelectTreeView('getSelectedItems')
                    + '&Employee=' + $('#Employee').val()
                    + '&EmployeeId=' + $('#EmployeeId').val()
                    + '&FromDate=' + $('#FromDate').val()
                    + '&ToDate=' + $('#ToDate').val()
                    + '&StatusId=' + $('#statusId').val()
                    + '&RequestType=' + $("#RequestType").val()
                    + '&IsLocal=' + $('#IsLocal').val()
                    + '&IsFilterByAffectedDate=' + $('#IsFilterByAffectedDate').val()
                    + '&leaveType=' + $('#LeaveType').val();

                location.href = url;
            }
        });

        floatingHeader();

        initEventExportExcel();
    });
});