'use strict';
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
        $.get(url, function (data) {
            $(domElement.Table).html(data);
            initEditEvent();
            initDeleteEvent();
        });
    }

    function validateAddPostnatal() {
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
            $.get(_global.root + "postnatal/create", function (data) {
                $(domElement.CreateDialog).html(data);
                initEventAddPostnatalModal();
            });
        });
    }

    function initDatePicker($dialog) {
        var mindate = $dialog.find('.datepicker').attr('mindate');

        $dialog.find('.datepicker').datepicker({
            dateFormat: 'dd M, yy',
            showOn: 'button',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            buttonImage: _global.root + 'images/calendar.png'
        });

        if (mindate !== 'null') {
            mindate = mindate.split('/');
            $dialog.find('.datepicker').datepicker('option', 'minDate', new Date(mindate[0], mindate[1] - 1, mindate[2]));
        }
    }

    function initAddPostnatalDialog() {
        $(domElement.CreateDialog).dialog({
            modal: true,
            title: 'Add Employee Postnatal',
            buttons: [
                {
                    text: 'Submit',
                    click: function () {
                        removeMsg($(domElement.CreateDialog));
                        if (validateAddPostnatal()) {
                            $.ajax({
                                type: 'POST',
                                url: _global.root + 'postnatal/Create',
                                data: $(domElement.CreateForm).serialize(),
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        $(domElement.CreateDialog).dialog('close');
                                        alert('Create employee postnatal successfully!');
                                        getPostnatal();
                                    } else {
                                        showErrorMsg($(domElement.CreateDialog), '<li>' + result.Message + '</li>');
                                    }
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
        })
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
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        $(domElement.EditDialog).dialog('close');
                                        alert('Update employee postnatal successfully!');
                                        getPostnatal();
                                    } else {
                                        showErrorMsg($(domElement.EditDialog), '<li>' + result.Message + '</li>');
                                    }
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
        })
    }

    function initAutoComplete($dialog) {
        var $selectorName = $dialog.find('#Employee'),
                    $selectorId = $dialog.find('#EmployeeId');
        $selectorName.autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: _global.root + 'postnatal/GetMemberToAddPostnatal',
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
                        alert('Remove employee postnatal successfully!');
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

    function initPostnatal() {
        getPostnatal();
        initAddEvent();
    }

    return {
        initPostnatal
    }
})(jQuery);

$(document).ready(function () {
    postnatal.initPostnatal();
})