import $ from 'jquery/dist/jquery';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'jquery-contextmenu/dist/jquery.contextMenu';
import 'jquery-ui-multiselect-widget/src/jquery.multiselect';

import { floatingHeader, showMessage, fadeOutMessage, confirm } from '../../shared/common.functions';
import '../../date';
import '../../shared/employee.search.in.team';
import './date.filter.new';

$(function () {
    function processRequest(requestId, roleId, command, comment, ignoredDetails) {
        const model = {
            RequestId: requestId,
            RoleId: roleId,
            Command: command,
            Comment: comment,
            IgnoredDisciplineDetails: JSON.stringify(ignoredDetails)
        };

        $.ajax({
            type: 'POST',
            url: _global.root + 'request/ProcessRequest',
            data: model,
            dataType: 'json',
            beforeSend: function () {
                showMessage('Processing');
            },
            success: function (response) {
                if (response.Code !== 0) {
                    alert(response.Message);
                } else {
                    window.location.reload();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            },
            complete: function () {
                fadeOutMessage();
            }
        });
    }

    function initDialogProcessRequest(requestId, roleId, employeeId, command, title) {
        $('#dialogProcessRequest').dialog({
            title: title,
            height: 'auto',
            width: 'auto',
            modal: true,
            buttons: {
                'Confirm': function () {
                    let ignoredDisciplines = [],
                        comment = $(this).find('textarea#Comment').val(),
                        isValid = true;

                    $(this).find('table.table tbody tr').each(function () {
                        let chkIgnored = $(this).find('td.violationIgnored input:radio'),
                            chkNotIgnored = $(this).find('td.violationRecorded input:radio');

                        if (chkIgnored.length === 1) {
                            if (!chkIgnored.prop('checked') && !chkNotIgnored.prop('checked')) {
                                isValid = false;
                            } else {
                                ignoredDisciplines.push({
                                    WorkDate: chkIgnored.data('workdate'),
                                    EmployeeId: employeeId,
                                    DisciplineTypeId: chkIgnored.data('disciplinetypeid'),
                                    IsIgnored: chkIgnored.prop('checked')
                                });
                            }
                        }
                    });

                    if (!isValid) {
                        $(this).find('.validation-summary-valid').show()
                            .html('You must select <b>Violation Ignored</b> or <b>Violation Recorded</b> on each type of request.');
                    } else {
                        $(this).find('.validation-summary-valid').hide();
                        processRequest(requestId, roleId, command, comment, ignoredDisciplines);
                        $(this).dialog('close');
                    }
                },
                'Cancel': function () {
                    $(this).dialog('close');
                }
            }
        });
    }

    function renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title) {
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

                initDialogProcessRequest(requestId, roleId, employeeId, command, title);

                $('#dialogProcessRequest').find('table.table tbody tr').each(function () {
                    let chkIgnored = $(this).find('td.violationIgnored input:radio'),
                        chkNotIgnored = $(this).find('td.violationRecorded input:radio');

                    chkIgnored.on('click', function () {
                        if ($(this).prop('checked')) {
                            chkNotIgnored.prop('checked', false);
                        }
                    });

                    chkNotIgnored.on('click', function () {
                        if ($(this).prop('checked')) {
                            chkIgnored.prop('checked', false);
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
                renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title);
                break;
            case 'NotAccept':
                title = 'Are you sure you dont want to verify?';
                renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title);
                break;
            case 'Approve':
                title = 'Are you sure you want to approve?';
                renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title);
                break;
            case 'Reject':
                title = 'Are you sure you want to reject?';
                renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title);
                break;
            case 'Process':
                title = 'Are you sure you want to process?';
                renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title);
                break;
            case 'Void':
            case 'VoidBySA':
                title = 'Are you sure you want to void?';
                renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title);
                break;
            case 'Reset':
                confirm('Are you sure you want to reset?', function () {
                    processRequest(requestId, roleId, command);
                });
                break;
        }
    }

    function renderIconOk(opt, $itemElement, itemKey, item) {
        $itemElement.html('<span class="processed"><i class="glyphicon glyphicon-ok"></i>' + item.name + '</span>');
        return '';
    }

    function renderIconRemove(opt, $itemElement, itemKey, item) {
        $itemElement.html('<span class="void"><i class="glyphicon glyphicon-remove"></i>' + item.name + '</span>');
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
            const teams = $('#TeamId').val() !== null ? $('#TeamId').val().join() : '';
            const url = _global.root + 'ExportRequest/' + action
                + '?DateOption=' + $('#Date').val()
                + '&TeamIds=' + teams
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
        $('.tooltip-tipsy').tipsy({ html: true, gravity: 'e' });

        $('#TeamId').multiselect({
            minWidth: 180,
            checkAllText: 'Check',
            uncheckAllText: 'Uncheck'
        });

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
                $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoNS });
                $('#dialoghistorycontent').dialog({
                    modal: true,
                    height: 'auto',
                    width: 1100,
                    title: 'Request History'
                });
            });
        });

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
                const teams = $('#TeamId').val() !== null ? $('#TeamId').val().join() : '';
                const url = _global.root + 'request/' + $('#ActionName').val()
                    + '?DateOption=' + $('#Date').val()
                    + '&TeamIds=' + teams
                    + '&Employee=' + $('#Employee').val()
                    + '&EmployeeId=' + $('#EmployeeId').val()
                    + '&FromDate=' + $('#FromDate').val()
                    + '&ToDate=' + $('#ToDate').val()
                    + '&StatusId=' + $('#statusId').val()
                    + '&RequestType=' + $("input[type='radio'][name='RequestType']:checked").val()
                    + '&IsLocal=' + $('#IsLocal').val()
                    + '&IsFilterByAffectedDate=' + $('#IsFilterByAffectedDate').val();

                location.href = url;
            }
        });

        floatingHeader();

        initEventExportExcel();
    });
});