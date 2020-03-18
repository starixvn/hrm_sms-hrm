import 'jquery.are-you-sure/jquery.are-you-sure';
import { LeaveType } from '../../../shared/constants';

var LeaveRequestDetail = (function ($) {
    return {
        leaveWorkType: null,
        onDutyWorkType: null,
        offWorkType: null,

        init: function () {
            LeaveRequestDetail.leaveWorkType = '2'; // hard code for worktype is leave
            LeaveRequestDetail.onDutyWorkType = '3';
            LeaveRequestDetail.offWorkType = '1';

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

                halfdayObject.click(function () {
                    $(this).parent().parent().remove();
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

                trObject.find(".chkhalfday").prop("checked", true).attr("disabled", "disabled");
                trCopy.find(".chkhalfday").attr("checked", true).attr("disabled", "disabled");

                // bind value for time and check box of copy row
                trCopy.find(".ddlfromtime").val(trObject.find(".ddltotime").val());
                trCopy.find(".ddltotime").val(trCopy.find(".ddltotime").attr("data-outtime"));
                if (trObject.find(".chkIsToAfterday").prop("checked")) {
                    trCopy.find(".chkIsFromAfterday").attr("checked", true);
                } else {
                    trCopy.find(".chkIsFromAfterday").prop("checked", false);
                }

                if (trCopy.find(".ddltotime").attr("data-isOutAfterday") === "true") {
                    trCopy.find(".chkIsToAfterday").prop("checked", trCopy.find(".ddltotime").attr("data-isOutAfterday"));
                } else {
                    trCopy.find(".chkIsToAfterday").prop("checked", false);
                }
                // end
            });

            $(".ddlleavetype").change(function () {
                const leaveTypeId = parseInt($(this).val(), 10);
                var trObject = $(this).parent().parent();

                LeaveRequestDetail.showHideCheckboxHalfDay(trObject, leaveTypeId);
                LeaveRequestDetail.getTotalLeave();
            });

            LeaveRequestDetail.disableControlForDetail();
        },
        halfdayChange: function (trObject) {
            if (trObject.find(".chkhalfday").prop("checked")) {
                $(".tdhalfday").show();
                trObject.find(".addhalfday").first().html("+ Add a half").show();
                var fromTimeObject = trObject.find(".ddlfromtime");
                var toTimeObject = trObject.find(".ddltotime");

                fromTimeObject.show();
                toTimeObject.show();
                trObject.find(".timehalfday").show();
                trObject.find(".tdnumberleaves").html("0.5");

                LeaveRequestDetail.fillTimeForHalfday(trObject);
            } else {
                trObject.find(".addhalfday").first().html("");
                trObject.find(".ddlfromtime").hide();
                trObject.find(".ddltotime").hide();
                trObject.find(".timehalfday").hide();
                trObject.find(".tdnumberleaves").html("1");

                var isShow = false;
                var otherTrObject = $(".tabledata tbody tr");
                for (var i = 0; i < otherTrObject.length; i++) {
                    var object = otherTrObject[i];
                    if ($(object).find(".chkhalfday").first().prop("checked")) {
                        isShow = true;
                        break;
                    }
                }

                if (isShow) {
                    $(".tdhalfday").show();
                } else {
                    $(".tdhalfday").hide();
                }
            }

            LeaveRequestDetail.getTotalLeave();
        },
        fillTimeForHalfday: function (trObject) {
            $.getJSON(_global.root + 'shift/getshiftbydate',
                { employeeId: $("#EmployeeId").val(), date: trObject.find(".tdleavedate").attr("data-date") },
                function (data) {
                    if (data !== null) {
                        var arrTimeIn = data.BeginCheckInTime.split(":");
                        var arrTimeOut = data.BeginCheckOutTime.split(":");

                        var minuteFrom = (parseInt(arrTimeIn[0], 10) * 60) + parseInt(arrTimeIn[1], 10);
                        var minuteTo = (parseInt(arrTimeOut[0], 10) * 60) + parseInt(arrTimeOut[1], 10);
                        var isFromAfterday = data.IsInAfterDay;
                        var isToAfterday = false;
                        if (data.IsInAfterDay) {
                            minuteFrom += 1440; //60*24
                        }

                        if (data.IsOutAfterDay) {
                            minuteTo += 1440;
                        }

                        var averageTime = (minuteFrom + minuteTo) / 2;
                        if (averageTime >= 1440) {
                            averageTime = averageTime - 1440;
                            isToAfterday = true;
                        }

                        var hour = Math.floor(averageTime / 60);
                        var minute = Math.floor((averageTime % 60) / 30) * 30;
                        minute = minute === 0 ? "00" : minute;
                        if (hour < 10) {
                            hour = "0" + hour;
                        }

                        var toTime = hour + ":" + minute;
                        trObject.find(".ddlfromtime").val(data.BeginCheckInTime);
                        trObject.find(".ddltotime").val(toTime);

                        trObject.find(".chkIsFromAfterday").prop("checked", isFromAfterday);
                        trObject.find(".chkIsToAfterday").prop("checked", isToAfterday);
                        trObject.find(".ddltotime").attr("data-outtime", data.BeginCheckOutTime);
                        trObject.find(".ddltotime").attr("data-isOutAfterday", data.IsOutAfterDay);
                    }
                });
        },
        workTypeChange: function (workTypeObject) {
            var workType = workTypeObject.val();

            if (workTypeObject.parent().parent().find(".chkhalfday").prop("checked")) {
                workTypeObject.parent().parent().find(".ddlfromtime").show();
                workTypeObject.parent().parent().find(".ddltotime").show();
                workTypeObject.parent().parent().find(".timehalfday").show();
            } else {
                workTypeObject.parent().parent().find(".ddlfromtime").hide();
                workTypeObject.parent().parent().find(".ddltotime").hide();
                workTypeObject.parent().parent().find(".timehalfday").hide();
            }

            if (workType === LeaveRequestDetail.leaveWorkType) {
                workTypeObject.parent().parent().find(".ddlleavetype").show();
                workTypeObject.parent().parent().find(".tdnumberleaves").show();
                workTypeObject.parent().parent().find(".chkhalfday").removeAttr("disabled");
            } else if (workType === LeaveRequestDetail.offWorkType || workType === LeaveRequestDetail.onDutyWorkType) {
                workTypeObject.parent().parent().find(".ddlleavetype").hide();
                workTypeObject.parent().parent().find(".tdnumberleaves").hide();
                workTypeObject.parent().parent().find(".ddlfromtime").hide();
                workTypeObject.parent().parent().find(".ddltotime").hide();
                workTypeObject.parent().parent().find(".timehalfday").hide();
                workTypeObject.parent().parent().find(".chkhalfday").prop("checked", false).attr("disabled", "disabled");
                workTypeObject.parent().parent().find(".addhalfday").hide();
                workTypeObject.parent().parent().find(".tdnumberleaves").html("1");
            } else {
                workTypeObject.parent().parent().find(".ddlleavetype").hide();
                workTypeObject.parent().parent().find(".tdnumberleaves").hide();
                workTypeObject.parent().parent().find(".chkhalfday").removeAttr("disabled");
            }

            LeaveRequestDetail.getTotalLeave();
        },
        getTotalLeave: function () {
            let totalAnnualLeave = 0,
                totalUnpaidLeave = 0,
                objectTr = $(".tabledata tbody tr");
            for (var i = 0; i < objectTr.length; i++) {
                var currentObject = objectTr[i];
                if ($(currentObject).find(".ddlWorkType").first().val() === LeaveRequestDetail.leaveWorkType) {
                    const leaveType = parseInt($(currentObject).find(".ddlleavetype").first().val(), 10);
                    if (leaveType === LeaveType.AnnualLeave) {
                        totalAnnualLeave += parseFloat($.trim($(currentObject).find(".tdnumberleaves").first().html()));
                    }

                    if (leaveType === LeaveType.UnpaidLeave) {
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
        showHideCheckboxHalfDay: function (trObject, leaveTypeId) {
            var $chkhalfday = trObject.find('.chkhalfday');
            if (leaveTypeId === LeaveType.AnnualLeave
                || leaveTypeId === LeaveType.UnpaidLeave
                || leaveTypeId === LeaveType.BusinessTrip
                || leaveTypeId === LeaveType.KeepInViewByDay) {
                $chkhalfday.show();
            } else {
                if ($chkhalfday.prop('checked')) {
                    $chkhalfday.prop('checked', false).change();
                }

                $chkhalfday.hide();
            }
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
                    if ($(object).find(".chkhalfday").first().prop("checked")) {
                        $(object).find(".addhalfday").first().html("+ Add a half");
                        if (i > 0) {
                            oldObject = objectTr[i - 1];
                            LeaveRequestDetail.bindSecondHalfDay(object, oldObject);
                        }

                        isShow = true;
                        $(object).find(".ddlfromtime").show();
                        $(object).find(".ddltotime").show();
                        $(object).find(".timehalfday").show();
                    }
                    else {
                        $(object).find(".ddlfromtime").hide();
                        $(object).find(".ddltotime").hide();
                        $(object).find(".timehalfday").hide();
                    }
                }
            }

            if (isShow) {
                $(".tdhalfday").show();
            } else {
                $(".tdhalfday").hide();
                $(".ddlfromtime").hide();
                $(".ddltotime").hide();
                $(".timehalfday").hide();
            }
        },
        bindSecondHalfDay: function (object, oldObject) {
            if ($.trim($(object).find(".tdleavedate").first().html()) === $.trim($(oldObject).find(".tdleavedate").first().html())) {
                var halfdayObject = $(object).find(".addhalfday").first().html("- Remove a half");
                halfdayObject.removeClass("addhalfday").addClass("removehaflday");
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
        },
        disableControlForDetail: function () {
            if ($("#IsView").val() === "True") {
                $('.form-horizontal input').attr('disabled', 'disabled');
                $("select").attr("disabled", "disabled");
                $("textarea").attr("disabled", "disabled");
                $(".tdhalfday").hide();
                LeaveRequestDetail.getTotalLeave();
            }
        },
        showHideControlByWorkType: function () {
            var objectTr = $(".tabledata tbody tr");
            var length = objectTr.length;

            if (length > 0) {
                var object = objectTr[0];
                for (var i = 0; i < objectTr.length; i++) {
                    object = objectTr[i];
                    if ($(object).find(".ddlWorkType").first().val() !== LeaveRequestDetail.leaveWorkType) {
                        $(object).find(".ddlleavetype").hide();
                        $(object).find(".tdnumberleaves").hide();
                        if ($(object).find(".ddlWorkType").first().val() === LeaveRequestDetail.offWorkType) {
                            $(object).find(".chkhalfday").attr("disabled", "disabled");
                        }
                    }
                }
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    LeaveRequestDetail.init();
    $('form').areYouSure();
});