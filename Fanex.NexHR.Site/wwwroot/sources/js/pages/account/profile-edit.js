import $ from 'jquery/dist/jquery';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import toastr from 'toastr';
import { confirm, alert, blockUI, unblockUI } from '../../shared/common.functions';
import { toast } from 'react-toastify';

var editAccount = (function ($) {
    const constants = {
        Attachments: "#Attachments",
        FileMaxSize: 5242880,
        DeleteFile: ".delete-file-upload",
        ListFileUpload: "#upload-list"
    };

    function initUploadFileHandler() {
        $(constants.Attachments).on('change', function (event) {
            var files = event.originalEvent.target.files;
            if (files.length > 0 && validateFileUpload(files)) {
                uploadFiles(files);
            } else {
                toast.error("File(s) invalid!");
            }
        });
    }

    function uploadFiles(files) {
        if (files === undefined) {
            return;
        }
        var fileNames = [];

        var fd = new FormData();

        for (var i = 0; i < files.length; i++) {
            var file = files[i];

            if (file.size / 1024 > 5120) {
                alert('file is to large (' + file.size / 1024 + 'kb) to upload. maximum = 5120kb');
                return;
            }

            fd.append('files[' + i + ']', file);
            fileNames.push(files[i].name);
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
                    $(constants.Attachments).val('');
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
        $(constants.DeleteFile).on('click', function () {
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
                            toast.error(result.message);
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
            if (files[i].size > constants.FileMaxSize) {
                isValid = false;
            }
            break;
        }
        return isValid;
    }

    function getFileUpload() {
        $.ajax({
            type: "GET",
            url: "/Account/GetFileUploaded",
            contentType: "html",
            success: function (data) {
                $(constants.ListFileUpload).html(data);
                initUploadFileHandler();
                initDeleteFileUpload();
            }
        });
    }

    return {
        init: getFileUpload
    };
})(jQuery);

$(document).ready(function () {
    editAccount.init();
});