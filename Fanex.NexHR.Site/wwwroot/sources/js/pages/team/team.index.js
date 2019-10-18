import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { axiosError, blockUI, floatThead, unblockUI, confirm, autoIndex, alert } from '../../shared/common.functions';

function deleteTeam(id) {
    confirm('Are you sure you want to delete this team?', function () {
        $.post(_global.root + 'Team/Delete', { id: id }, function(response) {
            if (response.IsSuccessful) {
                $('#row' + id).remove();
                autoIndex();
            } else {
                alert(response.Message);
            }
        });
    });
}

$(document).ready(function () {
    floatThead('table#data');

    $('table#data a.deleteTeam').on('click', function () {
        var teamId = $(this).data('team-id');
        deleteTeam(teamId);
    });
});