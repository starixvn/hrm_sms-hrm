import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';
import toastr from 'toastr';
import { stringify } from 'qs';

import { axiosError, confirm, resolveClientUrl, toastOptions } from '../../shared/common.functions';

const externalGroup = (function ($) {
    function validateTeamName(teamName) {
        let isValid = true;
        if (team.length < 3 || teamName.length > 100) {
            $('.validation-summary-errors').html('Length of external group must be 3 between and 100.').show();
            isValid = false;
        }

        return isValid;
    }

    function registerEventCreate() {
        $('#btnCreate').on('click', function () {
            $('#TeamName').val('');
            $('.validation-summary-errors').html('').hide();

            $('#dialog-form').dialog({
                width: 400,
                modal: true,
                title: 'Add new external group',
                buttons: [
                    {
                        text: 'Save',
                        class: 'btn btn-primary',
                        click: function () {
                            const teamName = $('#TeamName').val().trim();
                            if (!validateTeamName(teamName)) {
                                return;
                            }

                            axios({
                                url: resolveClientUrl('ExtensionNo/CreateExternalGroup'),
                                method: 'POST',
                                data: stringify({ teamName }),
                                headers: {
                                    'X-Requested-With': 'XMLHttpRequest',
                                    'Content-Type': 'application/x-www-form-urlencoded'
                                }
                            }).then(response => {
                                let data = response.data;
                                if (data.isSuccessful) {
                                    location.reload();
                                } else {
                                    toastr.error(data.message, '', toastOptions());
                                }
                            }).catch(axiosError);
                        }
                    },
                    {
                        text: 'Cancel',
                        class: 'btn btn-default',
                        click: function () {
                            $(this).dialog('close');
                        }
                    }
                ]
            });
        });
    }

    function registerEventEdit() {
        $('table#data a.edit').on('click', function () {
            const teamId = $(this).data('id'),
                name = $(this).data('name');
            $('#TeamName').val(name);
            $('.validation-summary-errors').html('').hide();

            $('#dialog-form').dialog({
                width: 400,
                modal: true,
                title: 'Edit external group',
                buttons: [
                    {
                        text: 'Save',
                        class: 'btn btn-primary',
                        click: function () {
                            const teamName = $('#TeamName').val().trim();
                            if (!validateTeamName(teamName)) {
                                return;
                            }

                            axios({
                                url: resolveClientUrl('ExtensionNo/EditExternalGroup'),
                                method: 'POST',
                                data: stringify({ teamId, teamName }),
                                headers: {
                                    'X-Requested-With': 'XMLHttpRequest',
                                    'Content-Type': 'application/x-www-form-urlencoded'
                                }
                            }).then(response => {
                                let data = response.data;
                                if (data.isSuccessful) {
                                    location.reload();
                                } else {
                                    toastr.error(data.message, '', toastOptions());
                                }
                            }).catch(axiosError);
                        }
                    },
                    {
                        text: 'Cancel',
                        class: 'btn btn-default',
                        click: function () {
                            $(this).dialog('close');
                        }
                    }
                ]
            });
        });
    }

    function registerEventDelete() {
        $('table#data a.delete').on('click', function () {
            const id = $(this).data('id');
            confirm('Are you sure you want to delete this external group?', function () {
                axios({
                    url: resolveClientUrl('ExtensionNo/DeleteExternalGroup' + '/' + id),
                    method: 'POST',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'Content-Type': 'application/x-www-form-urlencoded'
                    }
                }).then(response => {
                    let data = response.data;
                    if (data.isSuccessful) {
                        location.reload();
                    } else {
                        toastr.error(data.message, '', toastOptions());
                    }
                }).catch(axiosError);
            });
        });
    }

    function initModule() {
        registerEventCreate();
        registerEventEdit();
        registerEventDelete();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    externalGroup.initModule();
});