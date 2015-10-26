'use strict';

var ConstantRequest = (function () {
    return {
        CONSTANT_ACCEPT: null,
        CONSTANT_NOTACCEPT: null,
        CONSTANT_APPROVE: null,
        CONSTANT_REJECT: null,
        CONSTANT_PROCESS: null,
        CONSTANT_VOID: null,
        CONSTANT_RESET: null,
        CONSTANT_VOIDBYSA: null,

        init: function () {
            ConstantRequest.CONSTANT_ACCEPT = "Accept";
            ConstantRequest.CONSTANT_NOTACCEPT = "NotAccept";
            ConstantRequest.CONSTANT_APPROVE = "Approve";
            ConstantRequest.CONSTANT_REJECT = "Reject";
            ConstantRequest.CONSTANT_PROCESS = "Process";
            ConstantRequest.CONSTANT_VOID = "Void";
            ConstantRequest.CONSTANT_RESET = "Reset";
            ConstantRequest.CONSTANT_VOIDBYSA = "VoidBySA";
        }
    };
} (jQuery));

$(document).ready(function () {
    ConstantRequest.init();
});