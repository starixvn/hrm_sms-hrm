import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../shared/common.functions';

const modifiedExtensionNo = (function ($) {
    let domElements = {
        $empId: $('#EmpId'),
        $teamId: $('#TeamId'),
        $checkboxInternal: $('#IsInternal'),
        $employee: $('#Employee'),
        $fullName: $('#FullName'),
        $nickname: $('#NickName'),
        $employeeField: $('#EmployeeField'),
        $btnSubmit: $('#btnSubmit')
    };

    function handleCheckboxChange() {
        if (domElements.$checkboxInternal.prop('checked')) {
            domElements.$fullName.prop('readonly', true);
            domElements.$nickname.prop('readonly', true);
            domElements.$employeeField.show();

            if (domElements.$empId.val() > 0) {
                $employee.val(domElements.$nickname.val() + ' (' + domElements.$fullName.val() + ')');
            }
        } else {
            domElements.$fullName.prop('readonly', false);
            domElements.$nickname.prop('readonly', false);
            domElements.$employeeField.hide();
        }
    }

    function bindDataToGroupDropdown() {
        axios({
            url: resolveClientUrl('extensionno/GetGroupExtension'),
            method: 'GET',
            params: {
                isInternal: domElements.$checkboxInternal.prop('checked'),
                teamId: null
            }
        }).then(response => {
            const data = response.data;
            if (data !== null) {
                const totalItems = data.length;
                let options = '';
                for (var i = 0; i < totalItems; i++) {
                    options += '<option value="' + data[i].value + '">' + data[i].text + '</option>';
                }

                domElements.$teamId.html(options);
            }
        }).catch(axiosError);
    }

    function registerEvents() {
        domElements.$checkboxInternal.on('change', function () {
            bindDataToGroupDropdown();
            handleCheckboxChange();
        });

        domElements.$employee.autocomplete({
            source: function (request, response) {
                axios({
                    url: resolveClientUrl('employee/getemployees'),
                    method: 'GET',
                    params: {
                        query: request.term
                    }
                }).then(rs => {
                    response($.map(rs.data, function (item) {
                        return {
                            label: item.DisplayName,
                            value: item.Id,
                            fullname: item.FullName,
                            nickname: item.NickName
                        };
                    }));
                }).catch(axiosError);
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                domElements.$employee.val(ui.item.label);
                domElements.$empId.val(ui.item.value);
                domElements.$fullName.val(ui.item.fullname);
                domElements.$nickname.val(ui.item.nickname);
            },
            focus: function (event, ui) {
                event.preventDefault();
                domElements.$employee.val(ui.item.label);
                domElements.$empId.val(ui.item.value);
                domElements.$fullName.val(ui.item.fullname);
                domElements.$nickname.val(ui.item.nickname);
            },
            change: function (event, ui) {
                event.preventDefault();
                if (ui && ui.item) {
                    domElements.$employee.val(ui.item.label);
                    domElements.$empId.val(ui.item.value);
                    domElements.$fullName.val(ui.item.fullname);
                    domElements.$nickname.val(ui.item.nickname);
                } else {
                    domElements.$empId.val('');
                }
            }
        });
    }

    function initModule() {
        registerEvents();

        if ($('#IsCreateForm').prop('checked')) {
            bindDataToGroupDropdown();
        }
        handleCheckboxChange();
    }

    return {
        initModule: initModule
    };
})(jQuery);

$(document).ready(function () {
    modifiedExtensionNo.initModule();
});