import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import toastr from 'toastr';
import axios from 'axios';
import { axiosError, resolveClientUrl, toastOptions, showValidationSummaryErrors, hideValidationSummaryErrors } from '../../shared/common.functions';

function initManagerSearch() {
    $('#Manager').autocomplete({
        source: function (request, response) {
            axios({
                url: resolveClientUrl('employee/getmemberbymanager'),
                method: "GET",
                params: {
                    keyword: request.term
                }
            }).then(result => {
                response($.map(result.data, function (item) {
                    return {
                        label: item.NickName + ' (' + item.FullName + ')',
                        value: item.Id
                    };
                }));
            });
        },
        minLength: 2,
        select: function (event, ui) {
            event.preventDefault();
            $('#Manager').val(ui.item.label);
            $('#ManagerId').val(ui.item.value);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $('#Manager').val(ui.item.label);
            $('#ManagerId').val(ui.item.value);
        },
        change: function (event, ui) {
            event.preventDefault();
            if (ui && ui.item) {
                $('#Manager').val(ui.item.label);
                $('#ManagerId').val(ui.item.value);
            } else {
                if ($('#Manager').val() === '') {
                    $('#ManagerId').val('');
                } else {
                    $('#Manager').val('0');
                }
            }
        }
    });
}

function submitEvent() {
    $('#btnSubmit').on('click', function () {
        let isValid = validateFormData();
        let form = $('#EditForm');

        if (isValid) {
            hideValidationSummaryErrors(true);
            axios({
                url: form[0].action,
                method: "POST",
                data: form.serialize()
            }).then(response => {
                if (response.data.Code === 0) {
                    toastr.success('Successfully!', '', toastOptions());
                    setTimeout(function () {
                        document.location = resolveClientUrl("Department/Index");
                    }, 3000);
                }
                else {
                    toastr.error(response.data.Message, '', toastOptions());
                }
            }).catch(axiosError);
        }
    });
}

function validateFormData() {
    let isValid = true,
        errorMessages = [];

    if (!$('#Code').val()) {
        errorMessages.push('The Code field is required!');
    }

    if (!$('#Name').val()) {
        errorMessages.push('The Name field is required!');
    }

    if (!$('#ManagerId').val() || $('#ManagerId').val() == 0) {
        errorMessages.push('The Manager field is required!');
    }

    if (errorMessages.length > 0) {
        $('.validation-summary-valid').find('ul:first').html('');
        showValidationSummaryErrors(errorMessages);
        isValid = false;
    }

    return isValid;
}

$(document).ready(() => {
    initManagerSearch();
    submitEvent();
});
