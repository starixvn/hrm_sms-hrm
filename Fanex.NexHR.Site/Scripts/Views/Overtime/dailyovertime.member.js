'use strict';
/*global _global, ConstantRequest, CommandOvertime*/

function calculateTotalOTHours(selector) {
	var sumResult = 0;
	selector.each(function () {
		var value = $(this).text().trim();
		if (value !== "") {
			if (!isNaN(value) && value.lenght !== 0) {
				sumResult += parseFloat(value);
			}
		}
	});
	return sumResult;
}

function requestToCalculateOTHours() {
	var sumNormal = calculateTotalOTHours($(".sumnormal"));
	$("#sumnormal").text(sumNormal.toFixed(2));

	var sumOff = calculateTotalOTHours($(".sumoff"));
	$("#sumoff").text(sumOff.toFixed(2));

	var sumPublic = calculateTotalOTHours($(".sumpublic"));
	$("#sumpublic").text(sumPublic.toFixed(2));
}

var DailyOvertimeMember = (function ($) {
    return {
        btnAccept: null,
        btnNotAccept: null,
        btnApprove: null,
        btnReject: null,
        btnProcess: null,
        btnVoid: null,
        btnBack: null,
        btnVerifierReset: null,
        btnApproverReset: null,
        btnHRReset: null,

        btnAcceptFooter: null,
        btnNotAcceptFooter: null,
        btnApproveFooter: null,
        btnRejectFooter: null,
        btnProcessFooter: null,
        btnVoidFooter: null,
        btnBackFooter: null,
        btnVerifierResetFooter: null,
        btnApproverResetFooter: null,
        btnHRResetFooter: null,

        isHRView: null,
        requestId: null,
        verifyRole: null,
        approveRole: null,
        hrRole: null,

        init: function () {
            DailyOvertimeMember.verifyRole = 2;
            DailyOvertimeMember.approveRole = 3;
            DailyOvertimeMember.hrRole = 6;

            DailyOvertimeMember.btnAccept = $("#btnAccept");
            DailyOvertimeMember.btnNotAccept = $("#btnNotAccept");
            DailyOvertimeMember.btnApprove = $("#btnApprove");
            DailyOvertimeMember.btnReject = $("#btnReject");
            DailyOvertimeMember.btnProcess = $("#btnProcess");
            DailyOvertimeMember.btnVoid = $("#btnVoid");
            DailyOvertimeMember.btnBack = $("#btnBack");
            DailyOvertimeMember.btnVerifierReset = $("#btnVerifierReset");
            DailyOvertimeMember.btnApproverReset = $("#btnApproverReset");
            DailyOvertimeMember.btnHRReset = $("#btnHRReset");

            DailyOvertimeMember.btnAcceptFooter = $("#btnAcceptFooter");
            DailyOvertimeMember.btnNotAcceptFooter = $("#btnNotAcceptFooter");
            DailyOvertimeMember.btnApproveFooter = $("#btnApproveFooter");
            DailyOvertimeMember.btnRejectFooter = $("#btnRejectFooter");
            DailyOvertimeMember.btnProcessFooter = $("#btnProcessFooter");
            DailyOvertimeMember.btnVoidFooter = $("#btnVoidFooter");
            DailyOvertimeMember.btnBackFooter = $("#btnBackFooter");
            DailyOvertimeMember.btnVerifierResetFooter = $("#btnVerifierResetFooter");
            DailyOvertimeMember.btnApproverResetFooter = $("#btnApproverResetFooter");
            DailyOvertimeMember.btnHRResetFooter = $("#btnHRResetFooter");

            DailyOvertimeMember.isHRView = $("#IsHRView");
            DailyOvertimeMember.requestId = $("#RequestId");
            var requestId = DailyOvertimeMember.requestId.val();

            DailyOvertimeMember.btnBack.on("click", function () {
                DailyOvertimeMember.backAction();
            });

            DailyOvertimeMember.btnBackFooter.on("click", function () {
                DailyOvertimeMember.backAction();
            });

            DailyOvertimeMember.btnAccept.on("click", function () {
                CommandOvertime.acceptRequest(ConstantRequest.CONSTANT_ACCEPT, requestId, DailyOvertimeMember.verifyRole);
            });
            DailyOvertimeMember.btnAcceptFooter.on("click", function () {
                CommandOvertime.acceptRequest(ConstantRequest.CONSTANT_ACCEPT, requestId, DailyOvertimeMember.verifyRole);
            });

            DailyOvertimeMember.btnNotAccept.on("click", function () {
                CommandOvertime.notAcceptRequest(ConstantRequest.CONSTANT_NOTACCEPT, requestId, DailyOvertimeMember.verifyRole);
            });
            DailyOvertimeMember.btnNotAcceptFooter.on("click", function () {
                CommandOvertime.notAcceptRequest(ConstantRequest.CONSTANT_NOTACCEPT, requestId, DailyOvertimeMember.verifyRole);
            });

            DailyOvertimeMember.btnApprove.on("click", function () {
                CommandOvertime.approveRequest(ConstantRequest.CONSTANT_APPROVE, requestId, DailyOvertimeMember.approveRole);
            });
            DailyOvertimeMember.btnApproveFooter.on("click", function () {
                CommandOvertime.approveRequest(ConstantRequest.CONSTANT_APPROVE, requestId, DailyOvertimeMember.approveRole);
            });

            DailyOvertimeMember.btnReject.on("click", function () {
                CommandOvertime.rejectRequest(ConstantRequest.CONSTANT_REJECT, requestId, DailyOvertimeMember.approveRole);
            });
            DailyOvertimeMember.btnRejectFooter.on("click", function () {
                CommandOvertime.rejectRequest(ConstantRequest.CONSTANT_REJECT, requestId, DailyOvertimeMember.approveRole);
            });

            DailyOvertimeMember.btnProcess.on("click", function () {
                CommandOvertime.processRequest(ConstantRequest.CONSTANT_PROCESS, requestId, DailyOvertimeMember.hrRole);
            });
            DailyOvertimeMember.btnProcessFooter.on("click", function () {
                CommandOvertime.processRequest(ConstantRequest.CONSTANT_PROCESS, requestId, DailyOvertimeMember.hrRole);
            });

            DailyOvertimeMember.btnVoid.on("click", function () {
                CommandOvertime.voidRequest(ConstantRequest.CONSTANT_VOID, requestId, DailyOvertimeMember.hrRole);
            });
            DailyOvertimeMember.btnVoidFooter.on("click", function () {
                CommandOvertime.voidRequest(ConstantRequest.CONSTANT_VOID, requestId, DailyOvertimeMember.hrRole);
            });

            DailyOvertimeMember.btnVerifierReset.on("click", function () {
                CommandOvertime.resetRequest(ConstantRequest.CONSTANT_RESET, requestId, DailyOvertimeMember.verifyRole);
            });
            DailyOvertimeMember.btnVerifierResetFooter.on("click", function () {
                CommandOvertime.resetRequest(ConstantRequest.CONSTANT_RESET, requestId, DailyOvertimeMember.verifyRole);
            });

            DailyOvertimeMember.btnApproverReset.on("click", function () {
                CommandOvertime.resetRequest(ConstantRequest.CONSTANT_RESET, requestId, DailyOvertimeMember.approveRole);
            });
            DailyOvertimeMember.btnApproverResetFooter.on("click", function () {
                CommandOvertime.resetRequest(ConstantRequest.CONSTANT_RESET, requestId, DailyOvertimeMember.approveRole);
            });

            DailyOvertimeMember.btnHRReset.on("click", function () {
                CommandOvertime.resetRequest(ConstantRequest.CONSTANT_RESET, requestId, DailyOvertimeMember.hrRole);
            });
            DailyOvertimeMember.btnHRResetFooter.on("click", function () {
                CommandOvertime.resetRequest(ConstantRequest.CONSTANT_RESET, requestId, DailyOvertimeMember.hrRole);
            });
        },

        backAction: function () {
            var isHRView = DailyOvertimeMember.isHRView.val() === "True";
            var url = _global.root + (isHRView ? "Overtime/EmployeeOvertime" : "Overtime/MemberOvertime");
            $(location).attr('href', url);
        }
    };
} (jQuery));

$(document).ready(function () {
    DailyOvertimeMember.init();
    requestToCalculateOTHours();
});

