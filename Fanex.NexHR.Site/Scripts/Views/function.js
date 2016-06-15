'use strict';
/*global _global*/

function query() {
    var roleId = $('#Role').val();
    $.ajax({
        url: _global.root + 'Function/GetFunctionByRoleId',
        type: 'GET',
        dataType: 'html',
        data: { roleId: roleId },
        beforeSend: function () {

        },
        success: function (response) {
            $('#list-functions').html(response);
        },
        complete: function () {

        }
    });
}

function save() {
    var roleFunctions = [];
    $("#list-functions tbody tr").each(function () {
        var record = {
            FunctionId: parseInt($('td #functionId', this).val()),
            FunctionKey: $('td #functionKey', this).val(),
            FunctionName: $('td #functionName', this).val(),
            IsRead: $('td #IsRead', this).attr('checked') === 'checked',
            IsEdit: $('td #IsEdit', this).attr('checked') === 'checked',
            IsDelete: $('td #IsDelete', this).attr('checked') === 'checked',
            IsCreate: $('td #IsCreate', this).attr('checked') === 'checked'
        };

        roleFunctions.push(record);
    });

    var roleId = parseInt($("#Role").val());

    $.ajax({
        type: 'POST',
        url: '/Function/AssignFunctionToRole/',
        data: JSON.stringify({ Functions: roleFunctions, RoleId: roleId }),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                //$('form').removeClass("dirty");
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
    $('#query').click(function () {
        query();
    });

    $('#save').click(function () {
        save();
    });

    query();
});