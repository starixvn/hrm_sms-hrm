'use strict';
/*global _global, actingButton, jsonToFullDateTime2, noInformation, reloadButtonText, _messages, dontBlock:true*/
/*exported index*/

//var nickName;
var rowIndex = 1;

function getMessageDetail(requestType, fromDate, toDate, fromTime, toTime,
    createdDate, fromShift, toShift, totalDays, totalMins, overtimeType, isOvertimeHours) {
    var result = '';
    dontBlock = true;
    $.ajax({
        url: _global.root + "ajax/GetMessageDetailRequest",
        async: false,
        type: 'post',
        data: { "requesttype": requestType, "fromdate": fromDate, "todate": toDate, "requestedDate": fromDate,
            "fromtime": fromTime, "totime": toTime,
            "createddate": createdDate, "fromshiftCode": fromShift,
            "toshiftcode": toShift, "totaldays": totalDays, "totalmins": totalMins,
            "overtimetype": overtimeType, "isOvertimehours": isOvertimeHours
        },
        success: function (data) {
            result = data;
        }
    });
    dontBlock = false;
    return result;
}

function view() {
    var $form = $('form');
    if (!$form.valid()) {
        return;
    }
    actingButton('#view', true);

    //if no user is selected, clear the empId also
    if ($('#Employee').val() === '') {
        $('#EmployeeId').val('');
    }

    $('#nodata').remove();

    var groups;
    var isMember = $("#IsMember").val();

    if (isMember === "True") {
        groups = $("#EmployeeGroupId").val();
    }
    else {
        groups = $("#GroupId").val() != null ? $("#GroupId").val().join() : "";
    }
    var isLocal = $("#category").val();

    $.getJSON("/api/report/getemployeeattendances",
    {
        "SupervisorId": $("#SupervisorId").val(),
        "FromDate": $("#FromDate").val(),
        "ToDate": $("#ToDate").val(),
        "EmpId": $("#EmployeeId").val(),
        "Employee": $("#Employee").val(),
        "GroupIds": groups,
        "AttendanceType": $("#AttendanceType").val(),
        "DateOption": $("#Date").val(),
        "ExcludeDeletedUsers": $('#ExcludeDeletedUsers').attr('checked') ? true : false,
        "IsLocal": isLocal
    },
    function (response) {
        if (response.length > 0) {
            rowIndex = 1;
            $('#data tbody').children().remove();
            $('#itemTemplate').tmpl(response).appendTo('#data');

            $('.tooltip-request').tooltip({
                track: true,
                delay: 0,
                showURL: false,
                bodyHandler: function () {
                    var object = $(this);
                    var result = getMessageDetail(
                    object.attr('data-requesttype'),
                    object.attr('data-fromdate'),
                    object.attr('data-todate'),
                    object.attr('data-fromtime'),
                    object.attr('data-totime'),
                    object.attr('data-createddate'),
                    object.attr('data-fromshift'),
                    object.attr('data-toshift'),
                    object.attr('data-totaldays'),
                    object.attr('data-totalmins'),
                    object.attr('data-ottype'),
                    object.attr('data-isothours'));
                    return "<div class='tooltip'>" + "<table class='table-tooltip checkinout'>" +
                    "<tr class='hasborder'>" +
                    "<td class='col1'>Details</td>" +
                    "<td class='col2'>" + result +
                    "</td></tr><tr class='hasborder'><td class='col1'>Created Date</td><td class='col2'>" +
                    object.attr('data-createddate') +
                    "</td></tr><tr><td class='col1'>Remark</td><td class='col2'>" +
                    object.attr('data-remark') + "</td></tr></table></div>";
                }
            });

            $('.tooltip-overtime').tooltip({
                track: true,
                delay: 0,
                showURL: false,
                bodyHandler: function () {
                    var object = $(this);

                    return "<div class='tooltip '>" +
                    "<div class='overtime-tooltip'>" +
                    object.attr('data-fromtime') +
                    "-" + object.attr('data-totime') + "</div></div>";
                }
            });
        }
        else {
            $('#data tbody').children().remove();
            $(noInformation(15)).appendTo('#data');
        }
        reloadButtonText('#view');
    }).error(function () {
        reloadButtonText('#view');
        alert(_messages.errorTryAgain);
    });
}

$(document).ready(function () {
    $("#GroupId").multiselect();
//    $("#Employee").autocomplete({
//        source: function (request, response) {
//            $.ajax({
//                dataType: 'json',
//                url: _global.root + 'ajax/getmembersbymanager',
//                data: {
//                    query: request.term,
//                    $top: 10
//                },
//                success: function (data) {
//                    response($.map(data, function (item) {
//                        return {
//                            label: item.NickName + ' (' + item.FullName + ')',
//                            value: item.Id
//                        };
//                    }));
//                }
//            });
//        },
//        minLength: 1,
//        select: function (event, ui) {
//            if (ui.item != null) {
//                nickName = ui.item.label;
//                $('#EmpId').val(ui.item.value);
//            }
//        },
//        focus: function (event, ui) {
//            if (ui.item != null) {
//                nickName = ui.item.label;
//                $('#Employee').val(nickName);
//                $('#EmpId').val(ui.item.value);
//            }
//            event.preventDefault();
//        },
//        change: function (event, ui) {
//            if (ui.item != null) {
//                nickName = ui.item.label;
//                $('#Employee').val(nickName);
//                $('#EmpId').val(ui.item.value);
//            }
//        },
//        close: function () {
//            $('#Employee').val(nickName);
//        }
//    });

    $('input[type=text]').keypress(function (evt) {
        if (evt.keyCode === 13) {
            view();
        }
    });
    $('#view').click(function (evt) {
        evt.preventDefault();
        view();
    });
    $("#btnExcel").click(function () {
        var groups;
        var isMember = $("#IsMember").val();
        if ($('#Employee').val() === '') {
            $('#EmployeeId').val('');
        }
        if (isMember === "True") {
            groups = $("#EmployeeGroupId").val();
        }
        else {
            groups = $("#GroupId").val() != null ? $("#GroupId").val().join() : "";
        }
        var excludeUser = $('#ExcludeDeletedUsers').attr('checked') ? true : false;
        var isLocal = $("#category").val();
        var url = _global.root + "export/Attendance?" +
            "SupervisorId=" + $("#SupervisorId").val() +
            "&FromDate=" + $("#FromDate").val() +
            "&ToDate=" + $("#ToDate").val() +
            "&EmpId=" + $("#EmployeeId").val() +
            "&Employee=" + $("#Employee").val() +
            "&GroupIds=" + groups +
            "&AttendanceType=" + $("#AttendanceType").val() +
            "&DateOption=" + $("#Date").val() +
            "&ExcludeDeletedUsers=" + excludeUser +
            "&IsLocal=" + isLocal +
            "&IsMember=" + isMember;
        location.href = url;
    });
    view();
});



function index() {
    return rowIndex++;
}

