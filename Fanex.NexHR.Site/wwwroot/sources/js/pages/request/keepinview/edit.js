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
import { KeepInViewType } from '../../../shared/constants';

$(document).ready(function () {
    const requestId = $('#RequestId').val(),
        employeeId = $('#EmployeeId').val();
    let domElements = {
        $keepInViewType: $('input[type="radio"][name="KeepInViewType"]'),
        $keepInViewByDay: $('#KeepInViewByDay'),
        $groupHalfDay: $('#groupHalfDay'),
        $halfDay: $('#HalfDay'),
        $date: $('#Date'),
        $verifierId: $('#VerifierId'),
        $approverId: $('#ApproverId'),
        $timeGroup: $('#timeGroup'),
        $fromTime: $('#FromTime'),
        $toTime: $('#ToTime'),
        $crossDay: $('#CrossDay'),
        $remark: $('#Remark'),
        $btnSubmit: $('#btnSubmit')
    };
    let validateForm = function () {
        let isValid = true,
            errorMessages = [];
        const approverId = domElements.$approverId.val(),
            verifierId = domElements.$verifierId.val();

        if (!$('#IsTopManager').prop('checked')) {
            if (verifierId === null || verifierId === '') {
                errorMessages.push('Verified by field is required.');
            }
            if (approverId === null || approverId === '') {
                errorMessages.push('Approved by field is required.');
            }
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

    domElements.$keepInViewType.on('change', function () {
        if (domElements.$keepInViewByDay.prop('checked')) {
            domElements.$groupHalfDay.show();
            if (domElements.$halfDay.prop('checked')) {
                domElements.$timeGroup.show();
            } else {
                domElements.$timeGroup.hide();
            }
        } else {
            domElements.$groupHalfDay.hide();
            domElements.$timeGroup.show();
        }
    });

    domElements.$halfDay.on('change', function () {
        if (domElements.$halfDay.prop('checked')) {
            domElements.$timeGroup.show();
        } else {
            domElements.$timeGroup.hide();
        }
    });

    domElements.$date.on('change', function () {
        axios({
            url: _global.root + 'shift/getShiftByDate',
            method: 'GET',
            params: {
                employeeId: employeeId,
                date: domElements.$date.val()
            }
        }).then(response => {
            const shift = response.data;
            if (shift !== null) {
                domElements.$fromTime.val(shift.BeginCheckInTime);
                domElements.$toTime.val(shift.BeginCheckOutTime);
                domElements.$crossDay.prop('checked', shift.CrossDay);
            }
        }).catch(axiosError);
    });

    domElements.$btnSubmit.on('click', function () {
        hideValidationSummaryErrors(false);
        const typeId = domElements.$keepInViewByDay.prop('checked') ? KeepInViewType.KeepInViewByDay : KeepInViewType.KeepInViewByHour;
        const model = {
            requestId: requestId,
            employeeId: employeeId,
            keepInViewType: typeId,
            halfDay: domElements.$halfDay.prop('checked'),
            date: domElements.$date.val(),
            fromTime: domElements.$fromTime.val(),
            toTime: domElements.$toTime.val(),
            crossDay: domElements.$crossDay.prop('checked'),
            verifierId: domElements.$verifierId.val(),
            approverId: domElements.$approverId.val(),
            remark: $.trim(domElements.$remark.val())
        };

        const isValid = validateForm();
        if (isValid) {
            axios({
                url: _global.root + 'keepinview/edit',
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
                    setTimeout(function () { location.href = _global.root + 'keepinview/employeeview'; }, 3000);
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        }
    });

    $('form').areYouSure();

    initDatepicker();
});