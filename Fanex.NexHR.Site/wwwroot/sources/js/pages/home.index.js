import 'bootstrap/dist/js/bootstrap';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl, showFullAvatar } from '../shared/common.functions';

$(document).ready(function () {
    if ($('#IsResetPassword').prop('checked')) {
        $('#changePasswordModal').modal('show');

        $('#btnSubmit').on('click', function () {
            location.href = resolveClientUrl('Account/changepwd');
        });

        $('#btnCancel').on('click', function () {
            $('#changePasswordModal').modal('hide');
            const status = $('#ckb-dontshowagain').prop('checked');
            if (status) {
                axios({
                    url: resolveClientUrl('Account/EditPasswordResetStatus'),
                    method: 'POST',
                    data: stringify({ status: false }),
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'Content-Type': 'application/x-www-form-urlencoded'
                    }
                }).catch(axiosError);
            }
        });
    }

    $('.fullSizeAvatar').on('click', function () {
        const codeId = $(this).data('code');
        const nickname = $(this).data('nickname');
        showFullAvatar(codeId, nickname);
    });

    showMoreLessUserOnline();
});

function showMoreLessUserOnline() {
    var x = 10;
    $('.current-online-avatar a:lt(' + x + ')').show();

    if ($('.fullSizeAvatar').length > 10) {
        $('.fullSizeAvatar:gt(10)').hide();
        $('.show-more').show();
    }

    $('.show-more').on('click', function () {
        $('.fullSizeAvatar:gt(10)').toggle();
        $(this).text() === 'View all' ? $(this).text('View less') : $(this).text('View all');
    });
}