'use strict';
/*global _global*/

$(document).ready(function () {
    $("#SectionId").multiselect();

    $("form").submit(function (e) {
        e.preventDefault();
        $("#SectionId").val() != null ? $("#SectionId").val().join() : "";
        var postToUrl = "AnnualLeaveBalance/index?year=";
        var url = submitData(postToUrl);
        location.href = url;
    });

    $("#btnExcel").click(function () {
        var postToUrl = "export/leavesummary?year=";
        var url = submitData(postToUrl);
        location.href = url;
    });

    var submitData = function (postToUrl) {
        var sectionIds = $("#SectionId").val() != null ? $("#SectionId").val().join() : "";
        var url = _global.root + postToUrl + $("#Year").val() +
            "&SectionIds=" + sectionIds + "&Employee=" + $("#Employee").val() +
            "&EmployeeId=" + $("#EmployeeId").val() +
            "&MinAnnualBalance=" + $("#MinAnnualBalance").val() +
            "&Category=" + $("#Category").val();

        return url;
    };
});