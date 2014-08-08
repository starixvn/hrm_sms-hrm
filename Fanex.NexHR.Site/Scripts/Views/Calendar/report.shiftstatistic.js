'use strict';
/*global _global*/

var ShiftStatistic = (function ($) {
    return {
        groupId: null,
        fromDate: null,
        toDate: null,
        isShowWholeCompany: null,
        submit: null,

        init: function () {
            ShiftStatistic.groupId = $("#GroupId");
            ShiftStatistic.fromDate = $("#FromDate");
            ShiftStatistic.toDate = $("#ToDate");
            ShiftStatistic.submit = $("#btnSubmit");
            ShiftStatistic.isShowWholeCompany = $("#chkIsShowWholeCompany");
            ShiftStatistic.groupId.multiselect();
            ShiftStatistic.category = $("#category");

            ShiftStatistic.bindShiftStatisticContent(ShiftStatistic.fromDate.val(), ShiftStatistic.toDate.val(), '', 
				(ShiftStatistic.isShowWholeCompany.attr("checked") === "checked"), ShiftStatistic.category.val());

            ShiftStatistic.submit.on("click", function () {
                var groups = ShiftStatistic.groupId.val();
                var groupIds = groups != null ? groups.join() : "";
                //console.log(ShiftStatistic.isShowWholeCompany.attr("checked") == "checked");
                ShiftStatistic.bindShiftStatisticContent(ShiftStatistic.fromDate.val(), ShiftStatistic.toDate.val(),
                    groupIds, (ShiftStatistic.isShowWholeCompany.attr("checked") === "checked"), ShiftStatistic.category.val());

            });
        },
        bindShiftStatisticContent: function (fromDate, toDate, groupIds, isShowWholeCompany, isLocal) {
            var shiftStatisticContainer = $("#shiftStatistic");
            var params = {
                "fromDate": fromDate,
                "toDate": toDate,
                "groupIds": groupIds,
                "isShowWholeCompany": isShowWholeCompany,
                "isLocal": isLocal
            };

            if (shiftStatisticContainer.length > 0) {
                $.get(_global.root + "Calendar/ShiftStatisticContent", params)
                        .done(function (content) {
                            shiftStatisticContainer.empty();
                            shiftStatisticContainer.html(content);
                        });
            }
        }
    };
} (jQuery));

$(document).ready(function () {
    ShiftStatistic.init();
});