import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery.are-you-sure/jquery.are-you-sure';
import moment from 'moment';

import { confirm, initDatepicker, resolveClientUrl } from '../../../shared/common.functions';
import { LeaveType, DateFormat, RequestType } from '../../../shared/constants';

const RequestCommon = {
    showErrorMsg: function (msg, htmlObject) {
        $('.validation-summary-valid').show();
        $('.validation-summary-valid').addClass('validation-summary-errors');
        var msgWrp = $('.validation-summary-valid').find('ul:first');
        msgWrp.append("<li>" + RequestCommon.htmlDecode(msg) + "</li>");
        htmlObject = htmlObject;
    },
    showErrorMsgs: function (nessages, htmlObject) {
        $('.validation-summary-valid').show();
        $('.validation-summary-valid').addClass('validation-summary-errors');
        var msgWrp = $('.validation-summary-valid').find('ul:first');
        nessages.forEach(function (msg) {
            msgWrp.append("<li>" + RequestCommon.htmlDecode(msg) + "</li>");
        });

        htmlObject = htmlObject;
    },
    removeErrorMsg: function (isReset) {
        if (isReset) {
            $(".validation-summary-errors").addClass("validation-summary-valid").removeClass("validation-summary-errors");
        }
        var msgWrp = $('.validation-summary-valid').removeClass('validation-summary-errors');
        msgWrp.find('ul:first').html("");
        $('.validation-summary-valid').hide();
    },
    showSuccessMsg: function (msg, url) {
        $('.validation-summary-valid').show();
        var msgWrp = $('.validation-summary-valid').addClass('success').find('ul:first');
        msgWrp.append("<li>" + RequestCommon.htmlDecode(msg) + "</li>");
        confirm('Successfull!\nPress OK to continue', function () {
            location.href = resolveClientUrl(url);
        });
    },
    htmlDecode: function (value) {
        return $('<div/>').html(value).text();
    }
};

const LeaveRequest = (function ($) {
    return {
        verifierId: null,
        approverId: null,
        leaveTypeId: null,
        leavetypeInfo: null,
        fromDate: null,
        toDate: null,
        employeeId: null,
        employee: null,
        fromTime: null,
        toTime: null,
        fromTimeHour: null,
        fromTimeMinute: null,
        toTimeHour: null,
        toTimeMinute: null,
        totalDays: null,
        halfDayOff: null,
        requestType: null,
        requestId: null,
        annualBalance: null,
        keepInViewBalance: null,
        currentLeaveBalance: null,
        isFromTimeAfterDay: null,
        isToTimeAfterDay: null,

        init: function () {
            LeaveRequest.verifierId = $('#VerifierId');
            LeaveRequest.approverId = $('#ApproverId');
            LeaveRequest.leaveTypeId = $('#LeaveTypeId');
            LeaveRequest.leavetypeInfo = $('#leavetype-info');
            LeaveRequest.fromDate = $('#FromDate');
            LeaveRequest.toDate = $('#ToDate');
            LeaveRequest.employeeId = $('#EmployeeId');
            LeaveRequest.employee = $('#Employee');
            LeaveRequest.fromTime = $('#FromTime');
            LeaveRequest.toTime = $('#ToTime');
            LeaveRequest.fromTimeHour = $('#FromTimeHour');
            LeaveRequest.fromTimeMinute = $('#FromTimeMinute');
            LeaveRequest.toTimeHour = $('#ToTimeHour');
            LeaveRequest.toTimeMinute = $('#ToTimeMinute');
            LeaveRequest.totalDays = $('#TotalDays');
            LeaveRequest.halfDayOff = $('#HalfDayOff');
            LeaveRequest.requestType = $('#RequestType');
            LeaveRequest.requestId = $('#RequestId');
            LeaveRequest.annualBalance = $('#lblAnnualBalance');
            LeaveRequest.keepInViewBalance = $('#lblKeepInViewBalance');
            LeaveRequest.keepInViewByHourBalance = $('#lblKeepInViewByHourBalance');
            LeaveRequest.currentLeaveBalance = $('#lblCurrentLeaveBalance');
            LeaveRequest.weekendLeaveTaken = $('#lblWeekendLeave');
            LeaveRequest.isFromTimeAfterDay = $('#IsFromTimeAfterDay');
            LeaveRequest.isToTimeAfterDay = $('#IsToTimeAfterDay');
            LeaveRequest.autocomplete();
            LeaveRequest.getLeaveTypeInfo();
            $('#errors-model-state').hide();

            // for edit
            initDatepicker();
            LeaveRequest.enableControlForEdit();

            LeaveRequest.halfDayOff.change(function () {
                LeaveRequest.bindHalfDay();
            });
            LeaveRequest.fromDate.change(function () {
                LeaveRequest.dateChange();
            });
            LeaveRequest.toDate.change(function () {
                LeaveRequest.dateChange();
            });
            LeaveRequest.leaveTypeId.change(function () {
                LeaveRequest.leaveTypeChange();
            });
            LeaveRequest.isFromTimeAfterDay.change(function () {
                LeaveRequest.timeChange();
            });
            LeaveRequest.fromTime.change(function () {
                LeaveRequest.timeChange();
            });
            LeaveRequest.toTime.change(function () {
                LeaveRequest.timeChange();
            });
            $('#btnSubmit').click(function (evt) {
                evt.preventDefault();
                RequestCommon.removeErrorMsg(false);
                LeaveRequest.saveLeaveRequest();
            });

            if (parseInt(LeaveRequest.leaveTypeId.val(), 10) === LeaveType.GeneralLeave) {
                LeaveRequest.showLeaveDetail(
                    LeaveRequest.employeeId.val(),
                    LeaveRequest.fromDate.val(),
                    LeaveRequest.toDate.val(),
                    LeaveRequest.requestId.val());
            }

            LeaveRequest.disableControlForDetail();

            if (LeaveRequest.isFromTimeAfterDay.prop('checked')) {
                $('.time-isafterday').show();
                LeaveRequest.isFromTimeAfterDay.trigger('change');
            }
        },
        employeeChange: function () {
            LeaveRequest.getVerifier();
            LeaveRequest.getApprover();
            LeaveRequest.getLeaveBalance();
            LeaveRequest.getDisableDate();
            LeaveRequest.getLeaveDetail();
        },
        timeChange: function () {
            if (LeaveRequest.halfDayOff.prop('checked')) {
                if (LeaveRequest.isFromTimeAfterDay.prop('checked') || LeaveRequest.fromTime.val() > LeaveRequest.toTime.val()) {
                    LeaveRequest.isToTimeAfterDay.prop('checked', true);
                    let toDate = new Date(LeaveRequest.fromDate.val());
                    toDate.setDate(toDate.getDate() + 1);
                    LeaveRequest.toDate.val(moment(toDate).format(DateFormat));
                } else {
                    LeaveRequest.isToTimeAfterDay.prop('checked', false);
                    LeaveRequest.toDate.val(LeaveRequest.fromDate.val());
                }
            } else {
                LeaveRequest.isFromTimeAfterDay.prop('checked', false);
                LeaveRequest.isToTimeAfterDay.prop('checked', false);
                LeaveRequest.toDate.val(LeaveRequest.fromDate.val());
            }
        },
        showLeaveDetail: function (employeeId, fromDate, toDate, requestId) {
            var leaveDetailContainer = $('#leaveDetails');
            if (leaveDetailContainer.length > 0) {
                $.get(resolveClientUrl('Leave/LeaveRequestDetail'),
                    { 'employeeId': employeeId, 'fromDate': fromDate, 'toDate': toDate, 'requestId': requestId })
                    .done(function (content) {
                        leaveDetailContainer.empty();
                        leaveDetailContainer.html(content);
                    });
            }
        },
        saveLeaveRequest: function () {
            const isValid = LeaveRequest.validateForm();
            if (isValid) {
                RequestCommon.removeErrorMsg(false);
                const leaveTypeId = parseInt(LeaveRequest.leaveTypeId.val(), 10);
                let leaveRequest = {
                    RequestId: LeaveRequest.requestId.val(),
                    EmployeeId: LeaveRequest.employeeId.val(),
                    EmployeeName: LeaveRequest.employee.val(),
                    RequestType: RequestType.Leave,
                    FromDate: LeaveRequest.fromDate.val(),
                    ToDate: LeaveRequest.toDate.val(),
                    LeaveTypeId: leaveTypeId,
                    LeaveTypeName: LeaveRequest.leaveTypeId.find(':selected').text(),
                    TotalDays: LeaveRequest.totalDays.val(),
                    RefNo: $('#RefNo').val(),
                    Remark: $('#Remark').val(),
                    VerifierId: LeaveRequest.verifierId.val(),
                    ApproverId: LeaveRequest.approverId.val()
                };
                var requestDetails = [];
                var leaveDate = '';
                var isHalfDay = false;
                var fromTime = new Date();
                var toTime = new Date();
                if (leaveTypeId === LeaveType.GeneralLeave) {
                    $('.tabledata tbody tr').each(function () {
                        leaveDate = $.trim($('td.tdleavedate', this).first().attr('data-date'));
                        isHalfDay = $('td input.chkhalfday', this).first().prop('checked');
                        fromTime = new Date(leaveDate + (isHalfDay ? (' ' + $('td select.ddlfromtime', this).first().val()) : ''));
                        toTime = new Date(leaveDate + (isHalfDay ? (' ' + $('td select.ddltotime', this).first().val()) : ''));

                        if ($('td input.chkIsFromAfterday', this).first().prop('checked')) {
                            fromTime.setDate(fromTime.getDate() + 1);
                        }

                        if ($('td input.chkIsToAfterday', this).first().prop('checked')) {
                            toTime.setDate(toTime.getDate() + 1);
                        }

                        requestDetails.push({
                            OnDate: leaveDate,
                            WorkType: $('td select.ddlWorkType', this).first().val(),
                            LeaveTypeId: $('td select.ddlleavetype', this).first().val(),
                            IsHalfDay: isHalfDay,
                            FromTime: fromTime,
                            ToTime: toTime
                        });
                    });
                } else if (LeaveRequest.halfDayOff.prop('checked')) {
                    var fromDate = new Date(LeaveRequest.fromDate.val());
                    var toDate = new Date(LeaveRequest.fromDate.val());
                    var fromDateTime;
                    var toDateTime;

                    if (LeaveRequest.isFromTimeAfterDay.prop('checked')) {
                        fromDate.setDate(fromDate.getDate() + 1);
                    }

                    if (LeaveRequest.isToTimeAfterDay.prop('checked')) {
                        toDate.setDate(toDate.getDate() + 1);
                    }

                    if (leaveTypeId === LeaveType.BusinessTrip || leaveTypeId === LeaveType.KeepInViewByHour
                        || leaveTypeId === LeaveType.AnnualLeave || leaveTypeId === LeaveType.UnpaidLeave
                        || leaveTypeId === LeaveType.KeepInViewByDay || leaveTypeId === LeaveType.BusinessDuringWorking) {
                        fromDateTime = moment(fromDate).format(DateFormat) + ' ' + LeaveRequest.leftPadding(LeaveRequest.fromTimeHour.val(), 2)
                            + ':' + LeaveRequest.leftPadding(LeaveRequest.fromTimeMinute.val(), 2);
                        toDateTime = moment(toDate).format(DateFormat) + ' ' + LeaveRequest.leftPadding(LeaveRequest.toTimeHour.val(), 2)
                            + ':' + LeaveRequest.leftPadding(LeaveRequest.toTimeMinute.val(), 2);
                    } else {
                        fromDateTime = moment(fromDate).format(DateFormat) + ' ' + LeaveRequest.fromTime.val();
                        toDateTime = moment(toDate).format(DateFormat) + ' ' + LeaveRequest.toTime.val();
                    }

                    requestDetails.push({
                        OnDate: LeaveRequest.fromDate.val(),
                        WorkType: 2,
                        LeaveTypeId: LeaveRequest.leaveTypeId.val(),
                        IsHalfDay: true,
                        FromTime: fromDateTime,
                        ToTime: toDateTime
                    });
                }

                if (LeaveRequest.requestId.val() === '-1') {
                    LeaveRequest.insertLeaveRequest(leaveRequest, requestDetails);
                }
                else {
                    LeaveRequest.updateLeaveRequest(leaveRequest, requestDetails);
                }
            }
        },
        insertLeaveRequest: function (leaveRequest, requestDetails) {
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: resolveClientUrl('Leave/Create'),
                data: { request: JSON.stringify(leaveRequest), requestDetails: JSON.stringify(requestDetails) },
                success: function (response) {
                    if (response.IsSuccessful) {
                        $('form').removeClass('dirty');
                        var successMsg = 'Your request have been sent successfully!';
                        RequestCommon.showSuccessMsg(successMsg, 'Leave/EmployeeView');
                    }
                    else {
                        RequestCommon.showErrorMsg(response.Message, LeaveRequest.employee);
                    }
                }
            });
        },
        updateLeaveRequest: function (leaveRequest, requestDetails) {
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: resolveClientUrl('Leave/Update'),
                data: { request: JSON.stringify(leaveRequest), requestDetails: JSON.stringify(requestDetails) },
                success: function (response) {
                    if (response.IsSuccessful) {
                        $('form').removeClass('dirty');
                        var successMsg = 'Your request have been sent successfully!';
                        RequestCommon.showSuccessMsg(successMsg, 'Leave/EmployeeView');
                    }
                    else {
                        RequestCommon.showErrorMsg(response.Message, LeaveRequest.employee);
                    }
                }
            });
        },
        dateChange: function () {
            LeaveRequest.getLeaveDetail();
            LeaveRequest.getLeaveBalance();
        },
        getLeaveTypeInfo: function () {
            const description = LeaveRequest.leaveTypeId.children('option:selected').attr('title');
            LeaveRequest.leavetypeInfo.attr('title', description ? description : '');
            if (description && description.length > 0) {
                LeaveRequest.leavetypeInfo.show();
            } else {
                LeaveRequest.leavetypeInfo.hide();
            }
        },
        leaveTypeChange: function () {
            LeaveRequest.getLeaveTypeInfo();
            LeaveRequest.getDisableDate();
            LeaveRequest.getLeaveDetail();
        },
        bindHalfDay: function () {
            if (LeaveRequest.halfDayOff.prop('checked')) {
                LeaveRequest.enableControlForHalfday();
                LeaveRequest.totalDays.val(0.5);
                LeaveRequest.getTimeByShift();
            } else {
                LeaveRequest.disableControlForHalfDay();
                LeaveRequest.totalDays.val(1);
            }
        },
        enableControlForHalfday: function () {
            const leaveTypeId = parseInt(LeaveRequest.leaveTypeId.val(), 10);
            if (leaveTypeId === LeaveType.GeneralLeave) {
                $('.time-businesstrip').hide();
                $('.time').hide();
                $('#grbHalfDay').hide();
            } else if (leaveTypeId === LeaveType.KeepInViewByHour || leaveTypeId === LeaveType.BusinessDuringWorking) {
                $('.time-businesstrip').show();
                $('.time').hide();
                $('#grbHalfDay').hide();
                $('#grbTotalDay').hide();
            } else {
                $('.time-businesstrip').show();
                $('.time').hide();
            }

            LeaveRequest.toDate.val(LeaveRequest.fromDate.val());
            LeaveRequest.toDate.datepicker('disable').prop('disabled', false);
        },
        disableControlForHalfDay: function () {
            $('.time').hide();
            $('.time-businesstrip').hide();
            $('.time-isafterday').hide();

            LeaveRequest.toDate.datepicker('enable');
        },
        getTimeByShift: function () {
            $.getJSON(resolveClientUrl('shift/getshiftbydate'),
                { employeeId: LeaveRequest.employeeId.val(), date: LeaveRequest.fromDate.val() },
                function (data) {
                    if (data !== null) {
                        const arrTimeIn = data.BeginCheckInTime.split(':'),
                            arrTimeOut = data.BeginCheckOutTime.split(':');

                        let minuteFrom = parseInt(arrTimeIn[0], 10) * 60 + parseInt(arrTimeIn[1], 10),
                            minuteTo = parseInt(arrTimeOut[0], 10) * 60 + parseInt(arrTimeOut[1], 10),
                            isFromAfterday = data.IsInAfterDay,
                            isToAfterday = data.IsOutAfterDay;
                        if (data.IsInAfterDay) {
                            $('.time-isafterday').show();
                            minuteFrom += 1440; //60*24
                        } else {
                            $('.time-isafterday').hide();
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

                        var toTime = hour + ':' + minute;
                        LeaveRequest.fromTime.val(data.BeginCheckInTime);
                        LeaveRequest.toTime.val(toTime);
                        LeaveRequest.fromTimeHour.val(arrTimeIn[0]);
                        LeaveRequest.fromTimeMinute.val(arrTimeIn[1]);
                        LeaveRequest.toTimeHour.val(arrTimeOut[0]);
                        LeaveRequest.toTimeMinute.val(arrTimeOut[1]);

                        LeaveRequest.isFromTimeAfterDay.prop('checked', isFromAfterday);
                        LeaveRequest.isToTimeAfterDay.prop('checked', isToAfterday);

                        if (isToAfterday) {
                            let toDate = new Date(LeaveRequest.fromDate.val());
                            toDate.setDate(toDate.getDate() + 1);
                            LeaveRequest.toDate.val(moment(toDate).format(DateFormat));
                        }
                    }

                    LeaveRequest.totalDays.val(0.5);
                });
        },
        autocomplete: function () {
            const requestType = LeaveRequest.requestType.val();
            LeaveRequest.employee.autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: resolveClientUrl('request/GetEmployeesByActingUser'),
                        data: {
                            role: 5,
                            requestType: requestType,
                            query: request.term
                        },
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.NickName + ' (' + item.FullName + ')',
                                    value: item.OwnerId,
                                    isTopManager: item.IsTopManager
                                };
                            }));
                        }
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    event.preventDefault();
                    LeaveRequest.employee.val(ui.item.label);
                    LeaveRequest.employeeId.val(ui.item.value);
                    $('#IsTopManager').prop('checked', ui.item.isTopManager);
                    LeaveRequest.employeeChange();
                    RequestCommon.removeErrorMsg(true);
                },
                focus: function (event, ui) {
                    event.preventDefault();
                    LeaveRequest.employee.val(ui.item.label);
                    LeaveRequest.employeeId.val(ui.item.value);
                    $('#IsTopManager').prop('checked', ui.item.isTopManager);
                },
                change: function (event, ui) {
                    event.preventDefault();
                    if (ui && ui.item) {
                        LeaveRequest.employee.val(ui.item.label);
                        LeaveRequest.employeeId.val(ui.item.value);
                        $('#IsTopManager').prop('checked', ui.item.isTopManager);
                    } else {
                        LeaveRequest.employeeId.val('0');
                    }
                }
            });
        },
        hideHalfday: function () {
            const leaveTypeId = parseInt(LeaveRequest.leaveTypeId.val(), 10);
            if (!(leaveTypeId === LeaveType.AnnualLeave
                || leaveTypeId === LeaveType.UnpaidLeave
                || leaveTypeId === LeaveType.BusinessTrip
                || leaveTypeId === LeaveType.KeepInViewByDay)) {
                LeaveRequest.halfDayOff.prop('checked', false);
                LeaveRequest.disableControlForHalfDay();
                $('#grbHalfDay').hide();
            }
        },
        enableControlForEdit: function () {
            $('#RefNo').attr('disabled', 'disabled');
            if (parseInt(LeaveRequest.leaveTypeId.val(), 10) === LeaveType.GeneralLeave) {
                $('#grbHalfDay').hide();
                $('#grbTotalDay').hide();
            }

            if (LeaveRequest.halfDayOff.prop('checked')) {
                LeaveRequest.enableControlForHalfday();
            } else {
                LeaveRequest.hideHalfday();
            }
        },
        bindLeaveRequestDetailForEdit: function () {
            if (parseInt(LeaveRequest.leaveTypeId.val(), 10) === LeaveType.GeneralLeave) {
                LeaveRequest.showLeaveDetail(LeaveRequest.employeeId.val(),
                    LeaveRequest.fromDate.val(),
                    LeaveRequest.toDate.val(),
                    LeaveRequest.requestId.val());
            }
        },
        disableControlForDetail: function () {
            if ($('#IsView').val() === 'True') {
                $('.form-horizontal input').attr('disabled', 'disabled');
                $('select').attr('disabled', 'disabled');
                $('textarea').attr('disabled', 'disabled');

                if (!LeaveRequest.halfDayOff.prop('checked')) {
                    LeaveRequest.hideHalfday();
                }
            }
        },
        validateForm: function () {
            var isValid = true;
            var errorString = [];
            var empName = LeaveRequest.employee.val();
            var fromDate = LeaveRequest.fromDate.val();
            var toDate = LeaveRequest.toDate.val();
            var approver = LeaveRequest.approverId.val();
            var verifier = LeaveRequest.verifierId.val();
            var remark = $('#Remark').val();

            var start = $("[name='FromDate']").datepicker('getDate');
            var end = $("[name='ToDate']").datepicker('getDate');
            var days = (end - start) / 1000 / 60 / 60 / 24;

            if (empName !== undefined && (empName === null || empName === '')) {
                errorString.push('The Employee field is required.');
            }

            if (fromDate === null || fromDate === '') {
                errorString.push('From Date field is required.');
            }

            if (toDate === null || toDate === '') {
                errorString.push('To Date field is required.');
            }

            if (LeaveRequest.halfDayOff.prop('checked')) {
                if (parseInt(LeaveRequest.leaveTypeId.val(), 10) === LeaveType.BusinessTrip) {
                    if (parseInt(LeaveRequest.fromTimeHour.val()) > 23
                        || parseInt(LeaveRequest.fromTimeMinute.val()) > 59
                        || parseInt(LeaveRequest.toTimeHour.val()) > 23
                        || parseInt(LeaveRequest.toTimeMinute.val()) > 59
                    ) {
                        errorString.push('Time is not valid.');
                    }
                }
            }

            if (!$('#IsTopManager').prop('checked')) {
                if (verifier === null || verifier === '') {
                    errorString.push('Verified by field is required.');
                }
                if (approver === null || approver === '') {
                    errorString.push('Approved by field is required.');
                }
            }

            if (days < 0) {
                errorString.push('To Date must be greater than From Date.');
            }

            if ($.trim(remark).length === 0) {
                errorString.push('The Remark field is required.');
            }

            if (errorString.length > 0) {
                RequestCommon.showErrorMsgs(errorString);
                isValid = false;
            }

            return isValid;
        },
        getVerifier: function () {
            $.getJSON(resolveClientUrl('Request/GetVerifiers'),
                {
                    employeeId: LeaveRequest.employeeId.val(),
                    requestType: LeaveRequest.requestType.val()
                },
                function (response) {
                    if (response !== null) {
                        var options = '';
                        if (response.length > 1) {
                            options = "<option value=''>--Select a Verifier--</option>";
                        }
                        for (var i = 0; i < response.length; i++) {
                            options += "<option value='" + response[i].OwnerId + "'>" + response[i].NickName + " (" + response[i].FullName + ")</option>";
                        }

                        LeaveRequest.verifierId.empty();
                        LeaveRequest.verifierId.html(options);
                    }
                });
        },
        getApprover: function () {
            $.getJSON(resolveClientUrl('Request/GetApprovers'),
                { employeeId: LeaveRequest.employeeId.val(), requestType: LeaveRequest.requestType.val() },
                function (response) {
                    if (response !== null) {
                        var options = '';
                        if (response.length > 1) {
                            options = "<option value=''>--Select a Verifier--</option>";
                        }

                        for (var i = 0; i < response.length; i++) {
                            options += "<option value='" + response[i].OwnerId + "'>" + response[i].NickName + " (" + response[i].FullName + ")</option>";
                        }

                        LeaveRequest.approverId.empty();
                        LeaveRequest.approverId.html(options);
                    }
                });
        },
        getLeaveBalance: function () {
            $.getJSON(resolveClientUrl('leave/GetLeaveBalance'),
                {
                    employeeId: LeaveRequest.employeeId.val(),
                    requestId: LeaveRequest.requestId.val(),
                    toDate: LeaveRequest.toDate.val()
                },
                function (response) {
                    if (response !== null) {
                        LeaveRequest.annualBalance.html(response.AnnualBalance);
                        LeaveRequest.keepInViewBalance.html(response.KeepInViewBalance);
                        LeaveRequest.keepInViewByHourBalance.html(response.KeepInViewByHourBalance);
                        LeaveRequest.currentLeaveBalance.html(response.CurrentLeaveBalanceDay);
                        LeaveRequest.weekendLeaveTaken.html(response.WeekedLeaveDayTaken);
                    }
                });
        },
        getDateRange: function () {
            $.ajax({
                url: resolveClientUrl('employee/GetRangeDate'),
                data: {
                    leaveTypeId: LeaveRequest.leaveTypeId.val(),
                    employeeId: LeaveRequest.employeeId.val(),
                    fromDate: LeaveRequest.fromDate.val(),
                    toDate: LeaveRequest.toDate.val()
                },
                async: false,
                success: function (response) {
                    if (response !== null) {
                        LeaveRequest.totalDays.val(response);
                    }
                }
            });
        },
        getUnavailableDate: function () {
            $.getJSON(resolveClientUrl('employee/GetUnavailableDates'),
                {
                    employeeId: LeaveRequest.employeeId.val(),
                    date: new Date().toDateString(),
                    leaveTypeId: LeaveRequest.leaveTypeId.val()
                },
                function (response) {
                    if (!$.isEmptyObject(response)) {
                        LeaveRequest.fromDate.attr('disabledDate', response.join(','));
                        LeaveRequest.toDate.attr('disabledDate', response.join(','));
                    }
                    else {
                        LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                        LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
                    }
                });
        },
        getDisableDate: function () {
            var minDate = new Date(LeaveRequest.fromDate.attr('minDate'));
            var tempDate = new Date(new Date(minDate).setMonth(minDate.getMonth() + 12));
            const leaveTypeId = parseInt(LeaveRequest.leaveTypeId.val(), 10);

            if (leaveTypeId === LeaveType.GeneralLeave) {
                $('#grbHalfDay').hide();
                $('#grbTotalDay').hide();
                $('.time').hide();
                $('.time-businesstrip').hide();
                $('.time-isafterday').hide();
                LeaveRequest.toDate.datepicker('enable');
                LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
            } else if (leaveTypeId === LeaveType.KeepInViewByHour
                || leaveTypeId === LeaveType.BusinessDuringWorking) {
                LeaveRequest.halfDayOff.prop('checked', true);
                $('#grbTotalDay').hide();
            } else {
                $('#grbTotalDay').show();
                if (leaveTypeId === LeaveType.AnnualLeave
                    || leaveTypeId === LeaveType.UnpaidLeave
                    || leaveTypeId === LeaveType.BusinessTrip
                    || leaveTypeId === LeaveType.KeepInViewByDay) {
                    $('#grbHalfDay').show();
                } else {
                    LeaveRequest.halfDayOff.prop('checked', false);
                    LeaveRequest.bindHalfDay();
                    $('#grbHalfDay').hide();
                }

                if (leaveTypeId === LeaveType.MaternityLeave) {
                    tempDate = new Date(new Date(minDate).setMonth(minDate.getMonth() + 12));
                    LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                    LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
                } else if (leaveTypeId === LeaveType.PublicHoliday) {
                    LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                    LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
                } else if (leaveTypeId === LeaveType.CompensationLeave) {
                    tempDate = new Date(new Date(minDate).setMonth(minDate.getMonth() + 13));
                    LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                    LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
                } else {
                    LeaveRequest.getUnavailableDate();
                }
            }

            var maxDate = new Date(1900 + tempDate.getYear(), tempDate.getMonth(), 0);
            LeaveRequest.fromDate.datepicker('option', 'maxDate', maxDate);
            LeaveRequest.toDate.datepicker('option', 'maxDate', maxDate);
        },
        getLeaveDetail: function () {
            if (parseInt(LeaveRequest.leaveTypeId.val(), 10) === LeaveType.GeneralLeave) {
                $('#leaveDetailContainer').show();
                LeaveRequest.showLeaveDetail(
                    LeaveRequest.employeeId.val(),
                    LeaveRequest.fromDate.val(),
                    LeaveRequest.toDate.val(),
                    LeaveRequest.leaveTypeId.val());
            } else {
                $('#leaveDetailContainer').hide();
                $('#leaveDetails').empty();
                if (LeaveRequest.halfDayOff.prop('checked')) {
                    LeaveRequest.bindHalfDay();
                } else {
                    LeaveRequest.getDateRange();
                }
            }
        },
        leftPadding: function (i, l, s) {
            var o = i.toString();
            if (!s) { s = '0'; }
            while (o.length < l) {
                o = s + o;
            }
            return o;
        }
    };
}(jQuery));

$(document).ready(function () {
    LeaveRequest.init();
    $('form').areYouSure();
});