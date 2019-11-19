'use strict';
/*global _global, RequestCommon*/

var LeaveRequest = (function ($) {
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
        generalLeaveTypeId: null,
        maternityLeaveTypeId: null,
        publicHolidayLeaveTypeId: null,
        keepInViewByHourLeaveTypeId: null,
        isFromTimeAfterDay: null,
        isToTimeAfterDay: null,
        isTopManager: false,

        init: function () {
            LeaveRequest.generalLeaveTypeId = '0';
            LeaveRequest.annualLeaveTypeId = '2';
            LeaveRequest.maternityLeaveTypeId = '12';
            LeaveRequest.publicHolidayLeaveTypeId = '13';
            LeaveRequest.compensationLeaveTypeId = '27';
            LeaveRequest.businessTripLeaveTypeId = '9';
            LeaveRequest.businessDuringWorkingLeaveTypeId = '26';
            LeaveRequest.keepInViewByDayLeaveTypeId = '10';
            LeaveRequest.unpaidLeaveTypeId = '25';
            LeaveRequest.keepInViewByHourLeaveTypeId = '48';

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

            LeaveRequest.totalDays.prop('readonly', true);

            $('#errors-model-state').hide();
            $('.datepicker').datepicker().attr('readonly', 'readonly');
            // for edit
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

            if (LeaveRequest.leaveTypeId.val() === LeaveRequest.generalLeaveTypeId) {
                LeaveRequest.showLeaveDetail(
                    LeaveRequest.employeeId.val(),
                    LeaveRequest.fromDate.val(),
                    LeaveRequest.toDate.val(),
                    LeaveRequest.requestId.val());
            }
            LeaveRequest.disableControlForDetail();
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
                    var toDate = new Date(LeaveRequest.fromDate.val());
                    toDate.setDate(toDate.getDate() + 1);
                    LeaveRequest.toDate.val(toDate.format('dd mmm, yyyy'));
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
                $.get(_global.root + 'Leave/LeaveRequestDetail',
                    { 'employeeId': employeeId, 'fromDate': fromDate, 'toDate': toDate, 'requestId': requestId })
                    .done(function (content) {
                        leaveDetailContainer.empty();
                        leaveDetailContainer.html(content);
                    });
            }
        },
        saveLeaveRequest: function () {
            LeaveRequest.checkIsTopManager();
            var validate = LeaveRequest.validateForm();
            if (validate) {
                RequestCommon.removeErrorMsg(false);
                var leaveRequest = {
                    RequestId: LeaveRequest.requestId.val(),
                    EmployeeId: LeaveRequest.employeeId.val(),
                    EmployeeName: LeaveRequest.employee.val(),
                    RequestType: 6,
                    FromDate: LeaveRequest.fromDate.val(),
                    ToDate: (LeaveRequest.halfDayOff.prop('checked') ? LeaveRequest.fromDate.val() : LeaveRequest.toDate.val()),
                    LeaveTypeId: LeaveRequest.leaveTypeId.val(),
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
                if (LeaveRequest.leaveTypeId.val() === LeaveRequest.generalLeaveTypeId) {
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
                }
                else if (LeaveRequest.halfDayOff.prop('checked')) {
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

                    if (LeaveRequest.leaveTypeId.val() === LeaveRequest.businessTripLeaveTypeId) {
                        fromDateTime = fromDate.format('dd mmm, yyyy') + ' ' + LeaveRequest.leftPadding(LeaveRequest.fromTimeHour.val(), 2)
                            + ':' + LeaveRequest.leftPadding(LeaveRequest.fromTimeMinute.val(), 2);
                        toDateTime = toDate.format('dd mmm, yyyy') + ' ' + LeaveRequest.leftPadding(LeaveRequest.toTimeHour.val(), 2)
                            + ':' + LeaveRequest.leftPadding(LeaveRequest.toTimeMinute.val(), 2);
                    }
                    else {
                        fromDateTime = fromDate.format('dd mmm, yyyy') + ' ' + LeaveRequest.fromTime.val();
                        toDateTime = toDate.format('dd mmm, yyyy') + ' ' + LeaveRequest.toTime.val();
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
                url: _global.root + 'Leave/Create',
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
                url: _global.root + 'Leave/Update',
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
        leaveTypeChange: function () {
            var description = LeaveRequest.leaveTypeId.children('option:selected').attr('title');
            LeaveRequest.leavetypeInfo.attr('title', description ? description : '');
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
            var leaveTypeId = LeaveRequest.leaveTypeId.val();
            if (leaveTypeId === LeaveRequest.businessTripLeaveTypeId) {
                $('.time-businesstrip').show();
                $('.time').hide();
            } else if (leaveTypeId === LeaveRequest.keepInViewByHourLeaveTypeId) {
                $('.time-businesstrip').show();
                $('.time').hide();
                $('#grbHalfDay').hide();
                $('#grbTotalDay').hide();
            } else if (leaveTypeId === LeaveRequest.businessDuringWorkingLeaveTypeId) {
                $('.time-businesstrip').hide();
                $('.time').show();
                $('#grbHalfDay').hide();
                $('#grbTotalDay').hide();
            } else {
                $('.time-businesstrip').hide();
                $('.time').show();
            }

            LeaveRequest.toDate.val(LeaveRequest.fromDate.val());
            LeaveRequest.toDate.prop('readonly', true).datepicker('disable').prop('disabled', false);
        },
        disableControlForHalfDay: function () {
            $('.time').hide();
            $('.time-businesstrip').hide();
            $('.time-isafterday').hide();

            LeaveRequest.toDate.prop('readonly', false).datepicker('enable');
        },
        getTimeByShift: function () {
            $.getJSON(_global.root + 'shift/getshiftbydate',
                { employeeId: LeaveRequest.employeeId.val(), date: LeaveRequest.fromDate.val() },
                function (data) {
                    if (data !== null) {
                        var arrTimeIn = data.BeginCheckInTime.split(':');
                        var arrTimeOut = data.BeginCheckOutTime.split(':');

                        var minuteFrom = (parseInt(arrTimeIn[0], 10) * 60) + parseInt(arrTimeIn[1], 10);
                        var minuteTo = (parseInt(arrTimeOut[0], 10) * 60) + parseInt(arrTimeOut[1], 10);
                        var isFromAfterday = data.IsInAfterDay;
                        var isToAfterday = false;
                        if (data.IsInAfterDay) {
                            minuteFrom += 1440; //60*24
                        }

                        if (data.IsOutAfterDay) {
                            minuteTo += 1440;
                        }

                        var averageTime = (minuteFrom + minuteTo) / 2;
                        if (averageTime >= 1440) {
                            averageTime = averageTime - 1440;
                            isToAfterday = true;
                        }

                        var hour = Math.floor(averageTime / 60);
                        var minute = Math.floor((averageTime % 60) / 30) * 30;
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
                            var toDate = new Date(LeaveRequest.fromDate.val());
                            toDate.setDate(toDate.getDate() + 1);
                            LeaveRequest.toDate.val(toDate.format('dd mmm, yyyy'));
                        }
                    }

                    LeaveRequest.totalDays.val(0.5);
                });
        },
        autocomplete: function () {
            var nickName;
            var requestType = LeaveRequest.requestType.val();
            LeaveRequest.employee.autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: _global.root + 'request/GetEmployeesByActingUser',
                        data: {
                            role: 5,
                            requestType: requestType,
                            query: request.term
                        },
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.NickName + ' (' + item.FullName + ')',
                                    value: item.OwnerId
                                };
                            }));
                        }
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    nickName = ui.item.label;
                    LeaveRequest.employee.val(nickName);
                    LeaveRequest.employeeId.val(ui.item.value);
                    LeaveRequest.employeeChange();
                    RequestCommon.removeErrorMsg(true);
                },
                focus: function (event, ui) {
                    nickName = ui.item.label;
                    LeaveRequest.employee.val(nickName);
                    LeaveRequest.employeeId.val(ui.item.value);
                    event.preventDefault();
                },
                close: function () {
                    LeaveRequest.employee.val(nickName);
                }
            });
        },
        hideHalfday: function () {
            var leaveTypeId = LeaveRequest.leaveTypeId.val();
            if (!(leaveTypeId === LeaveRequest.annualLeaveTypeId
                || leaveTypeId === LeaveRequest.unpaidLeaveTypeId
                || leaveTypeId === LeaveRequest.businessTripLeaveTypeId
                || leaveTypeId === LeaveRequest.keepInViewByDayLeaveTypeId)) {
                LeaveRequest.halfDayOff.prop('checked', false);
                LeaveRequest.disableControlForHalfDay();
                $('#grbHalfDay').hide();
            }
        },
        enableControlForEdit: function () {
            $('#RefNo').attr('disabled', 'disabled');
            if (LeaveRequest.leaveTypeId.val() === LeaveRequest.generalLeaveTypeId) {
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
            if (LeaveRequest.leaveTypeId.val() === LeaveRequest.generalLeaveTypeId) {
                LeaveRequest.showLeaveDetail(LeaveRequest.employeeId.val(),
                    LeaveRequest.fromDate.val(),
                    LeaveRequest.toDate.val(),
                    LeaveRequest.requestId.val());
            }
        },
        disableControlForDetail: function () {
            if ($('#IsView').val() === 'True') {
                $('input').attr('disabled', 'disabled');
                $('select').attr('disabled', 'disabled');
                $('textarea').attr('disabled', 'disabled');

                if (!LeaveRequest.halfDayOff.prop('checked')) {
                    LeaveRequest.hideHalfday();
                }
            }
        },
        validateForm: function () {
            var check = true;
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
                if (LeaveRequest.leaveTypeId.val() === LeaveRequest.businessTripLeaveTypeId) {
                    if (parseInt(LeaveRequest.fromTimeHour.val()) > 23
                        || parseInt(LeaveRequest.fromTimeMinute.val()) > 59
                        || parseInt(LeaveRequest.toTimeHour.val()) > 23
                        || parseInt(LeaveRequest.toTimeMinute.val()) > 59
                    ) {
                        errorString.push('Time is not valid.');
                    }
                }
            }

            if (LeaveRequest.isTopManager === false) {
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
                check = false;
            }

            return check;
        },
        checkIsTopManager: function () {
            var employeeId = $('#EmployeeId').val();
            if (employeeId !== null) {
                $.ajax({
                    url: _global.root + 'employee/GetEmployee',
                    data: {
                        id: employeeId
                    },
                    async: false,
                    success: function (data) {
                        $(data).each(function (index, item) {
                            LeaveRequest.isTopManager = item.IsTopManager;
                        });
                    }
                });
            }
        },
        getVerifier: function () {
            $.getJSON(_global.root + 'Request/GetVerifiers', { employeeId: LeaveRequest.employeeId.val(), requestType: LeaveRequest.requestType.val() },
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
            $.getJSON(_global.root + 'Request/GetApprovers', { employeeId: LeaveRequest.employeeId.val(), requestType: LeaveRequest.requestType.val() },
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
            $.getJSON(_global.root + 'leave/GetLeaveBalance',
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
                url: _global.root + 'employee/GetRangeDate',
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
            $.getJSON(_global.root + 'employee/GetUnavailableDates',
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
            var leaveTypeId = LeaveRequest.leaveTypeId.val();

            if (leaveTypeId === LeaveRequest.generalLeaveTypeId) {
                $('#grbHalfDay').hide();
                $('#grbTotalDay').hide();
                $('.time').hide();
                $('.time-businesstrip').hide();
                $('.time-isafterday').hide();
                LeaveRequest.toDate.prop('readonly', false).datepicker('enable');
                LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
            } else if (leaveTypeId === LeaveRequest.keepInViewByHourLeaveTypeId
                || leaveTypeId === LeaveRequest.businessDuringWorkingLeaveTypeId) {
                LeaveRequest.halfDayOff.prop('checked', true);
                $('#grbTotalDay').hide();
            } else {
                $('#grbTotalDay').show();
                if (leaveTypeId === LeaveRequest.annualLeaveTypeId
                    || leaveTypeId === LeaveRequest.unpaidLeaveTypeId
                    || leaveTypeId === LeaveRequest.businessTripLeaveTypeId
                    || leaveTypeId === LeaveRequest.keepInViewByDayLeaveTypeId) {
                    $('#grbHalfDay').show();
                } else {
                    LeaveRequest.halfDayOff.prop('checked', false);
                    LeaveRequest.bindHalfDay();
                    $('#grbHalfDay').hide();
                }
                if (leaveTypeId === LeaveRequest.maternityLeaveTypeId) {
                    tempDate = new Date(new Date(minDate).setMonth(minDate.getMonth() + 12));
                    LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                    LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
                } else if (leaveTypeId === LeaveRequest.publicHolidayLeaveTypeId) {
                    LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                    LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
                } else if (leaveTypeId === LeaveRequest.compensationLeaveTypeId) {
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
            if (LeaveRequest.leaveTypeId.val() === LeaveRequest.generalLeaveTypeId) {
                LeaveRequest.showLeaveDetail(
                    LeaveRequest.employeeId.val(),
                    LeaveRequest.fromDate.val(),
                    LeaveRequest.toDate.val(),
                    LeaveRequest.leaveTypeId.val());
            }
            else {
                var leaveDetailContainer = $('#leaveDetails');
                leaveDetailContainer.empty();

                if (LeaveRequest.halfDayOff.prop('checked')) {
                    LeaveRequest.bindHalfDay();
                }
                else {
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