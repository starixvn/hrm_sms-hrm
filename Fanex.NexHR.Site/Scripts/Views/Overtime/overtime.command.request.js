'use strict';
/*global _global, ConstantRequest, fadeOutMessage, showMessage, MemberOvertime*/

window.CommandOvertime = (function ($) {
	return {
		isInvalid: null,

		init: function () {
			CommandOvertime.isInvalid = false;

			CommandOvertime.bindMenu('.verifier-pending', 'verifierPendingMenu');
			CommandOvertime.bindMenu('.verifier-accepted', 'acceptedMenu');
			CommandOvertime.bindMenu('.verifier-notAccepted', 'notAcceptedMenu');
			CommandOvertime.bindMenu('.approver-pending', 'approverPendingMenu');
			CommandOvertime.bindMenu('.approver-approved', 'approvedMenu');
			CommandOvertime.bindMenu('.approver-rejected', 'rejectedMenu');
			CommandOvertime.bindMenu('.hr-pending', 'hrPendingMenu');
			CommandOvertime.bindMenu('.hr-processed', 'processedMenu');
			CommandOvertime.bindMenu('.hr-void', 'voidMenu');
		},
		bindMenu: function (element, menu) {
			$(element).contextMenu({ menu: menu },
				function (command, eventElement) {
					var requestId = $(eventElement).attr('requestId');
					var roleId = $(eventElement).attr('roleId');
					switch (command) {
						case ConstantRequest.CONSTANT_ACCEPT:
							CommandOvertime.acceptRequest(command, requestId, roleId);
							break;
						case ConstantRequest.CONSTANT_NOTACCEPT:
							CommandOvertime.notAcceptRequest(command, requestId, roleId);
							break;
						case ConstantRequest.CONSTANT_APPROVE:
							CommandOvertime.approveRequest(command, requestId, roleId);
							break;
						case ConstantRequest.CONSTANT_REJECT:
							CommandOvertime.rejectRequest(command, requestId, roleId);
							break;
						case ConstantRequest.CONSTANT_PROCESS:
							CommandOvertime.processRequest(command, requestId, roleId);
							break;
						case ConstantRequest.CONSTANT_VOID:
							CommandOvertime.voidRequest(command, requestId, roleId);
							break;
						case ConstantRequest.CONSTANT_RESET:
							CommandOvertime.resetRequest(command, requestId, roleId);
							break;
					}
				});
		},
		acceptRequest: function (command, requestId, roleId) {
			confirm('Are you sure you want to vefiry?', function () {
				CommandOvertime.commandProcessRequest(command, requestId, roleId);
			});
		},
		notAcceptRequest: function (command, requestId, roleId) {
			confirm('Are you sure you dont want to verify?', function () {
				CommandOvertime.commandProcessRequest(command, requestId, roleId);
			});
		},
		approveRequest: function (command, requestId, roleId) {
			confirm('Are you sure you want to approve?', function () {
				CommandOvertime.commandProcessRequest(command, requestId, roleId);
			});
		},
		rejectRequest: function (command, requestId, roleId) {
			confirm('Are you sure you want to reject?', function () {
				CommandOvertime.commandProcessRequest(command, requestId, roleId);
			});
		},
		processRequest: function (command, requestId, roleId) {
			CommandOvertime.checkInvalidOvertime(requestId);
			if (CommandOvertime.isInvalid) {
				confirm('There are some overtimes are invalid. Are you sure you want to process?', function () {
					CommandOvertime.commandProcessRequest(command, requestId, roleId);
				});
			}
			else {
				confirm('Are you sure you want to process?', function () {
					CommandOvertime.commandProcessRequest(command, requestId, roleId);
				});
			}
		},
		voidRequest: function (command, requestId, roleId) {
			confirm('Are you sure you want to void?', function () {
				CommandOvertime.commandProcessRequest(command, requestId, roleId);
			});
		},
		resetRequest: function (command, requestId, roleId) {
			confirm('Are you sure you want to reset?', function () {
				CommandOvertime.commandProcessRequest(command, requestId, roleId);
			});
		},
		commandProcessRequest: function (command, requestId, roleId) {
			showMessage('Processing');
			$.post(_global.root + 'ajax/ProcessSingleOvertime',
			{ command: command, requestId: requestId, roleId: roleId },
			function (response) {
				fadeOutMessage();
				if (response !== true) {
					alert(response);
					return;
				}
				else {
					if (typeof DailyOvertimeMember === 'undefined' && typeof OvertimeMemberDetail !== 'undefined') {
						MemberOvertime.submitEvent();
					}
					else {
						window.location.reload();
					}
				}
			});
		},
		checkInvalidOvertime: function (requestIds) {
			$.ajax({
				url: _global.root + "overtime/CheckInvalidOvertime",
				data: { requestIds: requestIds },
				async: false,
				type: 'POST'
			}).done(function (response) {
				CommandOvertime.isInvalid = response;
			});
		}
	};
} (jQuery));

$(document).ready(function () {
	CommandOvertime.init();
});