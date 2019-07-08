import $ from 'jquery/dist/jquery';
import 'floatthead/dist/jquery.floatThead';
import 'bootstrap/dist/js/bootstrap';
import 'tipsy/src/javascripts/jquery.tipsy';
import toastr from 'toastr';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError, floatThead, toastOptions } from '../../shared/common.functions';
import { CommandRequest, RoleSystem } from '../../shared/constants';

$(document).ready(function () {
    const requestId = parseInt($('#RequestId').val(), 10);
    let $confirmModal = $('#confirmModal'),
        $btnConfirm = $confirmModal.find('#btnConfirm');

    $('.tooltip-tipsy').tipsy({
        gravity: 'e'
    });

    floatThead('table#data');

    let onUpdateRequestStatus = function (requestIds, command, roleId) {
        axios({
            url: _global.root + 'overtime/EditMonthlyRequestStatus',
            method: 'POST',
            data: stringify({ requestIds, command, roleId }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;
            if (data.IsSuccessful) {
                toastr.success('Updated Successfully!', '', toastOptions());
                setTimeout(function () { window.location.reload(); }, 1000);
            } else {
                toastr.error(data.Message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    $('#btnVerify').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Verify</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, CommandRequest.Accept, RoleSystem.Verifier);
        });
    });

    $('#btnNotVerify').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Not Verify</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, CommandRequest.NotAccept, RoleSystem.Verifier);
        });
    });

    $('#btnResetByVerifier').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Reset</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, CommandRequest.Reset, RoleSystem.Verifier);
        });
    });

    $('#btnApprove').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Approve</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, CommandRequest.Approve, RoleSystem.Approver);
        });
    });

    $('#btnReject').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Reject</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, CommandRequest.Reject, RoleSystem.Approver);
        });
    });

    $('#btnResetByApprover').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Reset</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, CommandRequest.Reset, RoleSystem.Approver);
        });
    });
});