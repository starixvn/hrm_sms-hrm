'use strict';
/*global _global*/

var nickName = '';
function calculateTotalOTHours(selector) {
    var sumResult = 0;
    selector.each(function () {
        var value = $(this).text().trim();
        if (value !== "") {
            if (!isNaN(value) && value.lenght !== 0) {
                sumResult += parseFloat(value);
            }
        }
    });
    return sumResult;
}

function requestToCalculateOTHours() {
    //Total
    var sumTotal = calculateTotalOTHours($(".total"));
    $("#sumtotal").text(sumTotal.toFixed(2));

    //Jan
    var sumJan = calculateTotalOTHours($(".sumjan"));
    $("#sumjan").text(sumJan.toFixed(2));

    //Feb
    var sumFeb = calculateTotalOTHours($(".sumfeb"));
    $("#sumfeb").text(sumFeb.toFixed(2));

    //Mar
    var sumMar = calculateTotalOTHours($(".summar"));
    $("#summar").text(sumMar.toFixed(2));

    //Apr
    var sumApr = calculateTotalOTHours($(".sumapr"));
    $("#sumapr").text(sumApr.toFixed(2));

    //May
    var sumMay = calculateTotalOTHours($(".summay"));
    $("#summay").text(sumMay.toFixed(2));

    //Jun
    var sumJun = calculateTotalOTHours($(".sumjun"));
    $("#sumjun").text(sumJun.toFixed(2));

    //Jul
    var sumJul = calculateTotalOTHours($(".sumjul"));
    $("#sumjul").text(sumJul.toFixed(2));

    //Jul
    var sumAug = calculateTotalOTHours($(".sumaug"));
    $("#sumaug").text(sumAug.toFixed(2));

    //Sep
    var sumSep = calculateTotalOTHours($(".sumsep"));
    $("#sumsep").text(sumSep.toFixed(2));

    //Oct
    var sumOct = calculateTotalOTHours($(".sumoct"));
    $("#sumoct").text(sumOct.toFixed(2));

    //Nov
    var sumNov = calculateTotalOTHours($(".sumnov"));
    $("#sumnov").text(sumNov.toFixed(2));

    //Dec
    var sumDec = calculateTotalOTHours($(".sumdec"));
    $("#sumdec").text(sumDec.toFixed(2));
}

function getYearlyReportDetails() {
    var employee = $('#Employee').val();
    var empId = $("#EmpId").val();
    var team = $("#group").val() != null ? $("#group").val().join() : "";
    var year = $("#Year").val();
    var calendarType = $("#calendar").val();
    if (employee == null || employee === "") {
        empId = "";
    }
    var reportDetails = $("#yearlyReportDetails");
    var params = {
        "empId": empId,
        "team": team,
        "year": year,
        "calendar": calendarType
    };

    $.ajax({
        url: _global.root + "overtime/GetYearlyOvertimeReport",
        data: params,
        async: false
    }).done(function (response) {
        reportDetails.empty();
        reportDetails.html(response);
    });
    requestToCalculateOTHours();
}

$(document).ready(function () {
    $("#OTSummary").toggleClass("active");
    //getYearlyReportDetails
    getYearlyReportDetails();

    $('#group').multiselect();
    //submit form
    $("#submit").click(function () {
        getYearlyReportDetails();
    });

    $("#excel").click(function () {
        var employee = $('#Employee').val();
        var empId = $("#EmpId").val();
        var team = $("#group").val() != null ? $("#group").val().join() : "";
        var year = $("#Year").val();
        var calendarType = $("#calendar").val();
        if (employee == null || employee === "") {
            empId = "";
        }

        var url = _global.root + "export/YearlyOvertimeReport?year=" + year + "&team=" + team +
"&empId=" + empId + "&calendar=" + calendarType;
        location.href = url;
    });

    //autocomplete
    $("#Employee").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: _global.root + 'api/employee/getemployees',
                data: {
                    query: request.term
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.NickName + ' (' + item.FullName + ')',
                            value: item.Id
                        };
                    }));
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item.value);
        },
        focus: function (event, ui) {
            nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item != null ? ui.item.value : "");
            event.preventDefault();
        },
        change: function (event, ui) {
            nickName = ui.item != null ? ui.item.label : "";
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item != null ? ui.item.value : "");
        },
        close: function () {
            $('#Employee').val(nickName);
        }
    });
});