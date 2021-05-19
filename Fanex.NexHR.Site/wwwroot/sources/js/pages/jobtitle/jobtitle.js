import 'jquery-ui-bundle/jquery-ui';
import 'bootstrap/dist/js/bootstrap';
import 'floatthead/dist/jquery.floatThead';
import { blockUI, floatThead, unblockUI, confirm, alert, resolveClientUrl } from '../../shared/common.functions';

const jobTitleModule = (function ($) {
    const domElement = {
        Keyword: '#keyword',
        Table: '#data',
        CreateDialog: '#create-dialog',
        EditDialog: '#edit-dialog',
        BtnGetJobtitle: '#btnGetJobtitle',
        BtnExcel: '#exportExcel',
        BtnCreate: '#btnNewJobTitle',
        EditJobtitle: '.edit-jobtitle',
        DelJobtitle: '.delete-jobtitle'
    };

    function initFilterEvent() {
        $(domElement.BtnGetJobtitle).on('click', function() {
            getJobtitle();
        });

        $(domElement.BtnExcel).on('click', function () {
            const keyword = $(domElement.Keyword).val();
            window.location.href = resolveClientUrl('jobTitle/ExportExcel?keyword=' + keyword);
        });

        $(domElement.Keyword).keyup(function (event) {
            if (event.keyCode === 13) {
                getJobtitle();
            }
        });
    }

    function initButtonCreateEvent() {
        $(domElement.BtnCreate).on('click', function() {
            removeMsg($(domElement.CreateDialog));
            $(domElement.CreateDialog).find('input[name="name"]').val('');
            $(domElement.CreateDialog).dialog({
                modal: true,
                title: 'Add Job Title',
                buttons: [
                    {
                        text: 'Submit',
                        class: 'btn btn-primary',
                        click: function() {
                           onCreateJobtitle();
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

    function onCreateJobtitle() {
        const data = {
            name: $(domElement.CreateDialog).find('input[name="name"]').val(),
            categoryId: $(domElement.CreateDialog).find('select[name="categoryId"]').val()
        };

        if (validate($(domElement.CreateDialog))) {
            $.ajax({
                method: 'POST',
                url: resolveClientUrl('JobTitle/Insert'),
                data: data,
                success: function (response) {
                    if (response.IsSuccessful) {
                        $(domElement.CreateDialog).dialog("close");
                        alert('Create successfully!', getJobtitle);
                    } else {
                        alert(response.Message);
                    }
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    alert(textStatus + ': ' + errorThrown);
                }
            });
        }
    }

    function validate($form) {
        let isValid = true,
            errorMsg = '',
            jobTitle = $form.find('input[name="name"]').val(),
            categoryId = $form.find('select[name="categoryId"]').val();
        if (!jobTitle) {
            errorMsg += '<li>Job title is required.</li>';
        }

        if (!categoryId && parseInt(categoryId, 10) <= 0) {
            errorMsg += '<li>Category is required.</li>';
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
        $(domElement.EditJobtitle).on('click', function () {
            const jobId = $(this).attr('data-id'),
                categoryId = $(this).attr('data-category-id'),
                jobTitle = $(this).attr('data-job-title');
            $(domElement.EditDialog).find('input[name="name"]').val(jobTitle);
            $(domElement.EditDialog).find('select[name="category"]').val(categoryId);

            $(domElement.EditDialog).dialog({
                modal: true,
                title: 'Edit Job Title',
                buttons: [
                    {
                        text: 'Submit',
                        class: 'btn btn-primary',
                        click: function () {
                            const newJobTitle = $(domElement.EditDialog).find('input[name="name"]').val(),
                                newCategoryId = $(domElement.EditDialog).find('select[name="category"]').val();
                            onEditJobtitle(jobId, newCategoryId, newJobTitle);
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

    function onEditJobtitle(jobId, categoryId, jobTitle) {
        const data = {
            id: jobId,
            name: jobTitle,
            categoryId: categoryId
        };

        if (validate($(domElement.EditDialog))) {
            $.ajax({
                method: 'POST',
                url: resolveClientUrl('JobTitle/Update'),
                data: data,
                success: function (response) {
                    if (response.IsSuccessful) {
                        $(domElement.EditDialog).dialog("close");
                        alert('Edit successfully!', getJobtitle);
                    } else {
                        alert(response.Message);
                    }
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    alert(textStatus + ': ' + errorThrown);
                }
            });
        }
    }

    function initDeleteEvent() {
        $(domElement.DelJobtitle).on('click', function() {
            const jobTitleId = $(this).attr("data-id");
            confirm('Are you sure you want to delete?', function () {
                $.ajax({
                    method: 'POST',
                    url: resolveClientUrl('JobTitle/Delete'),
                    data: {
                        id: jobTitleId
                    },
                    success: function(response) {
                        if (response.IsSuccessful) {
                            alert('Delete successfully!');
                            getJobtitle();
                        } else {
                            alert(response.Message);
                        }
                    },
                    error: function(jqXHR, textStatus, errorThrown) {
                        alert(textStatus + ': ' + errorThrown);
                    }
                });
            });
        });
    }

    function getJobtitle() {
        $.ajax({
            type: 'POST',
            url: resolveClientUrl('jobTitle/GetJobTitles'),
            dataType: 'html',
            data: {
                jobTitle: $(domElement.Keyword).val()
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

    function initModule() {
        getJobtitle();
        initFilterEvent();
        initButtonCreateEvent();
    }

    return {
        initModule
    };
})(jQuery);

$(document).ready(function () {
    jobTitleModule.initModule();
});