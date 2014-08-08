'use strict';
/*global _global, fadeOutMessage, showMessage, TimeRecord*/

var TimeRecordDetail = (function ($) {
	return {
		init: function () {
			TimeRecordDetail.bindMenu('.in-action', 'inMenu');
			TimeRecordDetail.bindMenu('.out-action', 'outMenu');
		},
		bindMenu: function (element, menu) {
			$(element).contextMenu({
				menu: menu
			},
			function (command, eventElement) {
				switch (command) {
					case 'in':
						confirm('Are you sure you change to in?', function () {
							TimeRecordDetail.processInOut(0, eventElement);
						});

						break;
					case 'out':
						confirm('Are you sure you change to out?', function () {
							TimeRecordDetail.processInOut(1, eventElement);
						});

						break;
				}
			});
		},
		processInOut: function (inoutType, eventElement) {
			var recordTimeId = $(eventElement).attr('data-id');
			showMessage('Processing');

			$.post(_global.root + 'ajax/ProcessInOut',
			{ inoutType: inoutType, recordTimeId: recordTimeId },
			function (response) {
				if (response.Code === 0) {
					TimeRecord.fillData();
				}
				else {
					alert(response.Message);
					return;
				}
			});
			fadeOutMessage();
		}
	};
} (jQuery));

$(document).ready(function () {
    TimeRecordDetail.init();
});
