import 'jquery-ui-bundle/jquery-ui';
import 'bootstrap/dist/js/bootstrap';
import 'floatthead/dist/jquery.floatThead';
import { blockUI, floatThead, unblockUI, confirm, alert, resolveClientUrl } from '../../shared/common.functions';

const jobLevel = (function ($) {
    const domElement = {
        Keyword: '#keyword',
        Table: '#data',
        CreateDialog: '#create-dialog',
        EditDialog: '#edit-dialog',
        BtnGetJobLevel: '#btnGetJobLevel',
        BtnCreate: '#btnNewJobLevel',
        EditJobLevel: '.edit-jobLevel',
        DelJobLevel: '.delete-jobLevel'
    };

    function initFilterEvent() {
        $(domElement.BtnGetJobLevel).on('click', function () {
            getJobLevel();
        });

        $(domElement.Keyword).keyup(function (event) {
            if (event.keyCode === 13) {
                getJobLevel();
            }
        });
    }

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
            isActive: $(domElement.CreateDialog).find('input[name="isActive"]').prop('checked')
        };

        if (isValid($(domElement.CreateDialog))) {
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

    function isValid($form) {
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
                isActive = $(this).attr('data-isActive'),
                jobLevel = $(this).attr('data-joblevel-name');
            $(domElement.EditDialog).find('input[name="name"]').val(jobLevel);
            document.getElementById("IsActive").checked = (isActive == 'True');

            $(domElement.EditDialog).dialog({
                modal: true,
                title: 'Edit Job Level',
                buttons: [
                    {
                        text: 'Submit',
                        class: 'btn btn-primary',
                        click: function () {
                            const newJobLevel = $(domElement.EditDialog).find('input[name="name"]').val(),
                                newIsActive = $(domElement.EditDialog).find('input[name="IsActive"]').prop('checked');
                            onEditJobLevel(jobLevelId, newJobLevel, newIsActive);
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

    function onEditJobLevel(jobLevelId, jobLevel, isActive) {
        const data = {
            id: jobLevelId,
            name: jobLevel,
            isActive: isActive
        };

        if (isValid($(domElement.EditDialog))) {
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
            data: {
                isActive: $('#Status').val()
            },
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

    function initJobLevel() {
        getJobLevel();
        initFilterEvent();
        initButtonCreateEvent();
    }

    return {
        initJobLevel
    };
})(jQuery);

$(document).ready(function () {
    jobLevel.initJobLevel();
});