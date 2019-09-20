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

$(document).ready(function () {
    let domElements = {
        $requestId: $('#RequestId'),
        $employeeId: $('#EmployeeId'),
        $typeId: $('#TypeId'),
        $fromDate: $('#FromDate'),
        $toDate: $('#ToDate'),
        $verifierId: $('#VerifierId'),
        $approverId: $('#ApproverId'),
        $remark: $('#Remark'),
        $btnSubmit: $('#btnSubmit')
    };
    let getChangeShiftDetails = function () {
        blockUI();
        axios({
            url: _global.root + 'changeshift/GetChangeShiftDetail',
            method: 'GET',
            params: {
                requestId: domElements.$requestId.val(),
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
        const fromDate = moment(domElements.$fromDate.val()),
            toDate = moment(domElements.$toDate.val()),
            approverId = domElements.$approverId.val(),
            verifierId = domElements.$verifierId.val();

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

        if (errorMessages.length > 0) {
            showValidationSummaryErrors(errorMessages);
            isValid = false;
        }

        return isValid;
    };

    initDatepicker();
    getChangeShiftDetails();

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
        let details = [];
        $('#tblchangeshift tbody tr').each(function () {
            let $tr = $(this);
            details.push({
                onDate: $tr.find('.onDate').val(),
                fromShiftId: $tr.find('.fromShiftId').val(),
                toShiftId: $tr.find('.toShiftId').val(),
                remark: $.trim($tr.find('.remark').val())
            });
        });
        const model = {
            requestId: domElements.$requestId.val(),
            employeeId: domElements.$employeeId.val(),
            typeId: domElements.$typeId.val(),
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
                url: _global.root + 'ChangeShift/Edit',
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
                    toastr.success('Edit successfully!', '', toastOptions());
                    setTimeout(function () { location.href = _global.root + 'changeshift/employeeview'; }, 3000);
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        }
    });

    $('form').areYouSure();
});