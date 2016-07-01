'use strict';
/*global _global, autoIndex*/
/*exported deleteHoliday*/

$(document).ready(function () {
	var year = $('#year').val();

	$('#submit').click(function () {
		year = $('#year').val();
		location.href = "/Holiday/Index?year=" + year;
	});
});


function deleteHoliday(id) {
	confirm('Are you sure you want to delete this holiday?', function () {
	    $.post(_global.root + 'Holiday/Delete', { id: id }, function () {
			$('#row' + id).remove();
			autoIndex();
		});
	});
}