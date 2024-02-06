import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import axios from 'axios';
import { resolveClientUrl, confirm, axiosError, alert } from '../../shared/common.functions';

$(function () {
    function initDeleteShiftEvent() {
        $('.js-delete-shift').on('click', function () {
            const id = $(this).attr('data-id');
            confirm('Are you sure you want to delete this shift?', () => {
                axios({
                    url: resolveClientUrl('shift/delete'),
                    method: 'POST',
                    data: { shiftId: id }
                }).then(response => {
                    if (response.data.IsSuccessful) {
                        document.location = resolveClientUrl('shift/index');
                    }
                    else {
                        alert(response.Message);
                    }
                }).catch(axiosError);
            });
        });
    }

    function initSubmitEvent() {
        $('form').bind('submit', function () {
            $(this).find(':input').removeAttr('disabled');
        });
    }

    function initInputAttr() {
        if ($('#IsUsed').val() === 'True') {
            $('#Name').prop('disabled', true);
            $('#HourTimeIn').prop('disabled', true);
            $('#MinuteTimeIn').prop('disabled', true);
            $('#HourTimeOut').prop('disabled', true);
            $('#MinuteTimeOut').prop('disabled', true);
            $('#CheckInDateType').prop('disabled', true);
            $('#CheckOutDateType').prop('disabled', true);
        }
    }

    $(document).ready(() => {
        initDeleteShiftEvent();
        initSubmitEvent();
        initInputAttr();
    });
});