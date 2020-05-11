import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-contextmenu/dist/jquery.contextMenu';
import '../../../libs/monthPicker/monPickr-0.0.3';
import toastr from 'toastr';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError, floatThead, toastOptions, resolveClientUrl, blockUI, unblockUI } from '../../shared/common.functions';
import { RequestCommand, CurrentRequestStatus, RoleSystem } from '../../shared/constants';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';

var sorts = [];

function query() {
    blockUI();
    axios({
        url: resolveClientUrl('overtime/ListEmployeeOvertime'),
        method: 'POST',
        data: stringify({
            employeeRequest: employeeRequests,
            sort: JSON.stringify(sorts)
        }),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    }).then(response => {
        $('#listEmployeeOvertime').html(response.data);
        floatThead('table#data');
    }).catch(axiosError).then(() => unblockUI());
}

function eventSorting() {
    $('th.col-sorter').on('click', function () {
        var $this = $(this).find('div.fa'),
            property = $this.attr('data-property'),
            direction, index;

        index = sorts.map(function (e) { return e.Property; }).indexOf(property);

        if ($this.hasClass('fa-sort-desc')) {
            $this.removeClass('fa-sort-desc').addClass('fa-sort-asc');
            direction = 'asc';
        } else if ($this.hasClass('fa-sort-asc')) {
            $this.removeClass('fa-sort-asc').addClass('fa-sort-desc');
            direction = 'desc';
        } else {
            refreshSorting();
            $this.addClass('fa-sort-desc');
            direction = 'desc';
        }

        if (index !== -1) {
            sorts[index].Direction = direction;
        } else {
            sorts.shift();
            sorts.push({ Property: property, Direction: direction });
        }

        query();
    });
}

function refreshSorting() {
    $('th.col-sorter').each(function () {
        $(this).find('div.fa').removeClass('fa-sort-asc').removeClass('fa-sort-desc');
    });
}

    $(document).ready(function () {

        eventSorting();

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

    axios(resolveClientUrl('Department/GetHierarchy'))
        .then(response => {
            $('#Section').multiSelectTreeView({
                data: response.data,
                selectedItems: $('#SectionIds').val()
            });
        }).catch(axiosError);

    floatThead('table#data');

    $('#btnSearch').on('click', function () {
        const statusId = parseInt($('#StatusId').val(), 10),
            month = $('#Month').val();

        if ((statusId === 0 || statusId === CurrentRequestStatus.Processed) && month === '') {
            toastr.error('Please select a month to search.', '', toastOptions());
        } else {
            $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
            $('form.filter').submit();
        }
    });

    $('#ckbAll').on('change', function (event) {
        $table.find('tbody input[type=checkbox]').prop('checked', event.target.checked);
    });

    let onUpdateRequestStatus = function (requestIds, command) {
        axios({
            url: resolveClientUrl('overtime/EditMonthlyRequestStatus'),
            method: 'POST',
            data: stringify({ requestIds, command, roleId: RoleSystem.HR }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;
            if (data.IsSuccessful) {
                toastr.success('Updated successfully!', '', toastOptions());
                setTimeout(function () { $('#btnSearch').trigger('click'); }, 1000);
            } else {
                toastr.error(data.Message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    let checkInvalidOvertime = function (requestIds) {
        axios({
            url: resolveClientUrl('overtime/CheckInvalidOvertime'),
            method: 'POST',
            data: stringify({ requestIds: requestIds }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let isInvalid = response.data;
            if (!isInvalid) {
                onUpdateRequestStatus(requestIds, RequestCommand.Process);
            } else {
                $confirmModal.find('.modal-body')
                    .html('There are some overtimes are invalid. Are you sure you want to <strong>Process</strong> this request(s)?');
                $confirmModal.modal('show');
                $btnConfirm.off('click');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestIds, RequestCommand.Process);
                });
            }
        }).catch(axiosError);
    };

    $('#btnProcess').on('click', function () {
        let objSelected = $table.find('tbody input[type=checkbox]:checked');
        if (objSelected.length > 0) {
            let requestIds = [];
            for (let i = 0; i < objSelected.length; i++) {
                let $obj = $(objSelected[i]);
                if (parseInt($obj.data('statusid'), 10) === CurrentRequestStatus.Approved) {
                    requestIds.push($obj.data('requestid'));
                } else {
                    toastr.error('The status of some requests is not approved.', '', toastOptions());
                    return;
                }
            }

            if (requestIds.length > 0) {
                $btnConfirm.off('click');
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Process</strong> this request(s)?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    $confirmModal.modal('hide');
                    checkInvalidOvertime(requestIds.join(','));
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
                if (parseInt($obj.data('statusid'), 10) === CurrentRequestStatus.Void) {
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
                    onUpdateRequestStatus(requestIds.join(','), RequestCommand.Void);
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
                url: resolveClientUrl('overtime/ReminderClaimOvertime'),
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

    let renderIconOk = function (opt, $itemElement, itemKey, item) {
        $itemElement.html('<span class="processed"><i class="fa fa-check"></i>' + item.name + '</span>');
        return '';
    };
    let renderIconRemove = function (opt, $itemElement, itemKey, item) {
        $itemElement.html('<span class="void"><i class="fa fa-times"></i>' + item.name + '</span>');
        return '';
    };
    let processCommand = function (command, options) {
        const eventElement = options.$trigger,
            requestId = parseInt(eventElement.attr('requestid'), 10);
        switch (command) {
            case 'Process':
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Process</strong> this request?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    checkInvalidOvertime(requestId);
                });
                break;
            case 'Void':
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Void</strong> this request?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestId, RequestCommand.Void);
                });
                break;
            case 'Reset':
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Reset</strong> this request?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestId, RequestCommand.Reset);
                });
                break;
        }
    };
    let bindMenu = function (element, menu) {
        let listItem = {};
        switch (menu) {
            case 'hrMenu':
                listItem = {
                    'Process': { name: 'Process', icon: renderIconOk },
                    'Void': { name: 'Void', icon: renderIconRemove }
                };
                break;
            case 'resetMenu':
                listItem = {
                    'Reset': { name: 'Reset', icon: renderIconRemove }
                };
                break;
        }

        $.contextMenu({
            selector: element,
            trigger: 'left',
            callback: processCommand,
            items: listItem
        });
    };

    bindMenu('.hr-pending', 'hrMenu');
    bindMenu('.hr-processed', 'resetMenu');
    bindMenu('.hr-void', 'resetMenu');
});