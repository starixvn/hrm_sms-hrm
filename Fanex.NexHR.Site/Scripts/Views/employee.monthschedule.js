'use strict';
/*global _global*/
/*exported*/

var _event = null;

$(document).ready(function () {
	var id = $("#currentViewUser").val();

	$('#scheduledefault').click(function () {
		document.location = _global.root + 'employee/defaultschedule/' + id;
	});
	$('#schedulemonth').attr('checked', true);

	$('#calendar').fullCalendar({
		theme: true,
		header: {
			left: '',
			center: 'title',
			right: 'prev,next'
		},
		firstDay: 1,
		editable: false,
		events: function (start, end, callback) {
			$.ajax({
				url: '/calendar/GetWorkingSchedule',
				dataType: 'json',
				data: {
					date: $('#calendar').fullCalendar('getDate').format("mm/dd/yyyy"),
					id: id
				},
				success: function (data) {
					var events = [];
					$(data).each(function (index, item) {
						events.push({
							title: item.Title,
							start: item.Start,
							className: 'cell-heigh',
							backgroundColor: item.Color
						});
					});
					callback(events);
					$('span.fc-event-title').removeClass("fc-event-title").css({
						display: "block",
						"text-align": "center",
						"padding-top": "20px"
					});
				}
			});
		},
		eventClick: function (event) {
			_event = event;
			$('#calendar').fullCalendar('updateEvent', event);
		}
	});
});

