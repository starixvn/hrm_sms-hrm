var MemberOvertime = function ($) {
    return {
        dialogMail: null,
        maxDate: null,
        subjectMail: null,
        contentMail: null,
        submit: null,
        teamId: null,

        init: function () {
            MemberOvertime.dialogMail = $("#dialog-mail");
            MemberOvertime.maxDate = $("#MaxDate");
            MemberOvertime.subjectMail = $("#txtSubjectMail");
            MemberOvertime.contentMail = $("#txtContentMail");
            MemberOvertime.submit = $("#btnSubmit");
            MemberOvertime.teamId = $("#TeamId");

            MemberOvertime.initMonthPick();
            MemberOvertime.teamId.multiselect();

            MemberOvertime.submit.on("click", function () {
                MemberOvertime.submitEvent();
            });

            MemberOvertime.bindMemberOvertimeDetail();

            MemberOvertime.dialogMail.dialog({
                autoOpen: false,
                height: 300,
                width: 500,
                modal: true,
                buttons: {
                    "Send": function () {
                        MemberOvertime.sendMailToWholeClaimOT();
                        $(this).dialog("close");
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                },
                close: function () {
                    MemberOvertime.subjectMail.val("");
                    MemberOvertime.contentMail.val("");
                }
            });

            $("#lknSendMail").on("click", function () {
                MemberOvertime.dialogMail.dialog("open");
            });


        },
        sendMailToWholeClaimOT: function () {
            var subject = $.trim(MemberOvertime.subjectMail.val());
            var content = $.trim(MemberOvertime.contentMail.val());

            $.ajax({
                type: 'POST',
                dataType: "json",
                url: _global.root + 'Overtime/ReminderClaimOvertime',
                data: { subject: subject, content: content },
                success: function (response) {
                    alert("Sent success!");
                }
            });
        },
        initMonthPick: function (today, month) {
            //Month Picker
            var dateRangePicker = $('#DateControl');

            dateRangePicker.monPickr({
                showDateRange: false,
                sortDesc: true,
                today: today,
                month: month,
                minDate: '2013/1/1',
                maxDate: MemberOvertime.maxDate.val(),
                afterChangeMonth: function () {
                    var Month = dateRangePicker.monPickr("getMonth", "MM");
                    var Year = dateRangePicker.monPickr("getMonth", "yyyy");
                    $('#Month').val(Month);
                    $('#Year').val(Year);
                }
            });

            $('.row').hover(
                function () {
                    $(this).addClass('rowhover');
                }, function () {
                    $(this).removeClass('rowhover');
                });
        },
        submitEvent: function () {
            var teams = MemberOvertime.teamId.val();
            var teamIds = teams != null ? teams.join() : "";

            var month = $("#Month").val();
            var year = $("#Year").val();
            var monthYear = year + "-" + month + "-01";
            var statusId = $("#statusId").val() == "0" ? "" : $("#statusId").val();
            var employeeId = $("#EmployeeId").val();

            MemberOvertime.bindMemberOvertimeDetail(monthYear, employeeId, teamIds, statusId);
        },
        bindMemberOvertimeDetail: function (month, employeeId, teamIds, statusId) {
            var isHRView = $("#IsHRView").val();
            var action = $("#IsHRView").val() == "True" ? "MemberOvertimeViewByHR" : "MemberOvertimeViewByLeader";

            var memberOvertimeContainer = $("#memberOvertimeDetail");
            var params = {
                "month": month,
                "employeeId": ((employeeId == 0) ? null : employeeId),
                "teamIds": teamIds,
                "statusId": statusId
            };

            if (memberOvertimeContainer.length > 0) {
                $.get(_global.root + "Overtime/" + action, params)
                        .done(function (content) {
                                memberOvertimeContainer.empty();
                                memberOvertimeContainer.html(content);
                                requestToCalculateOTHours();
                            });
            }
        }
    };
} (jQuery);

$(document).ready(function () {
    MemberOvertime.init();
    requestToCalculateOTHours();
});

function requestToCalculateOTHours() {
    var sumNormal = calculateTotalOTHours($(".sumnormal"));
    $("#sumnormal").text(sumNormal.toFixed(2));

    var sumOff = calculateTotalOTHours($(".sumoff"));
    $("#sumoff").text(sumOff.toFixed(2));

    var sumPublic = calculateTotalOTHours($(".sumpublic"));
    $("#sumpublic").text(sumPublic.toFixed(2));
}

function calculateTotalOTHours(selector) {
    var sumResult = 0;
    var formUpdate = $("#formUpdateOT").val();
    selector.each(function () {
        var value = $(this).text().trim();
        if (value != "") {
            if (!isNaN(value) && value.lenght != 0) {
                sumResult += parseFloat(value);
            }
        }
    });
    return sumResult;
}