import $ from 'jquery/dist/jquery';
import 'floatthead/dist/jquery.floatThead';
import { blockUI, floatThead, unblockUI, confirm, alert, getParam } from '../../shared/common.functions';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';

var jobTitle = (function($) {
    var domElement = {
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

        $(domElement.BtnExcel).on('click', function() {
            var keyword = $(domElement.Keyword).val();
            window.location.href = "/jobTitle/ExportExcel?keyword=" + keyword;
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
        var data = {
            name: $(domElement.CreateDialog).find('input[name="name"]').val(),
            categoryId: $(domElement.CreateDialog).find('select[name="categoryId"]').val()
        };

        if (isValid($(domElement.CreateDialog))) {
            $.ajax({
                method: 'POST',
                url: _global.root + 'JobTitle/Insert',
                data: data,
                success: function (response) {
                    if (response.IsSuccessful) {
                        $(domElement.CreateDialog).dialog("close");
                        alert('Create job title successfully!', getJobtitle);
                    }
                    else {
                        alert(response.Message);
                    }
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    alert(textStatus + ': ' + errorThrown);
                }
            });
        }
    }

    function isValid($form) {
        var isValid = true,
            errorMsg = '',
            jobTitle = $form.find('input[name="name"]').val(),
            categoryId = $form.find('select[name="categoryId"]').val();
        if (!jobTitle) {
            errorMsg += '<li>The Job title field is required.</li>';
        }

        if (!categoryId && parseInt(categoryId, 10) <= 0) {
            errorMsg += '<li>The Category field is required.</li>';
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
            .hide().removeClass('success').removeClass('validation-summary-errors').find('ul:first').html("");
    }

    function initEditEvent() {
        $(domElement.EditJobtitle).on('click', function() {
            var jobId = $(this).attr('data-id'),
                categoryId = $(this).attr('data-category-id'),
                jobTitle = $(this).attr('data-job-title');
            $(domElement.EditDialog).find('input[name="name"]').val(jobTitle);
            $(domElement.EditDialog).find('input[name="categoryId"]').val(categoryId);

            $(domElement.EditDialog).dialog({
                modal: true,
                title: 'Edit an job title',
                buttons: [
                    {
                        text: 'Submit',
                        class: 'btn btn-primary',
                        click: function() {
                           onEditJobtitle(jobId, categoryId, jobTitle);
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
        var data = {
            id: jobId,
            name: jobTitle,
            categoryId: categoryId
        };

        if (isValid($(domElement.EditDialog))) {
            $.ajax({
                method: 'POST',
                url: _global.root + 'JobTitle/Update',
                data: data,
                success: function (response) {
                    if (response.IsSuccessful) {
                        $(domElement.EditDialog).dialog("close");
                        alert('Edit job title successfully!', getJobtitle);
                    }
                    else {
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
            var jobTitleId = $(this).attr("data-id");
            confirm('Are you sure you want to delete?', function () {
                $.ajax({
                    method: 'POST',
                    url: _global.root + 'JobTitle/Delete',
                    data: {
                        id: jobTitleId
                    },
                    success: function(response) {
                        if (response.IsSuccessful) {
                            alert('Delete successful !')
                            getJobtitle();
                        }
                        else {
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
        var url = _global.root + 'jobTitle/GetJobTitles';
        var keyword = $(domElement.Keyword).val();
        $.ajax({
            type: 'POST',
            url: url,
            dataType: 'html',
            data: {
                jobTitle: keyword
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

    function initJobtitle() {
        getJobtitle();
        initFilterEvent();
        initButtonCreateEvent();
    }

    return {
        initJobtitle
    }
})(jQuery);

$(document).ready(function () {
    jobTitle.initJobtitle();
});