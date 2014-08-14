'use strict';
/*global _global, ScheduleArrangeCommon*/

var ScheduleArrange = (function ($) {
	return {
		groupId: null,
		jobTitle: null,
		fromDate: null,
		toDate: null,
		submit: null,
		btnSave: null,
		currentWeek: null,
		employeeId: null,
		employee: null,
		messageComfirm: null,
		scheduleContent: null,

		init: function () {
			ScheduleArrange.messageComfirm = "Are you sure to refresh the screen based on your filter condition? Data you have entered may not be saved." +
				" </br></br>  Click OK to proceed or Cancel to come back and save first";
			ScheduleArrange.groupId = $("#GroupId");
			ScheduleArrange.jobTitle = $("#JobTitle");
			ScheduleArrange.employeeId = $("#EmployeeId");
			ScheduleArrange.employee = $("#Employee");
			ScheduleArrange.fromDate = $("#FromDate");
			ScheduleArrange.toDate = $("#ToDate");
			ScheduleArrange.submit = $("#btnSubmit");
			ScheduleArrange.currentWeek = $("#btnCurrentWeek");
			ScheduleArrange.btnSave = $("#btnSave");
			ScheduleArrange.scheduleContent = $("#scheduleContent");
			ScheduleArrange.groupId.multiselect();
			ScheduleArrange.jobTitle.multiselect();
			ScheduleArrange.employeeAutocomplete();
			ScheduleArrange.bindScheduleArrangeByDate(ScheduleArrange.fromDate.val(), ScheduleArrange.toDate.val(), null, "", "");

			ScheduleArrange.submit.on("click", function () {
				if ($.trim(ScheduleArrange.employee.val()).length === 0) {
					ScheduleArrange.employeeId.val("");
				}

				if (ScheduleArrangeCommon.hasChangeValue) {
					confirm(ScheduleArrange.messageComfirm, function () {
						ScheduleArrange.queryAction(ScheduleArrange.fromDate.val(),
                        ScheduleArrange.toDate.val(), ScheduleArrange.employeeId.val());
					});
				}
				else {
					ScheduleArrange.queryAction(ScheduleArrange.fromDate.val(),
                        ScheduleArrange.toDate.val(), ScheduleArrange.employeeId.val());
				}
			});

			ScheduleArrange.currentWeek.on("click", function () {
				ScheduleArrange.fromDate.val($("#CurrentStartDateOfWeek").val());
				ScheduleArrange.toDate.val($("#CurrentEndDateOfWeek").val());

				if ($.trim(ScheduleArrange.employee.val()).length === 0) {
					ScheduleArrange.employeeId.val("");
				}

				if (ScheduleArrangeCommon.hasChangeValue) {
					confirm(ScheduleArrange.messageComfirm, function () {
						ScheduleArrange.queryAction(ScheduleArrange.fromDate.val(),
                        ScheduleArrange.toDate.val(), ScheduleArrange.employeeId.val());
					});
				}
				else {
					ScheduleArrange.queryAction(ScheduleArrange.fromDate.val(),
                        ScheduleArrange.toDate.val(), ScheduleArrange.employeeId.val());
				}
			});

			ScheduleArrange.btnSave.on("click", function () {
				ScheduleArrangeCommon.saveArrangeSchedule();
			});

			ScheduleArrange.employee.on("keypress", function (evt) {
				if (evt.keyCode === 13) {
					ScheduleArrange.submit.click();
				}
			});
		},
		queryAction: function (fromDate, toDate, employeeId) {
			var groups = ScheduleArrange.groupId.val();
			var groupIds = groups != null ? groups.join() : "";

			var jobTitle = ScheduleArrange.jobTitle.val();
			var jobTitles = jobTitle != null ? jobTitle.join() : "";

			ScheduleArrange.bindScheduleArrangeByDate(fromDate, toDate, employeeId, groupIds, jobTitles);
		},
		bindScheduleArrangeByDate: function (fromDate, toDate, employeeId, groupIds, jobTitles) {
			var params = {
				"fromDate": fromDate,
				"toDate": toDate,
				"employeeId": ((employeeId === "0") ? null : employeeId),
				"groupIds": groupIds,
				"jobTitles": jobTitles
			};

			if (ScheduleArrange.scheduleContent.length > 0) {
				$.get(_global.root + "Calendar/ArrangeScheduleDetail", params)
					.done(function (content) {
						ScheduleArrange.scheduleContent.empty();
						ScheduleArrange.scheduleContent.html(content);
					});
			}
		},
		employeeAutocomplete: function () {
			var nickName;
			ScheduleArrange.employee.autocomplete({
				source: function (request, response) {
					$.ajax({
						url: _global.root + 'Calendar/GetEmployeesByShiftMonitor',
						data: {
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
					ScheduleArrange.employee.val(nickName);
					ScheduleArrange.employeeId.val(ui.item.value);
				},
				focus: function (event, ui) {
					nickName = ui.item.label;
					ScheduleArrange.employee.val(nickName);
					ScheduleArrange.employeeId.val(ui.item.value);
					event.preventDefault();
				},
				close: function () {
					ScheduleArrange.employee.val(nickName);
				}
			});
		}
	};
} (jQuery));

$(document).ready(function () {
    ScheduleArrange.init();
});
