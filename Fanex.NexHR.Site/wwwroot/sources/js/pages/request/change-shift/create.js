import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery.are-you-sure/jquery.are-you-sure';
import toastr from 'toastr';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';

import {
    axiosError, blockUI, unblockUI, initDatepicker,
    showValidationSummaryErrors, hideValidationSummaryErrors, toastOptions
} from '../../../shared/common.functions';
import { RequestType, RoleSystem, ChangeShiftType, OffDayShiftId } from '../../../shared/constants';

$(document).ready(function () {
    let domElements = {
        $employeeId: $('#EmployeeId'),
        $typeId: $('#TypeId'),
        $fromDate: $('#FromDate'),
        $toDate: $('#ToDate'),
        $verifierId: $('#VerifierId'),
        $approverId: $('#ApproverId'),
        $remark: $('#Remark'),
        $btnSubmit: $('#btnSubmit')
    };
    let getDataAfterChangeEmployee = function () {
        const employeeId = $('#EmployeeId').val();
        const promises = [
            axios(_global.root + 'request/GetVerifiers', {
                params: {
                    employeeId: employeeId,
                    requestType: RequestType.ChangeShift
                }
            }),
            axios(_global.root + 'request/GetApprovers', {
                params: {
                    employeeId: employeeId,
                    requestType: RequestType.ChangeShift
                }
            })
        ];

        Promise.all(promises)
            .then(([verifierResponse, approverResponse]) => {
                const verifiers = verifierResponse.data,
                    approvers = approverResponse.data;
                let options = '';
                for (let i = 0; i < verifiers.length; i++) {
                    options += "<option value='" + verifiers[i].OwnerId + "'>" + verifiers[i].NickName + " (" + verifiers[i].FullName + ")</option>";
                }
                domElements.$verifierId.html(options);

                options = '';
                for (var i = 0; i < approvers.length; i++) {
                    options += "<option value='" + approvers[i].OwnerId + "'>" + approvers[i].NickName + " (" + approvers[i].FullName + ")</option>";
                }
                domElements.$approverId.html(options);
            }).catch(axiosError);
    };
    let getChangeShiftDetails = function () {
        blockUI();
        axios({
            url: _global.root + 'changeshift/GetChangeShiftDetail',
            method: 'GET',
            params: {
                employeeId: domElements.$employeeId.val(),
                fromDate: domElements.$fromDate.val(),
                toDate: domElements.$toDate.val(),
                typeId: domElements.$typeId.val()
            },
            responseType: 'text'
        }).then(response => {
            unblockUI();
            $('#changeShiftDetails').html(response.data);
            initDatepicker();
            $('#tblchangeshift input.datepicker').off('change').on('change', function () {
                $('#tblchangeshift input.datepicker').each(function () {
                    let disabledDate = $(this).attr('original-disabledDate');
                    $('#tblchangeshift input.datepicker').not(this).each(function () {
                        if ($(this).val) {
                            disabledDate = disabledDate + ',' + moment($(this).val()).format('YYYY/MM/DD');
                        }
                    });

                    $(this).attr('disabledDate', disabledDate);
                });
            });

            if ($('#tblchangeshift tbody tr').length > 0) {
                domElements.$btnSubmit.prop('disabled', false);
            } else {
                domElements.$btnSubmit.prop('disabled', true);
            }
        }).catch(axiosError);
    };
    let validateForm = function () {
        let isValid = true,
            errorMessages = [];
        const employeeId = parseInt(domElements.$employeeId.val(), 10),
            typeId = parseInt(domElements.$typeId.val(), 10),
            fromDate = moment(domElements.$fromDate.val()),
            toDate = moment(domElements.$toDate.val()),
            approverId = domElements.$approverId.val(),
            verifierId = domElements.$verifierId.val();

        if (employeeId === 0) {
            errorMessages.push('Employee field is required.');
        }

        if (!$('#IsTopManager').prop('checked')) {
            if (verifierId === null || verifierId === '') {
                errorMessages.push('Verified by field is required.');
            }
            if (approverId === null || approverId === '') {
                errorMessages.push('Approved by field is required.');
            }
        }

        if (!toDate.isSameOrAfter(fromDate)) {
            errorMessages.push('To Date must be greater than or equal From Date.');
        }

        if ($.trim(domElements.$remark.val()).length === 0) {
            errorMessages.push('Remark field is required.');
        }

        if (typeId === ChangeShiftType.MoveOffDay) {
            const moveToDates = $('#tblchangeshift input.datepicker').map(function () {
                if (!$(this).val()) { return $(this).val(); }
            });

            if (moveToDates.length > 0) {
                errorMessages.push('Move To Date field is required.');
            }
        }

        if (errorMessages.length > 0) {
            showValidationSummaryErrors(errorMessages);
            isValid = false;
        }

        return isValid;
    };

    initDatepicker();
    getChangeShiftDetails();

    $('#Employee').autocomplete({
        source: function (request, response) {
            axios({
                url: _global.root + 'request/GetEmployeesByActingUser',
                method: 'GET',
                params: {
                    role: RoleSystem.OnBehalfRequest,
                    requestType: RequestType.ChangeShift,
                    query: request.term
                }
            }).then(rs => {
                response($.map(rs.data, function (item) {
                    return {
                        label: item.NickName + ' (' + item.FullName + ')',
                        value: item.OwnerId,
                        isTopManager: item.IsTopManager
                    };
                }));
            }).catch(axiosError);
        },
        minLength: 2,
        select: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmployeeId').val(ui.item.value);
            $('#IsTopManager').prop('checked', ui.item.isTopManager);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmployeeId').val(ui.item.value);
            $('#IsTopManager').prop('checked', ui.item.isTopManager);
        },
        change: function (event, ui) {
            event.preventDefault();
            if (ui && ui.item) {
                $('#Employee').val(ui.item.label);
                $('#EmployeeId').val(ui.item.value);
                $('#IsTopManager').prop('checked', ui.item.isTopManager);
                getDataAfterChangeEmployee();
                domElements.$fromDate.trigger('change');
            } else {
                $('#EmployeeId').val('0');
            }
        }
    });

    domElements.$typeId.on('change', function () {
        getChangeShiftDetails();
    });
    domElements.$fromDate.on('change', function () {
        getChangeShiftDetails();
    });
    domElements.$toDate.on('change', function () {
        getChangeShiftDetails();
    });

    domElements.$btnSubmit.on('click', function () {
        hideValidationSummaryErrors(false);
        const typeId = parseInt(domElements.$typeId.val(), 10);
        let details = [];
        $('#tblchangeshift tbody tr').each(function () {
            let $tr = $(this);
            details.push({
                onDate: $tr.find('.onDate').val(),
                fromShiftId: $tr.find('.fromShiftId').val(),
                toShiftId: typeId === ChangeShiftType.ChangeShift ? $tr.find('.toShiftId').val() : OffDayShiftId,
                remark: $.trim($tr.find('.remark').val()),
                toDate: $tr.find('.moveToDate').val()
            });
        });
        const model = {
            employeeId: domElements.$employeeId.val(),
            typeId: typeId,
            fromDate: domElements.$fromDate.val(),
            toDate: domElements.$toDate.val(),
            remark: $.trim(domElements.$remark.val()),
            verifierId: domElements.$verifierId.val(),
            approverId: domElements.$approverId.val(),
            details: details
        };

        const isValid = validateForm();
        if (isValid) {
            axios({
                url: _global.root + 'ChangeShift/Create',
                method: 'POST',
                data: stringify({ model }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;
                if (data.isSuccessful) {
                    $('form').removeClass('dirty');
                    domElements.$btnSubmit.prop('disabled', true);
                    toastr.success('Create successfully!', '', toastOptions());
                    setTimeout(function () { location.href = _global.root + 'changeshift/employeeview'; }, 3000);
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        }
    });

    $('form').areYouSure();
});