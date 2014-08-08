'use strict';
/*global _global, fadeOutMessage, showMessage*/

function processRequest(command, eventElement, remark) {
    var requestId = $(eventElement).attr('requestId');
    var roleId = $(eventElement).attr('roleId');
    showMessage('Processing');
    $.post(_global.root + 'ajax/processrequest',
    {
        command: command, requestId: requestId, roleId: roleId, remark: remark
    },
    function (response) {
        fadeOutMessage();
        if (response !== true) {
            alert(response);
            return;
        }
        else {
            window.location.reload();
        }
    });
}

function showLeaveDetail(employeeId, fromDate, toDate, requestId) {
    var leaveDetailContainer = $("#tbldetail");
    if (leaveDetailContainer.length > 0) {
        $.get(_global.root + "Leave/LeaveRequestDetail",
                { "employeeId": employeeId, "fromDate": fromDate, "toDate": toDate, "requestId": requestId })
                        .done(function (content) {
                            leaveDetailContainer.empty();
                            leaveDetailContainer.html(content);
                            $("input").attr("disabled", "disabled");
                            $("input[type='button']").removeAttr("disabled");
                            $("textarea").attr("readonly", "readonly");
                            $("#confirmedDialog textarea").removeAttr('readonly');
                            $("select").attr("disabled", "disabled");
                            $(".tdhalfday").hide();
                        });
    }
}

$(document).ready(function () {
    $("#confirmedDialog").hide();
    var ConfirmedDialog = $("#confirmedDialog");
    $("#tblchangeshift tbody tr").each(function () {
        var selected = $(this).find("input[name='item.ShiftId']").val();
        $('td select', this).val(selected);
    });

    $("#btnProcess").click(function (e) {
        e.preventDefault();
        ConfirmedDialog.dialog({
            height: 210,
            width: 430,
            modal: true,
            buttons: {
                "OK": function () {
                    var remark = $("#reasonToProcess").val();
                    processRequest("Process", "#btnProcess", remark);
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            }
        });
        ConfirmedDialog.dialog('option', 'title', 'Are you sure you want to process?');

    });

    $("#btnVoid").click(function (e) {
        e.preventDefault();
        ConfirmedDialog.dialog({
            height: 210,
            width: 430,
            modal: true,
            buttons: {
                "OK": function () {
                    var remark = $("#reasonToProcess").val();
                    processRequest("Void", "#btnVoid", remark);
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            }
        });
        ConfirmedDialog.dialog('option', 'title', 'Are you sure you want to void?');

    });

    $("#btnReset").click(function (e) {
        e.preventDefault();
        confirm('Are you sure you want to reset?', function () {
            processRequest("Reset", "#btnReset");
        });
    });

    if ($("#LeaveTypeId").val() === "0") {
        showLeaveDetail($("#EmployeeId").val(), $("#FromDate").val(), $("#ToDate").val(), $("#RequestId").val());
    }
});