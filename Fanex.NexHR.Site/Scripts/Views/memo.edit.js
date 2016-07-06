'use strict';
/*global _global, initEditor, tinyMCE */

var editMemo = (function ($) {
    var me = {};
    var urlRoot = "/";
    var $attachedFileBlock, $memoType, $uploadBlock, $contentBlock,
        $hiddenMemoType, $fileList, $overrideBlock, $errorBlock, $fileAttachContent,
        $memoContent, $fileUploader, $form;

    var changeMemoType = function (value) {
        if (value === "1") {
            $uploadBlock.show();
            $attachedFileBlock.show();
            $overrideBlock.show();
            $contentBlock.hide();
        } else {
            $uploadBlock.hide();
            $attachedFileBlock.hide();
            $overrideBlock.hide();
            $contentBlock.show();

            $("#Contents_tbl").css("width", "612");
            $("#Contents_ifr").css("height", "335");
        }

        $hiddenMemoType.val(value);
    };

    var attachFile = function () {
        $fileList.html("");

        var attachPath = $fileAttachContent.val();
        if (attachPath) {
            var attachPathFiles = attachPath.split(";");

            for (var i = 0; i < attachPathFiles.length; i++) {
                var attachLink = attachPathFiles[i],
                    attachLinkSplit = attachLink.split('/'),
                    attachFileName = attachLinkSplit[attachLinkSplit.length - 1],
                    pattern = '<div id="file_' + i + '"><a href="' + attachLink + '">' + attachFileName + '</a>&nbsp\
								<a href="javascript:;" class="js-link-delete-file" data-file-name="'+ attachLink + '">\
									<span class="icon delete"></span>\
								</a></div>';

                $fileList.append(pattern);
            }
        }
    };

    var validateForm = function () {
        var check = true;
        var errorString = [];
        var Subject = $("#Subject").val();
        var RefNo = $("#RefNo").val();
        var SentTo = $("#SentTo").val();
        var SentFrom = $("#SentFrom").val();
        var attachFile = $fileList.children().length;
        var fileName = $fileUploader.val();

        var editorText = tinyMCE.activeEditor.getContent();

        if (Subject === null || Subject === "") {
            errorString.push("* The Subject field is required.");
        }
        if (RefNo === null || RefNo === "") {
            errorString.push("* The RefNo field is required.");
        }
        if (SentTo === null || SentTo === "") {
            errorString.push("* The SentTo field is required.");
        }
        if (SentFrom === null || SentFrom === "") {
            errorString.push("* The SentFrom field is required.");
        }

        if ($memoType.val() === "1") {
            if (attachFile === 0 && fileName === "") {
                errorString.push("* The Attached file is required.");
            }
        } else {
            if (editorText === null || editorText === "") {
                errorString.push("* The Content field is required.");
            }
        }

        if (errorString.length > 0) {
            $errorBlock.show();
            $errorBlock.html(errorString.join("</br>"));
            return false;
        }
        return check;
    };

    var uploadFailed = function (response) {
        $errorBlock.html(response.message);
    };

    var uploadComplete = function (evt) {
        if (evt.target.status === 200) {
            var attachFile = $fileAttachContent.val();

            if (attachFile.indexOf(urlRoot + evt.target.response) === -1) {
                if (attachFile) {
                    $fileAttachContent.val(attachFile + ";" + urlRoot + evt.target.response);
                }
                else {
                    $fileAttachContent.val(urlRoot + evt.target.response);
                }
            }

            $.ajax({
                url: $form.action,
                type: "Post",
                data: $form.serialize(),
                async: false,
                success: function () {
                    document.location = urlRoot + "Memo/Index";
                }
            });
        }
    };

    var uploadFile = function () {
        var files = document.getElementById('fileName').files;
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

        fd.append('categoryID', $("#Category").val());
        fd.append('overRide', $("#override").attr("checked") === "checked" ? "1" : "0");

        var xhr = new XMLHttpRequest();
        xhr.addEventListener("load", uploadComplete, false);
        xhr.addEventListener("error", uploadFailed, false);
        xhr.open("POST", urlRoot + "file/UploadFile");
        xhr.send(fd);
    };

    var initEvent = function () {
        $memoType.change(function () {
            var value = $(this).val();

            changeMemoType(value);
        });

        $attachedFileBlock.on("click", ".js-link-delete-file", function () {
            var $self = $(this);
            var fileName = $self.data("file-name");

            $.post(urlRoot + 'file/Delete', { FileName: fileName }, function (response) {
                if (response) {
                    var attachFiles = $fileAttachContent.val().split(";"), attachFileRemain = "";

                    for (var i = 0; i < attachFiles.length; i++) {
                        var attachFileItem = attachFiles[i];

                        if (attachFileItem !== fileName) {
                            if (attachFileRemain === "") {
                                attachFileRemain = attachFileItem;
                            }
                            else {
                                attachFileRemain += ";" + attachFileItem;
                            }
                        }
                    }

                    $fileAttachContent.val(attachFileRemain);
                    attachFile();
                }
            }).fail(function () {
                $errorBlock.html("Fail to remove attached file!");
            });
        });

        $("form").submit(function (e) {
            var isValid = validateForm();
            $form = $(this);

            if (!isValid) {
                $errorBlock.show();
                return false;
            }

            e.preventDefault();

            if ($memoType.val() === "1") {
                var attachFile = $fileList.children().length;

                if (attachFile === 0 || (attachFile !== 0 && $fileUploader.val() !== "")) {

                    uploadFile();
                } else {
                    $.ajax({
                        url: $form.action,
                        type: "Post",
                        data: $form.serialize(),
                        success: function () {
                            document.location = _global.root + "Memo/Index";
                        }
                    });
                }
            }
            else {
                $memoContent.val(tinyMCE.activeEditor.getContent());

                $.ajax({
                    url: $form.action,
                    type: "Post",
                    data: $form.serialize(),
                    success: function () {
                        document.location = _global.root + "Memo/Index";
                    }
                });
            }
        });
    };

    var initValue = function () {
        $attachedFileBlock = $("#attach");
        $fileList = $("#allAttachFile");
        $fileAttachContent = $("#AttachFile");
        $memoType = $("input[name=MemoType");
        $uploadBlock = $("#upload");
        $contentBlock = $("#content");
        $hiddenMemoType = $("#MemoType");
        $overrideBlock = $(".js-override-block");
        $errorBlock = $("#error");
        $memoContent = $("#Contents");
        $fileUploader = $("#fileName");

        $errorBlock.hide();

        var $sentDate = $("#SentDate");
        $sentDate.prop("readonly", true);
        $sentDate.datepicker({
            changeMonth: true,
            changeYear: true
        });
    };

    me.init = function (globalRoot) {
        urlRoot = globalRoot;
        initValue();
        initEvent();

        attachFile();
        changeMemoType($hiddenMemoType.val());
        initEditor('Contents', 'basic');
    };

    return me;
}(jQuery));

$(document).ready(function () {
    editMemo.init(_global.root);
});
