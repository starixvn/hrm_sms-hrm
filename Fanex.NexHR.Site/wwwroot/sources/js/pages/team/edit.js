import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import 'jquery-ui-bundle/jquery-ui';
import 'chosen-js/chosen.jquery';

import { resolveClientUrl } from '../../shared/common.functions';

(function () {
    function initSearchSectionHead() {
        $('#SectionHead').autocomplete({
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
                $('#SectionHead').val(ui.item.label);
                $('#HeadId').val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                $('#SectionHead').val(ui.item.label);
                $('#HeadId').val(ui.item.value);
            },
            change: function (event, ui) {
                event.preventDefault();
                if (ui && ui.item) {
                    $('#SectionHead').val(ui.item.label);
                    $('#HeadId').val(ui.item.value);
                } else {
                    if ($('#SectionHead').val() === '') {
                        $('#HeadId').val('');
                    } else {
                        $('#HeadId').val('0');
                    }
                }
            }
        });
    }

    function initSearchAssistantHead() {
        const assistIds = $('#AssistantHeadIds').val();
        if (assistIds) {
            $('#AssistantHeads').val(assistIds.split(','));
        }
        $('#AssistantHeads').chosen();
        $('#AssistantHeads').on('change', function () {
            const values = $(this).val(),
                assistantHeadIds = values ? values.join(',') : '';
            $('#AssistantHeadIds').val(assistantHeadIds);
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

    function initSearchAssistantLeader() {
        const assistIds = $('#AssistantLeaderIds').val();
        if (assistIds) {
            $('#AssistantLeaders').val(assistIds.split(','));
        }
        $('#AssistantLeaders').chosen();
        $('#AssistantLeaders').on('change', function () {
            const values = $(this).val(),
                assistantLeaderIds = values ? values.join(',') : '';
            $('#AssistantLeaderIds').val(assistantLeaderIds);
        });
    }

    $(document).ready(function () {
        initSearchSectionHead();
        initSearchAssistantHead();
        initSearchLeader();
        initSearchAssistantLeader();
    });
})();