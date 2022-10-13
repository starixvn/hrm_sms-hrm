import 'bootstrap/dist/js/bootstrap';

import { axiosError, resolveClientUrl, showFullAvatar } from '../shared/common.functions';

$(document).ready(function () {
    if ($('#IsResetPassword').prop('checked')) {
        $('#changePasswordModal').modal('show');

        $('#btnSubmit').on('click', function () {
            location.href = resolveClientUrl('Account/changepwd');
        });

        $('#btnCancel').on('click', function () {
            $('#changePasswordModal').modal('hide');
        });
    }

    $('.fullSizeImage').on('click', function () {
        const codeId = $(this).data('code');
        const nickname = $(this).data('nickname');
        showFullAvatar(codeId, nickname);
    });

    showMoreLessUserOnline();
});

function showMoreLessUserOnline() {
    var x = 10;
    $('.current-online-avatar a:lt(' + x + ')').show();

    if ($('.fullSizeImage').length > 10) {
        $('.fullSizeImage:gt(10)').hide();
        $('.show-more').show();
    }

    $('.show-more').on('click', function () {
        $('.fullSizeImage:gt(10)').toggle();
        $(this).text() === 'View all' ? $(this).text('View less') : $(this).text('View all');
    });
}