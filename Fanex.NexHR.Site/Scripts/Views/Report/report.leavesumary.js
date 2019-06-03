'use strict';
/*global _global*/
var leaveSummary = (function ($) {
    var dom = {
        Form: "form",
        ExcelBtn: "#btnExcel",
        Table: "#ListSummary"
    };

    function query() {
        var sectionIds = $('#SectionId').val() !== null ? $('#SectionId').val().join() : '';
        var url = _global.root + 'report/GetUnpaidLeaveSummary?year=' + $('#Year').val() +
            '&SectionIds=' + sectionIds + '&Employee=' + $('#Employee').val() +
            '&EmployeeId=' + $('#EmployeeId').val() + '&category=' + $('#Category').val() + '&leaveType=' + $('#LeaveType').val();
        $.ajax({
            url: url,
            success: function (data) {
                $(dom.Table).html(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    }

    function initSubmitForm() {
        $(dom.Form).on("submit", function (e) {
            e.preventDefault();
            query();
        });
    }

    function initExportExcel() {
        $(dom.ExcelBtn).click(function () {
            var sectionIds = $('#SectionId').val() !== null ? $('#SectionId').val().join() : '';
            var url = _global.root + 'ExportLeaveSummary/UnpaidLeaveSummary?year=' + $('#Year').val() +
                '&SectionIds=' + sectionIds + '&Employee=' + $('#Employee').val() +
                '&EmployeeId=' + $('#EmployeeId').val() + '&category=' + $('#Category').val() + '&leaveType=' + $('#LeaveType').val();
            location.href = url;
        });
    }

    function initLeaveSummary() {
        $("#SectionId").multiselect();
        initSubmitForm();
        initExportExcel();
        query();
    }

    return {
        init: initLeaveSummary
    };
})(jQuery);

$(document).ready(function () {
    leaveSummary.init();
});