import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import { axiosError, floatThead, confirm, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    floatThead('table#data');

    $('table#data a.delete').on('click', function () {
        const extensionNoId = $(this).data('extension-no-id');
        confirm('Are you sure you want to delete this extension number?', function () {
            axios({
                url: resolveClientUrl('ExtensionNo/Delete' + '/' + extensionNoId),
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