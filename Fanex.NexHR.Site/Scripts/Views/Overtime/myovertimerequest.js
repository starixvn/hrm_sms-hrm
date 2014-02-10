$(document).ready(function () {
    $("#errordialog").hide();

    //Save Button.
    $("#save, #savefooter").live("click", function () {
        var checkValidate = Validation();
        if (checkValidate == 0) {
            var OvertimeInfo = {
                VerifierId: $("#VerifierId").val(),
                ApproverId: $("#ApproverId").val(),
                EmpId: $("#EmpId").val()
            };
            var OvertimeDetails = [];
            $("#data tbody tr").each(function () {
                OvertimeDetails.push({
                    OvertimeDate: $('td:first-child', this).html().split(" (")[0].trim(),
                    Schedule: $('#request_ShiftId', this).val(),
                    TimeIn: $('td:eq(2)', this).html().trim(),
                    TimeOut: $('td:eq(3)', this).html().trim(),
                    TotalWorkHours: $('td:eq(5)', this).html().trim() != "" ? $('td:eq(5)', this).html().trim() : 0,
                    NormalHours: $('td #request_NormalHours', this).val(),
                    OffHours: $('td #request_OffHours', this).val(),
                    PublicHours: $('td #request_PublicHours', this).val(),
                    Remark: $('td #request_Remark', this).val()
                });
            });

            var OvertimeInfoJson = JSON.stringify(OvertimeInfo);
            var OvertimeDetailsJson = JSON.stringify(OvertimeDetails);

            $.ajax({
                type: 'POST',
                url: '/Overtime/Update/',
                data: { overtimeJson: OvertimeInfoJson, overtimeDetailsJson: OvertimeDetailsJson },
                dataType: 'json',
                success: function (response) {
                    if (response.success) {
                        $('form').removeClass("dirty");
                        alert("Data Saved!");
                    } else {
                        $("#errordialog").html(response.errors).dialog({
                            modal: true,
                            buttons: {
                                "OK": function () {
                                    $(this).dialog("close");
                                }
                            }
                        });
                    }
                }
            });
        }
        else {
            if (checkValidate == 1) {
                alert("Please select a Verifier");
            }
            if (checkValidate == 2) {
                alert("Please select an Approver");
            }
            if (checkValidate == 3) {
                alert("Please select Verifier and Approver");
            }
        }
    });

    //Back Button.
    $("#back, #backfooter, #backHrReport").click(function () {
        parent.history.back();
        return false;
    });

    //autocomplete
    $("#Employee").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: _global.root + 'overtime/GetActingEmployees',
                data: {
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
            $('#EmpId').val(ui.item.value);
        },
        focus: function (event, ui) {
            nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item.value);
            event.preventDefault();
        },
        change: function (event, ui) {
            nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item.value);
        },
        close: function () {
            $('#Employee').val(nickName);
        }
    });

    //Set range for numeric textbox.
    var textbox = $("input[type='text'].inputDay");
    textbox.keyup(function () {
        var row = $(this).parent().parent();
        var TotalHours = parseFloat($('td:eq(5)', row).html().trim());
        var NormalHours = parseFloat($('td #request_NormalHours', row).val());
        var OffHours = parseFloat($('td #request_OffHours', row).val());
        var PublicHours = parseFloat($('td #request_PublicHours', row).val());
        //var TotalShiftHours = parseFloat($("#request_TotalShiftHours", row).val());
        var TotalShiftHours = parseFloat($("#WorkingHours").val());

            var object = $(this).first();
            var focusedTexbox = $(this).attr("id");
            if (focusedTexbox == "request_NormalHours"
            || focusedTexbox == "request_OffHours"
            || focusedTexbox == "request_PublicHours") {
                if (focusedTexbox == "request_NormalHours" && TotalHours >= TotalShiftHours) {
                    var normalLitmit = (TotalHours - TotalShiftHours) + 0.5;
                    if (normalLitmit < 0) {
                        normalLitmit = 0;
                    }
                    else {
                        normalLitmit = parseFloat(normalLitmit.toFixed(1));
                    }

                    RangeTextBox(object, 0, normalLitmit);
                }
                else {
                    var offOrHolidayLimit = TotalHours + 0.5;
                    offOrHolidayLimit = parseFloat(offOrHolidayLimit.toFixed(1));
                    RangeTextBox(object, 0, offOrHolidayLimit);
                }
            }
    });
    requestToCalculateOTHours();
    $('form').areYouSure();
});

function RangeTextBox(textbox, min, max) {
    var textboxValue = textbox.val();
    console.log("aaa");
    console.log(min);
    console.log(max);
    var check = textboxValue % 0.5 == 0;
    if (check) {
        var isNotInRange = (textboxValue < min) || (textboxValue > parseFloat(max));
        if (isNotInRange) {
            textbox.val(0);
        }
    }
    else {
        textbox.val(0);
    }

    textbox.keyup(function (event) {
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });

    textbox.blur(function () {
        if (textbox.val() == "") {
            textbox.val(0);
        }
    });
    return false;
}

function calculateTotalOTHours(selector) {
    var sumResult = 0;
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

function requestToCalculateOTHours() {
    var sumTotal = calculateTotalOTHours($(".sumtotal"));
    $("#sumtotal").text(sumTotal.toFixed(2));

    var sumNormal = calculateTotalOTHours($(".sumnormal"));
    $("#sumnormal").text(sumNormal.toFixed(2));

    var sumOff = calculateTotalOTHours($(".sumoff"));
    $("#sumoff").text(sumOff.toFixed(2));

    var sumPublic = calculateTotalOTHours($(".sumpublic"));
    $("#sumpublic").text(sumPublic.toFixed(2));
}

function Validation() {
    var verifier = $("#VerifierId").val();
    var approver = $("#ApproverId").val();

    if (verifier == "" && approver == "") {
        return 3;
    }

    if (verifier == null || verifier == "") {
        return 1;
    }

    if (approver == null || approver == "") {
        return 2;
    }

    return 0;
}
