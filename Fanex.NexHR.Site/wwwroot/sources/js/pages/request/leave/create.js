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
import { LeaveType, RequestType, RoleSystem } from '../../../shared/constants';

$(document).ready(function () {
    let domElements = {
        $annualBalance: $('#lblAnnualBalance'),
        $keepInViewBalance: $('#lblKeepInViewBalance'),
        $currentLeaveBalance: $('#lblCurrentLeaveBalance'),
        $weekendLeaveTaken: $('#lblWeekendLeave'),
        $employeeId: $('#EmployeeId'),
        $leaveTypeId: $('#LeaveTypeId'),
        $halfDayOff: $('#HalfDayOff'),
        $fromDate: $('#FromDate'),
        $toDate: $('#ToDate'),
        $fromTime: $('#FromTime'),
        $toTime: $('#ToTime'),
        $fromTimeHour: $('#FromTimeHour'),
        $fromTimeMinute: $('#FromTimeMinute'),
        $toTimeHour: $('#ToTimeHour'),
        $toTimeMinute: $('#ToTimeMinute'),
        $totalDays: $('#TotalDays'),
        $verifierId: $('#VerifierId'),
        $approverId: $('#ApproverId'),
        $remark: $('#Remark'),
        $isFromTimeAfterDay: $('#IsFromTimeAfterDay'),
        $isToTimeAfterDay: $('#IsToTimeAfterDay'),
        $btnSubmit: $('#btnSubmit')
    };
    let getDataAfterChangeEmployee = function () {
        const employeeId = domElements.$employeeId.val();
        const promises = [
            axios(_global.root + 'request/GetVerifiers', {
                params: {
                    employeeId: employeeId,
                    requestType: RequestType.Leave
                }
            }),
            axios(_global.root + 'request/GetApprovers', {
                params: {
                    employeeId: employeeId,
                    requestType: RequestType.Leave
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
    let getLeaveBalance = function () {
        axios({
            url: _global.root + 'leave/GetDisabledDate',
            method: 'GET',
            params: {
                employeeId: domElements.$employeeId.val(),
                fromDate: domElements.$fromDate.val(),
                toDate: domElements.$toDate.val()
            }
        }).then(response => {
            const data = response.data;
            domElements.$annualBalance.html(data.annualBalance);
            domElements.$keepInViewBalance.html(data.keepInViewBalance);
            domElements.$currentLeaveBalance.html(data.currentLeaveBalanceDay);
            domElements.$weekendLeaveTaken.html(data.ưeekedLeaveDayTaken);
        }).catch(axiosError);
    };
    let getDisabledDates = function () {
        let minDate = new Date(LeaveRequest.fromDate.attr('minDate')),
            tempDate = minDate.setMonth(minDate.getMonth() + 12);
        const leaveTypeId = parseInt(domElements.$leaveTypeId.val(), 10);
        switch (leaveTypeId) {
            case LeaveType.GeneralLeave:
                $('#grbHalfDay').hide();
                $('#grbTotalDay').hide();
                $('.time').hide();
                $('.time-businesstrip').hide();
                $('.time-isafterday').hide();
                domElements.$toDate.prop('readonly', false).datepicker('enable');
                break;
            case LeaveType.KeepInViewByHour:
                domElements.$halfDayOff.prop('checked', true);
                $('#grbTotalDay').hide();
                break;
            default:
                $('#grbHalfDay').show();
                $('#grbTotalDay').show();
                switch (leaveTypeId) {
                    case LeaveType.MaternityLeave:
                        tempDate = minDate.setMonth(minDate.getMonth() + 12);
                        break;
                    case LeaveType.PublicHoliday:
                        // nothing
                        break;
                    case LeaveType.CompensationLeave:
                        tempDate = minDate.setMonth(minDate.getMonth() + 13);
                        break;
                    default:
                        axios({
                            url: _global.root + 'leave/GetDisabledDate',
                            method: 'GET',
                            params: {
                                employeeId: domElements.$employeeId.val(),
                                leaveTypeId: leaveTypeId,
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

                            domElements.$fromDate.attr('disabledDate', disabledDate);
                            domElements.$toDate.attr('disabledDate', disabledDate);
                        }).catch(axiosError);
                        break;
                }
                break;
        }

        const maxDate = new Date(tempDate.getYear() + 2, tempDate.getMonth(), 0);
        domElements.$fromDate.datepicker('option', 'maxDate', maxDate);
        domElements.$toDate.datepicker('option', 'maxDate', maxDate);
    };
    let enableControlForHalfday = function () {
        const leaveTypeId = parseInt(domElements.$leaveTypeId.val(), 10);
        if (leaveTypeId === LeaveType.BusinessTrip) {
            $('.time-businesstrip').show();
            $('.time').hide();
        } else if (leaveTypeId === LeaveType.KeepInViewByHour) {
            $('.time-businesstrip').show();
            $('.time').hide();
            $('#grbHalfDay').hide();
            $('#grbTotalDay').hide();
        } else {
            $('.time-businesstrip').hide();
            $('.time').show();
        }

        domElements.$toDate.val(domElements.$fromDate.val());
        domElements.$toDate.prop('readonly', true).datepicker('disable').prop('disabled', false);
    };
    let disableControlForHalfDay = function () {
        $('.time').hide();
        $('.time-businesstrip').hide();
        $('.time-isafterday').hide();
        domElements.$toDate.prop('readonly', false).datepicker('enable');
    };
    let getShiftByDate = function () {
        axios({
            url: _global.root + 'shift/getShiftByDate',
            method: 'GET',
            params: {
                employeeId: domElements.$employeeId.val(),
                date: domElements.$fromDate.val()
            }
        }).then(response => {
            const data = response.data;
            if (data !== null) {
                const arrTimeIn = data.BeginCheckInTime.split(':'),
                    arrTimeOut = data.BeginCheckOutTime.split(':');

                let minuteFrom = parseInt(arrTimeIn[0], 10) * 60 + parseInt(arrTimeIn[1], 10),
                    minuteTo = parseInt(arrTimeOut[0], 10) * 60 + parseInt(arrTimeOut[1], 10),
                    isFromAfterday = data.IsInAfterDay,
                    isToAfterday = false;
                if (data.IsInAfterDay) {
                    minuteFrom += 1440; //60*24
                }

                if (data.IsOutAfterDay) {
                    minuteTo += 1440;
                }

                let averageTime = (minuteFrom + minuteTo) / 2;
                if (averageTime >= 1440) {
                    averageTime = averageTime - 1440;
                    isToAfterday = true;
                }

                let hour = Math.floor(averageTime / 60),
                    minute = Math.floor((averageTime % 60) / 30) * 30;
                minute = minute === 0 ? '00' : minute;
                if (hour < 10) {
                    hour = '0' + hour;
                }

                const toTime = hour + ':' + minute;
                domElements.$fromTime.val(data.BeginCheckInTime);
                domElements.$toTime.val(toTime);
                domElements.$fromTimeHour.val(arrTimeIn[0]);
                domElements.$fromTimeMinute.val(arrTimeIn[1]);
                domElements.$toTimeHour.val(arrTimeOut[0]);
                domElements.$toTimeMinute.val(arrTimeOut[1]);

                $('#IsFromTimeAfterDay').prop('checked', isFromAfterday);
                $('#IsToTimeAfterDay').prop('checked', isToAfterday);

                if (isToAfterday) {
                    let toDate = new Date(domElements.$fromDate.val());
                    toDate.setDate(toDate.getDate() + 1);
                    domElements.$toDate.val(moment(toDate).format('DD MMM, YYYY'));
                }
            }

            domElements.$totalDays.val(0.5);
        }).catch(axiosError);
    };
    let bindHalfDay = function () {
        if (domElements.$halfDayOff.prop('checked')) {
            enableControlForHalfday();
            domElements.$totalDays.val(0.5);
            getShiftByDate();
        } else {
            disableControlForHalfDay();
            domElements.$totalDays.val(1);
        }
    };
    let getLeaveRequestDetails = function () {
        let $leaveDetails = $('#leaveDetails');
        if ($leaveDetails.length > 0) {
            axios({
                url: _global.root + 'leave/LeaveRequestDetail',
                method: 'GET',
                params: {
                    employeeId: domElements.$employeeId.val(),
                    fromDate: domElements.$fromDate.val(),
                    toDate: domElements.$toDate.val()
                },
                responseType: 'text'
            }).then(response => {
                $leaveDetails.html(response.data);
            }).catch(axiosError);
        }
    };
    let getTotalLeaveDays = function () {
        axios({
            url: _global.root + 'leave/getTotalLeaveDays',
            method: 'GET',
            params: {
                employeeId: domElements.$employeeId.val(),
                leaveTypeId: domElements.$leaveTypeId.val(),
                fromDate: domElements.$fromDate.val(),
                toDate: domElements.$toDate.val()
            }
        }).then(response => {
            domElements.$totalDays.html(response.data);
        }).catch(axiosError);
    };
    let getLeaveDetails = function () {
        const leaveTypeId = parseInt(domElements.$leaveTypeId.val(), 10);
        if (leaveTypeId === LeaveType.GeneralLeave) {
            getLeaveRequestDetails();
        } else {
            $('#leaveDetails').empty();
            if (domElements.$halfDayOff.prop('checked')) {
                bindHalfDay();
            } else {
                getTotalLeaveDays();
            }
        }
    };
    initDatepicker();
    $('#Employee').autocomplete({
        source: function (request, response) {
            axios({
                url: _global.root + 'request/GetEmployeesByActingUser',
                method: 'GET',
                params: {
                    role: RoleSystem.OnBehalfRequest,
                    requestType: RequestType.Leave,
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
            domElements.$employeeId.val(ui.item.value);
            $('#IsTopManager').prop('checked', ui.item.isTopManager);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            domElements.$employeeId.val(ui.item.value);
            $('#IsTopManager').prop('checked', ui.item.isTopManager);
        },
        change: function (event, ui) {
            event.preventDefault();
            if (ui && ui.item) {
                $('#Employee').val(ui.item.label);
                domElements.$employeeId.val(ui.item.value);
                $('#IsTopManager').prop('checked', ui.item.isTopManager);
                getDataAfterChangeEmployee();
                getLeaveBalance();
                getDisabledDates();
                getLeaveDetails();
            } else {
                domElements.$employeeId.val('0');
            }
        }
    });

    domElements.$leaveTypeId.on('change', function () {
        const description = LeaveRequest.leaveTypeId.children('option:selected').attr('title');
        $('#leavetype-info').attr('title', description);
        getDisabledDates();
        getLeaveDetails();
    });

    domElements.$halfDayOff.on('change', function () {
        bindHalfDay();
    });

    $('#FromDate, #ToDate').on('change', function () {
        getLeaveDetails();
        getLeaveBalance();
    });

    $('#FromTime, #ToTime, #IsFromTimeAfterDay').on('change', function () {
        if (domElements.$halfDayOff.prop('checked')) {
            if (domElements.$isFromTimeAfterDay.prop('checked') || domElements.$fromTime.val() > domElements.$toTime.val()) {
                domElements.$isToTimeAfterDay.prop('checked', true);
                let toDate = new Date(domElements.$fromDate.val());
                toDate.setDate(toDate.getDate() + 1);
                domElements.$toDate.val(moment(toDate).format('DD MMM, YYYY'));
            } else {
                domElements.$isToTimeAfterDay.prop('checked', false);
                domElements.$toDate.val(domElements.$fromDate.val());
            }
        } else {
            domElements.$isFromTimeAfterDay.prop('checked', false);
            domElements.$isToTimeAfterDay.prop('checked', false);
            domElements.$toDate.val(domElements.$fromDate.val());
        }
    });

    getLeaveRequestDetails();

    $('form').areYouSure();
});