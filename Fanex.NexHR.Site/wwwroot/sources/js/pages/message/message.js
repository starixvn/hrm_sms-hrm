import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import { initFromDateOnChange, confirm, initDatepicker, autoIndex } from '../../shared/common.functions';
import '../../date';

var deleteMessage = function (messageId) {
    confirm('Are you sure you want to delete this message?', function () {
        $.post(_global.root + 'Message/Delete', { id: messageId }, function () {
            $('#row_' + messageId).remove();
            autoIndex();
        });
    });
};

var detailMessage = function (messageId) {
    $.post(_global.root + 'Message/EditMessageStatus', { id: messageId }, function () {
    }).always(function () {
        window.location.href = _global.root + 'message/detailByEmployee/' + messageId;
    });
};

var init = function () {
    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));

    $('.message-detail').on('click', function () {
        var id = $(this).attr('data-id');
        detailMessage(id);
    });

    $('.icon-delete').on('click', function () {
        var id = $(this).attr('data-id');
        deleteMessage(id);
    });

}

$(document).ready(function () {
    init();
});