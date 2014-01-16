$(document).ready(function () {
    var nickName;
    var requestType = $("#RequestType").val();
    $("#Employee").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: _global.root + 'request/GetEmployeesByActingUser',
                data: {
                    role: 5,
                    requestType: requestType,
                    query: request.term
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.NickName + ' (' + item.FullName + ')',
                            value: item.OwnerId
                        }
                    }));
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmployeeId').val(ui.item.value);
            eventAfterChangeEmployee();
        },
        focus: function (event, ui) {
            nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmployeeId').val(ui.item.value);
            event.preventDefault();
        },
        close: function () {
            $('#Employee').val(nickName);
        }
    });
});

function eventAfterChangeEmployee() {
    $.getJSON(_global.root + 'api/employee/GetVerifiers', { employeeId: $("#EmployeeId").val(), requestType: $("#RequestType").val() },
    function (response) {
        if (response != null) {
            var options = "";
            for (var i = 0; i < response.length; i++) {
                options += "<option value='" + response[i].OwnerId + "'>" + response[i].FullName + "</option>"
            }

            $("#VerifierId").empty();
            $("#VerifierId").html(options);
        }
    });

    $.getJSON(_global.root + 'api/employee/GetApprovers', { employeeId: $("#EmployeeId").val(), requestType: $("#RequestType").val() },
    function (response) {
        if (response != null) {
            var options = "";
            for (var i = 0; i < response.length; i++) {
                options += "<option value='" + response[i].OwnerId + "'>" + response[i].FullName + "</option>"
            }
            $("#ApproverId").empty();
            $("#ApproverId").html(options);
        }
    });
}