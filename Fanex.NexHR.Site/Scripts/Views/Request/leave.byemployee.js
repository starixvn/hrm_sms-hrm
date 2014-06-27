var LeaveRequestView = function ($) {
    return {
        dialogReason: null,
        requestId: null,

        init: function () {
            LeaveRequestView.dialogReason = $("#dialog-reason");
            LeaveRequestView.bindMenu('.Pending', 'pendingMenu');
            LeaveRequestView.bindMenu('.reedit', 'reeditMenu');

            LeaveRequestView.dialogReason.dialog({
                autoOpen: false,
                height: 220,
                width: 270,
                modal: true,
                buttons: {
                    "Ok": function () {
                        LeaveRequestView.sendReasonReeditToSuperVisor();
                        $(this).dialog("close");
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                },
                close: function () {
                    $("#txtReason").val("");
                }
            });

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

            $('.viewdetail').tooltip({
                track: true,
                delay: 0,
                showURL: false,
                bodyHandler: function () {
                    var object = $(this);

                    return "<div class='tooltip'>"
                        + "<table class='table-tooltip checkinout'>"
                        + "<tr class='hasborder'>"
                        + "<td class='col11'>" + "<span class='bold'>" + object.attr("data-verifier") + "</span>" + " (Verifier)</td>"
                        + "<td class='col2'>"
                        + "<span class='bold'>" + object.attr("data-verifier-status") + "</span>"
                        + (object.attr("data-verified-date").length > 0 ? (" (" + object.attr("data-verified-date") + ")") : "")
                        + "</td>"
                        + "</tr>"
                        + "<tr class='hasborder'>"
                        + "<td class='col11'>" + "<span class='bold'>" + object.attr("data-approver") + "</span>"  + " (Approver)</td>"
                        + "<td class='col2'>"
                        + "<span class='bold'>" + object.attr("data-approver-status") + "</span>"
                        + (object.attr("data-approved-date").length > 0 ? (" (" + object.attr("data-approved-date") + ")") : "")
                        + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td class='col11'>" + "<span class='bold'>" + object.attr("data-hr") + "</span>" + " (HR)</td>"
                        + "<td class='col2'>"
                        + "<span class='bold'>" + object.attr("data-hr-status") + "</span>"
                        + (object.attr("data-processed-date").length > 0 ? (" (" + object.attr("data-processed-date") + ")") : "")
                        + "</td>"
                        + "</tr>"
                        + "</table></div>";
                }
            });
        },
        cancelRequest: function (id) {
            confirm('Are you sure you want to cancel this request?', function () {
                $.post(_global.root + 'request/deleteworkingtimerequest' + "/" + id, function () {
                    $('#row' + id).remove();
                    autoIndex();
                });
            });
        },
        processRequest: function (command, eventElement) {
            var requestId = $(eventElement).attr('requestId');
            var requestType = $("#RequestType").val();

            switch (command) {
                case 'Edit':
                    location.href = _global.root + requestType + "/Update?requestId=" + requestId;
                    break;
                case 'Reedit':
                    LeaveRequestView.requestId = requestId;
                    LeaveRequestView.reEditRequest(requestType, requestId);
                    break;
                case 'Cancel':
                    LeaveRequestView.cancelRequest(requestId);
                    break;
            }
        },
        bindMenu: function (element, menu) {
            $(element).contextMenu({
                menu: menu
            }, function (command, eventElement) {
                LeaveRequestView.processRequest(command, eventElement);
            });
        },
        reEditRequest: function () {
            confirm('Are you sure you want to reedit this request?', function () {
                LeaveRequestView.dialogReason.dialog("open");

            });
        },
        sendReasonReeditToSuperVisor: function () {
            var requestType = $("#RequestType").val();
            var reason = $("#txtReason").val();
            $.ajax({
                type: 'POST',
                dataType: "json",
                url: '/Leave/SendReasonReeditToSuperVisor',
                data: { requestId: LeaveRequestView.requestId, reason: reason },
                success: function (response) {
                    if (response.errorCode === 0) {
                        location.href = _global.root + requestType + "/Update?requestId=" + LeaveRequestView.requestId;
                    }
                    else {
                        alert(response.errorMessage);
                    }
                }
            });
        }
    };
} (jQuery);

$(document).ready(function () {
    LeaveRequestView.init();
});

