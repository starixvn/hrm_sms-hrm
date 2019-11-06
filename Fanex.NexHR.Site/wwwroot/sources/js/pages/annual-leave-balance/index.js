import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'floatthead/dist/jquery.floatThead';
import { axiosError, blockUI, floatThead, unblockUI, confirm, alert, initDatepicker } from '../../shared/common.functions';
import '../../date';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import '../../date';

var _ids;
var _checked = false;

var annualLeaveBalance = (function($) {
    function getDepartment() {
        $.ajax({
            url: _global.root + 'Department/GetHierarchy',
            success: function (data) {
                $('#Section').multiSelectTreeView({
                    data: data,
                    selectedItems: $('#SectionIds').val()
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    }

    function submitData(postToUrl) {
        var sectionIds = $('#Section').multiSelectTreeView('getSelectedItems');
        var url = _global.root + postToUrl + $('#Year').val() +
            '&SectionIds=' + sectionIds + '&Employee=' + $('#Employee').val() +
            '&EmployeeId=' + $('#EmployeeId').val() +
            '&MinAnnualBalance=' + $('#MinAnnualBalance').val() +
            '&Category=' + $('#Category').val();

        return url;
    }

    function initSubmitFilterEvent() {
        $('.filter form').submit(function (e) {
            e.preventDefault();
            var postToUrl = 'AnnualLeaveBalance/index?year=';
            var url = submitData(postToUrl);
            location.href = url;
        });
    }

    function initExportExcelEvent() {
        $('#btnExcel').click(function () {
            var postToUrl = 'ExportLeaveSummary/AnnualLeaveSummary?year=';
            var url = submitData(postToUrl);
            location.href = url;
        });
    }

    function initCheckAllEvent() {
        $('#checkuncheckall').on('click', function () {
            _checked = !_checked;
            $('.aba').prop('checked', _checked);
            $('.checkuncheckall').prop('checked', _checked);
            if (_checked) {
                $('tbody tr').addClass('selected');
            }
            else {
                $('tbody tr').removeClass('selected');
            }
        });
    }

    function initCheckSingleRowEvent() {
         $('.aba').click(function () {
            var tr = $(this).closest('tr');
            if (tr.hasClass('selected')) {
                tr.removeClass();
            }
            else {
                tr.addClass('selected');
            }
        });
    }

    function initAdjustmentBtnEvent() {
            $('#addadj').click(function () {
                _ids = [];
                $('.aba').each(function (index, item) {
                    var checked = $(item).prop('checked');
                    if (checked) {
                        _ids.push($(item).attr('data-id'));
                    }
                });

            $('#EmpIds').val(_ids);

            if (_ids.length > 0) {
                handleInsertForm();
            }
            else {
                alert('Please choose at least one employee to continue!');
            }
        });
    }

    function handleInsertForm() {
        $('#insertform').dialog({
            modal: true,
            width: 460,
            open: function () {
                $('#TotalDays').val('');
            },
            buttons: [
                {
                text: 'Submit',
                class: 'btn btn-primary',
                click: function () {
                    var $form = $(this);
                    if (validateInputInsertForm($form)) {
                        removeMsg($form);
                        var annualLeaveMuiltiple = {
                            EmpIds: $("#EmpIds").val(),
                            Description: $("#Description").val(),
                            AffectedFrom: $("#AffectedFrom").val(),
                            TotalDays: parseInt($("#TotalDays").val()),
                            LocalLeave: $("#LocalLeave").val()
                        };
                        var annualJsonFormat = JSON.stringify(annualLeaveMuiltiple);
                        $.ajax({
                            type: 'POST',
                            url: _global.root + 'AnnualLeaveBalance/Adjust',
                            data: { annualLeaveJson: annualJsonFormat },
                            dataType: 'json',
                            success: function (response) {
                                if (response.IsSuccessful) {
                                    alert('Adjust successful');
                                    setInterval(function() {
                                            document.location = _global.root + 'annualleavebalance/index';
                                        }, 2000)
                                } else {
                                    showErrorMsg($form, response.Message);
                                }
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
            dialogClass: 'modal-shadow'
        });
    }

    function initCloseBalanceButtonEvent() {
            $('#resetblc').click(function () {
            $('#resetform').dialog({
                modal: true,
                width: 410,
                open: function () {
                    removeMsg($(this));
                },
                buttons: [{
                        text: 'Submit',
                        class: 'btn btn-primary',
                        click: function () {
                            var $form = $(this).find('form');
                            if (validateInputResetForm($form)) {
                                removeMsg($form);
                                $.post(_global.root + 'AnnualLeaveBalance/Reset', $form.serialize(), function (response) {
                                    if (response.IsSuccessful) {
                                        alert('Close balance successful');
                                        $('#resetform').dialog('close');
                                    }
                                    else
                                    {
                                        alert(response.Message);
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
                    }],
                dialogClass: 'modal-shadow'
            });
        });
    }

    function validateInputInsertForm($form) {
        var check = true;
        var errorString = '';
        var totalDays = parseInt($form.find('input[name="TotalDays"]').val());

        if (totalDays == null || totalDays === "" || isNaN(totalDays)) {
                errorString = "* The Total days field is required.";
            }

            if (errorString.length > 0) {
                showErrorMsg($form, errorString);
                return false;
            }

            return check;
    }

    function validateInputResetForm($form) {
        var check = true;
        var errorString = '';
        var totalDays = parseInt($form.find('input[name="MinBalance"]').val());

        if (totalDays == null || totalDays === "" || isNaN(totalDays)) {
                errorString = "* The Min balance field is required.";
        }

        if (errorString.length > 0) {
            showErrorMsg($form, errorString);
            return false;
        }

        return check;
    }

    function removeMsg($dialog) {
        $dialog.find('.validation-summary-valid')
            .hide().removeClass('success').removeClass('validation-summary-errors').find('ul:first').html("");
    }

    function showErrorMsg($dialog, msg) {
        removeMsg($dialog);
        $dialog.find('.validation-summary-valid')
            .show().addClass('validation-summary-errors').find('ul:first').append(msg);
    }

    function init() {
        getDepartment();
        initDatepicker();
        initSubmitFilterEvent();
        initExportExcelEvent();
        initCheckAllEvent();
        initCheckSingleRowEvent();
        initAdjustmentBtnEvent();
        initCloseBalanceButtonEvent();
    }

    return {
        init
    };
})(jQuery);

$(document).ready(function () {
    annualLeaveBalance.init();
});