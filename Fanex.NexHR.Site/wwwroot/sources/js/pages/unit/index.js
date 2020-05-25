import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'bootstrap/dist/js/bootstrap';
import axios from 'axios';
import { resolveClientUrl, confirm, axiosError, alert } from '../../shared/common.functions';

$(function () {
    function initDeleteEvent() {
        $('.icon-delete').on('click', function () {
            const unitId = $(this).data('unit-id');
            confirm('Are you sure you want to delete this unit?', function () {
                axios({
                    url: resolveClientUrl('Unit/delete'),
                    method: 'POST',
                    data: { 'UnitId': unitId }
                }).then(response => {
                    if (response.data.IsSuccessful) {
                        alert('Delete successfully!');
                        $('#row_' + unitId).remove();
                    }
                    else {
                        alert(response.data.Message);
                    }
                }).catch(axiosError);
            });
        })
    }

    $(document).ready(() => {
        initDeleteEvent();
    });
});