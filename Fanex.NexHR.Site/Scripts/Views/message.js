'use strict';
/*global _global,autoIndex*/
/*exported deleteMessage*/

var deleteMessage = function (messageId) {
    confirm('Are you sure you want to delete this message?', function () {
        $.post(_global.root + 'Message/Delete', { id: messageId }, function () {
            $('#row_' + messageId).remove();
            autoIndex();
        });
    });
};

var init = function () {
    var $status = $('#status');

    var fromDateModel = $('#FromDate').val(),
		toDateModel = $('#ToDate').val(),
		status = $('#IsDisabled').val(),
		toDate = new Date(),
		fromDate = new Date();
    fromDate.setMonth(fromDate.getMonth() - 3);
    var $fromDate = $("#FromDate");
    var $toDate = $("#ToDate");

    $fromDate.inputReadonly();
    $toDate.inputReadonly();

    if (!fromDateModel) {
        $fromDate.val($.datepicker.formatDate('d M, yy', fromDate));
    }
    if (!toDateModel) {
        $toDate.val($.datepicker.formatDate('d M, yy', toDate));
    }

    $status.val(status);

    $status.change(function () {
        var value = $(this).val();

        $('#IsDisabled').val(value);
    });
};

$(document).ready(function () {
    init();
});