import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { stringify } from 'qs';
import toastr from 'toastr';

import { axiosError, floatThead, toastOptions } from '../shared/common.functions';

let assignPermissionToRole = (function ($) {
    let $functionTable,
        $formFilter,
        $btnSubmit,
        attrRead = 'criteria\\.funcRead',
        attrHost = 'criteria\\.funcHost';

    function updateParent(type) {
        let $this = $(this),
            pid = $this.data('pid');

        if (pid) {
            let checked = $this.prop('checked'),
                parent = $('#func' + type + pid);

            if (checked && !parent.prop('checked')) {
                parent.prop('checked', true);
            }

            if (!checked) {
                let siblings = $('[name=criteria\\.func' + type + '][data-pid=' + parent.data('fid') + ']:checked');

                if (siblings.length === 0) {
                    parent.prop('checked', false);

                    if (type === 'Read') {
                        parent.closest('tr').find('[name=' + attrHost + ']').prop('checked', false);
                    }
                }
            }

            updateParent.call(parent.get(0), type);
        }
    }

    function registerEventCheckRoleFunc() {
        $functionTable.find('input[type=checkbox]:not(:disabled)').on('change', function () {
            let $this = $(this),
                functionId = $this.data('fid'),
                isChecked = $this.prop('checked'),
                row = $this.closest('tr');

            switch ($this.attr('name')) {
                case 'criteria.funcHost':
                    // host checked -> read checked
                    if (isChecked) {
                        updateParent.call(
                            row.find('[type=checkbox][name=' + attrRead + ']').prop('checked', true).get(0),
                            'Read');
                        $functionTable.find('[type=checkbox][name=' + attrRead + '][data-pid=' + functionId + ']:not(:disabled)')
                            .attr('checked', true).change();
                    }

                    // parent changed -> children changed
                    $functionTable.find('[type=checkbox][name=' + attrHost + '][data-pid=' + functionId + ']:not(:disabled)')
                        .prop('checked', isChecked).change();

                    // child checked -> parent checked
                    // last child unchecked -> parent unchecked
                    updateParent.call($this.get(0), 'Host');
                    break;

                case 'criteria.funcRead':
                    // read unchecked -> host unchecked
                    if (!isChecked) {
                        updateParent.call(
                            row.find('[type=checkbox][name=' + attrHost + ']:not(:disabled)').prop('checked', false).get(0),
                            'Host');
                    }

                    // parent changed -> children changed
                    $functionTable.find('[type=checkbox][name=' + attrRead + '][data-pid=' + functionId + ']:not(:disabled)')
                        .prop('checked', isChecked).change();

                    // child checked -> parent checked
                    // last child unchecked -> parent unchecked
                    updateParent.call($this.get(0), 'Read');
                    break;
            }
        });
    }

    function registerEventChangeRole() {
        $('#RoleId').on('change', function () {
            $formFilter.submit();
        });
    }

    function registerEventSubmit() {
        $btnSubmit.on('click', function () {
            let roleId = $('#RoleId').val(),
                model = [],
                els = $functionTable.find('tbody tr'),
                totalItems = els.length;

            for (let i = 0; i < totalItems; i++) {
                let el = $(els[i]),
                    functionId = el.data('fid'),
                    isRead = el.find('input[type=checkbox][name=' + attrRead + '][data-fid=' + functionId + ']:not(:disabled)').prop('checked'),
                    isHost = el.find('input[type=checkbox][name=' + attrHost + '][data-fid=' + functionId + ']:not(:disabled)').prop('checked');

                model.push({ functionId, isRead, isHost });
            }

            if (totalItems > 0) {
                axios({
                    url: _global.root + 'Function/AssignPermission',
                    method: 'POST',
                    data: stringify({ roleId: roleId, permissions: model }),
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'Content-Type': 'application/x-www-form-urlencoded'
                    }
                }).then(response => {
                    let data = response.data;

                    if (data.isSuccessful) {
                        toastr.success('Updated Successfully!', '', toastOptions());
                    } else {
                        toastr.error(data.message, '', toastOptions());
                    }
                }).catch(axiosError);
            }
        });
    }

    function registerEvents() {
        registerEventChangeRole();
        registerEventCheckRoleFunc();
        registerEventSubmit();
    }

    function initModule() {
        $functionTable = $('#functionTable');
        $formFilter = $('#formFilter');
        $btnSubmit = $('#btnSubmit');

        floatThead('#functionTable');

        registerEvents();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    assignPermissionToRole.initModule();
});