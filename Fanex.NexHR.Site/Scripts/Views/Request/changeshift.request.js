$(document).ready(function () {

    //$('form').areYouSure();
    var _checkTopManager = false;
    $(".datepicker").attr('readonly', 'readonly');
    //$(".dropdown").chosen();
    $("#errors-model-state").hide();

    $("#shift-unavailable").hide();

    $("#tblchangeshift tbody tr").each(function () {
        var selected = $(this).find("input[name='item.ShiftId']").val();
        $('td select', this).val(selected);
    });

    checkHasData();

    function checkHasData() {
        var hasData = $("#item_HasData").val();
        if (hasData == "True") {
            return false;
        }
        else {
            getTable();
        }
        hasData = "False";
    }

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
            url: _global.root + "changeshift/ChangeShiftDetails",
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
        checkIsTopManager();
        var shift = {
            EmployeeId: $("#EmployeeId").val(),
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
                Date: $('td:first-child', this).html(),
                FromShift: $('#item_FromShift', this).val(),
                ToShift: $('td select', this).val(),
                Remark: $('td #item_Remark', this).val()
            });
        });


        var shiftJson = JSON.stringify(shift);
        var shiftDetailsJson = JSON.stringify(shiftDetails);

        var successed = $("#successed").val();
        var year = $("#year").val();
        var validate = ValidateForm();

        if (validate) {
            $.ajax({
                type: 'POST',
                url: '/ChangeShift/Create/',
                data: { shiftJson: shiftJson, shiftDetailJson: shiftDetailsJson },
                dataType: 'json',
                success: function (response) {
                    if (response.success) {
                        $('form').removeClass("dirty");
                        confirm('Successfull!\nPress OK to continue', function () {
                            document.location = _global.root + "ChangeShift/EmployeeView/" + year;
                        });
                    } else {
                        $("#errors-model-state").html("* " + response.errors);
                        $("#errors-model-state").show();
                    }
                }
            });
        }
        return false;
    });



    //Submit Edit

    $("input[name='submitEdit']").click(function () {
        var shift = {
            Employee: $("#EmployeeId").val(),
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
                Date: $('td:first-child', this).html(),
                FromShift: $('#item_FromShift', this).val(),
                ToShift: $('td select', this).val(),
                Remark: $('td #item_Remark', this).val()
            });
        });

        var successed = $("#successed").val();
        var year = $("#year").val();
        var validate = ValidateForm();
        var requestId = $("#Id").val();

        var shiftJson = JSON.stringify(shift);
        var shiftDetailsJson = JSON.stringify(shiftDetails);

        if (validate) {
            $.ajax({
                type: 'POST',
                url: '/ChangeShift/Update/',
                data: { shiftJson: shiftJson, shiftDetailJson: shiftDetailsJson, requestId: requestId },
                dataType: 'json',
                success: function (response) {
                    if (response.success) {
                        $('form').removeClass("dirty");
                        alert("Data Saved!");
                    } else {
                        $("#errors-model-state").html("* " + response.errors);
                    }
                }
            });
        }
        return false;

    });

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

        if (empName == null || empName == "") {
            errorString.push("* The Employee field is required.");
        }
        if (fromDate == null || fromDate == "") {
            errorString.push("* From Date field is required.");
        }
        if (toDate == null || toDate == "") {
            errorString.push("* To Date field is required.");
        }

        if (_checkTopManager == false) {
            if (verifier == null || verifier == "") {
                errorString.push("* Verifier is required.");
            }
            if (approver == null || approver == "") {
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
        if (employeeId != null || employee != "") {
            var employee = $.ajax({
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
    
});


