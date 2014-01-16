$(document).ready(function () {
    bindMenu('#forgotInOut', 'forgotCheckInOutMenu');
    bindMenu('#lateInEarlyOut', 'lateInEarlyOutMenu');
});

function bindMenu(element, menu) {
    $(element).contextMenu({
        menu: menu
    },
	function (command, eventElement) {
	    window.location.href = command;
	});
}