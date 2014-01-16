$(document).ready(function () {
    initEditor('Contents', 'basic');
    $("#error").hide();
    $("#SentDate").attr('readonly', 'readonly');

    $("#SentDate").datepicker({
        changeMonth: true,
        changeYear: true
    });
    var refNo = $("#RefNo").val();
    var subJect = $("#Subject").val();
    var forInfo = $("#ForInfo").val();
    var sentDate = $("#SentDate").val();
    var sentTo = $("#SentTo").val();
    var sentFrom = $("#SentFrom").val();
    var contents = $("#Contents").val();

    $("#btnClear").click(function () {
        $("#RefNo").val(refNo);
        $("#Subject").val(subJect);
        $("#ForInfo").val(forInfo);
        $("#SentDate").val(sentDate);
        $("#SentTo").val(sentTo);
        $("#SentFrom").val(sentFrom);
        $("#Contents").val(contents);
    });

    $("#Type_0, #Type_1").change(function () {
        ChangeType();
        $("#error").hide();
    });

    if ($("#Type").val() == "1") {
        $("#Type_1").attr("checked", "checked");

        if ($("#AttachFile").val().length > 0) {
            $('#attach').show();
            $("#attachFile").html($("#pathName").val());
        }
        else
            $('#attach').hide();
    }
    else
        $("#Type_0").attr("checked", "checked");
    ChangeType();

    $("form").submit(function (e) {

        if ($("#Type").val() == "1") {
            form = $(this);
            var validate = ValidateForm();
            var attachFile = $("#attachFile").children().length;
            var fileName = $("#fileName").val();

            if (!validate) {
                e.preventDefault();
                $("#error").show();
            }

            else {
                if (attachFile == 0 || (attachFile != 0 && fileName != "")) {
                    e.preventDefault();
                    upload();
                }
                else {
                    e.preventDefault();
                    $.ajax({
                        url: form.action,
                        type: "Post",
                        data: form.serialize(),
                        success: function (response) {
                            document.location = _global.root + "Memo/Index";

                        }
                    });
                }
            }
        }
        else {
            form = $(this);
            var validate = ValidateForm();
            if (!validate) {
                $("#error").show();
                return false;
            }
            else {
                e.preventDefault();
                $("#Contents").val(tinyMCE.activeEditor.getContent())
                $.ajax({
                    url: form.action,
                    type: "Post",
                    data: form.serialize(),
                    success: function (response) {
                        document.location = _global.root + "Memo/Index";

                    }
                });
            }

        }

    });

});

var form;

function ChangeType() {
    if ($("#Type_1").attr("checked") == "checked") {
        $("#content").hide();
        $("#upload").show();
        $("#Type").val("1");
    }
    else {
        $("#content").show();
        $("#upload").hide();
        $("#Type").val("0");
        $("#Contents_tbl").css("width", "612");
        $("#Contents_ifr").css("height", "335");

        if ($("#attachFile").children().length == 0) {
            $("span.delete").hide();
        }

    }
}

function upload() {

    var file = document.getElementById('fileName').files[0];
    if (file == undefined) return;
    if (file.size / 1024 > 5120) {
        alert('File is to large (' + file.size / 1024 + 'KB) to upload. Maximum = 5120KB');
        return;
    }

    var ext = file.name.substring(file.name.lastIndexOf('.') + 1, file.name.length).toLowerCase();
    //    var pass = /\.(gif|jpe?g|png)$/i.test(file.name);
    //    if (!pass) {
    //        alert('Please choose an image');
    //        return;
    //    }

    var fd = new FormData();

    fd.append('file', file);
    fd.append('categoryID', $("#Category").val());
    fd.append('overRide', $("#override").attr("checked") == "checked" ? "1" : "0");

    var xhr = new XMLHttpRequest();
    xhr.addEventListener("load", uploadComplete, false);
    xhr.addEventListener("error", uploadFailed, false);
    xhr.open("POST", _global.root + "file/UploadFile");
    xhr.send(fd);



}

function uploadComplete(evt) {
    if (evt.target.status == 200) {
        $("#AttachFile").val(_global.root + evt.target.response);
        // evt.stopPropagation;
        $.ajax({
            url: form.action,
            type: "Post",
            data: form.serialize(),
            async: false,
            success: function (response) {
                document.location = _global.root + "Memo/Index";
            }
        });
    }
}

function deleteFile() {
    deleteMemoFile($("#AttachFile").val());
    $("#attachFile").children().remove();
}

function ValidateForm() {
    var check = true;
    var errorString = [];
    var Subject = $("#Subject").val();
    var RefNo = $("#RefNo").val();
    var SentTo = $("#SentTo").val();
    var SentFrom = $("#SentFrom").val();
    var attachFile = $("#attachFile").children().length;
    var fileName = $("#fileName").val();

    var editorText = tinyMCE.activeEditor.getContent();

    if (Subject == null || Subject == "") {
        errorString.push("* The Subject field is required.");
    }
    if (RefNo == null || RefNo == "") {
        errorString.push("* The RefNo field is required.");
    }
    if (SentTo == null || SentTo == "") {
        errorString.push("* The SentTo field is required.");
    }
    if (SentFrom == null || SentFrom == "") {
        errorString.push("* The SentFrom field is required.");
    }

    if ($("#Type").val() == "1") {
        if (attachFile == 0 && fileName == "") {
            errorString.push("* The Attached file is required.");
        }
    }

    if ($("#Type").val() == "0") {
        if (editorText == null || editorText == "") {
            errorString.push("* The Content field is required.");
        }
    }

    if (errorString.length > 0) {
        $("#error").show();
        $("#error").html(errorString.join("</br>"));
        return false;
    }
    return check;
}
