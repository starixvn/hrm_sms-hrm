'use strict';
/*global _global, _messages, floatThead*/

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
        $("#sumtotal").text(sumTotal);

        //Jan
        var sumJan = calculateTotalOTHours($(".sumjan"));
        $("#sumjan").text(sumJan);

        //Feb
        var sumFeb = calculateTotalOTHours($(".sumfeb"));
        $("#sumfeb").text(sumFeb);

        //Mar
        var sumMar = calculateTotalOTHours($(".summar"));
        $("#summar").text(sumMar);

        //Apr
        var sumApr = calculateTotalOTHours($(".sumapr"));
        $("#sumapr").text(sumApr);

        //May
        var sumMay = calculateTotalOTHours($(".summay"));
        $("#summay").text(sumMay);

        //Jun
        var sumJun = calculateTotalOTHours($(".sumjun"));
        $("#sumjun").text(sumJun);

        //Jul
        var sumJul = calculateTotalOTHours($(".sumjul"));
        $("#sumjul").text(sumJul);

        //Jul
        var sumAug = calculateTotalOTHours($(".sumaug"));
        $("#sumaug").text(sumAug);

        //Sep
        var sumSep = calculateTotalOTHours($(".sumsep"));
        $("#sumsep").text(sumSep);

        //Oct
        var sumOct = calculateTotalOTHours($(".sumoct"));
        $("#sumoct").text(sumOct);

        //Nov
        var sumNov = calculateTotalOTHours($(".sumnov"));
        $("#sumnov").text(sumNov);

        //Dec
        var sumDec = calculateTotalOTHours($(".sumdec"));
        $("#sumdec").text(sumDec);
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