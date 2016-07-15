'use strict';
/*global _global, autoIndex, noInformation*/

(function () {
    function showSuccessMsg(msg) {
        $('#dialogAddOtherDiscipline .validation-summary-valid')
            .show()
            .addClass('success')
            .find('ul:first')
            .append('<li>' + msg + '</li>');
    }

    function showErrorMsg(msg) {
        $('#dialogAddOtherDiscipline .validation-summary-valid')
            .show()
            .addClass('validation-summary-errors')
            .find('ul:first').append(msg);
    }

    function removeMsg() {
        $('#dialogAddOtherDiscipline .validation-summary-valid')
            .hide()
            .removeClass('success')
            .removeClass('validation-summary-errors')
            .find('ul:first').html("");
    }

    function validateAddOtherDiscipline() {
        var isValid = true,
            errorString = '',
            empId = $('#dialogAddOtherDiscipline #EmployeeId').val(),
            workDate = $('#dialogAddOtherDiscipline #WorkDate').datepicker('getDate'),
            disciplineTypeId = $('#dialogAddOtherDiscipline #OtherDisciplineTypeId').val();

        if (empId && parseInt(empId, 10) <= 0) {
            errorString += '<li>The Employee field is required.</li>';
        }

        if (workDate === null || workDate === '') {
            errorString += '<li>Discipline Date field is required.</li>';
        }

        if (disciplineTypeId && parseInt(disciplineTypeId, 10) === 0) {
            errorString += '<li>Discipline Type field is required.</li>';
        }

        if (errorString.length > 0) {
            showErrorMsg(errorString);
            isValid = false;
        }

        return isValid;
    }

    function initAutocomplete() {
        var selectorName = '#dialogAddOtherDiscipline #Employee',
            selectorId = '#dialogAddOtherDiscipline #EmployeeId';
        $(selectorName).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: _global.root + 'discipline/GetMemberToAddOtherDiscipline',
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
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert(textStatus + ': ' + errorThrown);
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                $(selectorName).val(ui.item.label);
                $(selectorId).val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                $(selectorName).val(ui.item.label);
                $(selectorId).val(ui.item.value);
            },
            change: function (event, ui) {
                event.preventDefault();
                if (ui && ui.item) {
                    $(selectorName).val(ui.item.label);
                    $(selectorId).val(ui.item.value);
                } else {
                    if ($(selectorName).val() === '') {
                        $(selectorId).val('');
                    } else {
                        $(selectorId).val('0');
                    }
                }
            }
        }).autocomplete('widget').addClass('search-employee-autocomplete');
    }

    function initDatePicker() {
        var mindate = $('#dialogAddOtherDiscipline .datepicker').attr('mindate');

        $('#dialogAddOtherDiscipline .datepicker').datepicker({
            dateFormat: 'dd M, yy',
            showOn: 'button',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            buttonImage: _global.root + 'images/calendar.png'
        });

        if (mindate !== 'null') {
            mindate = mindate.split('/');
            $('#dialogAddOtherDiscipline .datepicker').datepicker('option', 'minDate', new Date(mindate[0], mindate[1] - 1, mindate[2]));
        }
    }

    function initEventForOtherDisciplineDropdown() {
        $('#dialogAddOtherDiscipline select#DisciplineTypeId').on('change', function () {
            var score = $('option:selected', this).data('score');
            $('#dialogAddOtherDiscipline #MinusScore').val(score);
        });
    }

    function initAddOtherDisciplineDialog() {
        initAutocomplete();
        initDatePicker();
        initEventForOtherDisciplineDropdown();
        $('#dialogAddOtherDiscipline select#DisciplineTypeId').change();

        $('#dialogAddOtherDiscipline').dialog({
            modal: true,
            buttons: [
                {
                    text: 'Submit',
                    click: function () {
                        removeMsg();
                        if (validateAddOtherDiscipline()) {
                            $.ajax({
                                type: 'POST',
                                url: _global.root + 'discipline/AddOtherDiscipline',
                                data: $('#frmAddOtherDiscipline').serialize(),
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        showSuccessMsg('Add other discipline successfully!');
                                    } else {
                                        showErrorMsg('<li>' + result.Message + '</li>');
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
                    click: function () {
                        $(this).dialog('close');
                    }
                }
            ],
            close: function () {
                query();
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

    function initEventForTeamDropdown() {
        $('form.form-filter #TeamId').on('change', function () {
            var teamId = $('form.form-filter #TeamId').val();

            if (teamId.length > 0) {
                $('form.form-filter #TeamName').val($('form.form-filter #TeamId option:selected').text());
            }
        });
    }

    function initEventViewOtherDiscipline() {
        $('table a.viewOtherDiscipline').on('click', function () {
            var empId = parseInt($(this).data('empid'), 10),
                nickname = $(this).data('nickname');

            if (empId > 0) {
                $.ajax({
                    type: 'GET',
                    url: _global.root + 'discipline/PartialViewOtherDiscipline',
                    dataType: 'html',
                    data: {
                        employeeId: empId,
                        fromDate: $('form.form-filter #FromDate').datepicker({ dateFormat: 'dd M, yy' }).val(),
                        toDate: $('form.form-filter #ToDate').datepicker({ dateFormat: 'dd M, yy' }).val()
                    },
                    success: function (data) {
                        $('#dialogViewOtherDiscipline').html(data);
                        initEventForDeleteOtherDiscipline();
                        $('#dialogViewOtherDiscipline').dialog({
                            modal: true,
                            title: 'Other Discipline - ' + nickname,
                            close: function () {
                                query();
                            },
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

    function initEventForViewDisciplineDetail() {
        $('table a.viewDisciplineDetail').on('click', function () {
            var empId = $(this).data('empid'),
                employee = $(this).data('employee'),
                teamId = $('form.form-filter #TeamId').val(),
                isLocal = $('form.form-filter #IsLocal').val(),
                reasonId = $('form.form-filter #ReasonId').val(),
                fromDate = $('form.form-filter #FromDate').datepicker({ dateFormat: 'dd M, yy' }).val(),
                toDate = $('form.form-filter #ToDate').datepicker({ dateFormat: 'dd M, yy' }).val();

            window.location.href = _global.root + 'discipline/DisciplineDetail?fromDate=' + fromDate + '&toDate=' + toDate
                + '&employeeId=' + empId + '&employee=' + employee + '&teamId=' + teamId
                + '&isLocal=' + isLocal + '&reasonId=' + reasonId + '&fromSummaryPage=true';
        });
    }

    function initEventForDeleteOtherDiscipline() {
        $('table a.deleteOtherDiscipline').on('click', function () {
            var empId = $(this).data('empid'),
                workdate = $(this).data('workdate'),
                disciplineTypeId = $(this).data('disciplinetype'),
                rowIndex = $(this).data('row-index');

            confirm('Are you sure you want to delete this other discipline?', function () {
                $.post(_global.root + 'discipline/DeleteOtherDiscipline', {
                    employeeId: empId,
                    workDate: workdate,
                    disciplineTypeId: disciplineTypeId
                }, function (error) {
                    if (error.length > 0) {
                        alert(error);
                        return;
                    }
                    $('#row_' + rowIndex).remove();
                    autoIndex();

                    if ($('#dialogViewOtherDiscipline table.data tbody > tr').length === 0) {
                        $(noInformation(6)).appendTo('#dialogViewOtherDiscipline table.data');
                    }
                });
            });
        });
    }

    function query() {
        $.ajax({
            url: _global.root + 'discipline/PartialListDisciplineSummary',
            data: $('form.form-filter').serialize(),
            success: function (data) {
                $('#discipline-summary').html(data);

                initEventViewOtherDiscipline();
                initEventForViewDisciplineDetail();

                $('#btnAddOtherDiscipline').on('click', function (event) {
                    event.preventDefault();

                    $.ajax({
                        type: 'GET',
                        url: _global.root + 'discipline/PartialAddOtherDiscipline',
                        dataType: 'html',
                        success: function (data) {
                            $('#dialogAddOtherDiscipline').html(data);
                            initAddOtherDisciplineDialog();
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
        $("#FromDate").inputReadonly();
        $("#ToDate").inputReadonly();

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