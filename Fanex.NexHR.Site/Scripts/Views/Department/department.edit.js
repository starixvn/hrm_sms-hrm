'use strict';
/*global _global*/
/*exported deleteDepartment*/

$(document).ready(function () {
    var nickName;
    var successed = $("#Successed").val();
    if (successed === "True") {
        confirm('Successfull!\nPress OK to continue', function () {
            document.location = _global.root + "department/index";
        });
    }

    $("#Manager").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: _global.root + 'api/employee/getemployees',
                data: {
                    query: request.term,
                    enable: false,
                    isDelete: false,
                    $top: 10
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.NickName + ' (' + item.FullName + ')',
                            value: item.Id
                        };
                    }));
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            nickName = ui.item.label;
            $('#ManagerId').val(ui.item.value);
            $('#Manager').val(nickName);
        },
        focus: function (event, ui) {
            nickName = ui.item.label;
            $('#ManagerId').val(ui.item.value);
            $('#Manager').val(nickName);
            event.preventDefault();
        },
        change: function (event, ui) {
            nickName = ui.item.label;
            $('#ManagerId').val(ui.item.value);
            $('#Manager').val(nickName);
            
        },
        close: function () {
            $('#Manager').val(nickName);
        }
    });
});
