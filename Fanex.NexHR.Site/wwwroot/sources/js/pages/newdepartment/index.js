import 'floatthead/dist/jquery.floatThead';
import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';

import { axiosError, floatThead, confirm, resolveClientUrl } from '../../shared/common.functions';

function deleteDepartment() {
    $('.deleteDepartment').on('click', function () {
        const id = $(this).data('department-id');
        confirm('Are you sure you want to delete this department?', function () {
            axios({
                url: resolveClientUrl('newdepartment/delete' + '/' + id),
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
}

$(document).ready(function () {
    floatThead('table#data');
    deleteDepartment();
});