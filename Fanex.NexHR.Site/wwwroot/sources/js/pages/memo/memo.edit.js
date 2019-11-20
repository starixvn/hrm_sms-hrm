import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import { initDatepicker, showValidationSummaryErrors, hideValidationSummaryErrors, resolveClientUrl, axiosError, toastOptions } from '../../shared/common.functions';
import axios from 'axios';
import toastr from 'toastr';
import tinymce from 'tinymce/tinymce';
import 'tinymce/themes/silver';
import '../../date';
import '../memo/tinymce.plugins';


$(function () {
    var $attachedFileBlock, $memoType, $uploadBlock, $contentBlock, $fileList, $fileAttachContent, $memoContent, $fileUploader, $form, $memoTypeValue;

    function initTextEditor(element) {
        tinymce.init({
            mode: "exact",
            height: 400,
            menubar: false,
            selector: "textarea#" + element,
            plugins: "spellchecker,pagebreak,table,save,emoticons,insertdatetime,preview,image,link,media,imagetools,searchreplace,contextmenu,paste,directionality,wordcount,fullscreen,noneditable,visualchars,nonbreaking,template,hr,charmap,autolink",
            toolbar1: 'undo redo | bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | fontselect | fontsizeselect ',
            toolbar2: '  forecolor backcolor | bullist numlist | outdent indent | link image | table | hr | superscript subscript | charmap emoticons | ltr rtl |',
            content_css: false,
            fullpage_font_size: "13px",
            skin: false,
            force_br_newlines: true,
            force_p_newlines: false,
            branding: false,
        });
    }

    function initDatePicker() {
        initDatepicker();
    }

    function changeMemoType(value) {
        if (value === "1") {
            $contentBlock.hide();
            $fileUploader.show();
        } else {
            $contentBlock.show();
            $fileUploader.hide();
        }
    }

    function initMemoType() {
        $memoTypeValue = "1";
        $memoType.change(function () {
            let value = $(this).val();
            $memoTypeValue = value;
            changeMemoType(value);
        });
    }

    function validateForm() {
        let Subject = $("#Subject").val();
        let RefNo = $("#RefNo").val();
        let SentTo = $("#SentTo").val();
        let SentFrom = $("#SentFrom").val();
        let attachFile = $fileList.children().length;
        let fileName = $fileUploader.val();

        let isValid = true;
        let errorString = [];

        var editorText = tinyMCE.activeEditor.getContent();

        if (Subject === null || Subject === "") {
            errorString.push("The Subject field is required.");
        }
        if (RefNo === null || RefNo === "") {
            errorString.push("The RefNo field is required.");
        }
        if (SentTo === null || SentTo === "") {
            errorString.push("The SentTo field is required.");
        }
        if (SentFrom === null || SentFrom === "") {
            errorString.push("The SentFrom field is required.");
        }

        if ($memoTypeValue === "1") {
            if (attachFile === 0 && fileName === "") {
                errorString.push("The Attached file is required.");
            }
        } else {
            if (editorText === null || editorText === "") {
                errorString.push("The Content field is required.");
            }
        }

        if (errorString.length > 0) {
            $('.validation-summary-valid').find('ul:first').html('');
            showValidationSummaryErrors(errorString);
            isValid = false;
        }

        return isValid;
    }

    function initValues() {
        $attachedFileBlock = $("#attach");
        $fileList = $("#allAttachFile");
        $fileAttachContent = $("#AttachFile");
        $memoType = $("input[name=MemoType]");
        $uploadBlock = $("#upload");
        $contentBlock = $("#content");
        $memoContent = $("#Contents");
        $fileUploader = $("#fileName");
    }

    function uploadFile() {
        let files = document.getElementById('fileName').files;
        if (files === undefined) {
            return;
        }

        let fd = new FormData();
        fd.append('categoryID', $("#Category").val());
        fd.append('isOverride', $("#override").val());

        for (let i = 0; i < files.length; i++) {
            let file = files[i];

            if (file.size / 1024 > 5120) {
                alert('file is to large (' + file.size / 1024 + 'kb) to upload. maximum = 5120kb');
                return;
            }

            fd.append('files[' + i + ']', file);
        }

        axios({
            url: resolveClientUrl("file/UploadFile"),
            method: "POST",
            data: fd,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'multipart/form-data'
            }
        }).then(response => {
            if (response.status === 200) {
                var attachFile = $fileAttachContent.val();

                if (attachFile.indexOf(resolveClientUrl(response.data)) === -1) {
                    if (attachFile) {
                        $fileAttachContent.val(attachFile + ";" + resolveClientUrl(response.data));
                    }
                    else {
                        $fileAttachContent.val(resolveClientUrl(response.data));
                    }
                }
                axios({
                    url: $form[0].action,
                    method: "POST",
                    data: $form.serialize(),
                    async: false,
                }).then(response => {
                    document.location = resolveClientUrl("Memo/Index");
                }).catch(axiosError);
            }

        }).catch(axiosError);
    };

    function initFormSubmitEvent() {
        $("#btnSubmit").click(function (e) {
            let isValid = validateForm();
            $form = $('#createForm');

            if (isValid) {
                hideValidationSummaryErrors();
                e.preventDefault();
                $memoType.val($memoTypeValue);
                if ($memoTypeValue === "1") {
                    uploadFile();
                }
                else {
                    $memoContent.val(tinymce.activeEditor.getContent());
                    axios({
                        url: $form[0].action,
                        method: "POST",
                        data: $form.serialize(),
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest',
                            'Content-Type': 'application/x-www-form-urlencoded'
                        }
                    }).then(response => {
                        document.location = resolveClientUrl("Memo/Index");
                    }).catch(axiosError);
                }
            }
        });
    }

    $(document).ready(function () {
        initValues();
        changeMemoType("1");
        initDatePicker();
        initTextEditor('Contents');
        initMemoType();
        initFormSubmitEvent();
    });
});