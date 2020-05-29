import $ from 'jquery/dist/jquery';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import axios from 'axios';

import '../../shared/multiselect-tree-view';
import { axiosError, showValidationSummaryErrors } from '../../shared/common.functions';

$(document).ready(function () {
    let domElements = {
        $fullName: $('#FullName'),
        $nickname: $('#Nickname'),
        $code: $('#Code'),
        $teamId: $('#TeamId'),
        $btnSubmit: $('#btnSubmit')
    };
    let validateForm = function () {
        let isValid = true,
            errorMessages = [];
        const fullName = $.trim(domElements.$fullName.val()),
            nickname = $.trim(domElements.$fullName.val()),
            code = $.trim(domElements.$code.val()),
            teamId = parseInt(domElements.$teamId.val(), 10);

        if (fullName.length === 0) {
            errorMessages.push('Full Name field is required.');
        }

        if (nickname.length === 0) {
            errorMessages.push('Nickname field is required.');
        }

        if (code.length === 0) {
            errorMessages.push('Code field is required.');
        }

        if (teamId === 0) {
            errorMessages.push('Section field is required.');
        }

        if (errorMessages.length > 0) {
            $('.validation-summary-valid').find('ul:first').html('');
            showValidationSummaryErrors(errorMessages);
            isValid = false;
        }

        return isValid;
    };

    axios(_global.root + 'Department/GetHierarchy')
        .then(response => {
            $('#Section').multiSelectTreeView({
                data: response.data,
                noneSelectedText: 'Select a section',
                singleSelection: true,
                onSelect: function () {
                    $('#TeamId').val($('#Section').multiSelectTreeView('getSelectedItems'));
                }
            });
        }).catch(axiosError);

    $('#generate').on('click', function () {
        axios(_global.root + 'employee/GenerateEmployeeCode')
            .then(response => {
                let data = response.data;

                $('#Code').val('X' + data);
            }).catch(axiosError);
    });

    domElements.$btnSubmit.on('click', function () {
        const isValid = validateForm();
        if (isValid) {
            $(this).closest('form').submit();
        }
    });
});