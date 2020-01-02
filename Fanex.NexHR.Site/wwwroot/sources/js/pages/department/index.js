import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import { axiosError, floatThead, confirm, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    floatThead('table#data');
    $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoNS });

    $('table#data a.delete').on('click', function () {
        const departmentId = $(this).data('id');
        confirm('Are you sure you want to delete this Division?', function () {
            axios({
                url: resolveClientUrl('department/delete/' + departmentId),
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;
                if (data.isSuccessful) {
                    location.reload();
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        });
    });
});