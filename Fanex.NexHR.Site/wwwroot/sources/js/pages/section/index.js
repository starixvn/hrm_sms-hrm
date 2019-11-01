import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import { axiosError, floatThead, confirm, resolveClientUrl } from '../../shared/common.functions';

function deleteSection(id) {
    confirm('Are you sure you want to delete this section?', function () {
        axios({
            url: resolveClientUrl('team/delete' + '/' + id),
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

    $('table#data a.deleteTeam').on('click', function () {
        const teamId = $(this).data('team-id');
        deleteSection(teamId);
    });
});