import $ from 'jquery/dist/jquery';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import toastr from 'toastr';
import { confirm, alert } from '../../shared/common.functions';

var editAccount = (function ($) {
    const domElement = {
        EditInformationForm: "#info-update",
        Attachments: "#Attachments",
        FileMaxSize: 5242880,
        DeleteFile: ".delete-file-upload",
        ListFileUpload: "#upload-list",
        Relationship: "#relationship",
        BtnAddRelationship: "#btn-add-relationship",
        CreateForm: "#create-form",
        EditForm: "#edit-form"
    };

    function updateInformationEvent() {
        $(domElement.EditInformationForm).on("submit", function (e) {
            e.preventDefault();
            removeErrorMsg($(domElement.EditInformationForm));
            $.ajax({
                url: $(domElement.EditInformationForm).attr("action"),
                method: $(domElement.EditInformationForm).attr("method"),
                data: $(domElement.EditInformationForm).serialize(),
                success: function (result) {
                    if (result.IsSuccessful) {
                        toastr.success("Updated information sucessfully!!!");
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
                toastr.error("File(s) invalid!");
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
            type: "POST",
            url: "/File/MyProfileUploadFile",
            contentType: false,
            processData: false,
            data: fd,
            success: function (result) {
                getFileUpload();
                if (result.isSuccessful) {
                    toastr.success("File upload successfully !");
                } else {
                    toastr.error(result.message);
                    $(domElement.Attachments).val('');
                }
            },
            error: function (xhr, status, p3, p4) {
                var err = "Error " + " " + status + " " + p3 + " " + p4;
                if (xhr.responseText && xhr.responseText[0] === "{")
                    err = JSON.parse(xhr.responseText).Message;
                toastr.error(err);
            }
        });
    }

    function initDeleteFileUpload() {
        $(domElement.DeleteFile).on('click', function () {
            var $btn = $(this);
            confirm('Are you sure to want to delete this file?', function () {
                var fileName = $btn.attr('data-file-name');
                $.ajax({
                    type: "POST",
                    url: "/File/DeleteMyProfileUploadedFile",
                    data: { fileName },
                    success: function (result) {
                        if (result.isSuccessful) {
                            toastr.success("Deleted file successfully!");
                            getFileUpload();
                        } else {
                            toastr.error(result.message);
                        }
                    },
                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] === "{")
                            err = JSON.parse(xhr.responseText).Message;
                        toastr.error(err);
                    }
                });
            });
        });
    }

    function validateFileUpload(files) {
        var isValid = true;
        for (var i = 0; i < files.length; i++) {
            if (files[i].size > domElement.FileMaxSize) {
                isValid = false;
            }
            break;
        }
        return isValid;
    }

    function getFileUpload() {
        $.ajax({
            type: "GET",
            url: "/Information/GetFileUploaded",
            contentType: "html",
            success: function (data) {
                $(domElement.ListFileUpload).html(data);
                initUploadFileHandler();
                initDeleteFileUpload();
            }
        });
    }

    function getRelationship() {
        $.ajax({
            type: "GET",
            url: "/Information/GetEmployeeRelationship",
            contentType: "html",
            success: function (data) {
                $(domElement.Relationship).html(data);
                initEditRelationshipEvent();
                initDeleteRelationshipEvent();
            }
        });
    }

    function initAddRelationshipEvent() {
        $(domElement.BtnAddRelationship).on("click", function () {
            $.get("/Information/InsertRelationship", function (data) {
                $(domElement.CreateForm).html(data);
                initDatePicker($(domElement.CreateForm));
                $(domElement.CreateForm).dialog({
                    title: "Add relationship",
                    height: 'auto',
                    width: 'auto',
                    modal: true,
                    buttons: {
                        'Submit': function () {
                            onAddRelationship();
                        },
                        'Cancel': function () {
                            $(this).dialog('close');
                        }
                    }
                });
            });
        });
    }

    function initEditRelationshipEvent() {
        $('.edit-relationship').on("click", function () {
            var id = $(this).data('id');
            if (id > 0) {
                $.get("/Information/EditRelationship/" + id, function (data) {
                    $(domElement.EditForm).html(data);
                    initDatePicker($(domElement.EditForm));
                    $(domElement.EditForm).dialog({
                        title: "Add relationship",
                        height: 'auto',
                        width: 'auto',
                        modal: true,
                        buttons: {
                            'Submit': function () {
                                onEditRelationship();
                            },
                            'Cancel': function () {
                                $(this).dialog('close');
                            }
                        }
                    });
                });
            }
        });
    }

    function initDatePicker($dialog) {
        if ($dialog.find('.datepicker').length > 0) {
            $('.datepicker').datepicker({
                dateFormat: "dd MMM, yyyy",
                changeMonth: true,
                changeYear: true
            });

            $('.datepicker').attr('readonly', 'readonly');
            $('.datepicker').keydown(function (e) {
                var tab = 9, enter = 13;
                if (e.keyCode === tab || e.keyCode === enter) {
                    return true;
                }

                return false;
            });
        }
    }

    function onAddRelationship() {
        $.ajax({
            method: "POST",
            url: $(domElement.CreateForm).find('form').attr("action"),
            data: $(domElement.CreateForm).find('form').serialize(),
            success: function (data) {
                if (data.IsSuccessful) {
                    $(domElement.CreateForm).dialog('close');
                    getRelationship();
                    toastr.success("Add relationship successfully!");
                } else {
                    showErrorMsg($(domElement.CreateForm), data.Message);
                }
            }
        });
    }

    function onEditRelationship() {
        $.ajax({
            method: "POST",
            url: $(domElement.EditForm).find('form').attr("action"),
            data: $(domElement.EditForm).find('form').serialize(),
            success: function (data) {
                if (data.IsSuccessful) {
                    $(domElement.EditForm).dialog('close');
                    getRelationship();
                    toastr.success("Update relationship successfully!");
                } else {
                    showErrorMsg($(domElement.EditForm), data.Message);
                }
            }
        });
    }

    function initDeleteRelationshipEvent() {
        $('.delete-relationship').on("click", function () {
            var id = $(this).data("id");
            confirm("Do you want to delete this relationship?", function () {
                $.ajax({
                    method: "POST",
                    url: "/Information/DeleteRelationship/" + id,
                    success: function (data) {
                        if (data.IsSuccessful) {
                            getRelationship();
                            toastr.success("Delete relationship successfully!");
                        } else {
                            toastr.error(data.message);
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
        initDatePicker($(domElement.EditInformationForm));
        getFileUpload();
        getRelationship();
        updateInformationEvent();
        initAddRelationshipEvent();
    }

    return {
        init: init
    };
})(jQuery);

$(document).ready(function () {
    editAccount.init();
});