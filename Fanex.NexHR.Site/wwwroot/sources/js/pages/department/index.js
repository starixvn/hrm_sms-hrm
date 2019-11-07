import $ from 'jquery/dist/jquery';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import { axiosError, floatThead, confirm, resolveClientUrl } from '../../shared/common.functions';

function deleteDepartment(departmentId) {
    confirm('Are you sure you want to delete this Division?', function () {
        axios({
            url: resolveClientUrl('department/delete' + '/' + departmentId),
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
    
    $('table#data a.deleteDepartment').on('click', function () {
        const departmentId = $(this).data('data-id');
        deleteDepartment(departmentId);
    });
    
});