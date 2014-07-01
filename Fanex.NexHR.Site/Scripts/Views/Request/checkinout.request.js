$(document).ready(function () {

    $("#model-state-error").hide();
    $("#ForDate").attr('readonly', 'readonly');
    $("#AttendantDate").attr('readonly', 'readonly');

    $('#shiftinfo').tooltip({
        track: true,
        delay: 0,
        showURL: false,
        bodyHandler: function () {
            return $('#shifts').html();
        }
    });

    //timeOut < $('#InTime').val() || $('#Hour').val().length == 0 || $('#Minute').val().length == 0
    //var timeOut = ($('#Hour').val().length < 2 ? "0" + $('#Hour').val() : $('#Hour').val()) + ":" + ($('#Minute').val().length < 2 ? "0" + $('#Minute').val() : $('#Minute').val());

    $("form").submit(function (e) {
        var requestDate = $("#ForDate").datepicker('getDate');
        var attendantDate = $("#AttendantDate").datepicker('getDate');
        var dayDiff = (requestDate - attendantDate) / 1000 / 60 / 60 / 24;
        var crossDay = $("#CrossDay").val();

        if ($("#Type_2").attr("checked") == "checked" || $('#Type').val() == "2") {
            if (crossDay == "true") {
                if (dayDiff <= 0) {
                    e.preventDefault();
                    alert('Request date must be greater than Attendant date!');
                }
            }
            else {
                if (dayDiff < 0) {
                    e.preventDefault();
                    alert('Request date must be equal or greater than Attendant date!');
                }
            }
        }
        if ($("#Type_1").attr("checked") == "checked" || $('#Type').val() == "1") {
            if (dayDiff < 0) {
                e.preventDefault();
                alert('Request date must be equal or greater than Attendant date.');
            }
        }

    });

    $("#Employee").blur(function () {
        $.getJSON(_global.root + 'api/employee/GetUnavailableDates', { employeeId: $('#EmployeeId').val(), date: new Date().toDateString() },
				function (response) {
				    if (response != null) {
				        $("#ForDate").attr("disabledDate", response.join(","));
				        $("#AttendantDate").attr("disabledDate", response.join(","));
				        $('#AttendantDate').change();
				    }
				}
			);
    });

    $('#AttendantDate').change(function () {
        $.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: $("#EmployeeId").val(), date: $("#AttendantDate").val() },
		function (data) {
		    if (data != null) {
		        $("#model-state-error").hide();
		        $('#shiftname').text(data.Name);
		        $('#Shift').val(data.Name);
		        $('#InTime').val(data.BeginCheckInTime);
		        $('#OutTime').val(data.BeginCheckOutTime);
		        $('#CrossDay').val(data.CrossDay);
		        bindingHourAndMinute();
		        $('input[type=submit]').show();
		    }
		    else {
		        $("#model-state-error").show();
		        $('#shiftname').text(_messages.notAvailable);
		        $('#Shift').val(_messages.notAvailable);
		        $('input[type=submit]').hide();

		    }
		});

    });

    $('#Type_1, #Type_2').change(function () {
        bindingHourAndMinute();
    });

    if ($('#Type_1').val() != undefined)
        $("#Employee").blur();
    else
        getShiftByDate();

    //Process redirect to Check In/Out request.
    var successed = $("#successed").val();
    var year = $("#year").val();

    if (successed == "true") {
        $('form').removeClass("dirty");
        confirm('Successfull!\nPress OK to continue', function () {
            document.location = _global.root + "checkinout/EmployeeView/" + year;
        });
    }

    updateZeroPrefix();

    //check dirty
    $('form').areYouSure();
});

function updateZeroPrefix() {
    //add zeroPrefix
    $('input[Name=Hour]').val(zeroPrefix($('input[Name=Hour]').val()));
    $('input[Name=Minute]').val(zeroPrefix($('input[Name=Minute]').val()));
}

function bindingHourAndMinute() {
    if (($('#Type').val() == "1") || ($('#Type_1').attr("checked") == "checked")) {
        $('#Hour').val($('#InTime').val().substring(0, 2));
        $('#Minute').val($('#InTime').val().trim().substring(3, 5));
    }
    else {
        $('#Hour').val($('#OutTime').val().substring(0, 2));
        $('#Minute').val($('#OutTime').val().trim().substring(3, 5));
    }
}

//this function for load Edit
function getShiftByDate() {
    $.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: $("#EmployeeId").val(), date: $("#AttendantDate").val() },
	function (data) {
	    if (data != null) {
	        $("#model-state-error").hide();
	        $('#shiftname').text(data.Name);
	        $('#Shift').val(data.Name);
	        $('#InTime').val(data.BeginCheckInTime);
	        $('#OutTime').val(data.BeginCheckOutTime);
	        $('#CrossDay').val(data.CrossDay);
	        $('input[type=submit]').show();

	    }
	    else {
	        $("#model-state-error").show();
	        $('#shiftname').text(_messages.notAvailable);
	        $('#Shift').val(_messages.notAvailable);
	        $('input[type=submit]').hide();
	    }
	});
}
