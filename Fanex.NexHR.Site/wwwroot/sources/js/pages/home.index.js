import 'bootstrap/dist/js/bootstrap';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../shared/common.functions';

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
});