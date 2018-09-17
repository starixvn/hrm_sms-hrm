'use strict';
/*global _global*/

(function () {
    function initSearchSupervisor() {
        $('#Supervisor').autocomplete({
            source: function (request, response) {
                var listIgnore = '',
                    supervisors = $('#tblSupervisor').find('tbody tr');
                for (var i = 0; i < supervisors.length; i++) {
                    var supervisor = $(supervisors[i]).children().eq(1);
                    listIgnore += $(supervisor).data('id') ? $(supervisor).data('id') + ',' : '';
                }

                $.ajax({
                    url: _global.root + 'employee/GetSupervisors',
                    data: {
                        roleIds: $('#SupervisorRole').val(),
                        query: $.trim(request.term),
                        excludeIds: listIgnore
                    },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.NickName + ' (' + item.FullName + ')' + ' - ' + item.Team.Name,
                                value: item.Id > 0 ? item.Id : item.EmpId
                            };
                        }));
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                var nickName = ui.item.label;
                $('#Supervisor').val(nickName.split('-')[0].trim());
                $('#SupervisorTeam').val(nickName.split('-')[1].trim());
                $('#SupervisorId').val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                var nickName = ui.item.label;
                $('#Supervisor').val(nickName.split('-')[0].trim());
                $('#SupervisorTeam').val(nickName.split('-')[1].trim());
                $('#SupervisorId').val(ui.item.value);
            },
            change: function (event, ui) {
                event.preventDefault();
                if (ui && ui.item) {
                    var nickName = ui.item.label;
                    $('#Supervisor').val(nickName.split('-')[0].trim());
                    $('#SupervisorTeam').val(nickName.split('-')[1].trim());
                    $('#SupervisorId').val(ui.item.value);
                } else {
                    if ($('#Supervisor').val() === '') {
                        $('#SupervisorId').val('');
                    } else {
                        $('#SupervisorId').val('0');
                    }
                }
            }
        });
    }

    function eventAddSupervisor() {
        $('#lknAdd').click(function () {
            var supervisorName = $('#Supervisor').val(),
                supervisorId = $('#SupervisorId').val();
            if (supervisorId.length > 0 && supervisorName.length > 0) {
                var html = $('<tr><td><a href="javascript:;" class="moveUp">Up | </a><a href="javascript:;" class="moveDown">Down | </a>' +
                    '<a href="javascript:;" class="moveTop">Top</a>' +
                    '</td><td class="align-left" data-id="' + supervisorId + '">' + supervisorName +
                    '</td><td><a href="javascript:;" class="lnkremove">Remove</a></td></tr>');

                var object = html.find('.lnkremove').first().click(function () {
                    confirm('Are you sure you want to remove this?', function () {
                        object.parent().parent().remove();
                    });
                });

                var isExist = false,
                    supervisors = $('#tblSupervisor tbody tr');
                for (var i = 0; i < supervisors.length; i++) {
                    var supervisor = $(supervisors[i]).children().eq(1);
                    if (supervisorId === $(supervisor).data('id')) {
                        isExist = true;
                    }
                }

                if (!isExist) {
                    if ($('#tblSupervisor tbody tr').first().length > 0) {
                        $('#tblSupervisor tbody tr').first().before(html);
                    } else {
                        $('#tblSupervisor tbody').html(html);
                    }

                    $('#Supervisor').val('');
                    $('#SupervisorId').val('');
                    $('#Supervisor').focus();
                }
            }
        });
    }

    function eventRemoveSupervisor() {
        $('.lnkremove').on('click', function () {
            var el = $(this);
            confirm('Are you sure you want to remove this?', function () {
                el.closest('tr').remove();
            });
        });
    }

    function eventRemoveAllSupervisor() {
        $('#removeAll').on('click', function () {
            confirm('Are you sure you want to remove all?', function () {
                $('#tblSupervisor tbody').empty();
            });
        });
    }

    function eventMoveUpDownTop() {
        $(".moveTop, .moveUp, .moveDown").live("click", function () {
            var firstRowInTable = $("#tblSupervisor tbody tr a.moveTop:first"),
                firstRow = $("#tblSupervisor tbody tr:first"),
                currentRow = $(this).parents("tr:first");

            if ($(this).is(".moveUp")) {
                currentRow.insertBefore(currentRow.prev());
            } else if ($(this).is(".moveDown")) {
                currentRow.insertAfter(currentRow.next());
            } else if ($(this).is(".moveTop")) {
                if ($(this).parent().get(0) === firstRowInTable.parent().get(0)) {
                    return false;
                } else {
                    currentRow.insertBefore(firstRow);
                }
            }
        });
    }

    function eventSave() {
        $("#btnSave").on("click", function () {
            var supervisors = $("#tblSupervisor").find("tbody tr"),
                empId = $("#EmpId").val(),
                supervisorIds = '';

            for (var i = 0; i < supervisors.length; i++) {
                var supervisorTR = $(supervisors[i]).children().eq(1);
                supervisorIds += supervisorTR.data("id") + ',';
            }

            $.ajax({
                type: 'POST',
                url: _global.root + 'employee/UpdateDisciplineSetting',
                data: {
                    EmployeeId: empId,
                    RoleDisciplines: supervisorIds
                },
                success: function (result) {
                    if (result.IsSuccessful) {
                        alert("Success!");
                    }
                    else {
                        alert(result.Message);
                    }
                }
            });
        });
    }

    $(document).ready(function () {
        initSearchSupervisor();
        eventAddSupervisor();
        eventRemoveSupervisor();
        eventRemoveAllSupervisor();
        eventMoveUpDownTop();
        eventSave();
    });
})();