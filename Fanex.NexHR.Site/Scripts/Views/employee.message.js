'use strict';
/*global _global*/
/*exported deleteMessage, detailMessage*/

var init = function () {
    var $effectedDate = $('#EffectedDate'),
        status = $('#IsDisabled').val(),
        $status = $("#status");

    $effectedDate.prop("readonly", true);
    $status.val(status);

    $status.change(function () {
        var value = $(this).val();

        $('#IsDisabled').val(value);
    });

    $('#FromDate').on("change", function () {
        var fromDate = $(this).val();
        $('#ToDate').val(fromDate);
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
    $.post(_global.root + 'Message/EditMessageStatus', { id: messageId }, function () {
    }).always(function () {
        window.location.href = _global.root + 'message/detailByEmployee/' + messageId;
    });
};

$(document).ready(function () {
    init();
});