import 'jquery-ui-bundle/jquery-ui';
import 'bootstrap/dist/js/bootstrap';
import 'floatthead/dist/jquery.floatThead';
import { blockUI, floatThead, unblockUI, confirm, alert, resolveClientUrl } from '../../shared/common.functions';

const jobLevelModule = (function ($) {
    const domElement = {
        Table: '#data',
        CreateDialog: '#create-dialog',
        EditDialog: '#edit-dialog',
        BtnCreate: '#btnNewJobLevel',
        EditJobLevel: '.edit-jobLevel',
        DelJobLevel: '.delete-jobLevel'
    };


    function initButtonCreateEvent() {
        $(domElement.BtnCreate).on('click', function () {
            removeMsg($(domElement.CreateDialog));
            $(domElement.CreateDialog).dialog({
                modal: true,
                title: 'Add Job Level',
                buttons: [
                    {
                        text: 'Submit',
                        class: 'btn btn-primary',
                        click: function () {
                            onCreateJobLevel();
                        }
                    },
                    {
                        text: 'Cancel',
                        class: 'btn btn-default',
                        click: function () {
                            $(this).dialog('close');
                        }
                    }
                ],
                minWidth: 425,
                dialogClass: 'modal-shadow'
            });
        });
    }

    function onCreateJobLevel() {
        const data = {
            name: $(domElement.CreateDialog).find('input[name="name"]').val(),
        };

        if (validate($(domElement.CreateDialog))) {
            $.ajax({
                method: 'POST',
                url: resolveClientUrl('JobLevel/Insert'),
                data: data,
                success: function (response) {
                    if (response.IsSuccessful) {
                        $(domElement.CreateDialog).dialog("close");
                        alert('Create successfully!', getJobLevel);
                    } else {
                        alert(response.Message);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(textStatus + ': ' + errorThrown);
                }
            });
        }
    }

    function validate($form) {
        let isValid = true,
            errorMsg = '',
            jobLevel = $form.find('input[name="name"]').val().trim();
        if (!jobLevel) {
            errorMsg += '<li>Job Level is required.</li>';
        }

        if (errorMsg.length > 0) {
            showErrorMsg($form, errorMsg);
            isValid = false;
        }

        return isValid;
    }

    function showErrorMsg($dialog, msg) {
        removeMsg($dialog);
        $dialog.find('.validation-summary-valid')
            .show().addClass('validation-summary-errors').find('ul:first').append(msg);
    }

    function removeMsg($dialog) {
        $dialog.find('.validation-summary-valid')
            .hide().removeClass('success').removeClass('validation-summary-errors').find('ul:first').html('');
    }

    function initEditEvent() {
        $(domElement.EditJobLevel).on('click', function () {
            removeMsg($(domElement.EditDialog));
            const jobLevelId = $(this).attr('data-id'),
                jobLevel = $(this).attr('data-joblevel-name');
            $(domElement.EditDialog).find('input[name="name"]').val(jobLevel);

            $(domElement.EditDialog).dialog({
                modal: true,
                title: 'Edit Job Level',
                buttons: [
                    {
                        text: 'Submit',
                        class: 'btn btn-primary',
                        click: function () {
                            const newJobLevel = $(domElement.EditDialog).find('input[name="name"]').val();
                            onEditJobLevel(jobLevelId, newJobLevel);
                        }
                    },
                    {
                        text: 'Cancel',
                        class: 'btn btn-default',
                        click: function () {
                            $(this).dialog('close');
                        }
                    }
                ],
                minWidth: 425,
                dialogClass: 'modal-shadow'
            });
        });
    }

    function onEditJobLevel(jobLevelId, jobLevel) {
        const data = {
            id: jobLevelId,
            name: jobLevel
        };

        if (validate($(domElement.EditDialog))) {
            $.ajax({
                method: 'POST',
                url: resolveClientUrl('JobLevel/Update'),
                data: data,
                success: function (response) {
                    if (response.IsSuccessful) {
                        $(domElement.EditDialog).dialog("close");
                        alert('Edit successfully!', getJobLevel);
                    } else {
                        alert(response.Message);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(textStatus + ': ' + errorThrown);
                }
            });
        }
    }

    function initDeleteEvent() {
        $(domElement.DelJobLevel).on('click', function () {
            const jobLevelId = $(this).attr("data-id");
            confirm('Are you sure you want to delete?', function () {
                $.ajax({
                    method: 'POST',
                    url: resolveClientUrl('JobLevel/Delete'),
                    data: {
                        id: jobLevelId
                    },
                    success: function (response) {
                        if (response.IsSuccessful) {
                            alert('Delete successfully!');
                            getJobLevel();
                        } else {
                            alert(response.Message);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert(textStatus + ': ' + errorThrown);
                    }
                });
            });
        });
    }

    function getJobLevel() {
        $.ajax({
            type: 'POST',
            url: resolveClientUrl('jobLevel/GetJobLevel'),
            dataType: 'html',
            data: {},
            beforeSend: function () {
                blockUI();
            },
            success: function (data) {
                $(domElement.Table).html(data);
                floatThead('table');
                initEditEvent();
                initDeleteEvent();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            },
            complete: function () {
                unblockUI();
            }
        });
    }

    function initModule() {
        getJobLevel();
        initButtonCreateEvent();
    }

    return {
        initModule
    };
})(jQuery);

$(document).ready(function () {
    jobLevelModule.initModule();
});