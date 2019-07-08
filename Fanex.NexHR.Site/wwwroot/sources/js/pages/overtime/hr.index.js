import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-ui-multiselect-widget/src/jquery.multiselect';
import 'floatthead/dist/jquery.floatThead';
import 'bootstrap/dist/js/bootstrap';
import '../../../libs/monthPicker/monPickr-0.0.3';
import toastr from 'toastr';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError, floatThead, toastOptions } from '../../shared/common.functions';
import { CommandRequest, CurrentRequestStatus, RoleSystem } from '../../shared/constants';
import '../../shared/employee.search.in.team';

$(document).ready(function () {
    const currentUserId = parseInt($('#CurrentUserId').val(), 10);
    let $confirmModal = $('#confirmModal'),
        $reminderMailModal = $('#reminderMailModal'),
        $btnConfirm = $confirmModal.find('#btnConfirm'),
        $monthPicker = $('#MonthControl'),
        $table = $('table#data');
    $monthPicker.monPickr({
        showDateRange: false,
        sortDesc: true,
        minDate: $('#MinDate').val(),
        maxDate: $('#MaxDate').val(),
        today: undefined,
        month: $('#Month').val() === '' ? undefined : $('#Month').val(),
        afterChangeMonth: function () {
            const month = $monthPicker.monPickr('getMonth', 'MM');
            const year = $monthPicker.monPickr('getMonth', 'yyyy');
            $('#Month').val(`${year}-${month}-01`);
        }
    });

    $('.monpickr-dropdown').hide();

    if ($('#Month').val() === '') {
        $('.monpickr-selected-month.monpickr-input.ui-widget-content').val('');
    }

    $('#Section').multiselect({
        minWidth: 180,
        checkAllText: 'Check',
        uncheckAllText: 'Uncheck'
    });

    const sectionIds = $('#SectionIds').val().split(',');
    $('#Section').val(sectionIds);
    $('#Section').multiselect('refresh');

    $('#Section').on('change', function () {
        const sectionIds = $(this).val() !== null ? $(this).val().join(',') : '';
        $('#SectionIds').val(sectionIds);
    });

    floatThead('table#data');

    $('#btnSearch').on('click', function () {
        const statusId = parseInt($('#StatusId').val(), 10),
            month = $('#Month').val();

        if (statusId === 0 && month === '') {
            toastr.error('Please select a month to search.', '', toastOptions());
        } else {
            $('form.filter').submit();
        }
    });

    $('#ckbAll').on('change', function (event) {
        $table.find('tbody input[type=checkbox]').prop('checked', event.target.checked);
    });

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
                setTimeout(function () { $('form.filter').submit(); }, 1000);
            } else {
                toastr.error(data.Message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    $('#btnProcess').on('click', function () {
        let objSelected = $table.find('tbody input[type=checkbox]:checked');
        if (objSelected.length > 0) {
            let requestIds = [];
            for (let i = 0; i < objSelected.length; i++) {
                let $obj = $(objSelected[i]);
                if ($obj.data('statusid') === CurrentRequestStatus.Approved) {
                    requestIds.push($obj.data('requestid'));
                } else {
                    toastr.error('The status of some requests is not approved.', '', toastOptions());
                    return;
                }
            }

            if (requestIds.length > 0) {
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Process</strong> this request(s)?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    $confirmModal.modal('hide');
                    axios({
                        url: _global.root + 'overtime/CheckInvalidOvertime',
                        method: 'POST',
                        data: stringify({ requestIds: requestIds.join(',') }),
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest',
                            'Content-Type': 'application/x-www-form-urlencoded'
                        }
                    }).then(response => {
                        let isValid = response.data;
                        if (isValid) {
                            onUpdateRequestStatus(requestIds.join(','), CommandRequest.Process, RoleSystem.HR);
                        } else {
                            $confirmModal.find('.modal-body')
                                .html('There are some overtimes are invalid. Are you sure you want to <strong>Process</strong> this request(s)?');
                            $confirmModal.modal('show');
                            $btnConfirm.off('click');
                            $btnConfirm.on('click', function () {
                                onUpdateRequestStatus(requestIds.join(','), CommandRequest.Process, RoleSystem.HR);
                            });
                        }
                    }).catch(axiosError);
                });
            }
        } else {
            toastr.error('Please choose at least one request to continue.', '', toastOptions());
        }
    });

    $('#btnVoid').on('click', function () {
        let objSelected = $table.find('tbody input[type=checkbox]:checked');
        if (objSelected.length > 0) {
            let requestIds = [];
            for (let i = 0; i < objSelected.length; i++) {
                let $obj = $(objSelected[i]);
                if ($obj.data('statusid') === CurrentRequestStatus.Void) {
                    requestIds.push($obj.data('requestid'));
                } else {
                    toastr.error('The status of some requests is not approved.', '', toastOptions());
                    return;
                }
            }

            if (requestIds.length > 0) {
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Void</strong> this request(s)?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestIds.join(','), CommandRequest.Void, RoleSystem.HR);
                });
            }
        } else {
            toastr.error('Please choose at least one request to continue.', '', toastOptions());
        }
    });

    $('#lkSendMail').on('click', function () {
        $reminderMailModal.modal('show');

        $('#btnSendMail').on('click', function () {
            $reminderMailModal.modal('hide');
            axios({
                url: _global.root + 'overtime/ReminderClaimOvertime',
                method: 'POST',
                data: stringify({
                    subject: $('#txtSubject').val().trim(),
                    content: $('#txtContent').val().trim()
                }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                toastr.success('Send Successfully!', '', toastOptions());
            }).catch(axiosError);
        });
    });
});