import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';

$(document).ready(function () {
    $('#Manager').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: _global.root + 'employee/getmemberbymanager',
                data: {
                    keyword: request.term
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
        minLength: 2,
        select: function (event, ui) {
            event.preventDefault();
            $('#Manager').val(ui.item.label);
            $('#ManagerId').val(ui.item.value);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $('#Manager').val(ui.item.label);
            $('#ManagerId').val(ui.item.value);
        },
        change: function (event, ui) {
            event.preventDefault();
            if (ui && ui.item) {
                $('#Manager').val(ui.item.label);
                $('#ManagerId').val(ui.item.value);
            } else {
                if ($('#Manager').val() === '') {
                    $('#ManagerId').val('');
                } else {
                    $('#Manager').val('0');
                }
            }
        }
    });
});