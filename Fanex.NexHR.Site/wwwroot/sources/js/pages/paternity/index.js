import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';

import { confirm, alert, blockUI, unblockUI, initDatepicker, resolveClientUrl } from '../../shared/common.functions';

const paternityModule = (function ($) {
    var domElement = {
        AddButton: '#add-paternity',
        Table: '#list-paternity',
        CreateDialog: '#dialogAddPaternity',
        EditDialog: '#dialogEditPaternity',
        CreateForm: '#create-form',
        EditForm: '#edit-form'
    };

    function query() {
        $.ajax({
            type: 'GET',
            url: resolveClientUrl("paternity/ListPaternity?year=" + $("#Year").val()),
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
            $.get(resolveClientUrl("Paternity/create"), function (data) {
                $(domElement.CreateDialog).html(data);
                initEventAddPaternityModal();
            });
        });
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
                                url: resolveClientUrl('Paternity/Create'),
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
        $.get(resolveClientUrl("Paternity/edit/" + PaternityId), function (data) {
            $(domElement.EditDialog).html(data);
            initEventEditPaternity(PaternityId);
            initDatepicker();
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
                                url: resolveClientUrl('Paternity/Update/' + PaternityId),
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
                    url: resolveClientUrl('Employee/GetMemberByGender'),
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
            const paternityId = $(this).data('id');
            confirm("Are you sure you want to remove this paternity?", function () {
                $.ajax({
                    type: 'POST',
                    url: resolveClientUrl('Paternity/delete/' + paternityId),
                    success: function (result) {
                        if (result.IsSuccessful) {
                            alert('Remove employee paternity successfully!', query);
                        } else {
                            alert(result.Message);
                        }
                    }
                });
            });
        });
    }

    function initEventAddPaternityModal() {
        initAddPaternityDialog($(domElement.CreateDialog));
        initAutoComplete($(domElement.CreateDialog));
        initDatepicker();
    }

    function initSubmitFiler() {
        $('form').on('submit', function (e) {
            e.preventDefault();
            query();
        });
    }

    function initModule() {
        initSubmitFiler();
        query();
        initAddEvent();
    }

    return {
        initModule
    };
})(jQuery);

$(document).ready(function () {
    paternityModule.initModule();
});