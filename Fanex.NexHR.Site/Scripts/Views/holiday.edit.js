"use strict";

var editHoliday = (function ($) {
    var me = {};
    var $numDay, $fromDate, $toDate;

    function setToDate() {
        var inputDay = parseInt($numDay.val(), 10) - 1;
        var fromDate = $fromDate.val();
        var toDate = new Date(Date.parse(fromDate));

        toDate.setDate(toDate.getDate() + inputDay);
        $toDate.val(toDate.format("dd mmm, yyyy"));
    }

    var initEvent = function () {
        $numDay.change(function () {
            setToDate();
        });

        $numDay.click(function () {
            setToDate();
        });
        
        $("#FromDate, #ToDate").change(function () {
            var fromDate = $fromDate.datepicker('getDate');
            var toDate = $toDate.datepicker('getDate');
            var dayDiff = (toDate - fromDate) / 1000 / 60 / 60 / 24;

            $numDay.val(dayDiff);
        });
    };

    me.init = function () {
        $numDay = $("#numDay");
        $fromDate = $("#FromDate");
        $toDate = $("#ToDate");

        $("#FromDate, #ToDate").datepicker({
            changeMonth: true,
            changeYear: true
        });

        initEvent();
    };

    return me;
}(jQuery));

$(document).ready(function () {
    editHoliday.init();
});