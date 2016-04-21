'use strict';
/*global _global*/

(function () {
    function showSuccessMsg(msg) {
        $('#dialogAddOtherViolation .validation-summary-valid')
            .show()
            .addClass('success')
            .find('ul:first')
            .append('<li>' + msg + '</li>');
    }

    function showErrorMsg(msg) {
        $('#dialogAddOtherViolation .validation-summary-valid')
            .show()
            .addClass('validation-summary-errors')
            .find('ul:first').append(msg);
    }

    function removeMsg() {
        $('#dialogAddOtherViolation .validation-summary-valid')
            .hide()
            .removeClass('success')
            .removeClass('validation-summary-errors')
            .find('ul:first').html("");
    }

    function validateAddOtherViolation() {
        var isValid = true,
            errorString = '',
            empId = $('#dialogAddOtherViolation #EmployeeId').val(),
            workDate = $('#dialogAddOtherViolation #WorkDate').datepicker('getDate'),
            violationTypeId = $('#dialogAddOtherViolation #OtherViolationTypeId').val();

        if (empId && parseInt(empId) <= 0) {
            errorString += '<li>The Employee field is required.</li>';
        }

        if (workDate === null || workDate === '') {
            errorString += '<li>Violation Date field is required.</li>';
        }

        if (violationTypeId && parseInt(violationTypeId) === 0) {
            errorString += '<li>Violation Type field is required.</li>';
        }

        if (errorString.length > 0) {
            showErrorMsg(errorString);
            isValid = false;
        }

        return isValid;
    }

    function initAutocomplete() {
        $('#dialogAddOtherViolation #Employee').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: _global.root + 'api/employee/getemployees',
                    data: {
                        query: request.term,
                        enable: false,
                        isDelete: true,
                        $top: 10
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
                $('#dialogAddOtherViolation #Employee').val(ui.item.label);
                $('#dialogAddOtherViolation #EmployeeId').val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                $('#dialogAddOtherViolation #Employee').val(ui.item.label);
                $('#dialogAddOtherViolation #EmployeeId').val(ui.item.value);
            },
            change: function (event, ui) {
                event.preventDefault();
                $('#dialogAddOtherViolation #Employee').val(ui.item.label);
                $('#dialogAddOtherViolation #EmployeeId').val(ui.item.value);
            }
        }).autocomplete('widget').addClass('search-employee-autocomplete');
    }

    function initDatePicker() {
        var mindate = $('#dialogAddOtherViolation .datepicker').attr('mindate');

        $('#dialogAddOtherViolation .datepicker').datepicker({
            dateFormat: 'dd M, yy',
            showOn: 'button',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            buttonImage: _global.root + 'images/calendar.png'
        });

        if (mindate !== 'null') {
            mindate = mindate.split('/');
            $('#dialogAddOtherViolation .datepicker').datepicker('option', 'minDate', new Date(mindate[0], mindate[1] - 1, mindate[2]));
        }
    }

    function initEventForOtherViolationDropdown() {
        $('#dialogAddOtherViolation select#OtherViolationTypeId').on('change', function () {
            var score = $('option:selected', this).data('score');
            $('#dialogAddOtherViolation #MinusScore').val(score);
        });
    }

    function initAddOtherViolationDialog() {
        initAutocomplete();
        initDatePicker();
        initEventForOtherViolationDropdown();
        $('#dialogAddOtherViolation select#OtherViolationTypeId').change();

        $('#dialogAddOtherViolation').dialog({
            modal: true,
            buttons: [
                {
                    text: 'Submit',
                    click: function () {
                        removeMsg();
                        if (validateAddOtherViolation()) {
                            $.ajax({
                                type: 'POST',
                                url: _global.root + 'discipline/addotherviolation',
                                data: $('#frmAddOtherViolation').serialize(),
                                success: function (data) {
                                    if (data.Code === 0) {
                                        showSuccessMsg('Add other violation successful!');
                                    } else {
                                        showErrorMsg('<li>' + data.Message + '</li>');
                                    }
                                }
                            });
                        }
                    }
                }
            ],
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

    function initEventForTeamDropdown() {
        $('form.form-filter #TeamId').on('change', function () {
            var teamId = $('form.form-filter #TeamId').val();

            if (teamId.length > 0) {
                $('form.form-filter #TeamName').val($('form.form-filter #TeamId option:selected').text());
            }
        });
    }

    function initEventViewOtherViolation() {
        $('table a.viewOtherViolation').on('click', function() {
            var empId = parseInt( $(this).data('empid')),
                nickname = $(this).data('nickname');

            if (empId > 0) {
                $.ajax({
                    type: 'GET',
                    url: _global.root + 'discipline/PartialViewOtherViolation',
                    dataType: 'html',
                    data: {
                        employeeId: empId,
                        fromDate: $('form.form-filter #FromDate').datepicker({ dateFormat: 'dd M, yy' }).val(),
                        toDate: $('form.form-filter #ToDate').datepicker({ dateFormat: 'dd M, yy' }).val()
                    },
                    success: function (data) {
                        $('#dialogViewOtherViolation').html(data);

                        $('#dialogViewOtherViolation').dialog({
                            modal: true,
                            title: 'Other Violations - ' + nickname,
                            show: {
                                effect: 'fade',
                                duration: 300
                            },
                            hide: {
                                effect: 'fade',
                                duration: 200
                            },
                            minWidth: 500,
                            dialogClass: 'modal-shadow'
                        });
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert(textStatus + ': ' + errorThrown);
                    }
                });
            }
        });
    }

    function query() {
        $.ajax({
            url: _global.root + 'discipline/listdisciplinesummary',
            data: $('form.form-filter').serialize(),
            success: function (data) {
                $('#discipline-summary').html(data);
                initEventViewOtherViolation();

                $('#btnAddOtherViolation').on('click', function (event) {
                    event.preventDefault();

                    $.ajax({
                        type: 'GET',
                        url: _global.root + 'discipline/PartialAddOtherViolation',
                        dataType: 'html',
                        success: function (data) {
                            $('#dialogAddOtherViolation').html(data);
                            initAddOtherViolationDialog();
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            alert(textStatus + ': ' + errorThrown);
                        }
                    });
                });
            }
        });
    }

    $(document).ready(function () {
        initEventForTeamDropdown();

        $('form.form-filter #submit').on('click', function (event) {
            event.preventDefault();
            query();
        });

        $('form.form-filter #btnExcel').on('click', function (event) {
            event.preventDefault();

            window.location.href = _global.root + 'Export/ExportDisciplineSummaryReport?' + $('form.form-filter').serialize();
        });

        query();
    });
})();