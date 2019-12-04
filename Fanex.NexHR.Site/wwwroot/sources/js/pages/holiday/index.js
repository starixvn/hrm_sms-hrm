import $ from 'jquery/dist/jquery';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import { axiosError, floatThead, confirm, resolveClientUrl } from '../../shared/common.functions';

function deleteHoliday(holidayId) {
    confirm('Are you sure you want to delete this Holiday?', function () {
        axios({
            url: resolveClientUrl('holiday/delete' + '/' + holidayId),
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
}

$(document).ready(function () {
    floatThead('table#data');

    $('table#data a.deleteHoliday').on('click', function () {
        const holidayId = $(this).data('data-id');
        deleteHoliday(holidayId);
    });

});