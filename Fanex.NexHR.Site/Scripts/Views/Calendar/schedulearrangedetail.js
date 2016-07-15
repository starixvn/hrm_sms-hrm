'use strict';
/*global _global, ScheduleArrangeCommon, ScheduleArrange, floatingHeader*/

var ScheduleArrangeDetail = (function ($) {
    return {
        previous: null,
        next: null,
        previousFromDate: null,
        previousToDate: null,
        currentFromDate: null,
        currentToDate: null,
        nextFromDate: null,
        nextToDate: null,
        employeeId: null,
        groupId: null,
        jobTitle: null,
        chkAllMonday: null,
        chkAllTuesday: null,
        chkAllWednesday: null,
        chkAllThursday: null,
        chkAllFriday: null,
        chkAllSaturday: null,
        chkAllSunday: null,
        allShifts: null,
        lnkSort: null,
        lnkSave: null,
        messageComfirm: null,

        init: function () {
            floatingHeader();
            ScheduleArrangeDetail.messageComfirm = "Are you sure to refresh the screen based on your filter condition? " +
				"Data you have entered may not be saved." +
				" </br></br>  Click OK to proceed or Cancel to come back and save first";
            ScheduleArrangeDetail.previous = $(".lnkPrevious");
            ScheduleArrangeDetail.next = $(".lnkNext");
            ScheduleArrangeDetail.previousFromDate = $("#PreviousFromDate");
            ScheduleArrangeDetail.previousToDate = $("#PreviousToDate");
            ScheduleArrangeDetail.currentFromDate = $("#CurrentFromDate");
            ScheduleArrangeDetail.currentToDate = $("#CurrentToDate");
            ScheduleArrangeDetail.nextFromDate = $("#NextFromDate");
            ScheduleArrangeDetail.nextToDate = $("#NextToDate");
            ScheduleArrangeDetail.employeeId = $("#EmployeeId");
            ScheduleArrangeDetail.groupId = $("#GroupId");
            ScheduleArrangeDetail.jobTitle = $("#JobTitle");
            ScheduleArrangeDetail.chkAllMonday = $(".chkAllMonday");
            ScheduleArrangeDetail.chkAllTuesday = $(".chkAllTuesday");
            ScheduleArrangeDetail.chkAllWednesday = $(".chkAllWednesday");
            ScheduleArrangeDetail.chkAllThursday = $(".chkAllThursday");
            ScheduleArrangeDetail.chkAllFriday = $(".chkAllFriday");
            ScheduleArrangeDetail.chkAllSaturday = $(".chkAllSaturday");
            ScheduleArrangeDetail.chkAllSunday = $(".chkAllSunday");
            ScheduleArrangeDetail.lnkSort = $(".lnkSort");
            ScheduleArrangeDetail.lnkSave = $(".lnkSave");

            window.onbeforeunload = ScheduleArrangeCommon.confirmExit;

            $(".shift").on("focus", function () {
                var $this = $(this);
                if (!$this.hasClass("ui-autocomplete-input")) {
                    ScheduleArrangeDetail.shiftAutocomplete($this);
                }
            });

            ScheduleArrangeDetail.lnkSave.on("click", function () {
                ScheduleArrangeCommon.saveArrangeSchedule();
                return false;
            });

            $(".lnkremove").on("click", function () {
                $(this).parent().parent().remove();
                return false;
            });

            ScheduleArrangeDetail.lnkSort.on("click", function () {
                var url = _global.root + "Calendar/MemberScheduleInOrder";
                $.get(url, function (data) {
                    $('#dialogSortcontent').html(data);
                    $("#dialogSortcontent").dialog({
                        modal: true,
                        minHeight: 0,
                        width: 650,
                        title: 'Member List',
                        buttons: {
                            "Save": function () {
                                ScheduleArrangeDetail.saveOrderOfMember();

                                $(this).dialog("close");
                            },
                            Cancel: function () {
                                $(this).dialog("close");
                            }
                        },
                        close: function () {
                            $("#shiftmulti").val("");
                            $("#shiftmulti").removeAttr("data-shiftid");
                        }
                    });
                });

                return false;
            });

            $(".lnkOffAndNoShift").on("click", function () {
                if ($("input[type='checkbox'].multiple:checked").length > 0) {
                    var objectSelected = $("input[type='checkbox'].multiple:checked").parent().prev().find("input[type='text']:not(.holiday)");
                    objectSelected.addClass("offday");
                    for (var i = 0; i < objectSelected.length; i++) {
                        $(objectSelected[i]).val("Off");
                        $(objectSelected[i]).removeAttr("data-shiftid");
                    }

                    var trObject = objectSelected.parent().parent();
                    ScheduleArrangeDetail.checkWarningRow(trObject);
                    ScheduleArrangeCommon.hasChangeValue = true;
                }
                else {
                    alert('Please choose at least one day to off!');
                }

                return false;
            });

            $(".shift").change(function () {
                ScheduleArrangeDetail.shiftChange($(this));
                return false;
            });


            $(".lnkDepartment").on("click", function (event) {
                event.preventDefault();
                location.href = _global.root + "Calendar/ShiftStatistic";
            });

            $(".lnkSaveRow").on("click", function () {
                var trObject = $(this).parent().parent();
                ScheduleArrangeDetail.saveRow(trObject);
            });

            ScheduleArrangeDetail.previous.on("click", function () {
                if (ScheduleArrangeCommon.hasChangeValue) {
                    confirm(ScheduleArrangeDetail.messageComfirm, function () {
                        ScheduleArrangeDetail.queryPreviousAction();
                    });
                }
                else {
                    ScheduleArrangeDetail.queryPreviousAction();
                }
            });

            ScheduleArrangeDetail.next.on("click", function () {
                if (ScheduleArrangeCommon.hasChangeValue) {
                    confirm(ScheduleArrangeDetail.messageComfirm, function () {
                        ScheduleArrangeDetail.queryNextAction();
                    });
                }
                else {
                    ScheduleArrangeDetail.queryNextAction();
                }
            });

            $("#dialog-multiple").dialog({
                autoOpen: false,
                height: 180,
                width: 270,
                modal: true,
                buttons: {
                    "Ok": function () {
                        var shiftCode = $.trim($("#shiftmulti").val());
                        var shiftId = $("#shiftmulti").attr("data-shiftid");
                        var isOffday = $("#chkIsShiftInOffday").attr("checked") === "checked" || shiftCode.toLowerCase() === "off";
                        var objectSelected = $("input[type='checkbox'].multiple:checked").parent().prev().find("input[type='text']:not(.holiday)");

                        if (shiftId !== "undefined" && shiftCode.length > 0 && shiftCode.toLowerCase() !== "off") {
                            objectSelected.val(shiftCode);
                            objectSelected.attr("data-shiftid", shiftId);
                        }
                        else {
                            objectSelected.val("Off");
                            objectSelected.removeAttr("data-shiftid");
                        }
                        if (isOffday) {
                            objectSelected.addClass("offday");
                        }
                        else {
                            objectSelected.removeClass("offday");
                        }

                        var trObject = objectSelected.parent().parent();
                        ScheduleArrangeDetail.checkWarningRow(trObject);
                        ScheduleArrangeCommon.hasChangeValue = true;
                        $(this).dialog("close");
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                },
                close: function () {
                    $("#shiftmulti").val("");
                    $("#shiftmulti").removeAttr("data-shiftid");
                }
            });

            $("#dialog-halfday").dialog({
                autoOpen: false,
                height: 160,
                width: 180,
                modal: true,
                buttons: {
                    "Ok": function () {
                        var shiftCode = $.trim($("#shifthalfday").val());
                        var shiftId = $("#shifthalfday").attr("data-shiftid");
                        var inputObject = ScheduleArrangeCommon.currentShiftDate.parent().prev().find("input[type='text']");
                        if (shiftId !== "undefined" && shiftCode.length > 0) {
                            ScheduleArrangeCommon.currentShiftDate.attr("title", shiftCode).addClass("tooltip");
                            inputObject.attr("data-shiftid", shiftId);
                        }

                        ScheduleArrangeCommon.hasChangeValue = true;
                        $(this).dialog("close");
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                },
                close: function () {
                    $("#shiftmulti").val("");
                    $("#shiftmulti").removeAttr("data-shiftid");
                }
            });

            $(".lnkmultiple").on("click", function () {
                if ($("input[type='checkbox'].multiple:checked").length > 0) {
                    $("#shiftmulti").val("");
                    $("#shiftmulti").removeAttr("data-shiftid");
                    $("#chkIsShiftInOffday").removeAttr("checked");

                    $("#dialog-multiple").dialog("open");

                }
                else {
                    alert('Please choose at least one day to continue!');
                }

                return false;
            });

            $(".lnkAddShift").on("click", function () {
                $("#shifthalfday").val("");
                $("#shifthalfday").removeAttr("data-shiftid");
                ScheduleArrangeCommon.currentShiftDate = $(this);
                $("#dialog-halfday").dialog("open");
            });

            $(".lnkUnCheckAll").on("click", function () {
                $("#tblScheduleArrange input[type='checkbox']:not(.chkIsPublic)").removeAttr("checked");
                ScheduleArrangeCommon.hasChangeValue = true;

                return false;
            });

            $(".lnkCheckAll").on("click", function () {
                $("#tblScheduleArrange input[type='checkbox']:not(.chkIsPublic)").attr("checked", true);
                ScheduleArrangeCommon.hasChangeValue = true;

                return false;
            });

            $(".lnkOffday").on("click", function () {
                if ($("input[type='checkbox'].multiple:checked").length > 0) {
                    var objectSelected = $("input[type='checkbox'].multiple:checked").parent().prev().find("input[type='text']:not(.holiday)");
                    objectSelected.addClass("offday");
                    for (var i = 0; i < objectSelected.length; i++) {
                        if ($.trim($(objectSelected[i]).val()).length === 0) {
                            $(objectSelected[i]).val("Off");
                        }
                    }

                    var trObject = objectSelected.parent().parent();
                    ScheduleArrangeDetail.checkWarningRow(trObject);
                    ScheduleArrangeCommon.hasChangeValue = true;
                }
                else {
                    alert('Please choose at least one day to off!');
                }

                return false;
            });

            $(".lnkUnOffday").on("click", function () {
                if ($("input[type='checkbox'].multiple:checked").length > 0) {
                    var objectSelected = $("input[type='checkbox'].multiple:checked").parent().prev().find("input[type='text']");
                    objectSelected.removeClass("offday");
                    for (var i = 0; i < objectSelected.length; i++) {
                        if ($.trim($(objectSelected[i]).val()) === "Off") {
                            $(objectSelected[i]).val("");
                        }
                    }

                    var trObject = objectSelected.parent().parent();
                    ScheduleArrangeDetail.checkWarningRow(trObject);
                    ScheduleArrangeCommon.hasChangeValue = true;
                }
                else {
                    alert('Please choose at least one day to work!');
                }

                return false;
            });

            $(".copyrow").on("click", function () {
                var thisObject = $(this);
                var shiftObject = thisObject.parent().find("input").first();
                var shiftCode = shiftObject.val();
                var shiftId = shiftObject.attr("data-shiftid");
                if (shiftCode.length > 0 && shiftId !== "undefined") {
                    var inputShifts = thisObject.parent().parent().find("input:not(:disabled)");
                    inputShifts.val(shiftCode);
                    inputShifts.attr("data-shiftid", shiftId);
                    inputShifts.removeClass("offday");

                    ScheduleArrangeCommon.hasChangeValue = true;
                }

                return false;
            });

            $(".lnkPublicAll").on("click", function () {
                var isChecked = true;
                var chkIsPublic = $(".chkIsPublic");
                for (var i = 0; i < chkIsPublic.length; i++) {
                    if ($(chkIsPublic[i]).attr("checked") !== "checked") {
                        isChecked = false;
                        break;
                    }
                }

                $(".chkIsPublic").attr("checked", !isChecked);
            });

            ScheduleArrangeDetail.changeWeekDay();
            ScheduleArrangeDetail.getAllShift();
            ScheduleArrangeDetail.checkWarningAllRow();
        },
        shiftChange: function (object) {
            var isOffDay = false;
            var isShiftOrOffDay = false;
            var shiftId = "";
            var shiftCode = "";
            for (var i = 0; i < ScheduleArrangeDetail.allShifts.length; i++) {
                if (ScheduleArrangeDetail.allShifts[i].Code.toLowerCase() === $.trim(object.val().toLowerCase())) {
                    shiftId = ScheduleArrangeDetail.allShifts[i].Id;
                    shiftCode = ScheduleArrangeDetail.allShifts[i].Code;
                    if (ScheduleArrangeDetail.allShifts[i].Id === -1) {
                        isOffDay = true;
                    }

                    isShiftOrOffDay = true;
                    break;
                }
            }

            if (isShiftOrOffDay) {
                if (isOffDay) {
                    if (!object.hasClass("holiday")) {
                        object.addClass("offday");
                        object.removeAttr("data-shiftid");
                        object.val(shiftCode);
                    }
                    else {
                        object.removeClass("offday");
                        object.removeAttr("data-shiftid");
                        object.val("");
                    }
                }
                else {
                    object.removeClass("offday");
                    object.attr("data-shiftid", shiftId);
                }
            }
            else {
                object.removeClass("offday");
                object.removeAttr("data-shiftid");
                object.val("");
            }

            // for warning
            var trObject = object.parent().parent();
            ScheduleArrangeDetail.checkWarningRow(trObject);
        },
        saveOrderOfMember: function () {
            var employeeOrders = [];
            var employees = $("#tblSupervisor").find("tbody tr");

            for (var i = 0; i < employees.length; i++) {
                var employeeId = $(employees[i]).children().eq(3).attr("data-id");
                var orderCode = $($(employees[i]).find("input.ordercode").first()).val();

                employeeOrders.push({
                    EmployeeId: employeeId,
                    OrderCode: $.trim(orderCode)
                });
            }

            $.ajax({
                type: 'POST',
                url: _global.root + 'calendar/MemberScheduleInOrder',
                data: {
                    employeeOrders: JSON.stringify(employeeOrders)
                },
                success: function (data) {
                    if (data.Code === 0) {
                        location.href = _global.root + "Calendar/arrangeschedule";
                    }
                    else {
                        alert(data.Message);
                    }
                }
            });
        },
        changeWeekDay: function () {
            ScheduleArrangeDetail.chkAllMonday.on("change", function () {
                var isChecked = $(this).attr("checked") === "checked";
                $(".chkMonday").attr("checked", isChecked);
                ScheduleArrangeDetail.chkAllMonday.attr("checked", isChecked);
            });

            ScheduleArrangeDetail.chkAllTuesday.on("change", function () {
                var isChecked = $(this).attr("checked") === "checked";
                $(".chkTuesday").attr("checked", isChecked);
                ScheduleArrangeDetail.chkAllTuesday.attr("checked", isChecked);
            });

            ScheduleArrangeDetail.chkAllWednesday.on("change", function () {
                var isChecked = $(this).attr("checked") === "checked";
                $(".chkWednesday").attr("checked", isChecked);
                ScheduleArrangeDetail.chkAllWednesday.attr("checked", isChecked);
            });

            ScheduleArrangeDetail.chkAllThursday.on("change", function () {
                var isChecked = $(this).attr("checked") === "checked";
                $(".chkThursday").attr("checked", isChecked);
                ScheduleArrangeDetail.chkAllThursday.attr("checked", isChecked);
            });

            ScheduleArrangeDetail.chkAllFriday.on("change", function () {
                var isChecked = $(this).attr("checked") === "checked";
                $(".chkFriday").attr("checked", isChecked);
                ScheduleArrangeDetail.chkAllFriday.attr("checked", isChecked);
            });

            ScheduleArrangeDetail.chkAllSaturday.on("change", function () {
                var isChecked = $(this).attr("checked") === "checked";
                $(".chkSaturday").attr("checked", isChecked);
                ScheduleArrangeDetail.chkAllSaturday.attr("checked", isChecked);
            });

            ScheduleArrangeDetail.chkAllSunday.on("change", function () {
                var isChecked = $(this).attr("checked") === "checked";
                $(".chkSunday").attr("checked", isChecked);
                ScheduleArrangeDetail.chkAllSunday.attr("checked", isChecked);
            });

            $(".chkMonday").on("change", function () {
                var isChecked = true;
                var chkMondays = $(".chkMonday");
                for (var i = 0; i < chkMondays.length; i++) {
                    if ($(chkMondays[i]).attr("checked") !== "checked") {
                        isChecked = false;
                        break;
                    }
                }

                ScheduleArrangeDetail.chkAllMonday.attr("checked", isChecked);
            });

            $(".chkTuesday").on("change", function () {
                var isChecked = true;
                var chkTuesdays = $(".chkTuesday");
                for (var i = 0; i < chkTuesdays.length; i++) {
                    if ($(chkTuesdays[i]).attr("checked") !== "checked") {
                        isChecked = false;
                        break;
                    }
                }

                ScheduleArrangeDetail.chkAllTuesday.attr("checked", isChecked);
            });

            $(".chkWednesday").on("change", function () {
                var isChecked = true;
                var chkWednesdays = $(".chkWednesday");
                for (var i = 0; i < chkWednesdays.length; i++) {
                    if ($(chkWednesdays[i]).attr("checked") !== "checked") {
                        isChecked = false;
                        break;
                    }
                }

                ScheduleArrangeDetail.chkAllWednesday.attr("checked", isChecked);
            });

            $(".chkThursday").on("change", function () {
                var isChecked = true;
                var chkThursdays = $(".chkThursday");
                for (var i = 0; i < chkThursdays.length; i++) {
                    if ($(chkThursdays[i]).attr("checked") !== "checked") {
                        isChecked = false;
                        break;
                    }
                }

                ScheduleArrangeDetail.chkAllThursday.attr("checked", isChecked);
            });

            $(".chkFriday").on("change", function () {
                var isChecked = true;
                var chkFridays = $(".chkFriday");
                for (var i = 0; i < chkFridays.length; i++) {
                    if ($(chkFridays[i]).attr("checked") !== "checked") {
                        isChecked = false;
                        break;
                    }
                }

                ScheduleArrangeDetail.chkAllFriday.attr("checked", isChecked);
            });

            $(".chkSaturday").on("change", function () {
                var isChecked = true;
                var chkSaturdays = $(".chkSaturday");
                for (var i = 0; i < chkSaturdays.length; i++) {
                    if ($(chkSaturdays[i]).attr("checked") !== "checked") {
                        isChecked = false;
                        break;
                    }
                }

                ScheduleArrangeDetail.chkAllSaturday.attr("checked", isChecked);
            });

            $(".chkSunday").on("change", function () {
                var isChecked = true;
                var chkSundays = $(".chkSunday");
                for (var i = 0; i < chkSundays.length; i++) {
                    if ($(chkSundays[i]).attr("checked") !== "checked") {
                        isChecked = false;
                        break;
                    }
                }

                ScheduleArrangeDetail.chkAllSunday.attr("checked", isChecked);
            });
        },
        checkWarningAllRow: function () {
            var trObjects = $("#tblScheduleArrange tbody tr");
            for (var i = 0; i < trObjects.length; i++) {
                ScheduleArrangeDetail.checkWarningRow(trObjects[i]);
            }
        },
        checkWarningRow: function (trObject) {
            var numberOffDefault = $(trObject).find(".multipleShift").first().attr("data-numberoffbyweek");
            var numberOffCurrent = $(trObject).find("td.shiftdate input.offday").length;
            if (numberOffDefault < numberOffCurrent) {
                $(trObject).find("td a.lnkremove").parent().addClass("overOffdayInWeek").removeClass("upOffdayInWeek");
                $(trObject).find("td a.lnkremove").parent().html('<span class="icon-warning"></span>');
            }
            else if (numberOffDefault > numberOffCurrent) {
                $(trObject).find("td a.lnkremove").parent().addClass("upOffdayInWeek").removeClass("overOffdayInWeek");
                $(trObject).find("td a.lnkremove").parent().html('<a class="lnkremove" href="javascript:;">-</a>');
            }
            else {
                $(trObject).find("td a.lnkremove").parent().removeClass("overOffdayInWeek").removeClass("upOffdayInWeek");
                $(trObject).find("td a.lnkremove").parent().html('<a class="lnkremove" href="javascript:;">-</a>');
            }
        },
        getAllShift: function () {
            $.ajax({
                type: 'POST',
                dataType: "json",
                url: _global.root + 'Calendar/GetShift',
                data: {
                    query: " ",
                    isShowOff: true
                },
                success: function (data) {
                    ScheduleArrangeDetail.allShifts = data;
                }
            });
        },
        queryNextAction: function () {
            $("#FromDate").val(ScheduleArrangeDetail.nextFromDate.val());
            $("#ToDate").val(ScheduleArrangeDetail.nextToDate.val());

            var groups = ScheduleArrangeDetail.groupId.val();
            var groupIds = groups != null ? groups.join() : "";
            var jobTitle = ScheduleArrangeDetail.jobTitle.val();
            var jobTitles = jobTitle != null ? jobTitle.join() : "";

            ScheduleArrange.bindScheduleArrangeByDate(ScheduleArrangeDetail.nextFromDate.val(),
                        ScheduleArrangeDetail.nextToDate.val(), ScheduleArrangeDetail.employeeId.val(), groupIds, jobTitles);
        },
        queryPreviousAction: function () {
            $("#FromDate").val(ScheduleArrangeDetail.previousFromDate.val());
            $("#ToDate").val(ScheduleArrangeDetail.previousToDate.val());

            var groups = ScheduleArrangeDetail.groupId.val();
            var groupIds = groups != null ? groups.join() : "";
            var jobTitle = ScheduleArrangeDetail.jobTitle.val();
            var jobTitles = jobTitle != null ? jobTitle.join() : "";

            ScheduleArrange.bindScheduleArrangeByDate(ScheduleArrangeDetail.previousFromDate.val(),
                        ScheduleArrangeDetail.previousToDate.val(), ScheduleArrangeDetail.employeeId.val(), groupIds, jobTitles);
        },
        saveRow: function (trObject) {
            var scheduleArranges = [];
            var shiftMultiples = [];
            var employeeId = "";
            var workDate = "";
            var shiftId = "";
            var isOffDay = false;
            var isHoliday = false;
            var isPublic = false;
            var shiftMultipleId = "";

            employeeId = $("td.tdemployee", trObject).first().attr("data-employeeid");

            if (employeeId !== "undefined" && employeeId > 0) {
                var tdObjects = $("td.shiftdate", trObject);
                isPublic = $("input[type='checkbox'].chkIsPublic", trObject).attr("checked") === "checked";
                shiftMultipleId = $("input[type='text'].multipleShift", trObject).first().attr("data-shiftid");
                workDate = $("input[type='text'].multipleShift", trObject).first().attr("data-date");

                // for shift multiple
                shiftMultiples.push({
                    EmployeeId: employeeId,
                    ForDate: workDate,
                    ShiftId: shiftMultipleId
                });

                // for schedule arrange
                for (var j = 0; j < tdObjects.length; j++) {
                    workDate = $("input[type='text'].shift", tdObjects[j]).first().attr("data-date");
                    shiftId = $("input[type='text'].shift", tdObjects[j]).first().attr("data-shiftid");
                    isOffDay = $("input[type='text'].shift", tdObjects[j]).first().hasClass("offday");
                    isHoliday = $("input[type='text'].shift", tdObjects[j]).first().hasClass("holiday");

                    if (shiftId === "undefined" || shiftId === "-1") {
                        shiftId = "";
                    }

                    if (isHoliday) {
                        isOffDay = false;
                    }

                    if (workDate !== "undefined" && Date.parse(workDate)) {
                        scheduleArranges.push({
                            EmployeeId: employeeId,
                            WorkDate: workDate,
                            ShiftId: shiftId,
                            IsHoliday: isHoliday,
                            IsOffDay: isOffDay,
                            IsPublic: isPublic
                        });
                    }
                }

                $.ajax({
                    type: 'POST',
                    dataType: "json",
                    url: '/Calendar/ArrangeSchedule',
                    data: { scheduleArranges: JSON.stringify(scheduleArranges), shiftMultiples: JSON.stringify(shiftMultiples) },
                    success: function (response) {
                        if (response.errorCode === 0) {
                            ScheduleArrangeCommon.showSuccessMsgInRow();
                        }
                        else {
                            ScheduleArrangeCommon.showErrorMsgInRow(response.message);
                        }
                    }
                });
            }

            return false;
        },
        shiftAutocomplete: function (evt) {
            var shiftCode = "";
            evt.autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: _global.root + 'Calendar/GetShift',
                        data: {
                            query: request.term,
                            isShowOff: !evt.hasClass("holiday")
                        },
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.Code,
                                    value: item.Id
                                };
                            }));
                        }
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    shiftCode = ui.item.label;
                    $(this).val(shiftCode);
                    $(this).attr("data-shiftid", ui.item.value);
                    ScheduleArrangeCommon.hasChangeValue = true;
                    ScheduleArrangeDetail.shiftChange($(this));
                    return false;
                },
                focus: function (event, ui) {
                    shiftCode = ui.item.label;
                    $(this).val(shiftCode);
                    $(this).attr("data-shiftid", ui.item.value);
                    event.preventDefault();
                    return false;
                },
                close: function () {
                    return false;
                }
            });
        }
    };
} (jQuery));

$(document).ready(function () {
    ScheduleArrangeDetail.init();
});
