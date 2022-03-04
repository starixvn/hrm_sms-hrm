import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import moment from 'moment';
import axios from 'axios';

import '../../shared/multiselect-tree-view';

import {
    axiosError, floatThead, confirm, alert,
    blockUI, unblockUI, initDatepicker, resolveClientUrl
} from '../../shared/common.functions';

const workFromHomeModule = (function ($) {
    const domElement = {
        AddButton: '#add-workfromhome',
        Table: '#list-workfromhome',
        $dialog: $('#dialog'),
        CreateForm: '#create-form',
        EditForm: '#edit-form'
    };

    function getEmployeeWorkFromHome() {
        blockUI();
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        axios({
            url: resolveClientUrl('EmployeeWorkFromHome/GetEmployeeWorkFromHomeGrid'),
            method: 'GET',
            params: {
                fromDate: $("#FromDateFilter").val(),
                toDate: $("#ToDateFilter").val(),
                teamIds: $('#TeamIds').val(),
                isLocal: $('#IsLocal').val()
            },
            responseType: 'text'
        }).then(response => {
            $(domElement.Table).html(response.data);
            floatThead('#data');
            initEditEvent();
            initDeleteEvent();
            initToolTip();
        }).catch(axiosError).then(() => unblockUI());
    }

    function initToolTip() {
        $('.tooltip-tipsy').tipsy({
            html: true,
            gravity: 'e'
        });
    }

    function validateAddEmployeeWorkFromHome() {
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
            $.get(resolveClientUrl("EmployeeWorkFromHome/create"), function (data) {
                domElement.$dialog.html(data);
                initAddEmployeeWorkFromHomeDialog();
                initAutoComplete();
                initDatepicker();
            });
        });
    }

    function initAddEmployeeWorkFromHomeDialog() {
        domElement.$dialog.dialog({
            modal: true,
            title: 'Add Employee Work From Home',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function (e) {
                        removeMsg(domElement.$dialog);
                        if (validateAddEmployeeWorkFromHome()) {
                            $.ajax({
                                type: 'POST',
                                url: resolveClientUrl('EmployeeWorkFromHome/Create'),
                                data: $(domElement.CreateForm).serialize(),
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        domElement.$dialog.dialog('close');
                                        alert('Create employee work from home successfully!', getEmployeeWorkFromHome);
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

    function initEventEditWorkFromHome(employeeworkfromhomeId) {
        domElement.$dialog.dialog({
            modal: true,
            title: 'Edit Employee Work From Home',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function () {
                        removeMsg(domElement.$dialog);
                        if (validateAddEmployeeWorkFromHome()) {
                            $.ajax({
                                type: 'POST',
                                url: resolveClientUrl('EmployeeWorkFromHome/Edit/' + employeeworkfromhomeId),
                                data: $(domElement.EditForm).serialize(),
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        domElement.$dialog.dialog('close');
                                        alert('Update employee work from home successfully!', getEmployeeWorkFromHome);
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
                    url: resolveClientUrl('employee/getemployees'),
                    data: {
                        query: request.term,
                        enable: true,
                        isDelete: false
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
        $('.edit-employeeworkfromhome').on('click', function (event) {
            event.preventDefault();

            const employeeworkfromhomeId = $(this).data('id');
            $.get(resolveClientUrl('EmployeeWorkFromHome/Edit/' + employeeworkfromhomeId), function (data) {
                domElement.$dialog.html(data);
                initEventEditWorkFromHome(employeeworkfromhomeId);
                initAutoComplete();
                initDatepicker();
            });
        });
    }

    function initDeleteEvent() {
        var $selector = $('.delete-employeeworkfromhome');
        $selector.on('click', function (event) {
            event.preventDefault();
            const employeeworkfromhomeId = $(this).data('id');
            confirm("Are you sure you want to remove this employee work from home?", function () {
                $.ajax({
                    type: 'POST',
                    url: resolveClientUrl('EmployeeWorkFromHome/Delete/' + employeeworkfromhomeId),
                    success: function (result) {
                        if (result.IsSuccessful) {
                            alert('Remove employee work from home successfully!', getEmployeeWorkFromHome);
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
            getEmployeeWorkFromHome();
        });
    }

    function initModule() {
        initSubmitFiler();
        initAddEvent();
        initDatepicker();

        axios(resolveClientUrl('Section/GetOrgChart'))
            .then(response => {
                $('#Team').multiSelectTreeView({
                    data: response.data,
                    isSelectedAll: true
                });
                getEmployeeWorkFromHome();
            }).catch(axiosError);
    }

    return {
        initModule
    };
})(jQuery);

$(document).ready(function () {
    workFromHomeModule.initModule();
});