'use strict';
/*global _global*/

$(document).ready(function () {
    var nickName = '';
    $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });
    $('#Employee').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: _global.root + 'employee/getemployees',
                data: {
                    query: request.term,
                    enable: true,
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
            $('#EmpId').val(ui.item.value);
            $('#Employee').val(nickName);
        },
        focus: function (event, ui) {
            nickName = ui.item.label;
            $('#EmpId').val(ui.item.value);
            $('#Employee').val(nickName);
            event.preventDefault();
        },
        change: function (event, ui) {
            nickName = ui.item.label;
            $('#EmpId').val(ui.item.value);
            $('#Employee').val(nickName);
        },
        close: function () {
            $('#Employee').val(nickName);
        }
    });
});