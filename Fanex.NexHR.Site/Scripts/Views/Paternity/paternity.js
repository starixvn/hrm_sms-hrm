'use strict';
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
        $.get(url, function (data) {
            $(domElement.Table).html(data);
            initEditEvent();
            initDeleteEvent();
            initToolTip();
        });
    }

    function initToolTip() {
        $('.tooltip').tipsy({
            gravity: $.fn.tipsy.autoNS
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
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        $(domElement.CreateDialog).dialog('close');
                                        alert('Create employee paternity successfully!');
                                        query();
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
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        $(domElement.EditDialog).dialog('close');
                                        alert('Update employee paternity successfully!');
                                        query();
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
        })
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

    function initPaternity() {
        query();
        initAddEvent();
    }

    return {
        initPaternity
    }
})(jQuery);

$(document).ready(function () {
    paternity.initPaternity();
})