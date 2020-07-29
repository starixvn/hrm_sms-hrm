import axios from 'axios';
import { axiosError, floatThead, blockUI, unblockUI, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    blockUI();
    axios({
        url: resolveClientUrl('account/HistoryDetail'),
        method: 'POST',
        params: { id: $('.Id').val() },
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    }).then(response => {
        $('#historyDetail').html(response.data);
        floatThead('table#data');
    }).catch(axiosError).then(() => unblockUI());
});