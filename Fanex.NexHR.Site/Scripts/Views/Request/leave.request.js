'use strict';
/*global _global, RequestCommon*/

var LeaveRequest = (function ($) {
	return {
		verifierId: null,
		approverId: null,
		leaveTypeId: null,
		fromDate: null,
		toDate: null,
		employeeId: null,
		employee: null,
		fromTime: null,
		toTime: null,
		totalDays: null,
		halfDayOff: null,
		requestType: null,
		requestId: null,
		annualBalance: null,
		keepInViewBalance: null,
		currentLeaveBalance: null,
		generalLeaveType: null,
		maternityLeaveType: null,
		isFromTimeAfterDay: null,
		isToTimeAfterDay: null,
		isTopManager: false,

		init: function () {
			LeaveRequest.generalLeaveType = "0";
			LeaveRequest.maternityLeaveType = "12";
			LeaveRequest.verifierId = $("#VerifierId");
			LeaveRequest.approverId = $("#ApproverId");
			LeaveRequest.leaveTypeId = $("#LeaveTypeId");
			LeaveRequest.fromDate = $("#FromDate");
			LeaveRequest.toDate = $("#ToDate");
			LeaveRequest.employeeId = $("#EmployeeId");
			LeaveRequest.employee = $("#Employee");
			LeaveRequest.fromTime = $("#FromTime");
			LeaveRequest.toTime = $("#ToTime");
			LeaveRequest.totalDays = $("#TotalDays");
			LeaveRequest.halfDayOff = $("#HalfDayOff");
			LeaveRequest.requestType = $("#RequestType");
			LeaveRequest.requestId = $("#RequestId");
			LeaveRequest.annualBalance = $("#lblAnnualBalance");
			LeaveRequest.keepInViewBalance = $("#lblKeepInViewBalance");
			LeaveRequest.currentLeaveBalance = $("#lblCurrentLeaveBalance");
			LeaveRequest.isFromTimeAfterDay = $("#IsFromTimeAfterDay");
			LeaveRequest.isToTimeAfterDay = $("#IsToTimeAfterDay");

			LeaveRequest.autocomplete();
			LeaveRequest.totalDays.prop('readonly', true);
			$("#errors-model-state").hide();
			$(".datepicker").datepicker().attr('readonly', 'readonly');
			// for edit
			LeaveRequest.enableControlForEdit();
			LeaveRequest.bindLeaveRequestDetailForEdit();
			//
			LeaveRequest.halfDayOff.change(function () {
				LeaveRequest.bindHalfDay();
			});
			LeaveRequest.fromDate.change(function () {
				var isHalfday = LeaveRequest.halfDayOff.attr("checked");
				LeaveRequest.dateChange(isHalfday);

			});
			LeaveRequest.toDate.change(function () {
				var isHalfday = LeaveRequest.halfDayOff.attr("checked");
				LeaveRequest.dateChange(isHalfday);
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

			LeaveRequest.timeChange();
			LeaveRequest.disableControlForDetail();
		},
		timeChange: function () {
			if (LeaveRequest.halfDayOff.attr("checked") === "checked") {
				var checked = LeaveRequest.isFromTimeAfterDay.attr("checked");
				if (checked === "checked" || LeaveRequest.fromTime.val() > LeaveRequest.toTime.val()) {
					LeaveRequest.isToTimeAfterDay.attr("checked", "checked");
					var toDate = new Date(LeaveRequest.fromDate.val());
					toDate.setDate(toDate.getDate() + 1);
					LeaveRequest.toDate.val(toDate.format("dd mmm, yyyy"));
				}
				else {
					LeaveRequest.isToTimeAfterDay.removeAttr("checked");
					LeaveRequest.toDate.val(LeaveRequest.fromDate.val());
				}
			}
		},
		showLeaveDetail: function (employeeId, fromDate, toDate, requestId) {
			var leaveDetailContainer = $("#tbldetail");
			if (leaveDetailContainer.length > 0) {
				$.get(_global.root + "Leave/LeaveRequestDetail",
				{ "employeeId": employeeId, "fromDate": fromDate, "toDate": toDate, "requestId": requestId })
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
					ToDate: (LeaveRequest.halfDayOff.attr("checked") === "checked" ? LeaveRequest.fromDate.val() : LeaveRequest.toDate.val()),
					LeaveTypeId: LeaveRequest.leaveTypeId.val(),
					LeaveTypeName: LeaveRequest.leaveTypeId.find(":selected").text(),
					TotalDays: LeaveRequest.totalDays.val(),
					RefNo: $("#RefNo").val(),
					Remark: $("#Remark").val(),
					VerifierId: LeaveRequest.verifierId.val(),
					ApproverId: LeaveRequest.approverId.val()
				};
				var requestDetails = [];
				var leaveDate = "";
				var isHalfDay = false;
				var fromTime = new Date();
				var toTime = new Date();
				if (LeaveRequest.leaveTypeId.val() === "0") {
					$(".tabledata tbody tr").each(function () {
						leaveDate = $.trim($("td.tdleavedate", this).first().attr("data-date"));
						isHalfDay = $("td input.chkhalfday", this).first().attr("checked") === "checked";
						fromTime = new Date(leaveDate + (isHalfDay ? (" " + $("td select.ddlfromtime", this).first().val()) : ""));
						toTime = new Date(leaveDate + (isHalfDay ? (" " + $("td select.ddltotime", this).first().val()) : ""));

						if ($("td input.chkIsFromAfterday", this).first().attr("checked") === "checked") {
							fromTime.setDate(fromTime.getDate() + 1);
						}

						if ($("td input.chkIsToAfterday", this).first().attr("checked") === "checked") {
							toTime.setDate(toTime.getDate() + 1);
						}

						requestDetails.push({
							LeaveDate: leaveDate,
							WorkType: $("td select.ddlWorkType", this).first().val(),
							LeaveTypeId: $("td select.ddlleavetype", this).first().val(),
							IsHalfDay: isHalfDay,
							FromTime: fromTime,
							ToTime: toTime
						});

					});
				}
				else if (LeaveRequest.halfDayOff.attr("checked") === "checked") {
					fromTime = new Date(LeaveRequest.fromDate.val());
					toTime = new Date(LeaveRequest.fromDate.val());
					if ($("#IsFromTimeAfterDay").attr("checked") === "checked") {
						fromTime.setDate(fromTime.getDate() + 1);
					}

					if ($("#IsToTimeAfterDay").attr("checked") === "checked") {
						toTime.setDate(toTime.getDate() + 1);
					}

					requestDetails.push({
						LeaveDate: LeaveRequest.fromDate.val(),
						WorkType: 2,
						LeaveTypeId: LeaveRequest.leaveTypeId.val(),
						IsHalfDay: true,
						FromTime: fromTime.format("dd mmm, yyyy") + " " + LeaveRequest.fromTime.val(),
						ToTime: toTime.format("dd mmm, yyyy") + " " + LeaveRequest.toTime.val()
					});
				}

				if (LeaveRequest.requestId.val() === "-1") {
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
				dataType: "json",
				url: '/Leave/Create',
				data: { request: JSON.stringify(leaveRequest), requestDetails: JSON.stringify(requestDetails) },
				success: function (response) {
					if (response.Code === 0) {
						$('form').removeClass("dirty");
						var successMsg = "Your request have been sent successfully!";
						RequestCommon.showSuccessMsg(successMsg, "Leave/EmployeeView");
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
				dataType: "json",
				url: '/Leave/Update',
				data: { request: JSON.stringify(leaveRequest), requestDetails: JSON.stringify(requestDetails) },
				success: function (response) {
					if (response.Code === 0) {
						$('form').removeClass("dirty");
						var successMsg = "Your request have been sent successfully!";
						RequestCommon.showSuccessMsg(successMsg, "Leave/EmployeeView");
					}
					else {
						RequestCommon.showErrorMsg(response.Message, LeaveRequest.employee);
					}
				}
			});
		},
		dateChange: function (isHalfDay) {
			var leaveType = LeaveRequest.leaveTypeId.val();
			var fromDate = LeaveRequest.fromDate.val();
			var toDate = LeaveRequest.toDate.val();
			var employeeId = LeaveRequest.employeeId.val();

			if (leaveType === "0") {
				$("#grbHalfDay").hide();
				$("#grbTotalDay").hide();
				LeaveRequest.showLeaveDetail(employeeId, fromDate, toDate, LeaveRequest.requestId.val());
			}
			else {
				$("#grbHalfDay").show();
				$("#grbTotalDay").show();
				var leaveDetailContainer = $("#tbldetail");
				leaveDetailContainer.empty();
			}

			if (isHalfDay && leaveType !== "0") {
				LeaveRequest.bindHalfDay();
			}
			else {
				$.ajax({
					url: _global.root + 'api/employee/GetRangeDate',
					data: {
						leaveTypeId: LeaveRequest.leaveTypeId.val(),
						employeeId: LeaveRequest.employeeId.val(),
						fromDate: LeaveRequest.fromDate.val(),
						toDate: LeaveRequest.toDate.val()
					},
					async: false,
					success: function (response) {
						if (response != null) {
							LeaveRequest.totalDays.val(response);
						}
					}
				});

				$.getJSON(_global.root + 'leave/GetLeaveBalance',
				{
					employeeId: LeaveRequest.employeeId.val(),
					requestId: LeaveRequest.requestId.val(),
					toDate: LeaveRequest.toDate.val()
				},
				function (response) {
					if (response != null) {
						LeaveRequest.annualBalance.html(response.AnnualBalance);
						LeaveRequest.keepInViewBalance.html(response.KeepInViewBalance);
						LeaveRequest.currentLeaveBalance.html(response.CurrentLeaveBalanceDay);
					}
				});
			}
		},
		leaveTypeChange: function () {
			var leaveType = LeaveRequest.leaveTypeId.val();
			var fromDate = LeaveRequest.fromDate.val();
			var toDate = LeaveRequest.toDate.val();
			var employeeId = LeaveRequest.employeeId.val();
			var maxDate = new Date(LeaveRequest.fromDate.attr("maxDate"));
			var newMaxDate = new Date(maxDate);

			var minDate = new Date(LeaveRequest.fromDate.attr("minDate"));
			var tempDate = new Date(new Date(minDate).setMonth(minDate.getMonth() + 5));

			if (leaveType === LeaveRequest.generalLeaveType) {
				$("#grbHalfDay").hide();
				$("#grbTotalDay").hide();
				LeaveRequest.toDate.prop('readonly', false).datepicker('enable');
				$('.time').hide();
				LeaveRequest.showLeaveDetail(employeeId, fromDate, toDate, LeaveRequest.requestId.val());
				LeaveRequest.fromDate.attr("disabledDate", '2010-01-01');
				LeaveRequest.toDate.attr("disabledDate", '2010-01-01');
			}
			else {
				$("#grbHalfDay").show();
				$("#grbTotalDay").show();
				LeaveRequest.enableControlforHalfday();
				var leaveDetailContainer = $("#tbldetail");
				leaveDetailContainer.empty();

				if (leaveType === LeaveRequest.maternityLeaveType) {
					tempDate = new Date(new Date(minDate).setMonth(minDate.getMonth() + 8));
					LeaveRequest.fromDate.attr("disabledDate", '2010-01-01');
					LeaveRequest.toDate.attr("disabledDate", '2010-01-01');
				}
				else {
					$.getJSON(_global.root + 'api/employee/GetUnavailableDates',
					{
						employeeId: LeaveRequest.employeeId.val(),
						date: new Date().toDateString(),
						leaveTypeId: leaveType
					},
					function (response) {
						if (response != null) {
							LeaveRequest.fromDate.attr("disabledDate", response.join(","));
							LeaveRequest.toDate.attr("disabledDate", response.join(","));
						}
					});
				}

				LeaveRequest.dateChange(false);
			}

			newMaxDate = new Date(1900 + tempDate.getYear(), tempDate.getMonth(), 0);
			LeaveRequest.fromDate.datepicker("option", "maxDate", newMaxDate);
			LeaveRequest.toDate.datepicker("option", "maxDate", newMaxDate);
		},
		bindHalfDay: function () {
			LeaveRequest.enableControlforHalfday();
			if (LeaveRequest.halfDayOff.prop('checked')) {
				LeaveRequest.totalDays.val(0.5);
				//get shift information
				LeaveRequest.fillTimeForHalfday();
			}
			else {
				LeaveRequest.totalDays.val(1);
			}
		},
		enableControlforHalfday: function () {
			if (LeaveRequest.halfDayOff.prop('checked')) {
				LeaveRequest.toDate.val(LeaveRequest.fromDate.val());
				$('.time').show();
				LeaveRequest.toDate.prop('readonly', true).datepicker('disable').prop('disabled', false);
			}
			else {
				$('.time').hide();
				LeaveRequest.toDate.prop('readonly', false).datepicker('enable');
			}
		},
		fillTimeForHalfday: function () {
			$.getJSON(_global.root + 'api/shift/getshiftbydate',
			{ employeeId: LeaveRequest.employeeId.val(), date: LeaveRequest.fromDate.val() },
			function (data) {
				if (data != null) {
					var arrTimeIn = data.BeginCheckInTime.split(":");
					var arrTimeOut = data.BeginCheckOutTime.split(":");

					var minuteFrom = parseInt(arrTimeIn[0]) * 60 + parseInt(arrTimeIn[1]);
					var minuteTo = parseInt(arrTimeOut[0]) * 60 + parseInt(arrTimeOut[1]);
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
					minute = minute === 0 ? "00" : minute;
					if (hour < 10) {

						hour = "0" + hour;
					}

					var toTime = hour + ":" + minute;
					LeaveRequest.fromTime.val(data.BeginCheckInTime);
					LeaveRequest.toTime.val(toTime);

					$("#IsFromTimeAfterDay").attr("checked", isFromAfterday);
					$("#IsToTimeAfterDay").attr("checked", isToAfterday);

					if (isToAfterday) {
						var toDate = new Date(LeaveRequest.fromDate.val());
						toDate.setDate(toDate.getDate() + 1);
						LeaveRequest.toDate.val(toDate.format("dd mmm, yyyy"));
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
					LeaveRequest.eventAfterChangeEmployee();
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
		eventAfterChangeEmployee: function () {
			$.getJSON(_global.root + 'leave/GetLeaveBalance', { employeeId: LeaveRequest.employeeId.val(), requestId: LeaveRequest.requestId.val() },
					function (response) {
						if (response != null) {
							LeaveRequest.annualBalance.html(response.AnnualBalance);
							LeaveRequest.keepInViewBalance.html(response.KeepInViewBalance);
							LeaveRequest.currentLeaveBalance.html(response.CurrentLeaveBalanceDay);
						}
					});

			$.getJSON(_global.root + 'api/employee/GetUnavailableDates', { employeeId: LeaveRequest.employeeId.val(), date: new Date().toDateString() },
					function (response) {
						if (response != null) {
							LeaveRequest.fromDate.attr("disabledDate", response.join(","));
							LeaveRequest.toDate.attr("disabledDate", response.join(","));
						}
					});

			$.getJSON(_global.root + 'Request/GetVerifiers', { employeeId: LeaveRequest.employeeId.val(), requestType: LeaveRequest.requestType.val() },
					function (response) {
						if (response != null) {
							var options = "";
							for (var i = 0; i < response.length; i++) {
								options += "<option value='" + response[i].OwnerId + "'>" + response[i].NickName + " (" + response[i].FullName + ")</option>";
							}

							LeaveRequest.verifierId.empty();
							LeaveRequest.verifierId.html(options);
						}
					});

			$.getJSON(_global.root + 'Request/GetApprovers', { employeeId: LeaveRequest.employeeId.val(), requestType: LeaveRequest.requestType.val() },
					function (response) {
						if (response != null) {
							var options = "";
							for (var i = 0; i < response.length; i++) {
								options += "<option value='" + response[i].OwnerId + "'>" + response[i].NickName + " (" + response[i].FullName + ")</option>";
							}

							LeaveRequest.approverId.empty();
							LeaveRequest.approverId.html(options);
						}
					});

			LeaveRequest.leaveTypeId.val(2);
		},
		enableControlForEdit: function () {
			$("#RefNo").attr("disabled", "disabled");
			if (LeaveRequest.leaveTypeId.val() === "0") {
				$("#grbHalfDay").hide();
				$("#grbTotalDay").hide();
			}

			if (LeaveRequest.halfDayOff.attr("checked")) {
				$('.time').show();
				LeaveRequest.toDate.prop('readonly', true).datepicker('disable').prop('disabled', false);
			}
		},
		bindLeaveRequestDetailForEdit: function () {
			if (LeaveRequest.leaveTypeId.val() === "0") {
				LeaveRequest.showLeaveDetail(LeaveRequest.employeeId.val(),
                LeaveRequest.fromDate.val(),
                LeaveRequest.toDate.val(),
                LeaveRequest.requestId.val());
			}
		},
		disableControlForDetail: function () {
			if ($("#IsView").val() === "True") {
				$("input").attr("disabled", "disabled");
				$("select").attr("disabled", "disabled");
				$("textarea").attr("disabled", "disabled");
			}
		},
		validateForm: function () {
			var check = true;
			var errorString = [];
			var empName = $("#Employee").val();
			var fromDate = $("#FromDate").val();
			var toDate = $("#ToDate").val();
			var approver = $("#ApproverId").val();
			var verifier = $("#VerifierId").val();

			var start = $("[name='FromDate']").datepicker('getDate');
			var end = $("[name='ToDate']").datepicker('getDate');
			var days = (end - start) / 1000 / 60 / 60 / 24;

			if (empName !== undefined && (empName === null || empName === "")) {
				errorString.push("The Employee field is required.");
			}

			if (fromDate === null || fromDate === "") {
				errorString.push("From Date field is required.");
			}

			if (toDate === null || toDate === "") {
				errorString.push("To Date field is required.");
			}

			if (LeaveRequest.isTopManager === false) {
				if (verifier === null || verifier === "") {
					errorString.push("Verifier is required.");
				}
				if (approver === null || approver === "") {
					errorString.push("Approver is required.");
				}
			}

			if (days < 0) {
				errorString.push("To Date must be greater than From Date.");
			}

			if (errorString.length > 0) {
				RequestCommon.showErrorMsg(errorString.join(" - "));
				//                $("#errors-model-state").show();
				//                $("#errors-model-state").html(errorString.join("</br>"));
				return false;
			}

			return check;
		},
		checkIsTopManager: function () {
			var employeeId = $("#EmployeeId").val();
			if (employeeId !== null) {
				$.ajax({
					url: _global.root + 'api/employee/GetEmployee',
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
		}
	};
} (jQuery));

$(document).ready(function () {
	LeaveRequest.init();
	$('form').areYouSure();
});
