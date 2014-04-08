$(document).ready(function () {
    bindMenu('#forgotInOut', 'forgotCheckInOutMenu');
    bindMenu('#lateInEarlyOut', 'lateInEarlyOutMenu');
    $("#changePasswordDialog").hide();

    if ($("#ChangePassword").val() == "True") {
        $("#changePasswordDialog").show().dialog({
            autoOpen: true,
            height: 210,
            width: 380,
            modal: true,
            buttons: {
                "OK": function () {
                    location.href = _global.root + 'employee/changepwd';
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
                        url: _global.root + 'ajax/UpdatePasswordResetStatus',
                        data: { status: false },
                        success: function (response) {
                        }
                    });
                }
            }
        });
    }
});

function bindMenu(element, menu) {
    $(element).contextMenu({
        menu: menu
    },
	function (command, eventElement) {
	    window.location.href = command;
	});
}