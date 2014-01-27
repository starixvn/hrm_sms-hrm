$(document).ready(function () {


    $('#group').multiselect();
    $("#title").multiselect({
        noneSelectedText: 'Select positions',
        selectedText: '# selected',
        selectedList: 0
    });

    GetOTReportDetails();

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
                        }
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

    $("#submitOTDetails").click(function () {
        GetOTReportDetails();
    });

    $("#excel").click(function () {
        var employee = $('#Employee').val();
        var empId = $("#EmpId").val();
        var groupIds = $("#group").val() != null ? $("#group").val().join() : "";
        var jobTitle = $("#title").val() != null ? $("#title").val().join() : "";
        var fromDate = $("#fromDate").val();
        var toDate = $("#toDate").val();
        if (employee == null || employee == "") {
            empId = "";
        }

        var url = _global.root + "export/OvertimeReport?fromDate=" + fromDate + "&toDate=" + toDate
                               + "&empId=" + empId + "&jobTitle=" + jobTitle + "&groups=" + groupIds;
        location.href = url;

    });

});

function GetOTReportDetails() {
    var employee = $('#Employee').val();
    var empId = $("#EmpId").val();
    var groupIds = $("#group").val() != null ? $("#group").val().join() : "";
    var jobTitle = $("#title").val() != null ? $("#title").val().join() : "";
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    var isLocal = $("#category").val();
    if (isLocal == "0") {
        isLocal = 0;
    }
    if (isLocal == "1") {
        isLocal = 1;
    }

    if (employee == null || employee == "") {
        empId = "";
    }

    var reportDetails = $("#OTExcelReportDetails");
    var params = {
        "fromDate": fromDate,
        "toDate": toDate,
        "empId": empId,
        "jobTitle": jobTitle,
        "groups": groupIds,
        "isLocal": isLocal
    };

    $.ajax({
        url: _global.root + "overtime/OTExcelReportDetails",
        data: params,
        async: false
    }).done(function (response) {
        reportDetails.empty();
        reportDetails.html(response);
    });
}