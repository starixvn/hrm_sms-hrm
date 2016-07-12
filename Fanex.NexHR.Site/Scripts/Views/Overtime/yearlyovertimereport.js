'use strict';
/*global _global, _messages*/

(function () {
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

    function query() {
        $.ajax({
            url: _global.root + 'overtime/YearlyOvertimeReportDetails',
            type: 'POST',
            dataType: 'html',
            data: $('form.form-filter').serialize(),
            success: function (response) {
                $('#yearlyReportDetails').html(response);
                floatThead('table#data');
                requestToCalculateOTHours();
            },
            error: function () {
                alert(_messages.errorTryAgain);
            }
        });
    }

    $(document).ready(function () {
        query();

        $('form.form-filter #section').multiselect();
        $('form.form-filter #section').on('change', function () {
            var sectionIds = $(this).val() !== null ? $(this).val().join(',') : '';
            $('#SectionIds').val(sectionIds);
        });

        $("#btnSubmit").click(function () {
            query();
        });

        $("#btnExcel").click(function () {
            var url = _global.root + "export/YearlyOvertimeReport?" + $('form.form-filter').serialize();
            location.href = url;
        });
    });
})();