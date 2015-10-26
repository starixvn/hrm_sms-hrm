'use strict';
/*global _global, fadeOutMessage, showMessage*/

function processRequest(command, eventElement, remark) {
    var requestId = $(eventElement).attr('requestId');
    var roleId = $(eventElement).attr('roleId');
    showMessage('Processing');
    $.post(_global.root + 'ajax/processrequest',
    { command: command, requestId: requestId, roleId: roleId, remark: remark },
        function (response) {
            fadeOutMessage();
            if (response.Code !== 0) {
                alert(response.Message);
                return;
            }
            else {
                window.location.reload();
            }
        });
}

function bindMenu(element, menu) {
    $(element).contextMenu({
        menu: menu
    },
    function (command, eventElement) {
        var ConfirmedDialog = $("#confirmedDialog");
        switch (command) {
            case 'Process':
                ConfirmedDialog.dialog({
                    height: 210,
                    width: 430,
                    autoOpen: true,
                    modal: true,
                    buttons: {
                        "OK": function () {
                            var remark = $("#reasonToProcess").val();
                            processRequest(command, eventElement, remark);
                        },
                        "Cancel": function () {
                            $(this).dialog("close");
                        }
                    }
                });
                ConfirmedDialog.dialog('option', 'title', 'Are you sure you want to process?');

                break;
            case 'Void':
            case 'VoidBySA':
                ConfirmedDialog.dialog({
                    height: 210,
                    width: 430,
                    autoOpen: true,
                    modal: true,
                    buttons: {
                        "OK": function () {
                            var remark = $("#reasonToProcess").val();
                            processRequest(command, eventElement, remark);
                        },
                        "Cancel": function () {
                            $(this).dialog("close");
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

$(document).ready(function () {
    // menu
    $("#TeamId").multiselect();
    $("#confirmedDialog").hide();

    bindMenu('.hr-actions', 'hrMenu');
    bindMenu('.hr-actions-reset', 'resetMenu');
    bindMenu('.sa-void', 'voidMenu');

    $(".viewhistory").click(function () {
        var url = $(this).attr("id");
        $.get(url, function (data) {
            $('#dialoghistorycontent').html(data);
            $("#dialoghistorycontent").dialog({
                modal: true,
                minHeight: 0,
                width: 1100,
                title: 'Request History'
            });
        });
    });

    $("form").submit(function (e) {
        e.preventDefault();
        var from = new Date($("#FromDate").val());
        var to = new Date($("#ToDate").val());
        if (from > to) {
            alert("From Date must be less than To Date");
            $("#submit").removeClass("disabled");
            $("#submit").val("Submit");
            e.preventDefault();
        }
        else {
            var action = $("#ActionRequest").val();
            var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
            var requestType = $("input[type='radio'][name='RequestType']:checked").val();
            var isLocal = $("#IsLocal").val();
            var isDailyRequest = $("#IsDailyRequest").val();
            var url = _global.root + "request/" + action + "?DateOption=" + $("#Date").val() +
            "&TeamIds=" + teams + "&Employee=" + $("#Employee").val() +
            "&EmployeeId=" + $("#EmployeeId").val() +
            "&FromDate=" + $("#FromDate").val() +
            "&ToDate=" + $("#ToDate").val() +
            "&StatusId=" + $("#statusId").val() +
            "&RequestType=" + requestType +
            "&IsLocal=" + isLocal +
            "&IsDailyRequest=" + isDailyRequest;

            location.href = url;
        }
    });

    $("#btnExcel").click(function () {
        var action = $("#ActionRequest").val() + "WorkingTimeRequest";
        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var requestType = $("input[type='radio'][name='RequestType']:checked").val();
        var isLocal = $("#IsLocal").val();
        var isDailyRequest = $("#IsDailyRequest").val();
        var url = _global.root + "export/" + action + "?DateOption=" + $("#Date").val() +
            "&TeamIds=" + teams + "&Employee=" + $("#Employee").val() +
            "&EmployeeId=" + $("#EmployeeId").val() +
            "&FromDate=" + $("#FromDate").val() +
            "&ToDate=" + $("#ToDate").val() +
            "&StatusId=" + $("#statusId").val() +
            "&RequestType=" + requestType +
            "&IsLocal=" + isLocal +
            "&IsDailyRequest=" + isDailyRequest;
        location.href = url;
    });
});