import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import 'chosen-js/chosen.jquery';
import '../../shared/multiselect-tree-view';
import { initFromDateOnChange, initDatepicker, showValidationSummaryErrors, hideValidationSummaryErrors} from '../../shared/common.functions';
import '../../date';

function bindingDataIfEditMode() {
    var messageId = $('#messageId').val(),
        messageSubject = $('#messageSubject').val(),
        messageContents = $('#messageContents').val(),
        messageFromDate = $('#messageFromDate').val(),
        messageToDate = $('#messageToDate').val(),
        messageEmployeeIds = $('#messageEmployeeIds').val(),
        messageGroupIds = $('#messageGroupIds').val(),
        messageCategory = $('#messageCategory').val();

    $('#Subject').val(messageSubject);
    $('#Contents').val(messageContents);
    $('#MessageId').val(messageId);
    $('#category').val(messageCategory);

    if (messageFromDate) {
        var newFromDate = new Date(messageFromDate);
        $('#FromDate').val($.datepicker.formatDate('d M, yy', newFromDate));
    }
    if (messageToDate) {
        var newToDate = new Date(messageToDate);
        $('#ToDate').val($.datepicker.formatDate('d M, yy', newToDate));
    }
    if (messageEmployeeIds) {
        $('#Employees').val(messageEmployeeIds.split(','));
    }
    if (messageGroupIds) {
        $('#GroupIds').val(messageGroupIds);
    }
};

function initSearchEmployee() {
    const employeeIds = $('#EmployeeIds').val();

    if (employeeIds) {
        $('#Employees').val(employeeIds.split(','));
    }

    $('#Employees').chosen();

    $('#Employees').on('change', function () {
        const id = $(this).val(),
            employeeIds = id ? id.join(',') : '';
        $('#EmployeeIds').val(employeeIds);
    });
}

function initSearchGroup() {
    $.ajax({
        url: _global.root + 'Department/GetHierarchy',
        type: "Get",
        success: function(response) {
            $('#Groups').multiSelectTreeView({
                data: response,
                selectedItems: $('#GroupIds').val()
            });
        },
        error: function () {
        }
    });
}

function initDatePicker() {
    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));
}

function GetSelectedEmployeeEmails() {
    let result = "";
    let employeeEmails = $('#Employees option:selected');

    if (employeeEmails) {
        for (var i = 0; i < employeeEmails.length; i++) {
            var item = employeeEmails[i],
                email = $(item).attr('email');

            if (email) {
                if (!result) {
                    result = email;
                }
                else {
                    result += "," + email;
                }
            }
        }
    }

    return result;
}

function GetSelectedGroupEmails() {
    let selectedGroups = $('#Groups').multiSelectTreeView('getSelectedItems');

    var result = "";
    var selectedArr = selectedGroups.split(',');
    for (var i = 0; i < selectedArr.length; i++) {
        let email = GroupsCollection.filter(group => group.Id.toString() == selectedArr[i])[0].MailGroup;
        if (email) {
            if (!result) {
                result = email;
            }
            else {
                result += "," + email;
            }
        }
    }

    return result;
}

function getSendToEmails() {
    var employeeEmails = GetSelectedEmployeeEmails();
    var groupsEmail = GetSelectedGroupEmails();

    var result = employeeEmails;

    if (groupsEmail) {
        result = result + ',' + groupsEmail;
    }

    return result;
}

function validateForm() {
    let isValid = true,
        errorMessages = [];

    if (!$('#Subject').val()) {
        errorMessages.push('The Subject field is required!');
    }
    if (!$('#Category').val()) {
        errorMessages.push('The Category field is required!');
    }
    if (!$('#SendToEmail').val()) {
        errorMessages.push('The Send To field is required!');
    }
    if (!$('#Contents').val()) {
        errorMessages.push('The Contents field is required!');
    }

    if (errorMessages.length > 0) {
        $('.validation-summary-valid').find('ul:first').html('');
        showValidationSummaryErrors(errorMessages);
        isValid = false;
    }

    return isValid;
}

function Submit() {
    $('#btnSubmit').on('click', function () {
        //validation
        let selectedGroups = $('#Groups').multiSelectTreeView('getSelectedItems');
        let sendToEmail = getSendToEmails();
        $('#SendToEmail').val(sendToEmail);
        $('#GroupIds').val(selectedGroups);

        var isValid = validateForm();

        var form = $('#CreateForm');
        var $error = $("#error");

        if (isValid) {
            hideValidationSummaryErrors(true);
            debugger;
            $.ajax({
                url: form.action,
                type: "Post",
                data: form.serialize(),
                success: function (commandResult) {
                    if (commandResult.Code === 0) {
                        $('#messageInfo').show();
                        setTimeout(function () {
                            document.location = _global.root + "Message/Index";
                        }, 2000);
                    }
                    else {
                        $error.html(commandResult.Message);
                        $error.show();
                    }
                },
                error: function () {
                    $error.html(_messages.errorTryAgain);
                    $error.show();
                }
            });
        }
    });
}

$(document).ready(function () {
    bindingDataIfEditMode();
    initSearchEmployee();
    initSearchGroup();
    initDatePicker();
    Submit();
});
