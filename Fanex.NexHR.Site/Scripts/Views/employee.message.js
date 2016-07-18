'use strict';
/*exported deleteMessage, detailMessage*/

var init = function () {
    var $effectedDate = $('#EffectedDate'),
		status = $('#IsDisabled').val(),
        $status = $("#status");

    $effectedDate.val($.datepicker.formatDate('d M, yy', new Date()));
    $effectedDate.prop("readonly", true);
    $status.val(status);

    $status.change(function () {
        var value = $(this).val();

        $('#IsDisabled').val(value);
    });
};

var deleteMessage = function (messageId) {
    confirm('Are you sure you want to delete this message?', function () {
        $.post(_global.root + 'Message/DeleteMessageByEmployee', { id: messageId }, function () {
            window.location.href = _global.root + 'Message/ViewMessageOfEmployee';
        });
    });
};

var detailMessage = function (messageId) {
    $.post(_global.root + 'Message/UpdateReadStatusOfMessage', { id: messageId }, function () {
    }).always(function () {
        window.location.href = '/message/detailByEmployee/' + messageId;
    });
};

$(document).ready(function () {
    init();
});