'use strict';
/*global _global*/

var _checked = false;
var _totalDays = 0;
var _ids;

$(document).ready(function () {
    $('.checkuncheckall').click(function () {
        _checked = !_checked;
        $('.aba').prop('checked', _checked);
        $('.checkuncheckall').prop('checked', _checked);
        if (_checked) {
            $('tbody tr').addClass('selected');
        }
        else {
            $('tbody tr').removeClass('selected');
        }
    });

    $("#errors-model-state").hide();

    $('.aba').click(function () {
        var tr = $(this).closest('tr');
        if (tr.hasClass('selected')) {
            tr.removeClass();
        }
        else {
            tr.addClass('selected');
        }
    });

    $('#addadj').click(function () {
        _ids = [];
        $('.aba').each(function (index, item) {
            var checked = $(item).prop('checked');
            if (checked) {
                _ids.push($(item).attr('data-id'));
            }
        });

        $('#EmpIds').val(_ids);

        if (_ids.length > 0) {
            $('#insertform').dialog({
                modal: true,
                width: 410,
                height: 315,
                open: function () {
                    $('#TotalDays').val('');
                }
            });
        }
        else {
            alert('Please choose at least one employee to continue!');
        }
    });

    $('#resetblc').click(function () {
        $('#resetform').dialog({
            modal: true,
            width: 410,
            height: 235,
            open: function () {
                $('#TotalDays').val('');
            }
        });
    });

    $('#insert').click(function () {
        var annualLeaveMuiltiple = {
            EmpIds: $("#EmpIds").val(),
            Description: $("#Description").val(),
            AffectedFrom: $("#AffectedFrom").val(),
            TotalDays: $("#TotalDays").val(),
            LocalLeave: $("#LocalLeave").val()
        };
        var annualJsonFormat = JSON.stringify(annualLeaveMuiltiple);
        var validate = validateInput();
        if (validate) {
            $.ajax({
                type: 'POST',
                url: _global.root + 'AnnualLeaveBalance/Adjust',
                data: { annualLeaveJson: annualJsonFormat },
                dataType: 'json',
                success: function (response) {
                    if (response.IsSuccessful) {
                        document.location = _global.root + 'annualleavebalance/index';
                    } else {
                        $("#errors-model-state").show();
                        $("#errors-model-state").html("* " + response.Message);
                    }
                }
            });
        }
    });

    $('#resetform form').submit(function (evt) {
        evt.preventDefault();
        var $form = $(this);
        _totalDays = parseFloat($('#TotalDays').val());

        if ($form.valid()) {
            $.post(_global.root + 'AnnualLeaveBalance/Reset', $('form').serialize(), function (response) {
                if (response.IsSuccessful) {
                    $('#resetform').dialog('close');
                }
                else
                {
                    alert(response.Message);
                }
            });
        }
    });

    $('#cancel').click(function () {
        $('#insertform').dialog('close');
    });

    $('#cancelReset').click(function () {
        $('#resetform').dialog('close');
    });

    function validateInput() {
        var check = true;
        var errorString = [];
        _totalDays = parseFloat($('#TotalDays').val());

        if (_totalDays == null || _totalDays === "") {
            errorString.push("* The Total days field is required.");
        }

        if (errorString.length > 0) {
            $("#errors-model-state").show();
            $("#errors-model-state").html(errorString.join("</br>"));
            return false;
        }

        return check;
    }
});