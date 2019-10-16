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
import { RequestType } from '../../../shared/constants';

$(document).ready(function () {
    const requestTypeId = parseInt($('#RequestTypeId').val(), 10);
    let domElements = {
        $requestId: $('#RequestId'),
        $employeeId: $('#EmployeeId'),
        $attendantDate: $('#AttendantDate'),
        $forDate: $('#ForDate'),
        $verifierId: $('#VerifierId'),
        $approverId: $('#ApproverId'),
        $hour: $('#Hour'),
        $minute: $('#Minute'),
        $remark: $('#Remark'),
        $btnSubmit: $('#btnSubmit')
    };
    let bindingHourAndMinute = function () {
        if (requestTypeId === RequestType.CheckIn) {
            let intime = $('#InTime').val().trim();
            domElements.$hour.val(intime.substring(0, 2));
            domElements.$minute.val(intime.substring(3, 5));
        } else {
            let outtime = $('#OutTime').val().trim();
            domElements.$hour.val(outtime.substring(0, 2));
            domElements.$minute.val(outtime.substring(3, 5));
        }
    };
    let checkInOrOutInNextDay = function (isInOutAfterDay) {
        const attendantDate = new Date(domElements.$attendantDate.val());
        if (isInOutAfterDay === 'true') {
            const requestDate = new Date(attendantDate.getTime() + 86400000);
            domElements.$forDate.val(moment(requestDate).format('DD MMM, YYYY'));
        } else {
            domElements.$forDate.val(moment(attendantDate).format('DD MMM, YYYY'));
        }
    };
    let checkCrossDay = function () {
        const isInAfterDay = $('#IsInAfterDay').val(),
            isOutAfterDay = $('#IsOutAfterDay').val();

        if (requestTypeId === RequestType.CheckIn) {
            checkInOrOutInNextDay(isInAfterDay);
        } else {
            checkInOrOutInNextDay(isOutAfterDay);
        }
    };
    let validateForm = function () {
        let isValid = true,
            errorMessages = [];
        const employeeId = parseInt(domElements.$employeeId.val(), 10),
            attendantDate = new Date(domElements.$attendantDate.val()),
            requestDate = new Date(domElements.$forDate.val()),
            hour = parseInt(domElements.$hour.val(), 10),
            minute = parseInt(domElements.$minute.val(), 10),
            approverId = domElements.$approverId.val(),
            verifierId = domElements.$verifierId.val();
        const dayDiff = (requestDate - attendantDate) / 1000 / 60 / 60 / 24;

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

        if (dayDiff < 0) {
            errorMessages.push('Request Date must be equal or greater than On Date!');
        }

        if (hour < 0 || hour >= 24) {
            errorMessages.push('Hour field must be between 0 and 23.');
        }

        if (minute < 0 || minute >= 60) {
            errorMessages.push('Minute field must be between 0 and 60.');
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

    domElements.$attendantDate.on('change', function () {
        axios({
            url: _global.root + 'shift/getShiftByDate',
            method: 'GET',
            params: {
                employeeId: domElements.$employeeId.val(),
                date: domElements.$fromDate.val()
            }
        }).then(response => {
            const shift = response.data;
            if (shift !== null) {
                $('#shiftname').text(shift.Name);
                $('#InTime').val(shift.BeginCheckInTime);
                $('#OutTime').val(shift.BeginCheckOutTime);
                $('#CrossDay').val(shift.CrossDay);
                $('#IsInAfterDay').val(shift.IsInAfterDay);
                $('#IsOutAfterDay').val(shift.IsOutAfterDay);
                bindingHourAndMinute();
                checkCrossDay();
            }
        }).catch(axiosError);
    });

    domElements.$btnSubmit.on('click', function () {
        hideValidationSummaryErrors(false);

        const isValid = validateForm();
        if (isValid) {
            const model = {
                requestId: domElements.$requestId.val(),
                employeeId: domElements.$employeeId.val(),
                refNo: $('#RefNO').val().trim(),
                requestTypeId: requestTypeId,
                attendantDate: domElements.$attendantDate.val(),
                forDate: domElements.$forDate.val(),
                reasonId: $('#reasonId').val(),
                hour: domElements.$hour.val(),
                minute: domElements.$minute.val(),
                remark: $.trim(domElements.$remark.val()),
                verifierId: domElements.$verifierId.val(),
                approverId: domElements.$approverId.val()
            };
            axios({
                url: _global.root + 'checkinout/edit',
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
                    setTimeout(function () { location.href = _global.root + 'checkinout/employeeview'; }, 3000);
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        }
    });

    $('form').areYouSure();
});