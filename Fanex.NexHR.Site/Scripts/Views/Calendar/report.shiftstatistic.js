var ShiftStatistic = (function ($) {
    return {
        fromDate: null,
        toDate: null,
        isShowWholeCompany: null,
        isLocal: null,
        submit: null,

        init: function () {
            ShiftStatistic.fromDate = $('#FromDate');
            ShiftStatistic.toDate = $('#ToDate');
            ShiftStatistic.submit = $('#btnSubmit');
            ShiftStatistic.isShowWholeCompany = $('#IsShowWholeCompany');
            ShiftStatistic.isLocal = $('#IsLocal');

            $.ajax({
                url: _global.root + 'Department/GetHierarchy',
                success: function (data) {
                    $('#Section').multiSelectTreeView({
                        data: data,
                        isSelectedAll: true
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(textStatus + ': ' + errorThrown);
                }
            });

            ShiftStatistic.bindShiftStatisticContent(
                ShiftStatistic.fromDate.val(),
                ShiftStatistic.toDate.val(), '',
                ShiftStatistic.isShowWholeCompany.prop('checked'),
                ShiftStatistic.isLocal.val());

            ShiftStatistic.submit.on('click', function () {
                ShiftStatistic.bindShiftStatisticContent(
                    ShiftStatistic.fromDate.val(),
                    ShiftStatistic.toDate.val(),
                    $('#Section').multiSelectTreeView('getSelectedItems'),
                    ShiftStatistic.isShowWholeCompany.prop('checked'),
                    ShiftStatistic.isLocal.val());
            });
        },
        bindShiftStatisticContent: function (fromDate, toDate, groupIds, isShowWholeCompany, isLocal) {
            var shiftStatisticContainer = $('#shiftStatistic');
            var params = {
                'fromDate': fromDate,
                'toDate': toDate,
                'groupIds': groupIds,
                'isShowWholeCompany': isShowWholeCompany,
                'isLocal': isLocal
            };

            if (shiftStatisticContainer.length > 0) {
                $.get(_global.root + 'Calendar/ShiftStatisticContent', params)
                    .done(function (content) {
                        shiftStatisticContainer.empty();
                        shiftStatisticContainer.html(content);
                    });
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    ShiftStatistic.init();
});