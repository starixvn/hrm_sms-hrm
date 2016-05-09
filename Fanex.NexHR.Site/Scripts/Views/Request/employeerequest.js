'use strict';
/*global _global, fadeOutMessage, showMessage*/

(function () {
    function processRequest(command, eventElement, remark) {
        var requestId = $(eventElement).attr('requestId'),
            roleId = $(eventElement).attr('roleId');
        showMessage('Processing');
        $.post(_global.root + 'ajax/processrequest',
        { command: command, requestId: requestId, roleId: roleId, remark: remark },
            function (response) {
                fadeOutMessage();
                if (response.Code !== 0) {
                    alert(response.Message);
                    return;
                } else {
                    window.location.reload();
                }
            });
    }

    function initDialogProcessRequest() {
        $('#dialogProcessRequest').dialog({
            height: 'auto',
            width: 'auto',
            modal: true,
            buttons: {
                'OK': function () {
                    var requestDetails = [],
                        requestId = $(this).find('input#RequestId').val(),
                        employeeId = $(this).find('input#EmployeeId').val(),
                        action = $(this).find('input#Action').val();

                    $(".tabledata tbody tr").each(function () {
                        var checkbox = $('td.ignoredDiscipline input:checkbox');

                        requestDetails.push({
                            WorkDate: checkbox.data('workdate'),
                            EmployeeId: employeeId,
                            DisciplineTypeId: checkbox.data('disciplinetypeid'),
                            Ignored: checkbox.prop('checked')
                        });
                    });

                    console.log(requestDetails);
                },
                'Cancel': function () {
                    $(this).dialog('close');
                }
            }
        });

        $('#dialogProcessRequest').dialog('option', 'title', 'Are you sure you want to approve?');
    }

    function renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command) {
        var model = {
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
                initDialogProcessRequest();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    }

    function bindMenu(element, menu) {
        $(element).contextMenu({
            menu: menu
        },
        function (command, eventElement) {
            var ConfirmedDialog = $('#confirmedDialog'),
                height = 'auto',
                width = 430;

            var requestTypeId = parseInt($(eventElement).attr('requestTypeId')),
                requestId = parseInt($(eventElement).attr('requestId')),
                roleId = parseInt($(eventElement).attr('roleId')),
                employeeId = parseInt($(eventElement).attr('employeeId'));

            switch (command) {
                case 'Accept':
                    ConfirmedDialog.dialog({
                        height: height,
                        width: width,
                        modal: true,
                        buttons: {
                            'OK': function () {
                                var remark = $('#reasonToProcess').val();
                                processRequest(command, eventElement, remark);
                            },
                            'Cancel': function () {
                                $(this).dialog('close');
                            }
                        }
                    });
                    ConfirmedDialog.dialog('option', 'title', 'Are you sure you want to verify?');
                    break;
                case 'NotAccept':
                    ConfirmedDialog.dialog({
                        height: height,
                        width: width,
                        modal: true,
                        buttons: {
                            'OK': function () {
                                var remark = $('#reasonToProcess').val();
                                processRequest(command, eventElement, remark);
                            },
                            'Cancel': function () {
                                $(this).dialog('close');
                            }
                        }
                    });
                    ConfirmedDialog.dialog('option', 'title', 'Are you sure you dont want to verify?');
                    break;
                case 'Approve':
                    renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command);
                    break;
                case 'Reject':
                    ConfirmedDialog.dialog({
                        height: height,
                        width: width,
                        modal: true,
                        buttons: {
                            'OK': function () {
                                var remark = $('#reasonToProcess').val();
                                processRequest(command, eventElement, remark);
                            },
                            'Cancel': function () {
                                $(this).dialog('close');
                            }
                        }
                    });
                    ConfirmedDialog.dialog('option', 'title', 'Are you sure you want to reject?');
                    break;
                case 'Process':
                    ConfirmedDialog.dialog({
                        height: height,
                        width: width,
                        autoOpen: true,
                        modal: true,
                        buttons: {
                            'OK': function () {
                                var remark = $('#reasonToProcess').val();
                                processRequest(command, eventElement, remark);
                            },
                            'Cancel': function () {
                                $(this).dialog('close');
                            }
                        }
                    });
                    ConfirmedDialog.dialog('option', 'title', 'Are you sure you want to process?');
                    break;
                case 'Void':
                case 'VoidBySA':
                    ConfirmedDialog.dialog({
                        height: height,
                        width: width,
                        autoOpen: true,
                        modal: true,
                        buttons: {
                            'OK': function () {
                                var remark = $('#reasonToProcess').val();
                                processRequest(command, eventElement, remark);
                            },
                            'Cancel': function () {
                                $(this).dialog('close');
                            }
                        }
                    });
                    ConfirmedDialog.dialog('option', 'title', 'Are you sure you want to void?');
                    break;
                case 'Reset':
                    confirm('Are you sure you want to reset?', function () {
                        processRequest(command, eventElement);
                    });
                    break;
            }
        });
    }

    function initEventExportExcel() {
        $('#btnExcel').click(function () {
            var action = $('#ActionName').val() + 'WorkingTimeRequest',
                teams = $('#TeamId').val() !== null ? $('#TeamId').val().join() : '',
                requestType = $("input[type='radio'][name='RequestType']:checked").val(),
                isLocal = $('#IsLocal').val(),
                isFilterByAffectedDate = $('#IsFilterByAffectedDate').val();
            var url = _global.root + 'export/' + action + '?DateOption=' + $('#Date').val() +
                '&TeamIds=' + teams + '&Employee=' + $('#Employee').val() +
                '&EmployeeId=' + $('#EmployeeId').val() +
                '&FromDate=' + $('#FromDate').val() +
                '&ToDate=' + $('#ToDate').val() +
                '&StatusId=' + $('#statusId').val() +
                '&RequestType=' + requestType +
                '&IsLocal=' + isLocal +
                '&IsFilterByAffectedDate=' + isFilterByAffectedDate;
            location.href = url;
        });
    }

    $(document).ready(function () {
        $('#TeamId').multiselect();
        $('#confirmedDialog').hide();

        bindMenu('.hr-actions', 'hrMenu');
        bindMenu('.verifier-actions', 'verifierMenu');
        bindMenu('.approver-actions', 'approverMenu');
        bindMenu('.verifier-actions-reset', 'resetMenu');
        bindMenu('.approver-actions-reset', 'resetMenu');
        bindMenu('.hr-actions-reset', 'resetMenu');
        bindMenu('.sa-void', 'voidMenu');

        $('.viewhistory').click(function () {
            var url = $(this).attr('id');
            $.get(url, function (data) {
                $('#dialoghistorycontent').html(data);
                $('#dialoghistorycontent').dialog({
                    modal: true,
                    minHeight: 0,
                    width: 1100,
                    title: 'Request History'
                });
            });
        });

        $('form').submit(function (e) {
            e.preventDefault();
            var from = new Date($('#FromDate').val()),
                to = new Date($('#ToDate').val());
            if (from > to) {
                alert('From Date must be less than To Date');
                $('#submit').removeClass('disabled');
                $('#submit').val('Submit');
                e.preventDefault();
            } else {
                var action = $('#ActionName').val(),
                    teams = $('#TeamId').val() !== null ? $('#TeamId').val().join() : '',
                    requestType = $("input[type='radio'][name='RequestType']:checked").val(),
                    isLocal = $('#IsLocal').val(),
                    isFilterByAffectedDate = $('#IsFilterByAffectedDate').val();
                var url = _global.root + 'request/' + action + '?DateOption=' + $('#Date').val() +
                '&TeamIds=' + teams + '&Employee=' + $('#Employee').val() +
                '&EmployeeId=' + $('#EmployeeId').val() +
                '&FromDate=' + $('#FromDate').val() +
                '&ToDate=' + $('#ToDate').val() +
                '&StatusId=' + $('#statusId').val() +
                '&RequestType=' + requestType +
                '&IsLocal=' + isLocal +
                '&IsFilterByAffectedDate=' + isFilterByAffectedDate;

                location.href = url;
            }
        });

        initEventExportExcel();
    });
})();