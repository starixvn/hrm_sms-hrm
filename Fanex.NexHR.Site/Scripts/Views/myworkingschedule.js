$(document).ready(function () {
    $('#calendar').fullCalendar({
        theme: true,
        header:
		{
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
                data:
				{
				    date: $('#calendar').fullCalendar('getDate').format("mm/dd/yyyy"),
				    id: _global.employeeId
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
                    $('.OFF').find('.fc-event-title').addClass('OFF');
                    $('span.fc-event-title').removeClass("fc-event-title").css({
                        display: "block",
                        "text-align": "center",
                        "padding-top": "20px"
                    });
                }
            });
        }
    });
});