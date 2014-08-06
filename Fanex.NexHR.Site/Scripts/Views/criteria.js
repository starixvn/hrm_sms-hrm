'use strict';
/*global  actingButton, reloadButtonText, sleep*/

$(document).ready(function () {
	$("#submit").click(function () {
		actingButton('#submit', true);
		sleep(1000);
		reloadButtonText('#submit');
	});
});