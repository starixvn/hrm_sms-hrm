'use strict';
/*global _global, RequestCommon*/

$(document).ready(function () {

    //$('form').areYouSure();
    var _checkTopManager = false;
    $(".datepicker").attr('readonly', 'readonly');
    //$(".dropdown").chosen();
    $("#errors-model-state").hide();
    $("#shift-unavailable").hide();

    $('#shiftinfo').tooltip({
        track: true,
        delay: 0,
        showURL: false,
        bodyHandler: function () {
            return $('#shifts').html();
        }
    });

    $("#Employee").blur(function () {
        $.getJSON(_global.root + 'api/employee/GetUnavailableDates', { employeeId: $('#EmployeeId').val(), date: new Date().toDateString() },
                function (response) {
                    if (response != null) {
                        $(".datepicker").attr("disabledDate", response.join(","));
                    }
                }
            );
    });

    function getTable() {
        var shiftContainer = $("#shiftDetailSchedule");
        var fromDate = $("[name='FromDate']").val();
        var toDate = $("[name='ToDate']").val();
        var params = {
            "employeeId": $("#EmployeeId").val(),
            "fromDate": fromDate,
            "toDate": toDate
        };

        $.ajax({
            url: _global.root + "changeshift/ChangeShiftDetail",
            data: params,
            async: false
        }).done(function (response) {
            shiftContainer.empty();
            shiftContainer.html(response);
            if ($("#tblchangeshift tbody tr").length > 0) {
                $('input[name="submit"]').show();
                $("#shift-unavailable").hide();
            }
            else {
                $('input[name="submit"]').hide();
                $("#shift-unavailable").show();
            }
        });
    }


    $('.datepicker').change(function () {
        getTable();
    });


    $("input[name='submit']").click(function () {
        RequestCommon.removeErrorMsg(false);
        checkIsTopManager();
        var shift = {
            EmpId: $("#EmployeeId").val(),
            FromDate: $("#FromDate").val(),
            ToDate: $("#ToDate").val(),
            Remark: $("#Remark").val(),
            VerifierId: $("#VerifierId").val(),
            ApproverId: $("#ApproverId").val(),
            RequestType: $("#RequestType").val()
        };

        var shiftDetails = [];
        $("#tblchangeshift tbody tr").each(function () {
            shiftDetails.push({
                OnDate: $('td:first-child', this).html(),
                FromShiftId: $('#item_FromShiftId', this).val(),
                ToShiftId: $('td select', this).val(),
                Remark: $('td #item_Remark', this).val()
            });
        });
        var shiftJson = JSON.stringify(shift);
        var shiftDetailsJson = JSON.stringify(shiftDetails);
        var year = $("#year").val();

        if (new ValidateForm()) {
            $.ajax({
                type: 'POST',
                url: '/ChangeShift/Create/',
                data: { shiftJson: shiftJson, shiftDetailJson: shiftDetailsJson },
                dataType: 'json',
                success: function (response) {
                    if (response.Code === 0) {
                        $('form').removeClass("dirty");
                        var successMsg = "Your request have been sent successfully!";
                        RequestCommon.showSuccessMsg(successMsg, "ChangeShift/EmployeeView");
                    } else {
                        RequestCommon.showErrorMsg(response.message, $("#Employee"));
                    }
                }
            });
        }
        return false;
    });

    //Submit Edit
    $("input[name='submitEdit']").click(function () {
        RequestCommon.removeErrorMsg(false);
        var requestId = $("#RequestId").val();
        var shift = {
            RequestId: requestId,
            EmpId: $("#EmployeeId").val(),
            FromDate: $("#FromDate").val(),
            ToDate: $("#ToDate").val(),
            Remark: $("#Remark").val(),
            VerifierId: $("#VerifierId").val(),
            ApproverId: $("#ApproverId").val(),
            RequestType: $("#RequestType").val()
        };

        var shiftDetails = [];
        $("#tblchangeshift tbody tr").each(function () {
            shiftDetails.push({
                OnDate: $('td:first-child', this).html(),
                FromShiftId: $('#item_FromShiftId', this).val(),
                ToShiftId: $('td select', this).val(),
                Remark: $('td #item_Remark', this).val()
            });
        });

        var validate = new ValidateForm();
        var shiftJson = JSON.stringify(shift);
        var shiftDetailsJson = JSON.stringify(shiftDetails);

        if (validate) {
            $.ajax({
                type: 'POST',
                url: '/ChangeShift/Update/',
                data: { shiftJson: shiftJson, shiftDetailJson: shiftDetailsJson },
                dataType: 'json',
                success: function (response) {
                    if (response.Code === 0) {
                        $('form').removeClass("dirty");
                        var successMsg = "Your request have been sent successfully!";
                        RequestCommon.showSuccessMsg(successMsg, "ChangeShift/EmployeeView");
                    } else {
                        RequestCommon.showErrorMsg(response.message, $("#Employee"));
                    }
                }
            });
        }
        return false;

    });

    function disableControlForDetail() {
        if ($("#IsView").val() === "True") {
            $("input").attr("disabled", "disabled");
            $("select").attr("disabled", "disabled");
            $("textarea").attr("disabled", "disabled");
            $("#searchbox").removeAttr("disabled");
        }
    }

    function ValidateForm() {

        var check = true;
        var errorString = [];
        var empName = $("#Employee").val();
        var fromDate = $("#FromDate").val();
        var toDate = $("#ToDate").val();
        var approver = $("#ApproverId").val();
        var verifier = $("#VerifierId").val();

        var start = $("[name='FromDate']").datepicker('getDate');
        var end = $("[name='ToDate']").datepicker('getDate');
        var days = (end - start) / 1000 / 60 / 60 / 24;

        if (empName == null || empName === "") {
            errorString.push("* The Employee field is required.");
        }
        if (fromDate == null || fromDate === "") {
            errorString.push("* From Date field is required.");
        }
        if (toDate == null || toDate === "") {
            errorString.push("* To Date field is required.");
        }

        if (_checkTopManager === false) {
            if (verifier == null || verifier === "") {
                errorString.push("* Verifier is required.");
            }
            if (approver == null || approver === "") {
                errorString.push("* Approver is required.");
            }
        }

        if (days < 0) {
            errorString.push("* To Date must be greater than From Date.");
        }
        if (errorString.length > 0) {
            $("#errors-model-state").show();
            $("#errors-model-state").html(errorString.join("</br>"));
            return false;
        }
        return check;
    }

    function checkIsTopManager() {
        var employeeId = $("#EmployeeId").val();
        if (employeeId != null) {
            $.ajax({
                url: _global.root + 'api/employee/GetEmployee',
                data: {
                    id: employeeId
                },
                async: false,
                success: function (data) {
                    $(data).each(function (index, item) {
                        _checkTopManager = item.IsTopManager;
                    });
                }
            });
        }
    }

    $('form').areYouSure();
    disableControlForDetail();
});


