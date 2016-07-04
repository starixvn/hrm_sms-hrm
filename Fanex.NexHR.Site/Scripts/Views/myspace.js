'use strict';
/*global _global*/

$(document).ready(function () {
    $("#changePasswordDialog").hide();

    if ($("#ChangePassword").val() === "True") {
        $("#changePasswordDialog").show().dialog({
            autoOpen: true,
            height: 210,
            width: 380,
            modal: true,
            buttons: {
                "OK": function () {
                    location.href = _global.root + 'Account/changepwd';
                    $(this).dialog("close");
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            },
            close: function () {
                var status = $("#chk-change").prop("checked");
                if (status) {
                    $.ajax({
                        type: 'POST',
                        url: _global.root + 'Account/UpdatePasswordResetStatus',
                        data: { status: false },
                        success: function () {
                        }
                    });
                }
            }
        });
    }
});