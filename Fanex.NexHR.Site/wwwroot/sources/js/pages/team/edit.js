import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import 'jquery-ui-bundle/jquery-ui';
import 'chosen-js/chosen.jquery';

import { resolveClientUrl } from '../../shared/common.functions';

(function () {
    function initSearchUnitHead() {
        $('#UnitHead').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: resolveClientUrl('employee/getemployees'),
                    data: {
                        query: request.term,
                        enable: true,
                        isDelete: false,
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
                $('#UnitHead').val(ui.item.label);
                $('#HeadId').val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                $('#UnitHead').val(ui.item.label);
                $('#HeadId').val(ui.item.value);
            },
            change: function (event, ui) {
                event.preventDefault();
                if (ui && ui.item) {
                    $('#UnitHead').val(ui.item.label);
                    $('#HeadId').val(ui.item.value);
                } else {
                    if ($('#UnitHead').val() === '') {
                        $('#HeadId').val('');
                    } else {
                        $('#HeadId').val('0');
                    }
                }
            }
        });
    }

    function initSearchManager() {
        const assistIds = $('#ManagerIds').val();
        if (assistIds) {
            $('#Managers').val(assistIds.split(','));
        }
        $('#Managers').chosen();
        $('#Managers').on('change', function () {
            const values = $(this).val(),
                managerIds = values ? values.join(',') : '';
            $('#ManagerIds').val(managerIds);
        });
    }

    function initSearchSupervisor() {
        const assistIds = $('#SupervisorIds').val();
        if (assistIds) {
            $('#Supervisors').val(assistIds.split(','));
        }
        $('#Supervisors').chosen();
        $('#Supervisors').on('change', function () {
            const values = $(this).val(),
                supervisorIds = values ? values.join(',') : '';
            $('#SupervisorIds').val(supervisorIds);
        });
    }

    function initSearchLeader() {
        const leaderIds = $('#LeaderIds').val();
        if (leaderIds) {
            $('#Leaders').val(leaderIds.split(','));
        }
        $('#Leaders').chosen();

        $('#Leaders').on('change', function () {
            const values = $(this).val(),
                leaderIds = values ? values.join(',') : '';
            $('#LeaderIds').val(leaderIds);
        });
    }

    $(document).ready(function () {
        initSearchUnitHead();
        initSearchManager();
        initSearchSupervisor();
        initSearchLeader();
    });
})();