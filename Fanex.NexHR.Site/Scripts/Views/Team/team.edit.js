'use strict';
/*global _global*/

(function () {
    function initSearchLeader() {
        $("#Leader").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: _global.root + 'api/employee/getemployees',
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
                $('#Leader').val(ui.item.label);
                $('#LeaderId').val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                $('#Leader').val(ui.item.label);
                $('#LeaderId').val(ui.item.value);
            },
            change: function (event, ui) {
                event.preventDefault();
                if (ui && ui.item) {
                    $('#Leader').val(ui.item.label);
                    $('#LeaderId').val(ui.item.value);
                } else {
                    if ($('#Leader').val() === '') {
                        $('#LeaderId').val('');
                    } else {
                        $('#LeaderId').val('0');
                    }
                }
            }
        });
    }

    function initSearchAssistantLeader() {
        $("#AssistantLeader").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: _global.root + 'api/employee/getemployees',
                    data: {
                        query: request.term,
                        enable: false,
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
                $('#AssistantLeader').val(ui.item.label);
                $('#AssistantLeaderId').val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                $('#AssistantLeader').val(ui.item.label);
                $('#AssistantLeaderId').val(ui.item.value);
            },
            change: function (event, ui) {
                event.preventDefault();
                if (ui && ui.item) {
                    $('#AssistantLeader').val(ui.item.label);
                    $('#AssistantLeaderId').val(ui.item.value);
                } else {
                    if ($('#AssistantLeader').val() === '') {
                        $('#AssistantLeaderId').val('');
                    } else {
                        $('#AssistantLeaderId').val('0');
                    }
                }
            }
        });
    }

    $(document).ready(function () {
        //Validation Leader field
        $("#Leader").rules("add", {
            required: true,
            messages: { required: "The Leader field is required" }
        });

        initSearchLeader();
        initSearchAssistantLeader();
    });
})();