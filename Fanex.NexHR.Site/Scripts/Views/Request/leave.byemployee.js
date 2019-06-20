'use strict';
/*global _global, autoIndex*/

var LeaveRequestView = (function ($) {
    return {
        dialogReason: null,
        requestId: null,

        init: function () {
            LeaveRequestView.dialogReason = $("#dialog-reason");
            LeaveRequestView.bindMenu('.Pending', 'pendingMenu');

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

                    return "<div class='tooltip'>" +
                        "<table class='table-tooltip checkinout'>" +
                        "<tr class='hasborder'>" + 
                        "<td class='col11'>" + "<span class='bold'>" + object.attr("data-verifier") + "</span>" + " (Verifier)</td>" + 
                        "<td class='col2'>" + 
                        "<span class='bold'>" + object.attr("data-verifier-status") + "</span>" + 
                        (object.attr("data-verified-date").length > 0 ? (" (" + object.attr("data-verified-date") + ")") : "") + 
                        "</td>" + 
                        "</tr>" + 
                        "<tr class='hasborder'>" + 
                        "<td class='col11'>" + "<span class='bold'>" + object.attr("data-approver") + "</span>"  + " (Approver)</td>" + 
                        "<td class='col2'>" + 
                        "<span class='bold'>" + object.attr("data-approver-status") + "</span>" +
                        (object.attr("data-approved-date").length > 0 ? (" (" + object.attr("data-approved-date") + ")") : "") +
                        "</td>" + 
                        "</tr>" + 
                        "<tr>" + 
                        "<td class='col11'>" + "<span class='bold'>" + object.attr("data-hr") + "</span>" + " (HR)</td>" + 
                        "<td class='col2'>" + 
                        "<span class='bold'>" + object.attr("data-hr-status") + "</span>" + 
                        (object.attr("data-processed-date").length > 0 ? (" (" + object.attr("data-processed-date") + ")") : "") + 
                        "</td>" + 
                        "</tr>" + 
                        "</table></div>";
                }
            });
        },
        cancelRequest: function (id) {
            confirm('Are you sure you want to cancel this request?', function () {
                $.post(_global.root + 'leave/delete' + "/" + id, function (response) {
                    if (response.IsSuccessful) {
                        $('#row' + id).remove();
                        autoIndex();
                    }
                    else {
                        alert(response.Message);
                        return;
                    }
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
        }
    };
} (jQuery));

$(document).ready(function () {
    LeaveRequestView.init();
});

