import 'floatthead/dist/jquery.floatThead';
import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'bootstrap/dist/js/bootstrap';
import axios from 'axios';
import { axiosError, floatThead, confirm, resolveClientUrl, alert, autoIndex } from '../../shared/common.functions';

function deleteSection() {
    $('.deleteSection').on('click', function () {
        const id = $(this).data('section-id');
        confirm('Are you sure you want to delete this section?', function () {
            axios({
                url: resolveClientUrl('section/delete' + '/' + id),
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest', 
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;
                if (data.isSuccessful) {
                    alert('Delete successfully!');
                    $('#row' + id).remove();
                    autoIndex();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
        });
    });
}

$(document).ready(function () {
    floatThead('table#data');
    deleteSection();
});