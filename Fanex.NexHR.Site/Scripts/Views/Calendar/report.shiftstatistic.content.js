'use strict';
/*global ShiftStatistic*/

var ShiftStatisticContent = (function ($) {
    return {
        groupId: null,
        isShowWholeCompany: null,
        previous: null,
        next: null,
        previousFromDate: null,
        previousToDate: null,
        nextFromDate: null,
        nextToDate: null,

        init: function () {
            ShiftStatisticContent.previous = $("#lnkPrevious");
            ShiftStatisticContent.next = $("#lnkNext");
            ShiftStatisticContent.previousFromDate = $("#PreviousFromDate");
            ShiftStatisticContent.previousToDate = $("#PreviousToDate");
            ShiftStatisticContent.nextFromDate = $("#NextFromDate");
            ShiftStatisticContent.nextToDate = $("#NextToDate");
            ShiftStatisticContent.groupId = $("#GroupId");
            ShiftStatisticContent.isShowWholeCompany = $("#chkIsShowWholeCompany");

            ShiftStatisticContent.previous.on("click", function () {
                var groups = ShiftStatisticContent.groupId.val();
                var groupIds = groups != null ? groups.join() : "";

                ShiftStatistic.fromDate.val(ShiftStatisticContent.previousFromDate.val());
                ShiftStatistic.toDate.val(ShiftStatisticContent.previousToDate.val());

                ShiftStatistic.bindShiftStatisticContent(ShiftStatisticContent.previousFromDate.val(),
                        ShiftStatisticContent.previousToDate.val(), groupIds, (ShiftStatisticContent.isShowWholeCompany.attr("checked") === "checked"));
            });

            ShiftStatisticContent.next.on("click", function () {
                var groups = ShiftStatisticContent.groupId.val();
                var groupIds = groups != null ? groups.join() : "";

                ShiftStatistic.fromDate.val(ShiftStatisticContent.nextFromDate.val());
                ShiftStatistic.toDate.val(ShiftStatisticContent.nextToDate.val());

                ShiftStatistic.bindShiftStatisticContent(ShiftStatisticContent.nextFromDate.val(),
                    ShiftStatisticContent.nextToDate.val(), groupIds, (ShiftStatisticContent.isShowWholeCompany.attr("checked") === "checked"));
            });
        }
    };
} (jQuery));

$(document).ready(function () {
    ShiftStatisticContent.init();
});