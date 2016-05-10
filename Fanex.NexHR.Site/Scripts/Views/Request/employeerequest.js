'use strict';
/*global _global, fadeOutMessage, showMessage*/

(function () {
    function processRequest(requestId, roleId, command, comment, ignoredDetails) {
        var model = {
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
                    return;
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
                'OK': function () {
                    var ignoredDisciplines = [],
                        comment = $(this).find('textarea#Comment').val();

                    $(this).find(".tabledata tbody tr").each(function () {
                        var checkbox = $('td.ignoredDiscipline input:checkbox');

                        ignoredDisciplines.push({
                            WorkDate: checkbox.data('workdate'),
                            EmployeeId: employeeId,
                            DisciplineTypeId: checkbox.data('disciplinetypeid'),
                            Ignored: checkbox.prop('checked')
                        });
                    });

                    processRequest(requestId, roleId, command, comment, ignoredDisciplines);
                },
                'Cancel': function () {
                    $(this).dialog('close');
                }
            }
        });
    }

    function renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title) {
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
                initDialogProcessRequest(requestId, roleId, employeeId, command, title);
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
            var title,
                requestTypeId = parseInt($(eventElement).attr('requestTypeId')),
                requestId = parseInt($(eventElement).attr('requestId')),
                roleId = parseInt($(eventElement).attr('roleId')),
                employeeId = parseInt($(eventElement).attr('employeeId'));

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