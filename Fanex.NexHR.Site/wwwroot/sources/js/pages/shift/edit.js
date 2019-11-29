import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import axios from 'axios';
import { resolveClientUrl, confirm, axiosError } from '../../shared/common.functions';


$(function () {
    function InitDeleteShiftEvent() {
        $('.js-delete-shift').on('click', function() {
            const id = $(this).attr('data-id');
            confirm('Are you sure you want to delete this shift?',() => {
                axios({
                    url: resolveClientUrl('shift/delete'),
                    method: "POST",
                    data: { shiftId: id }
                }).then(response => {
                    if (response.data.IsSuccessful) {
                        document.location = resolveClientUrl("shift/index");
                    }
                    else {
                        alert(response.Message);
                    }
                }).catch(axiosError);
            });

            $('form').bind('submit', function () {
                $(this).find(':input').removeAttr('disabled');
            });
        })
    }

    function InitInputAttr() {
        if ($("#IsUsed").val() === "True") {
            $("#Name").attr("disabled", "disabled");
            $("#HourTimeIn").attr("disabled", "disabled");
            $("#MinuteTimeIn").attr("disabled", "disabled");
            $("#HourTimeOut").attr("disabled", "disabled");
            $("#MinuteTimeOut").attr("disabled", "disabled");
            $("#IsInAfterDay").attr("disabled", "disabled");
            $("#IsOutAfterDay").attr("disabled", "disabled");
        }
    }

    $(document).ready(() => {
        InitInputAttr();
        InitDeleteShiftEvent();
    });
});