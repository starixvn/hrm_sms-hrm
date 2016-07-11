'use strict';
/*global _global*/

$(document).ready(function () {
    $('#SectionId').multiselect();

    $('form').submit(function (e) {
        e.preventDefault();
        var sectionIds = $('#SectionId').val() != null ? $('#SectionId').val().join() : '';
        var url = _global.root + 'report/UnpaidLeaveSummary?year=' + $('#Year').val() +
            '&SectionIds=' + sectionIds + '&Employee=' + $('#Employee').val() +
            '&EmployeeId=' + $('#EmployeeId').val() + '&category=' + $('#Category').val();
        location.href = url;
    });

    $('#btnExcel').click(function () {
        var sectionIds = $('#SectionId').val() != null ? $('#SectionId').val().join() : '';
        var url = _global.root + 'export/UnpaidLeaveSummary?year=' + $('#Year').val() +
            '&SectionIds=' + sectionIds + '&Employee=' + $('#Employee').val() +
            '&EmployeeId=' + $('#EmployeeId').val() + '&category=' + $('#Category').val();
        location.href = url;
    });
});