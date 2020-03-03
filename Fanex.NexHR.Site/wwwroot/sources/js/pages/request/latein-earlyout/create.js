import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery.are-you-sure/jquery.are-you-sure';
import toastr from 'toastr';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';

import {
    axiosError, initDatepicker, showValidationSummaryErrors, hideValidationSummaryErrors, toastOptions
} from '../../../shared/common.functions';
import { RequestType, RoleSystem } from '../../../shared/constants';

$(document).ready(function () {
    let domElements = {
        $employeeId: $('#EmployeeId'),
        $lateInTypeId: $('#RequestType_3'),
        $earlyOutTypeId: $('#RequestType_4'),
        $onDate: $('#OnDate'),
        $verifierId: $('#VerifierId'),
        $approverId: $('#ApproverId'),
        $minute: $('#Minute'),
        $remark: $('#Remark'),
        $actualLateIn: $('#actualLateIn'),
        $lateInHours: $('#lateInHours'),
        $actualEarlyOut: $('#actualEarlyOut'),
        $earlyOutHours: $('#earlyOutHours'),
        $btnSubmit: $('#btnSubmit')
    };
    let getDataAfterChangeEmployee = function () {
        const employeeId = domElements.$employeeId.val();
        const promises = [
            axios(_global.root + 'request/GetVerifiers', {
                params: {
                    employeeId: employeeId,
                    requestType: RequestType.LateIn
                }
            }),
            axios(_global.root + 'request/GetApprovers', {
                params: {
                    employeeId: employeeId,
                    requestType: RequestType.LateIn
                }
            })
        ];

        Promise.all(promises)
            .then(([verifierResponse, approverResponse]) => {
                const verifiers = verifierResponse.data,
                    approvers = approverResponse.data;
                let options = '';
                for (let i = 0; i < verifiers.length; i++) {
                    options += "<option value='" + verifiers[i].OwnerId + "'>" + verifiers[i].NickName + " (" + verifiers[i].FullName + ")</option>";
                }
                domElements.$verifierId.html(options);

                options = '';
                for (var i = 0; i < approvers.length; i++) {
                    options += "<option value='" + approvers[i].OwnerId + "'>" + approvers[i].NickName + " (" + approvers[i].FullName + ")</option>";
                }
                domElements.$approverId.html(options);
            }).catch(axiosError);
    };
    let getDisabledDates = function (callback) {
        const requestTypeId = domElements.$lateInTypeId.prop('checked') ? RequestType.LateIn : RequestType.EarlyOut;
        axios({
            url: _global.root + 'lateinearlyout/GetDisabledDate',
            method: 'GET',
            params: {
                requestTypeId: requestTypeId,
                employeeId: domElements.$employeeId.val(),
                fromDate: domElements.$onDate.attr('mindate'),
                toDate: domElements.$onDate.attr('maxdate')
            }
        }).then(response => {
            const totalItems = response.data.length;
            let disabledDate = '';
            for (let i = 0; i < totalItems; i++) {
                disabledDate = disabledDate.length > 0
                    ? disabledDate + ',' + moment(response.data[i]).format('YYYY/MM/DD')
                    : moment(response.data[i]).format('YYYY/MM/DD');
            }

            domElements.$onDate.attr('disabledDate', disabledDate);
            if (callback !== undefined && typeof callback === 'function') {
                callback();
            }
        }).catch(axiosError);
    };
    let showHideActualLateInEarlyOut = function () {
        if (domElements.$lateInTypeId.prop('checked')) {
            domElements.$actualLateIn.show();
            domElements.$actualEarlyOut.hide();
        } else {
            domElements.$actualLateIn.hide();
            domElements.$actualEarlyOut.show();
        }
    };
    let getActualLateInEarlyOut = function () {
        axios({
            url: _global.root + 'lateinearlyout/GetActualLateInEarlyOut',
            method: 'GET',
            params: {
                employeeId: domElements.$employeeId.val(),
                workDate: domElements.$onDate.val()
            }
        }).then(response => {
            if (response.data.lateIn > 0) {
                domElements.$lateInHours.html(data.LateIn + "'");
            } else {
                domElements.$lateInHours.html('N/A');
            }

            if (response.data.earlyOut > 0) {
                domElements.$earlyOutHours.html(data.EarlyOut + "'");
            } else {
                domElements.$earlyOutHours.html('N/A');
            }
        }).catch(axiosError);
    };
    let validateForm = function () {
        let isValid = true,
            errorMessages = [];
        const employeeId = parseInt(domElements.$employeeId.val(), 10),
            minute = parseInt(domElements.$minute.val(), 10),
            approverId = domElements.$approverId.val(),
            verifierId = domElements.$verifierId.val();

        if (employeeId === 0) {
            errorMessages.push('Employee field is required.');
        }

        if (!$('#IsTopManager').prop('checked')) {
            if (verifierId === null || verifierId === '') {
                errorMessages.push('Verified by field is required.');
            }
            if (approverId === null || approverId === '') {
                errorMessages.push('Approved by field is required.');
            }
        }

        if (minute <= 0 || minute > 60) {
            errorMessages.push('Minute field must be between 1 and 60.');
        }

        if ($.trim(domElements.$remark.val()).length === 0) {
            errorMessages.push('Remark field is required.');
        }

        if (errorMessages.length > 0) {
            showValidationSummaryErrors(errorMessages);
            isValid = false;
        }

        return isValid;
    };

    getDisabledDates(initDatepicker);
    showHideActualLateInEarlyOut();

    $('#Employee').autocomplete({
        source: function (request, response) {
            axios({
                url: _global.root + 'request/GetEmployeesByActingUser',
                method: 'GET',
                params: {
                    role: RoleSystem.OnBehalfRequest,
                    requestType: RequestType.LateIn,
                    query: request.term
                }
            }).then(rs => {
                response($.map(rs.data, function (item) {
                    return {
                        label: item.NickName + ' (' + item.FullName + ')',
                        value: item.OwnerId,
                        isTopManager: item.IsTopManager
                    };
                }));
            }).catch(axiosError);
        },
        minLength: 2,
        select: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmployeeId').val(ui.item.value);
            $('#IsTopManager').prop('checked', ui.item.isTopManager);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmployeeId').val(ui.item.value);
            $('#IsTopManager').prop('checked', ui.item.isTopManager);
        },
        change: function (event, ui) {
            event.preventDefault();
            if (ui && ui.item) {
                $('#Employee').val(ui.item.label);
                $('#EmployeeId').val(ui.item.value);
                $('#IsTopManager').prop('checked', ui.item.isTopManager);
                getDataAfterChangeEmployee();
                getDisabledDates();
                getActualLateInEarlyOut();
            } else {
                $('#EmployeeId').val('0');
            }
        }
    });

    domElements.$lateInTypeId.on('change', function () {
        getDisabledDates();
        getActualLateInEarlyOut();
        showHideActualLateInEarlyOut();
    });

    domElements.$earlyOutTypeId.on('change', function () {
        getDisabledDates();
        getActualLateInEarlyOut();
        showHideActualLateInEarlyOut();
    });

    domElements.$onDate.on('change', function () {
        getActualLateInEarlyOut();
    });

    domElements.$btnSubmit.on('click', function () {
        hideValidationSummaryErrors(false);
        const typeId = domElements.$lateInTypeId.prop('checked') ? RequestType.LateIn : RequestType.EarlyOut;
        const model = {
            employeeId: domElements.$employeeId.val(),
            requestType: typeId,
            onDate: domElements.$onDate.val(),
            ReasonId: $('#reasonId').val(),
            minute: domElements.$minute.val(),
            remark: $.trim(domElements.$remark.val()),
            verifierId: domElements.$verifierId.val(),
            approverId: domElements.$approverId.val()
        };

        const isValid = validateForm();
        if (isValid) {
            axios({
                url: _global.root + 'lateinearlyout/Create',
                method: 'POST',
                data: stringify({ model }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;
                if (data.isSuccessful) {
                    $('form').removeClass('dirty');
                    domElements.$btnSubmit.prop('disabled', true);
                    toastr.success('Create successfully!', '', toastOptions());
                    setTimeout(function () { location.href = _global.root + 'lateinearlyout/employeeview'; }, 3000);
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        }
    });

    $('form').areYouSure();
});