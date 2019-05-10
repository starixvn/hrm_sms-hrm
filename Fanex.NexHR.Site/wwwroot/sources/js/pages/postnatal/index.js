import $ from 'jquery/dist/jquery';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'jquery-contextmenu/dist/jquery.contextMenu';

import { floatingHeader, showMessage, fadeOutMessage, confirm, alert, blockUI, unblockUI } from '../../shared/common.functions';
import '../../date';

var postnatal = (function ($) {
    var domElement = {
        AddButton: '#add-postnatal',
        Table: '#list-postnatal',
        CreateDialog: '#dialogAddPostnatal',
        EditDialog: '#dialogEditPostnatal',
        CreateForm: '#create-form',
        EditForm: '#edit-form'
    };

    function getPostnatal() {
        var url = _global.root + "postnatal/ListPostnatal?year=" + $("#Year").val();
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
                floatingHeader();
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

    function validateAddPostnatal() {
        var isValid = true,
            errorMsg = '',
            empId = $(domElement.CreateDialog).find('#EmployeeId').val(),
            fromDate = $(domElement.CreateDialog).find('#FromDate').val(),
            toDate = $(domElement.CreateDialog).find('#ToDate').val();

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
            $.get(_global.root + "postnatal/create", function (data) {
                $(domElement.CreateDialog).html(data);
                initEventAddPostnatalModal();
            });
        });
    }

    function initDatePicker($dialog) {
        if ($('.datepicker').length > 0) {
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

    function initAddPostnatalDialog() {
        $(domElement.CreateDialog).dialog({
            modal: true,
            title: 'Add Employee Postnatal',
            buttons: [
                {
                    text: 'Submit',
                    click: function (e) {
                        removeMsg($(domElement.CreateDialog));
                        if (validateAddPostnatal()) {
                            $.ajax({
                                type: 'POST',
                                url: _global.root + 'postnatal/Create',
                                data: $(domElement.CreateForm).serialize(),
                                beforeSend: function () {
                                    showMessage('Processing');
                                    blockUI();
                                },
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        $(domElement.CreateDialog).dialog('close');
                                        alert('Create employee postnatal successfully!');
                                        getPostnatal();
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
                    getPostnatal();
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

    function initEditModel(postnatalId) {
        $.get(_global.root + "postnatal/edit/" + postnatalId, function (data) {
            $(domElement.EditDialog).html(data);
            initEventEditPostnatal(postnatalId);
            initDatePicker($(domElement.EditDialog));
            initAutoComplete($(domElement.EditDialog));
        });
    }

    function initEventEditPostnatal(postnatalId) {
        $(domElement.EditDialog).dialog({
            modal: true,
            title: 'Edit Employee Postnatal',
            buttons: [
                {
                    text: 'Submit',
                    click: function () {
                        removeMsg($(domElement.EditDialog));
                        if (validateAddPostnatal()) {
                            $.ajax({
                                type: 'POST',
                                url: _global.root + 'postnatal/Update/' + postnatalId,
                                data: $(domElement.EditForm).serialize(),
                                beforeSend: function () {
                                    showMessage('Processing');
                                    blockUI();
                                },
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        $(domElement.EditDialog).dialog('close');
                                        alert('Update employee postnatal successfully!');
                                        getPostnatal();
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
                                },
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
                    getPostnatal();
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
                    url: _global.root + 'employee/GetMemberByGender',
                    data: {
                        keyword: request.term,
                        gender: 0 //female
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
        var $selector = $('.edit-postnatal');
        $selector.on('click', function (event) {
            event.preventDefault();
            var postnatalId = $(this).data("id");
            initEditModel(postnatalId);
        });
    }

    function initDeleteEvent() {
        var $selector = $('.delete-postnatal');
        $selector.on('click', function (event) {
            event.preventDefault();
            var postnatalId = $(this).data('id');
            confirm("Are you sure you want to remove this?", function () {
                $.ajax({
                    type: 'POST',
                    url: _global.root + 'postnatal/delete/' + postnatalId,
                    success: function (result) {
                        if (result.IsSuccessful) {
                            alert('Remove employee postnatal successfully!');
                        } else {
                            alert(result.message);
                        }
                        getPostnatal();
                    }
                });
            });
        });
    }

    function initEventAddPostnatalModal() {
        initAddPostnatalDialog($(domElement.CreateDialog));
        initAutoComplete($(domElement.CreateDialog));
        initDatePicker($(domElement.CreateDialog));
    }

    function initSubmitFiler() {
        $('form').on('submit', function (e) {
            e.preventDefault();
            getPostnatal();
        });
    }

    function initPostnatal() {
        initSubmitFiler();
        getPostnatal();
        initAddEvent();
    }

    return {
        initPostnatal
    };
})(jQuery);

$(document).ready(function () {
    postnatal.initPostnatal();
})