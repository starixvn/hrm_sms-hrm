'use strict';
/*global _global*/

function ChangeData() {
    $("#save").removeAttr('disabled');
}
function Onclick_IsHost() {
    var id = $(this).data('id');
    var checked = $(this).prop('checked');
    $("#" + id + "_IsRead").prop('disabled', checked);
    $("#" + id + "_IsRead").prop('checked', checked);

    ChangeData();
}

function generateEventForCheckbox() {
    var input = $('input');
    for (var i = 0, len = input.length; i < len; i++) {
        if (input[i].type === 'checkbox') {
            input[i].onclick = ChangeData;
        }
        if (input[i].name === 'IsHost') {
            input[i].onclick = Onclick_IsHost;
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
            $('#save').prop('disabled', 'disabled');
            generateEventForCheckbox();
        }
    });
}

function save() {
    var roleFunctions = [];
    $("#list-functions tbody tr").each(function () {
        var functionId = parseInt($('td #functionId', this).val(), 10);
        var record = {
            FunctionId: functionId,
            IsRead: $('#' + functionId + '_IsRead').attr('checked') === 'checked',
            IsHost: $('#' + functionId + '_IsHost').attr('checked') === 'checked'
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