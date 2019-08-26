import $ from 'jquery/dist/jquery';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';

import { confirm, alert, blockUI, unblockUI } from '../../shared/common.functions';

var paternity = (function ($) {
    var domElement = {
        AddButton: '#add-paternity',
        Table: '#list-paternity',
        CreateDialog: '#dialogAddPaternity',
        EditDialog: '#dialogEditPaternity',
        CreateForm: '#create-form',
        EditForm: '#edit-form'
    };

    function query() {
        var url = _global.root + "paternity/ListPaternity?year=" + $("#Year").val();
        $.ajax({
            type: 'GET',
            url: url,
            dataType: 'html',
            beforeSend: function () {
                blockUI();
            },
            success: function (data) {
                $(domElement.Table).html(data);
                initEditEvent();
                initDeleteEvent();
                initToolTip();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            },
            complete: function () {
                unblockUI();
            }
        });
    }

    function initToolTip() {
        $('.tooltip-tipsy').tipsy({
            html: true,
            gravity: 'e'
        });
    }

    function validateAddPaternity(form) {
        var isValid = true,
            errorMsg = '',
            empId = $(form).find('#EmployeeId').val(),
            bornDate = $(form).find('#ChildBornDate').val(),
            leaveDay = $(form).find('#LeaveDay').val();
        if (empId && parseInt(empId, 10) <= 0) {
            errorMsg += '<li>The Employee field is required.</li>';
        }

        if (bornDate === null || bornDate === '') {
            errorMsg += "<li>To Child's born date field is required.</li>";
        }

        if (parseInt(leaveDay <= 0)) {
            errorMsg += '<li>Leave day must be more than zero.</li>';
        }

        if (errorMsg.length > 0) {
            showErrorMsg($(form), errorMsg);
            isValid = false;
        }

        return isValid;
    }

    function initAddEvent() {
        $(domElement.AddButton).on("click", function (e) {
            e.preventDefault();
            $.get(_global.root + "Paternity/create", function (data) {
                $(domElement.CreateDialog).html(data);
                initEventAddPaternityModal();
            });
        });
    }

    function initDatePicker($dialog) {
        if ($dialog.find('.datepicker').length > 0) {
            var today = new Date();
            var mindate = new Date(2010, 1, 1);
            var maxdate = new Date(today.getFullYear(), today.getMonth(), today.getDate());

            var disabledDates = $('.datepicker').attr('disabledDate');

            if (disabledDates.length > 0) {
                $('.datepicker').datepicker({
                    dateFormat: "dd M, yy",
                    minDate: mindate,
                    maxDate: maxdate,
                    beforeShowDay: checkBadDates,
                    changeMonth: true,
                    changeYear: true
                });
            }
            else {
                $('.datepicker').datepicker({
                    dateFormat: "dd M, yy",
                    minDate: mindate,
                    maxDate: maxdate,
                    changeMonth: true,
                    changeYear: true
                });
            }

            $('.datepicker').attr('readonly', 'readonly');
            $('.datepicker').keydown(function (e) {
                var tab = 9, enter = 13;
                if (e.keyCode === tab || e.keyCode === enter) {
                    return true;
                }

                return false;
            });
        }
    }

    function initAddPaternityDialog() {
        $(domElement.CreateDialog).dialog({
            modal: true,
            title: 'Add Employee Paternity',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function () {
                        removeMsg($(domElement.CreateDialog));
                        if (validateAddPaternity()) {
                            $.ajax({
                                type: 'POST',
                                url: _global.root + 'Paternity/Create',
                                data: $(domElement.CreateForm).serialize(),
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        $(domElement.CreateDialog).dialog('close');
                                        alert('Create employee paternity successfully!', query);
                                    } else {
                                        showErrorMsg($(domElement.CreateDialog), '<li>' + result.Message + '</li>');
                                    }
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    alert(textStatus + ': ' + errorThrown);
                                }
                            });
                        }
                    }
                },
                {
                    text: 'Cancel',
                    class: 'btn btn-default',
                    click: function () {
                        $(this).dialog('close');
                    }
                }
            ],
            minWidth: 460,
            dialogClass: 'modal-shadow'
        });
    }

    function removeMsg($dialog) {
        $dialog.find('.validation-summary-valid')
            .hide().removeClass('success').removeClass('validation-summary-errors').find('ul:first').html("");
    }

    function showErrorMsg($dialog, msg) {
        $dialog.find('.validation-summary-valid')
            .show().addClass('validation-summary-errors').find('ul:first').append(msg);
    }

    function initEditModel(PaternityId) {
        $.get(_global.root + "Paternity/edit/" + PaternityId, function (data) {
            $(domElement.EditDialog).html(data);
            initEventEditPaternity(PaternityId);
            initDatePicker($(domElement.EditDialog));
            initAutoComplete($(domElement.EditDialog));
        });
    }

    function initEventEditPaternity(PaternityId) {
        $(domElement.EditDialog).dialog({
            modal: true,
            title: 'Edit Employee Paternity',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function () {
                        removeMsg($(domElement.EditDialog));
                        if (validateAddPaternity()) {
                            $.ajax({
                                type: 'POST',
                                url: _global.root + 'Paternity/Update/' + PaternityId,
                                data: $(domElement.EditForm).serialize(),

                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        $(domElement.EditDialog).dialog('close');
                                        alert('Update employee paternity successfully!', query);
                                    } else {
                                        showErrorMsg($(domElement.EditDialog), '<li>' + result.Message + '</li>');
                                    }
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    alert(textStatus + ': ' + errorThrown);
                                }
                            });
                        }
                    }
                },
                {
                    text: 'Cancel',
                    class: 'btn btn-default',
                    click: function () {
                        $(this).dialog('close');
                    }
                }
            ],
            minWidth: 460,
            dialogClass: 'modal-shadow'
        });
    }

    function initAutoComplete($dialog) {
        var $selectorName = $dialog.find('#Employee'),
            $selectorId = $dialog.find('#EmployeeId');
        $selectorName.autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: _global.root + 'Employee/GetMemberByGender',
                    data: {
                        keyword: request.term,
                        gender: 1 //male
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
                $selectorName.val(ui.item.label);
                $selectorId.val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                $selectorName.val(ui.item.label);
                $selectorId.val(ui.item.value);
            },
            change: function (event, ui) {
                event.preventDefault();
                if (ui && ui.item) {
                    $selectorName.val(ui.item.label);
                    $selectorId.val(ui.item.value);
                } else {
                    if ($selectorName.val() === '') {
                        $selectorId.val('');
                    }
                }
            }
        });
    }

    function initEditEvent() {
        var $selector = $('.edit-paternity');
        $selector.on('click', function (event) {
            event.preventDefault();
            var PaternityId = $(this).data("id");
            initEditModel(PaternityId);
        });
    }

    function initDeleteEvent() {
        var $selector = $('.delete-paternity');
        $selector.on('click', function (event) {
            event.preventDefault();
            var PaternityId = $(this).data('id');
            confirm("Are you sure you want to remove this paternity?", function () {
                $.ajax({
                    type: 'POST',
                    url: _global.root + 'Paternity/delete/' + PaternityId,
                    success: function (result) {
                        if (result.IsSuccessful) {
                            alert('Remove employee Paternity successfully!', query);
                        } else {
                            alert(result.message);
                        }
                    }
                });
            });
        });
    }

    function initEventAddPaternityModal() {
        initAddPaternityDialog($(domElement.CreateDialog));
        initAutoComplete($(domElement.CreateDialog));
        initDatePicker($(domElement.CreateDialog));
    }

    function initSubmitFiler() {
        $('form').on('submit', function (e) {
            e.preventDefault();
            query();
        });
    }

    function initPaternity() {
        initSubmitFiler();
        query();
        initAddEvent();
    }

    return {
        initPaternity
    };
})(jQuery);

$(document).ready(function () {
    paternity.initPaternity();
});