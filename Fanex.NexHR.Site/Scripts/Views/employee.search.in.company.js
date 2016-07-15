'use strict';
/*global _global*/
/*exported*/

var nickName;
$(document).ready(function () {
    var $employee = $("#Employee");
    var $employeeId = $('#EmployeeId');

    $employee.autocomplete({
        source: function (request, response) {
            $.ajax({
                url: _global.root + 'api/employee/getemployees',
                data: {
                    query: request.term,
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
            if (ui.item != null) {
                nickName = ui.item.label;
                $employee.val(nickName);
                $employeeId.val(ui.item.value);
            }
        },
        focus: function (event, ui) {
            if (ui.item != null) {
                nickName = ui.item.label;
                $employee.val(nickName);
                $employeeId.val(ui.item.value);
            }
            event.preventDefault();
        },
        change: function (event, ui) {
            if (ui.item != null) {
                nickName = ui.item.label;
                $employee.val(nickName);
                $employeeId.val(ui.item.value);
            }
        },
        close: function () {
            $employee.val(nickName);
        }
    });

    // when user clear the textbox, we must set employeeId field to -1 for seaching all employees
    $('form').submit(function () {
        var employee = $employee.val();
        if (employee.length === 0) {
            $employeeId.val(-1);
        }
        return true;
    });
});
