'use strict';

import { alert } from "../../wwwroot/sources/js/shared/common.functions";

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
                showErrorMessageFileUpload();
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
            url: "File/MyProfileUploadFile",
            contentType: false,
            processData: false,
            data: fd,
            success: function (result) {
                if (result.isSuccessful) {
                    toastr.success("File upload successful !");
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
    }

    function showErrorMessageFileUpload(message) {
        alert(message);
    }

    function initDeleteFileUpload() {
        $(constants.DeleteFile).on('click', function () {
            var $btn = $(this);
            confirm('Are you sure to want to delete this file?', function () {
                var fileName = $btn.data('fileName');
                $.ajax({
                    type: "POST",
                    url: "/File/DeleteMyProfileUploadedFile",
                    data: { fileName },
                    success: function (result) {
                        if (result.isSuccessful) {
                            toastr.success('The file have been deleted.');
                            $btn.parent().remove();
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
            if (files[i].size > constants.FileMaxSize) {
                isValid = false;
            }
            break;
        }
        return isValid;
    }

    return {
        init: initUploadFileHandler
    };
})(jQuery);

$(document).ready(function () {
    editAccount.init();
});