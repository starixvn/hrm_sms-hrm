import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import toastr from 'toastr';
import axios from 'axios';

import { axiosError, floatThead, confirm, resolveClientUrl, toastOptions} from '../../shared/common.functions';

$(document).ready(function () {
    floatThead('table#data');

    $('table#data a.delete').on('click', function () {
        const departmentId = $(this).data('id');
        confirm('Are you sure you want to delete this Division?', function () {
            axios({
                url: resolveClientUrl('department/delete'),
                method: 'POST',
                data: {
                    departmentId: departmentId
                },
            }).then(response => {
                let data = response.data;
                if (data.IsSuccessful) {
                    toastr.success('Delete successfully!', '', toastOptions());
                    setTimeout(function () { location.href = resolveClientUrl('department/index'); }, 2000);
                } else {
                    toastr.error(data.Message, '', toastOptions());
                }
            }).catch(axiosError);
        });
    });
});