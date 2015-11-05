'use strict';
/*global _global*/

$(document).ready(function () {
    $("#btnExcel").click(function () {
        var year = $("#Year").val();
        var url = _global.root + "export/Manpower?year=" + year;
        location.href = url;
    });
});

