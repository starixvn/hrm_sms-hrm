import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';

$(document).ready(function () {
    $('#Employee').autocomplete({
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
                        };
                    }));
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmployeeId').val(ui.item.value);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmployeeId').val(ui.item.value);
        },
        change: function (event, ui) {
            event.preventDefault();
            if (ui && ui.item) {
                $('#Employee').val(ui.item.label);
                $('#EmployeeId').val(ui.item.value);
            } else {
                if ($('#Employee').val() === '') {
                    $('#EmployeeId').val('');
                } else {
                    $('#EmployeeId').val('0');
                }
            }
        }
    });
});