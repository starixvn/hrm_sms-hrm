$(document).ready(function () {
    $.ajax({
        url: _global.root + 'Department/GetHierarchy',
        success: function (data) {
            $('#Section').multiSelectTreeView({
                data: data,
                selectedItems: $('#SectionIds').val()
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        }
    });

    $('form').submit(function (e) {
        e.preventDefault();
        var postToUrl = 'AnnualLeaveBalance/index?year=';
        var url = submitData(postToUrl);
        location.href = url;
    });

    $('#btnExcel').click(function () {
        var postToUrl = 'ExportLeaveSummary/AnnualLeaveSummary?year=';
        var url = submitData(postToUrl);
        location.href = url;
    });

    var submitData = function (postToUrl) {
        var sectionIds = $('#Section').multiSelectTreeView('getSelectedItems');
        var url = _global.root + postToUrl + $('#Year').val() +
            '&SectionIds=' + sectionIds + '&Employee=' + $('#Employee').val() +
            '&EmployeeId=' + $('#EmployeeId').val() +
            '&MinAnnualBalance=' + $('#MinAnnualBalance').val() +
            '&Category=' + $('#Category').val();

        return url;
    };
});