import toastr from 'toastr';
import axios from 'axios';
import { axiosError, resolveClientUrl, toastOptions, showValidationSummaryErrors, hideValidationSummaryErrors } from '../../shared/common.functions';

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

    if ($('#UnitId').val() == 0) {
        errorMessages.push('The Unit field is required!');
    }

    if (errorMessages.length > 0) {
        $('.validation-summary-valid').find('ul:first').html('');
        showValidationSummaryErrors(errorMessages);
        isValid = false;
    }

    return isValid;
}

$(document).ready(() => {
    submitEvent();
});
