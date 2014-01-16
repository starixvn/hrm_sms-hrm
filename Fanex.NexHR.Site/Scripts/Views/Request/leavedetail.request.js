
var LeaveRequestDetail = function ($) {
    return {
        leaveWorkType: null,
        onDutyWorkType: null,
        offWorkType: null,
        annualLeaveType: null,
        unpaidLeaveType: null,

        init: function () {
            LeaveRequestDetail.leaveWorkType = 2; // hard code for worktype is leave
            LeaveRequestDetail.onDutyWorkType = 3;
            LeaveRequestDetail.offWorkType = 1;
            LeaveRequestDetail.annualLeaveType = 2;
            LeaveRequestDetail.unpaidLeaveType = 25;

            $("th.tdhalfday").width(100);

            LeaveRequestDetail.showHideHalfDay();
            LeaveRequestDetail.showHideControlByWorkType();
            $(".ddlWorkType").change(function () {
                LeaveRequestDetail.workTypeChange($(this));
            });

            $(".chkhalfday").change(function () {
                var trObject = $(this).parent().parent();
                LeaveRequestDetail.halfdayChange(trObject);
            });

            $(".addhalfday").on("click", function () {
                var object = $(this);
                var trObject = object.parent().parent();
                var trCopy = trObject.clone();
                var halfdayObject = trCopy.find(".addhalfday").first().html("- Remove a half").removeClass("addhalfday").addClass("removehaflday");
                var leaveTypeObject = trCopy.find(".ddlleavetype").first();
                var workTypeObject = trCopy.find(".ddlWorkType").first();

                trCopy.find(".chkhalfday").attr("checked", "checked").attr("disabled", "disabled");
                trObject.find(".chkhalfday").attr("checked", "checked").attr("disabled", "disabled");

                halfdayObject.click(function () {
                    var objectRemove = $(this).parent().parent().remove();
                    trObject.find(".addhalfday").first().html("+ Add a half");
                    trObject.find(".chkhalfday").removeAttr("disabled");
                    LeaveRequestDetail.getTotalLeave();
                });
                leaveTypeObject.change(function () {
                    LeaveRequestDetail.getTotalLeave();
                });
                workTypeObject.change(function () {
                    LeaveRequestDetail.workTypeChange(workTypeObject);
                });

                workTypeObject.change();

                trObject.after(trCopy);
                trObject.find(".addhalfday").first().html("");
                LeaveRequestDetail.getTotalLeave();
            });

            $(".ddlleavetype").change(function () {
                LeaveRequestDetail.getTotalLeave();
            });

            LeaveRequestDetail.disableControlForDetail();
        },
        halfdayChange: function (trObject) {
            if (trObject.find(".chkhalfday").attr("checked")) {

                $(".tdhalfday").show();
                trObject.find(".addhalfday").first().html("+ Add a half").show();
                var fromTimeObject = trObject.find(".ddlfromtime");
                var toTimeObject = trObject.find(".ddltotime");

                //if (trObject.find(".ddlWorkType").val() == LeaveRequestDetail.leaveWorkType) {
                fromTimeObject.show();
                toTimeObject.show();
                //}

                trObject.find(".tdnumberleaves").html("0.5");
                var scheduleStatus = $.trim(trObject.find(".tdschedulestatus").html());

                var arrTime = scheduleStatus.split("&gt;");
                if (arrTime.length < 2 && arrTime.length > 0) {
                    arrTime = arrTime[0].split(">");
                }

                if (arrTime.length > 1) {
                    var fromTime = $.trim(arrTime[0]);
                    var toTime = $.trim(arrTime[1]);
                    if (fromTime.indexOf(":") == -1) {
                        fromTime = fromTime + ":00";
                    }

                    if (toTime.indexOf(":") == -1) {
                        toTime = toTime + ":00";
                    }

                    var arr = fromTime.split(":");
                    var hour = parseInt(arr[0], 10);
                    hour = hour + 4;
                    if (hour >= 24)
                        hour = hour - 24;

                    if (hour < 10)
                        toTime = "0" + hour;
                    else
                        time = hour;
                    toTime = hour + ":" + arr[1]

                    fromTimeObject.val(fromTime);
                    toTimeObject.val(toTime);
                }
            }
            else {
                trObject.find(".addhalfday").first().html("");
                trObject.find(".ddlfromtime").hide();
                trObject.find(".ddltotime").hide();
                trObject.find(".tdnumberleaves").html("1");

                var isShow = false;
                var otherTrObject = $(".tabledata tbody tr");
                for (var i = 0; i < otherTrObject.length; i++) {
                    var object = otherTrObject[i];
                    if ($(object).find(".chkhalfday").first().attr("checked")) {
                        isShow = true;
                        break;
                    }
                }

                if (isShow) {
                    $(".tdhalfday").show();
                }
                else {
                    $(".tdhalfday").hide();
                }
            }

            LeaveRequestDetail.getTotalLeave();
        },
        workTypeChange: function (workTypeObject) {
            var workType = workTypeObject.val();

            if (workTypeObject.parent().parent().find(".chkhalfday").attr("checked") == "checked") {
                workTypeObject.parent().parent().find(".ddlfromtime").show();
                workTypeObject.parent().parent().find(".ddltotime").show();
            }
            else {
                workTypeObject.parent().parent().find(".ddlfromtime").hide();
                workTypeObject.parent().parent().find(".ddltotime").hide();
            }

            if (workType == LeaveRequestDetail.leaveWorkType) {
                workTypeObject.parent().parent().find(".ddlleavetype").show();
                workTypeObject.parent().parent().find(".tdnumberleaves").show();
                workTypeObject.parent().parent().find(".chkhalfday").removeAttr("disabled");
                //workTypeObject.parent().parent().find(".addhalfday").show();
            }
            else if (workType == LeaveRequestDetail.offWorkType || workType == LeaveRequestDetail.onDutyWorkType) {
                workTypeObject.parent().parent().find(".ddlleavetype").hide();
                workTypeObject.parent().parent().find(".tdnumberleaves").hide();
                workTypeObject.parent().parent().find(".ddlfromtime").hide();
                workTypeObject.parent().parent().find(".ddltotime").hide();
                workTypeObject.parent().parent().find(".chkhalfday").removeAttr("checked").attr("disabled", "disabled");
                workTypeObject.parent().parent().find(".addhalfday").hide();
                workTypeObject.parent().parent().find(".tdnumberleaves").html("1");
            }
            else {
                workTypeObject.parent().parent().find(".ddlleavetype").hide();
                workTypeObject.parent().parent().find(".tdnumberleaves").hide();
                workTypeObject.parent().parent().find(".chkhalfday").removeAttr("disabled");
            }



            LeaveRequestDetail.getTotalLeave();
        },
        getTotalLeave: function () {
            var totalAnnualLeave = 0;
            var totalUnpaidLeave = 0;
            var objectTr = $(".tabledata tbody tr");
            for (var i = 0; i < objectTr.length; i++) {
                var currentObject = objectTr[i];
                if ($(currentObject).find(".ddlWorkType").first().val() == LeaveRequestDetail.leaveWorkType) {
                    var leaveType = $(currentObject).find(".ddlleavetype").first().val();
                    if (leaveType == LeaveRequestDetail.annualLeaveType) { 
                        totalAnnualLeave += parseFloat($.trim($(currentObject).find(".tdnumberleaves").first().html()));
                    }

                    if (leaveType == LeaveRequestDetail.unpaidLeaveType) { 
                        totalUnpaidLeave += parseFloat($.trim($(currentObject).find(".tdnumberleaves").first().html()));
                    }
                }
            }

            $("#totalAnnualLeave").html(totalAnnualLeave);
            $("#totalUnpaidLeave").html(totalUnpaidLeave);
        },
        enableControlForEdit: function () {
            LeaveRequestDetail.showHideHalfDay();
        },
        showHideHalfDay: function () {
            var isShow = false;
            var objectTr = $(".tabledata tbody tr");
            var length = objectTr.length;
            if (length > 0) {
                var oldObject = objectTr[0];
                var object = objectTr[0];
                for (var i = 0; i < objectTr.length; i++) {
                    object = objectTr[i];
                    if ($(object).find(".chkhalfday").first().attr("checked")) {
                        $(object).find(".addhalfday").first().html("+ Add a half");
                        if (i > 0) {
                            oldObject = objectTr[i - 1];
                            if ($.trim($(object).find(".tdleavedate").first().html()) == $.trim($(oldObject).find(".tdleavedate").first().html())) {
                                var halfdayObject = $(object).find(".addhalfday").first().html("- Remove a half").removeClass("addhalfday").addClass("removehaflday");
                                $(oldObject).find(".addhalfday").first().html("");
                                $(object).find(".chkhalfday").attr("disabled", "disabled");
                                $(oldObject).find(".chkhalfday").attr("disabled", "disabled");

                                halfdayObject.click(function () {
                                    $(this).parent().parent().remove();
                                    $(oldObject).find(".addhalfday").first().html("+ Add a half");
                                    $(oldObject).find(".chkhalfday").removeAttr("disabled");
                                    LeaveRequestDetail.getTotalLeave();
                                });
                            }
                        }

                        isShow = true;
                        $(object).find(".ddlfromtime").show();
                        $(object).find(".ddltotime").show();
                    }
                    else {
                        $(object).find(".ddlfromtime").hide();
                        $(object).find(".ddltotime").hide();
                    }
                }
            }

            if (isShow) {
                $(".tdhalfday").show();
            }
            else {
                $(".tdhalfday").hide();
                $(".ddlfromtime").hide();
                $(".ddltotime").hide();
            }
        },
        disableControlForDetail: function () {
            if ($("#IsView").val() == "True") {
                $("input").attr("disabled", "disabled");
                $("select").attr("disabled", "disabled");
                $("textarea").attr("disabled", "disabled");
                $(".tdhalfday").hide();
            }
        },
        showHideControlByWorkType: function () {
            var objectTr = $(".tabledata tbody tr");
            var length = objectTr.length;

            if (length > 0) {
                var object = objectTr[0];
                for (var i = 0; i < objectTr.length; i++) {
                    object = objectTr[i];
                    if ($(object).find(".ddlWorkType").first().val() == LeaveRequestDetail.leaveWorkType) {
                    }
                    else {
                        $(object).find(".ddlleavetype").hide();
                        $(object).find(".tdnumberleaves").hide();
                        if ($(object).find(".ddlWorkType").first().val() == LeaveRequestDetail.offWorkType) {
                            $(object).find(".chkhalfday").attr("disabled", "disabled");
                        }
                    }
                }
            }
        }
    };
} (jQuery);

$(document).ready(function () {
    LeaveRequestDetail.init();
    $('form').areYouSure();
});