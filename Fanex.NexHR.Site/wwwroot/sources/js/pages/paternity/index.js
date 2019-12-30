import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';

import { confirm, alert, blockUI, unblockUI, initDatepicker, resolveClientUrl } from '../../shared/common.functions';

const paternityModule = (function ($) {
    var domElement = {
        AddButton: '#add-paternity',
        Table: '#list-paternity',
        $dialog: $('#dialog'),
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

    function validateAddPaternity() {
        let isValid = true,
            errorMsg = '',
            empId = domElement.$dialog.find('#EmployeeId').val(),
            bornDate = domElement.$dialog.find('#ChildBornDate').val(),
            leaveDay = domElement.$dialog.find('#LeaveDay').val();
        if (empId === '' || parseInt(empId, 10) <= 0) {
            errorMsg += '<li>Employee is required.</li>';
        }

        if (bornDate === null || bornDate === '') {
            errorMsg += "<li>To Child's born date field is required.</li>";
        }

        if (leaveDay === '' || parseInt(leaveDay, 10) <= 0) {
            errorMsg += '<li>Leave day must be greater than zero.</li>';
        }

        if (errorMsg.length > 0) {
            showErrorMsg(domElement.$dialog, errorMsg);
            isValid = false;
        }

        return isValid;
    }

    function initAddEvent() {
        $(domElement.AddButton).on("click", function (e) {
            e.preventDefault();
            $.get(resolveClientUrl("Paternity/create"), function (data) {
                domElement.$dialog.html(data);
                initAddPaternityDialog();
                initAutoComplete();
                initDatepicker();
            });
        });
    }

    function initAddPaternityDialog() {
        domElement.$dialog.dialog({
            modal: true,
            title: 'Add Employee Paternity',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function () {
                        removeMsg(domElement.$dialog);
                        if (validateAddPaternity()) {
                            $.ajax({
                                type: 'POST',
                                url: resolveClientUrl('Paternity/Create'),
                                data: $(domElement.CreateForm).serialize(),
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        domElement.$dialog.dialog('close');
                                        alert('Create employee paternity successfully!', query);
                                    } else {
                                        showErrorMsg(domElement.$dialog, '<li>' + result.Message + '</li>');
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

    function initEventEditPaternity(PaternityId) {
        domElement.$dialog.dialog({
            modal: true,
            title: 'Edit Employee Paternity',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function () {
                        removeMsg(domElement.$dialog);
                        if (validateAddPaternity()) {
                            $.ajax({
                                type: 'POST',
                                url: resolveClientUrl('Paternity/Update/' + PaternityId),
                                data: $(domElement.EditForm).serialize(),

                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        domElement.$dialog.dialog('close');
                                        alert('Update employee paternity successfully!', query);
                                    } else {
                                        showErrorMsg(domElement.$dialog, '<li>' + result.Message + '</li>');
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

    function initAutoComplete() {
        let $selectorName = domElement.$dialog.find('#Employee'),
            $selectorId = domElement.$dialog.find('#EmployeeId');
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
        $('.edit-paternity').on('click', function (event) {
            event.preventDefault();
            const paternityId = $(this).data("id");
            $.get(resolveClientUrl("Paternity/edit/" + paternityId), function (data) {
                domElement.$dialog.html(data);
                initEventEditPaternity(paternityId);
                initDatepicker();
                initAutoComplete();
            });
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