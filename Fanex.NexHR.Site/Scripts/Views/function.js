'use strict';
/*global _global*/

function CheckboxOnChange() {
    $("#save").removeAttr('disabled');
}

function generateEventForCheckbox() {
    var input = $('input');
    for (var i = 0, len = input.length; i < len; i++) {
        if (input[i].type === 'checkbox') {
            input[i].onclick = CheckboxOnChange;
        }
    }
}

function query() {
    var roleId = $('#Role').val();
    $.ajax({
        url: _global.root + 'Function/GetFunctionByRoleId',
        type: 'GET',
        dataType: 'html',
        data: { roleId: roleId },
        success: function (response) {
            $('#list-functions').html(response);
            $('#save').attr('disabled', 'disabled');
            generateEventForCheckbox();
        }
    });
}

function save() {
    var roleFunctions = [];
    $("#list-functions tbody tr").each(function () {
        var record = {
            FunctionId: parseInt($('td #functionId', this).val(), 10),
            IsRead: $('td #IsRead', this).attr('checked') === 'checked',
            IsHost: $('td #IsHost', this).attr('checked') === 'checked'
        };

        roleFunctions.push(record);
    });

    var roleId = parseInt($("#Role").val(), 10);

    $.ajax({
        type: 'POST',
        url: '/Function/AssignFunctionToRole/',
        data: JSON.stringify({ Functions: roleFunctions, RoleId: roleId }),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
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

$(document).ready(function () {
    $('#Role').change(function () {
        query();
    });

    $('#save').click(function () {
        save();
    });

    query();
});