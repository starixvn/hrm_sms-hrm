'use strict';
/*global _global, _messages*/
/*exported*/

$(document).ready(function () {
    $("#group").multiselect();

    var nickName;
    $("#Supervisor").autocomplete({
        source: function (request, response) {
            var listIgnore = "";
            var supervisors = $("#tblSupervisor").find("tbody tr");
            for (var i = 0; i < supervisors.length; i++) {
                var supervisor = $(supervisors[i]).children().eq(1);
                listIgnore += $(supervisor).attr("data-id") + ",";
            }

            $.ajax({
                url: _global.root + 'employee/getemployees',
                data: {
                    query: $.trim(request.term),
                    $top: 10
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.NickName + ' (' + item.FullName + ')' + ' - ' + item.Team.Name,
                            value: item.EmpId > 0 ? item.EmpId : item.Id
                        };
                    }));
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            nickName = ui.item.label;
            $('#Supervisor').val(nickName.split("-")[0].trim());
            $('#SupervisorTeam').val(nickName.split("-")[1].trim());
            $('#SupervisorId').val(ui.item.value);
        },
        focus: function (event, ui) {
            nickName = ui.item.label;
            $('#Supervisor').val(nickName.split("-")[0].trim());
            $('#SupervisorTeam').val(nickName.split("-")[1].trim());
            $('#SupervisorId').val(ui.item.value);
            event.preventDefault();
        },
        change: function (event, ui) {
            nickName = ui.item.label;
            $('#Supervisor').val(nickName.split("-")[0].trim());
            $('#SupervisorTeam').val(nickName.split("-")[1].trim());
            $('#SupervisorId').val(ui.item.value);
        },
        close: function () {
            //nickName = ui.item.label;
            $('#Supervisor').val(nickName.split("-")[0].trim());
            $('#SupervisorTeam').val(nickName.split("-")[1].trim());
        }
    });

    function addToGrid(supervisorName, supervisorId, supervisorTeam) {
        if (supervisorId.toString().length > 0 && supervisorName.length > 0) {
            var html = $('<tr><td><a href="javascript:;" class="up">Up | </a><a href="javascript:;" class="down">Down | </a>' +
            '<a href="javascript:;" class="moveTop">Top</a>' +
            '</td><td class="align-left" data-id="' + supervisorId + '">' + supervisorName + '</td>' +
            '<td class="align-left">' + supervisorTeam + '</td><td><a href="javascript:;" class="lnkremove">Remove</a></td></tr>');

            var object = html.find(".lnkremove").first().click(function () {
                confirm("Are you sure you want to remove this?", function () {
                    object.parent().parent().remove();
                });
            });

            var isExist = false;

            var supervisors = $("#tblSupervisor tbody tr");
            for (var i = 0; i < supervisors.length; i++) {
                var supervisor = $(supervisors[i]).children().eq(1);
                if (supervisorId === $(supervisor).attr("data-id")) {
                    isExist = true;
                }
            }

            if (!isExist) {
                if ($("#tblSupervisor tbody tr").first().length > 0) {
                    $("#tblSupervisor tbody tr").first().before(html);
                }
                else {
                    $("#tblSupervisor tbody").html(html);
                }
            }
        }
    }

    $("#lknAdd").click(function () {
        var supervisorName = $("#Supervisor").val();
        var supervisorId = $("#SupervisorId").val();
        var supervisorTeam = $("#SupervisorTeam").val();
        if (supervisorId.length > 0 && supervisorName.length > 0) {
            var html = $('<tr><td><a href="javascript:;" class="up">Up | </a><a href="javascript:;" class="down">Down | </a>' +
            '<a href="javascript:;" class="moveTop">Top</a>' +
            '</td><td class="align-left" data-id="' + supervisorId + '">' + supervisorName + '</td>' +
            '<td class="align-left">' + supervisorTeam + '</td><td><a href="javascript:;" class="lnkremove">Remove</a></td></tr>');

            var object = html.find(".lnkremove").first().click(function () {
                confirm("Are you sure you want to remove this?", function () {
                    object.parent().parent().remove();
                });
            });

            var isExist = false;

            var supervisors = $("#tblSupervisor tbody tr");
            for (var i = 0; i < supervisors.length; i++) {
                var supervisor = $(supervisors[i]).children().eq(1);
                if (supervisorId === $(supervisor).attr("data-id")) {
                    isExist = true;
                }
            }

            if (!isExist) {
                if ($("#tblSupervisor tbody tr").first().length > 0) {
                    $("#tblSupervisor tbody tr").first().before(html);
                }
                else {
                    $("#tblSupervisor tbody").html(html);
                }

                $("#Supervisor").val("");
                $("#SupervisorId").val("");
                $("#Supervisor").focus();
            }
        }
    });

    $("#lnkAddEmployeesOfTeam").click(function () {
        var groups = $("#group").val() !== null ? $("#group").val().join() : "";

        $.getJSON(
        _global.root + 'employee/getemployees',
        {
            query: '',
            groupIds: groups,
            enable: null,
            isDelete: null,
            titleId: null
        },
        function (data) {
            for (var i = 0; i < data.length; i++) {
                addToGrid(data[i].NickName + ' (' + data[i].FullName + ')', data[i].EmpId > 0 ? data[i].EmpId : data[i].Id, data[i].Team.Name);
            }
        })
        .error(function () {
            alert(_messages.errorTryAgain);
        });
    });

    $(".lnkremove").on("click", function () {
        var object = $(this);
        confirm("Are you sure you want to remove this?", function () {
            object.parent().parent().remove();
        });
    });

    $("#removeAll").on("click", function () {
        var object = $("#tblSupervisor tbody");
        confirm("Are you sure you want to remove all?", function () {
            object.empty();
        });
    });

    $("#btnSave").on("click", function () {
        var arrangeShift = "";
        var supervisors = $("#tblSupervisor").find("tbody tr");
        var empId = $("#EmployeeId").val();
        var teamId = $("#TeamId").val();
        var ForView = $("#ForView").val();
        var supervisorRole = $('#SupervisorRole').val();

        for (var i = 0; i < supervisors.length; i++) {
            var supervisorTR = $(supervisors[i]).children().eq(1);

            var supervisorId = supervisorTR.attr("data-id");
            arrangeShift += supervisorId + ",";
        }
        if (ForView === "Employee") {
            $.ajax({
                type: 'POST',
                url: _global.root + 'employee/ShiftManager',
                data: {
                    EmployeeId: empId,
                    SupervisorRole: supervisorRole,
                    RoleShiftArrange: arrangeShift
                },
                success: function (data) {
                    if (data.IsSuccessful) {
                        alert("Success!");
                    }
                    else {
                        alert(data.Message);
                    }
                }
            });
        }
        else {
            $.ajax({
                type: 'POST',
                url: _global.root + 'team/ShiftManager',
                data: {
                    TeamId: teamId,
                    SupervisorRole: supervisorRole,
                    RoleShiftArrange: arrangeShift
                },
                success: function (data) {
                    if (data.IsSuccessful) {
                        alert("Success!");
                    }
                    else {
                        alert(data.Message);
                    }
                }
            });
        }
    });

    $(".moveTop, .up, .down").on("click", function () {
        var firstRowInTable = $("#tblSupervisor tbody tr a.moveTop:first");
        var firstRow = $("#tblSupervisor tbody tr:first");
        var currentRow = $(this).parents("tr:first");

        if ($(this).is(".up")) {
            currentRow.insertBefore(currentRow.prev());
        }
        else if ($(this).is(".down")) {
            currentRow.insertAfter(currentRow.next());
        }
        else if ($(this).is(".moveTop")) {
            if ($(this).parent().get(0) === firstRowInTable.parent().get(0)) {
                return false;
            }
            else {
                currentRow.insertBefore(firstRow);
            }
        }
    });
});