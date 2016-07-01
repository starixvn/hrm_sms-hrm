'use strict';
/*global _global, CommandOvertime, ConstantRequest, fadeOutMessage, MemberOvertime*/

var OvertimeMemberDetail = (function ($) {
    return {
        init: function () {
            $("#chkAll").change(function () {
                OvertimeMemberDetail.checkAllChange($(this).attr("checked") === "checked");
            });

            $("#lnkAccept").on("click", function () {
                OvertimeMemberDetail.acceptAllRequest();
            });

            $("#lnkNotAccept").on("click", function () {
                OvertimeMemberDetail.notAcceptAllRequest();
            });

            $("#lnkApprove").on("click", function () {
                OvertimeMemberDetail.approveAllRequest();
            });

            $("#lnkReject").on("click", function () {
                OvertimeMemberDetail.rejectAllRequest();
            });

            $("#lnkProcess").on("click", function () {
                OvertimeMemberDetail.processAllRequest();
            });

            $("#lnkVoid").on("click", function () {
                OvertimeMemberDetail.voidAllRequest();
            });
        },
        checkAllChange: function (isChecked) {
            $(".chkSelect").attr("checked", isChecked);
        },
        acceptAllRequest: function () {
            var objectSelected = $("input[type='checkbox'].chkSelect:checked");
            var isShowWarning = false;
            if (objectSelected.length > 0) {
                confirm('Are you sure you want to verify all?', function () {
                    var objectSelected = $("input[type='checkbox'].chkSelect:checked");
                    var requestIds = "";
                    for (var i = 0; i < objectSelected.length; i++) {
                        if ($(objectSelected[i]).parent().parent().find(".verifier-pending").length > 0) {
                            requestIds += $(objectSelected[i]).attr("data-requestId") + ",";
                        }
                        else {
                            isShowWarning = true;
                        }
                    }

                    if (isShowWarning) {
                        alert("There are some request which you doesn't role to Verify.");
                    }

                    if (requestIds.length > 0) {
                        requestIds = requestIds.substring(0, requestIds.length - 1);
                    }

                    $.post(_global.root + 'overtime/ProcessMultiple',
                        { command: ConstantRequest.CONSTANT_ACCEPT, requestIds: requestIds }, function (response) {
                            fadeOutMessage();
                            if (response.IsSuccessful) {
                                MemberOvertime.submitEvent();
                            }
                            else {
                                alert("There are some request which you doesn't role to Verify.");
                                return;
                            }
                        });
                });
            }
            else {
                alert('Please choose at least one request to continue!');
            }

            return false;
        },
        notAcceptAllRequest: function () {
            var objectSelected = $("input[type='checkbox'].chkSelect:checked");
            var requestIds = "";
            var isShowWarning = false;
            if (objectSelected.length > 0) {
                confirm('Are you sure you want to Not Verify all?', function () {

                    for (var i = 0; i < objectSelected.length; i++) {
                        if ($(objectSelected[i]).parent().parent().find(".verifier-pending").length > 0) {
                            requestIds += $(objectSelected[i]).attr("data-requestId") + ",";
                        }
                        else {
                            isShowWarning = true;
                        }
                    }

                    if (isShowWarning) {
                        alert("There are some request which you doesn't role to Not Verify.");
                    }

                    if (requestIds.length > 0) {
                        requestIds = requestIds.substring(0, requestIds.length - 1);
                    }

                    $.post(_global.root + 'overtime/ProcessMultiple', { command: ConstantRequest.CONSTANT_NOTACCEPT, requestIds: requestIds },
                        function (response) {
                            fadeOutMessage();
                            if (response.IsSuccessful) {
                                MemberOvertime.submitEvent();
                            }
                            else {
                                alert("There are some request which you doesn't role to Not Verify.");
                                return;
                            }
                        });
                });
            }
            else {
                alert('Please choose at least one request to continue!');
            }

            return false;
        },
        approveAllRequest: function () {
            var objectSelected = $("input[type='checkbox'].chkSelect:checked");
            var requestIds = "";
            var isShowWarning = false;
            if (objectSelected.length > 0) {
                confirm('Are you sure you want to approve all?', function () {
                    for (var i = 0; i < objectSelected.length; i++) {
                        if ($(objectSelected[i]).parent().parent().find(".approver-pending").length > 0) {
                            requestIds += $(objectSelected[i]).attr("data-requestId") + ",";
                        }
                        else {
                            isShowWarning = true;
                        }
                    }

                    if (isShowWarning) {
                        alert("There are some request has verified yet.");
                    }

                    if (requestIds.length > 0) {
                        requestIds = requestIds.substring(0, requestIds.length - 1);
                    }

                    $.post(_global.root + 'overtime/ProcessMultiple', { command: ConstantRequest.CONSTANT_APPROVE, requestIds: requestIds },
                    function (response) {
                        fadeOutMessage();
                        if (response.IsSuccessful) {
                            MemberOvertime.submitEvent();
                        }
                        else {
                            alert(response.Message);
                            return;
                        }
                    });
                });
            }
            else {
                alert('Please choose at least one request to continue!');
            }

            return false;
        },
        rejectAllRequest: function () {
            var objectSelected = $("input[type='checkbox'].chkSelect:checked");
            var requestIds = "";
            var isShowWarning = false;
            if (objectSelected.length > 0) {
                confirm('Are you sure you want to reject all?', function () {
                    for (var i = 0; i < objectSelected.length; i++) {
                        if ($(objectSelected[i]).parent().parent().find(".approver-pending").length > 0) {
                            requestIds += $(objectSelected[i]).attr("data-requestId") + ",";
                        }
                        else {
                            isShowWarning = true;
                        }
                    }

                    if (isShowWarning) {
                        alert("There are some request has verified yet.");
                    }

                    if (requestIds.length > 0) {
                        requestIds = requestIds.substring(0, requestIds.length - 1);
                    }

                    $.post(_global.root + 'overtime/ProcessMultiple', { command: ConstantRequest.CONSTANT_REJECT, requestIds: requestIds },
                        function (response) {
                            fadeOutMessage();
                            if (response.IsSuccessful) {
                                MemberOvertime.submitEvent();
                            }
                            else {
                                alert(response.Message);
                                return;
                            }
                        });
                });
            }
            else {
                alert('Please choose at least one request to continue!');
            }

            return false;
        },
        processAllRequest: function () {
            var objectSelected = $("input[type='checkbox'].chkSelect:checked");
            var requestIds = "";
            if (objectSelected.length > 0) {
                confirm('Are you sure you want to process all?', function () {
                    for (var i = 0; i < objectSelected.length; i++) {
                        if ($(objectSelected[i]).parent().parent().find(".hr-pending").length > 0) {
                            requestIds += $(objectSelected[i]).attr("data-requestId") + ",";
                        }
                    }

                    if (requestIds.length > 0) {
                        requestIds = requestIds.substring(0, requestIds.length - 1);
                    }

                    CommandOvertime.checkInvalidOvertime(requestIds);
                    if (CommandOvertime.isInvalid) {
                        confirm('There are some overtimes are invalid. Are you sure you want to process?', function () {
                            $.post(_global.root + 'overtime/ProcessMultiple', { command: ConstantRequest.CONSTANT_PROCESS, requestIds: requestIds },
                                function (response) {
                                    fadeOutMessage();
                                    if (response.IsSuccessful) {
                                        MemberOvertime.submitEvent();
                                    }
                                    else {
                                        alert(response.Message);
                                        return;
                                    }
                                });
                        });
                    }
                    else {
                        $.post(_global.root + 'overtime/ProcessMultiple', { command: ConstantRequest.CONSTANT_PROCESS, requestIds: requestIds },
                            function (response) {
                                fadeOutMessage();
                                if (response.IsSuccessful) {
                                    MemberOvertime.submitEvent();
                                }
                                else {
                                    alert(response.Message);
                                    return;
                                }
                            });
                    }
                });
            }
            else {
                alert('Please choose at least one request to continue!');
            }

            return false;
        },
        voidAllRequest: function () {
            var objectSelected = $("input[type='checkbox'].chkSelect:checked");
            var requestIds = "";
            if (objectSelected.length > 0) {
                confirm('Are you sure you want to void all?', function () {
                    for (var i = 0; i < objectSelected.length; i++) {
                        if ($(objectSelected[i]).parent().parent().find(".hr-pending").length > 0) {
                            requestIds += $(objectSelected[i]).attr("data-requestId") + ",";
                        }
                    }

                    if (requestIds.length > 0) {
                        requestIds = requestIds.substring(0, requestIds.length - 1);
                    }

                    $.post(_global.root + 'overtime/ProcessMultiple', { command: ConstantRequest.CONSTANT_VOID, requestIds: requestIds },
                    function (response) {
                        fadeOutMessage();
                        if (response.IsSuccessful) {
                            MemberOvertime.submitEvent();
                        }
                        else {
                            alert(response.Message);
                            return;
                        }
                    });
                });
            }
            else {
                alert('Please choose at least one request to continue!');
            }
            return false;
        }
    };
}(jQuery));

$(document).ready(function () {
    OvertimeMemberDetail.init();
});