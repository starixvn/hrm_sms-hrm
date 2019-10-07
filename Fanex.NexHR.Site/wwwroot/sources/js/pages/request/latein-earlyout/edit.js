import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery.are-you-sure/jquery.are-you-sure';
import toastr from 'toastr';
import axios from 'axios';
import { stringify } from 'qs';

import {
    axiosError, initDatepicker, showValidationSummaryErrors, hideValidationSummaryErrors, toastOptions
} from '../../../shared/common.functions';
import { RequestType } from '../../../shared/constants';

$(document).ready(function () {
    let domElements = {
        $requestId: $('#RequestId'),
        $employeeId: $('#EmployeeId'),
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
    const requestTypeId = parseInt($('#RequestType').val(), 10);
    let showHideActualLateInEarlyOut = function () {
        if (requestTypeId === RequestType.LateIn) {
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

    initDatepicker();
    showHideActualLateInEarlyOut();

    domElements.$onDate.on('change', function () {
        getActualLateInEarlyOut();
    });

    domElements.$btnSubmit.on('click', function () {
        hideValidationSummaryErrors(false);
        const model = {
            requestId: domElements.$requestId.val(),
            employeeId: domElements.$employeeId.val(),
            requestType: requestTypeId,
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
                url: _global.root + 'lateinearlyout/edit',
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
                    toastr.success('Edit successfully!', '', toastOptions());
                    setTimeout(function () { location.href = _global.root + 'lateinearlyout/employeeview'; }, 3000);
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        }
    });

    $('form').areYouSure();
});