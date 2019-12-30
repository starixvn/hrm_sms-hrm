import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import moment from 'moment';

import { floatThead, confirm, alert, blockUI, unblockUI, initDatepicker, resolveClientUrl } from '../../shared/common.functions';

const postnatalModule = (function ($) {
    const domElement = {
        AddButton: '#add-postnatal',
        Table: '#list-postnatal',
        $dialog: $('#dialog'),
        CreateForm: '#create-form',
        EditForm: '#edit-form'
    };

    function getPostnatal() {
        $.ajax({
            type: 'GET',
            url: resolveClientUrl("postnatal/getPostnatalGrid?year=" + $("#Year").val()),
            dataType: 'html',
            beforeSend: function () {
                blockUI();
            },
            success: function (data) {
                $(domElement.Table).html(data);
                floatThead('#data');
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

    function validateAddPostnatal() {
        let isValid = true,
            errorMsg = '',
            empId = domElement.$dialog.find('#EmployeeId').val(),
            fromDate = domElement.$dialog.find('#FromDate').val(),
            toDate = domElement.$dialog.find('#ToDate').val(),
            workingHours = domElement.$dialog.find('#WorkingHours').val();

        if (!empId || parseInt(empId, 10) <= 0) {
            errorMsg += '<li>Employee is required.</li>';
        }

        if (!moment(toDate).isSameOrAfter(moment(fromDate))) {
            errorMsg += '<li>To Date must be greater than or equal to From Date.</li>';
        }

        if (workingHours === '' || parseInt(workingHours, 10) < 1 || parseInt(workingHours, 10) > 24) {
            errorMsg += '<li>Working hours per day must be an integer and between 1 and 24.</li>';
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
            $.get(resolveClientUrl("postnatal/create"), function (data) {
                domElement.$dialog.html(data);
                initAddPostnatalDialog();
                initAutoComplete();
                initDatepicker();
            });
        });
    }

    function initAddPostnatalDialog() {
        domElement.$dialog.dialog({
            modal: true,
            title: 'Add Employee Postnatal',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function (e) {
                        removeMsg(domElement.$dialog);
                        if (validateAddPostnatal()) {
                            $.ajax({
                                type: 'POST',
                                url: resolveClientUrl('postnatal/Create'),
                                data: $(domElement.CreateForm).serialize(),
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        domElement.$dialog.dialog('close');
                                        alert('Create employee postnatal successfully!', getPostnatal);
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

    function initEventEditPostnatal(postnatalId) {
        domElement.$dialog.dialog({
            modal: true,
            title: 'Edit Employee Postnatal',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function () {
                        removeMsg(domElement.$dialog);
                        if (validateAddPostnatal()) {
                            $.ajax({
                                type: 'POST',
                                url: resolveClientUrl('postnatal/Update/' + postnatalId),
                                data: $(domElement.EditForm).serialize(),
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        domElement.$dialog.dialog('close');
                                        alert('Update employee postnatal successfully!', getPostnatal);
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
                    url: resolveClientUrl('employee/GetMemberByGender'),
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
        });
    }

    function initEditEvent() {
        $('.edit-postnatal').on('click', function (event) {
            event.preventDefault();

            const postnatalId = $(this).data('id');
            $.get(resolveClientUrl('postnatal/edit/' + postnatalId), function (data) {
                domElement.$dialog.html(data);
                initEventEditPostnatal(postnatalId);
                initAutoComplete();
                initDatepicker();
            });
        });
    }

    function initDeleteEvent() {
        var $selector = $('.delete-postnatal');
        $selector.on('click', function (event) {
            event.preventDefault();
            const postnatalId = $(this).data('id');
            confirm("Are you sure you want to remove this postnatal?", function () {
                $.ajax({
                    type: 'POST',
                    url: resolveClientUrl('postnatal/delete/' + postnatalId),
                    success: function (result) {
                        if (result.IsSuccessful) {
                            alert('Remove employee postnatal successfully!', getPostnatal);
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
            getPostnatal();
        });
    }

    function initModule() {
        initSubmitFiler();
        getPostnatal();
        initAddEvent();
    }

    return {
        initModule
    };
})(jQuery);

$(document).ready(function () {
    postnatalModule.initModule();
});