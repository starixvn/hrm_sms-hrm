import $ from 'jquery/dist/jquery';
import 'bootstrap/dist/js/bootstrap';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError } from '../shared/common.functions';

$(document).ready(function () {
    if ($('#IsResetPassword').prop('checked')) {
        $('#changePasswordModal').modal('show');

        $('#btnSubmit').on('click', function () {
            location.href = _global.root + 'Account/changepwd';
        });

        $('#btnCancel').on('click', function () {
            $('#changePasswordModal').modal('hide');
            const status = $('#ckb-dontshowagain').prop('checked');
            if (status) {
                axios({
                    url: _global.root + 'Account/EditPasswordResetStatus',
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