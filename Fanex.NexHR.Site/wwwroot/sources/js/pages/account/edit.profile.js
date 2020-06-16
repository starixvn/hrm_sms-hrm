import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import toastr from 'toastr';
import { confirm, alert, toastOptions, initDatepicker, resolveClientUrl } from '../../shared/common.functions';

var editAccount = (function ($) {
    const domElement = {
        EditInformationForm: '#info-update',
        Attachments: '#Attachments',
        DeleteFile: '.delete-file-upload',
        ListFileUpload: '#upload-list',
        Relationship: '#relationship',
        BtnAddRelationship: '#btn-add-relationship',
        CreateForm: '#create-form'
    };

    const constants = {
        FileMaxSize: 5242880,
        CreateRelationshipUrl: resolveClientUrl('Information/InsertRelationship'),
        EditRelationshipUrl: resolveClientUrl('Information/EditRelationship/'),
        DeleteRelationship: resolveClientUrl('Information/DeleteRelationship/')
    };

    function updateInformationEvent() {
        $(domElement.EditInformationForm).on('submit', function (e) {
            e.preventDefault();
            removeErrorMsg($(domElement.EditInformationForm));
            $.ajax({
                url: $(domElement.EditInformationForm).attr('action'),
                method: $(domElement.EditInformationForm).attr('method'),
                data: $(domElement.EditInformationForm).serialize(),
                success: function (result) {
                    if (result.IsSuccessful) {
                        toastr.success('Updated information sucessfully!', '', toastOptions());
                    } else {
                        showErrorMsg($(domElement.EditInformationForm), result.Message);
                    }
                }
            });
        });
    }

    function initUploadFileHandler() {
        $(domElement.Attachments).on('change', function (event) {
            var files = event.originalEvent.target.files;
            if (files.length > 0 && validateFileUpload(files)) {
                uploadFiles(files);
            } else {
                toastr.error('File(s) invalid!', '', toastOptions());
            }
        });
    }

    function uploadFiles(files) {
        if (files === undefined) {
            return;
        }

        var fd = new FormData();

        for (var i = 0; i < files.length; i++) {
            var file = files[i];

            if (file.size / 1024 > 5120) {
                alert('file is to large (' + file.size / 1024 + 'kb) to upload. maximum = 5120kb');
                return;
            }

            fd.append('files[' + i + ']', file);
        }

        $.ajax({
            type: 'POST',
            url: resolveClientUrl('File/MyProfileUploadFile'),
            contentType: false,
            processData: false,
            data: fd,
            success: function (result) {
                getFileUpload();
                if (result.isSuccessful) {
                    toastr.success('File upload successfully!', '', toastOptions());
                } else {
                    toastr.error(result.message, '', toastOptions());
                    $(domElement.Attachments).val('');
                }
            },
            error: function (xhr, status, p3, p4) {
                var err = 'Error ' + ' ' + status + ' ' + p3 + ' ' + p4;
                if (xhr.responseText && xhr.responseText[0] === '{')
                    err = JSON.parse(xhr.responseText).Message;
                toastr.error(err, '', toastOptions());
            }
        });
    }

    function initDeleteFileUpload() {
        $(domElement.DeleteFile).on('click', function () {
            var $btn = $(this);
            confirm('Are you sure to want to delete this file?', function () {
                var fileName = $btn.attr('data-file-name');
                $.ajax({
                    type: 'POST',
                    url: resolveClientUrl('File/DeleteMyProfileUploadedFile'),
                    data: { fileName },
                    success: function (result) {
                        if (result.isSuccessful) {
                            toastr.success('Deleted file successfully!', '', toastOptions());
                            getFileUpload();
                        } else {
                            toastr.error(result.message, '', toastOptions());
                        }
                    },
                    error: function (xhr, status, p3, p4) {
                        var err = 'Error ' + ' ' + status + ' ' + p3 + ' ' + p4;
                        if (xhr.responseText && xhr.responseText[0] === '{')
                            err = JSON.parse(xhr.responseText).Message;
                        toastr.error(err, '', toastOptions());
                    }
                });
            });
        });
    }

    function validateFileUpload(files) {
        var isValid = true;
        for (var i = 0; i < files.length; i++) {
            if (files[i].size > constants.FileMaxSize) {
                isValid = false;
            }
            break;
        }
        return isValid;
    }

    function getFileUpload() {
        $.ajax({
            type: 'GET',
            url: resolveClientUrl('Information/GetFileUploaded'),
            contentType: 'html',
            success: function (data) {
                $(domElement.ListFileUpload).html(data);
                initUploadFileHandler();
                initDeleteFileUpload();
            }
        });
    }

    function getRelationship() {
        $.ajax({
            type: 'GET',
            url: resolveClientUrl('Information/GetEmployeeRelationship'),
            contentType: 'html',
            success: function (data) {
                $(domElement.Relationship).html(data);
                initEditEvent($(domElement.Relationship),
                    constants.EditRelationshipUrl,
                    'Edit Relationship',
                    function () { getRelationship(); });
                initDeleteEvent($(domElement.Relationship),
                    'Do you want to delete this relationship?',
                    constants.DeleteRelationship,
                    function () { getRelationship(); });
            }
        });
    }

    function initEditEvent($table, baseUrl, title, onEditSuccess) {
        $table.find('.edit-icon').on('click', function () {
            var id = $(this).data('id');
            if (id > 0) {
                $.get(baseUrl + id, function (data) {
                    $(domElement.CreateForm).html(data);
                    initDatepicker();
                    $(domElement.CreateForm).dialog({
                        title: title,
                        height: 'auto',
                        width: 'auto',
                        modal: true,
                        buttons: [
                            {
                                text: 'Submit',
                                class: 'btn btn-primary',
                                click: function () {
                                    $.ajax({
                                        method: $(domElement.CreateForm).find('form').attr('method'),
                                        url: $(domElement.CreateForm).find('form').attr('action'),
                                        data: $(domElement.CreateForm).find('form').serialize(),
                                        success: function (data) {
                                            if (data.IsSuccessful) {
                                                $(domElement.CreateForm).dialog('close');
                                                toastr.success('Updated successfully!', '', toastOptions());
                                                if (onEditSuccess) {
                                                    onEditSuccess();
                                                }
                                            } else {
                                                showErrorMsg($(domElement.CreateForm), data.Message);
                                            }
                                        }
                                    });
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
        });
    }

    function initButtonInsertEvent($btn, onCreateSuccess) {
        $btn.on('click', function () {
            var url = $(this).attr('data-url');
            var title = $(this).attr('title');
            $.get(url, function (data) {
                $(domElement.CreateForm).html(data);
                initDatepicker();
                $(domElement.CreateForm).dialog({
                    title: title,
                    height: 'auto',
                    width: 'auto',
                    modal: true,
                    buttons: [
                        {
                            text: 'Submit',
                            class: 'btn btn-primary',
                            click: function () {
                                $.ajax({
                                    method: $(domElement.CreateForm).find('form').attr('method'),
                                    url: $(domElement.CreateForm).find('form').attr('action'),
                                    data: $(domElement.CreateForm).find('form').serialize(),
                                    success: function (data) {
                                        if (data.IsSuccessful) {
                                            $(domElement.CreateForm).dialog('close');
                                            if (onCreateSuccess) {
                                                onCreateSuccess();
                                            }
                                            toastr.success('Created successfully!', '', toastOptions());
                                        } else {
                                            showErrorMsg($(domElement.CreateForm), data.Message);
                                        }
                                    }
                                });
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
        });
    }

    function initDeleteEvent($table, title, url, onDeleteSuccess) {
        $table.find('.delete-icon').on('click', function () {
            var id = $(this).data('id');
            confirm(title, function () {
                $.ajax({
                    method: 'POST',
                    url: url + id,
                    success: function (data) {
                        if (data.IsSuccessful) {
                            onDeleteSuccess();
                            toastr.success('Deleted successfully!', '', toastOptions());
                        } else {
                            toastr.error(data.message, '', toastOptions());
                        }
                    }
                });
            });
        });
    }

    function showErrorMsg($form, msg) {
        $form.find('.validation-summary-valid').find('ul:first').html('');
        var htmlObj = '<li>' + msg + '</li>';
        $form.find('.validation-summary-valid')
            .show().addClass('validation-summary-errors').find('ul:first').append(htmlObj);
    }

    function removeErrorMsg($form) {
        if ($form.find('.validation-summary-valid')) {
            $form.find('.validation-summary-valid').hide();
        }
    }

    function init() {
        initDatepicker();
        getFileUpload();
        updateInformationEvent();
        getRelationship();
        initButtonInsertEvent($(domElement.BtnAddRelationship), function () {
            getRelationship();
        });
    }

    return {
        init: init
    };
})(jQuery);

$(document).ready(function () {
    editAccount.init();
});