import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import { confirm, autoIndex } from '../../shared/common.functions';

function deleteMemo(memoId) {
    confirm('Are you sure you want to delete this memo?', function () {
        $.post(_global.root + 'Memo/delete', { id: memoId }, function () {
            $('#row_' + memoId).remove();
            autoIndex();
        });
    });
}

function init() {
    $('.icon-delete').on('click', function () {
        var id = $(this).attr('data-id');
        deleteMemo(id);
    });
}

$(document).ready(function () {
    init();
});