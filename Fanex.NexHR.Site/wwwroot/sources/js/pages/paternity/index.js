import $ from 'jquery/dist/jquery';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'jquery-contextmenu/dist/jquery.contextMenu';

import { floatingHeader, showMessage, fadeOutMessage, confirm, alert, blockUI, unblockUI } from '../../shared/common.functions';
import '../../date';

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
                showMessage('Processing');
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
                fadeOutMessage();
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

    function validateAddPaternity() {
        var isValid = true,
            errorMsg = '',
            empId = $(domElement.CreateDialog).find('#EmployeeId').val(),
            fromDate = $(domElement.CreateDialog).find('#FromDate').val(),
            toDate = $(domElement.CreateDialog).find('#ToDate').val()

        if (empId && parseInt(empId, 10) <= 0) {
            errorMsg += '<li>The Employee field is required.</li>';
        }

        if (fromDate === null || fromDate === '') {
            errorMsg += '<li>From date field is required.</li>';
        }

        if (toDate === null || toDate === '') {
            errorMsg += '<li>To date field is required.</li>';
        }

        if (fromDate > toDate) {
            errorMsg += '<li>From date must be less than to date.</li>';
        }

        if (errorMsg.length > 0) {
            showErrorMsg($(domElement.CreateDialog), errorMsg);
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
        if ($('.datepicker').length > 0) {
            // get minDate, maxDate, disabledDates
            var mindate = $('.datepicker').attr('mindate');
            var maxdate = $('.datepicker').attr('maxdate');
            var disabledDates = $('.datepicker').attr('disabledDate');

            if (disabledDates.length > 0) {
                $('.datepicker').datepicker({
                    dateFormat: "dd M, yy",
                    beforeShowDay: checkBadDates,
                    changeMonth: true,
                    changeYear: true
                });
            }
            else {
                $('.datepicker').datepicker({
                    dateFormat: "dd M, yy",
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
                    click: function () {
                        removeMsg($(domElement.CreateDialog));
                        if (validateAddPaternity()) {
                            $.ajax({
                                type: 'POST',
                                url: _global.root + 'Paternity/Create',
                                data: $(domElement.CreateForm).serialize(),
                                beforeSend: function () {
                                    showMessage('Processing');
                                    blockUI();
                                },
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        $(domElement.CreateDialog).dialog('close');
                                        alert('Create employee paternity successfully!');
                                        query();
                                    } else {
                                        showErrorMsg($(domElement.CreateDialog), '<li>' + result.Message + '</li>');
                                    }
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    alert(textStatus + ': ' + errorThrown);
                                },
                                complete: function () {
                                    fadeOutMessage();
                                    unblockUI();
                                }
                            });
                        }
                    }
                },
                {
                    text: 'Cancel',
                    click: function () {
                        $(this).dialog('close');
                    }
                }
            ],
            close: function () {
                if ($('#frmAddOtherDiscipline').find('#IsSuccessful').prop('checked')) {
                    query();
                }
            },
            show: {
                effect: 'fade',
                duration: 300
            },
            hide: {
                effect: 'fade',
                duration: 200
            },
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
                    click: function () {
                        removeMsg($(domElement.EditDialog));
                        if (validateAddPaternity()) {
                            $.ajax({
                                type: 'POST',
                                url: _global.root + 'Paternity/Update/' + PaternityId,
                                data: $(domElement.EditForm).serialize(),
                                beforeSend: function () {
                                    showMessage('Processing');
                                    blockUI();
                                },
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        $(domElement.EditDialog).dialog('close');
                                        alert('Update employee paternity successfully!');
                                        query();
                                    } else {
                                        showErrorMsg($(domElement.EditDialog), '<li>' + result.Message + '</li>');
                                    }
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    alert(textStatus + ': ' + errorThrown);
                                },
                                complete: function () {
                                    fadeOutMessage();
                                    unblockUI();
                                }
                            });
                        }
                    }
                },
                {
                    text: 'Cancel',
                    click: function () {
                        $(this).dialog('close');
                    }
                }
            ],
            close: function () {
                if ($(domElement.EditForm).find('#IsSuccessful').prop('checked')) {
                    query();
                }
            },
            show: {
                effect: 'fade',
                duration: 300
            },
            hide: {
                effect: 'fade',
                duration: 200
            },
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
        }).autocomplete('widget').addClass('search-employee-autocomplete');
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
            confirm("Are you sure you want to remove this?", function () {
                $.ajax({
                    type: 'POST',
                    url: _global.root + 'Paternity/delete/' + PaternityId,
                    success: function (result) {
                        alert('Remove employee Paternity successfully!');
                        query();
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
})