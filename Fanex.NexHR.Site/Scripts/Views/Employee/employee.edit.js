'use strict';
/*global _global, _messages, reloadButtonText, actingButton*/
/* exported */

function bindSARole(onload) {
    var combineSARole = $('#Roles2Array2, #Roles2Array3, #Roles2Array4, #Roles2Array5, #Roles2Array6,#Roles2Array8');
    if ($('#Roles2Array7').prop('checked')) {
        combineSARole.prop('checked', true).prop('disabled', true);
    }
    else if (!onload) {
        combineSARole.prop('checked', false).prop('disabled', false);
    }
}

function bindDisciplineRole(onload) {
    var combineDisciplineRole = $('#Roles2Array3');
    if ($('#Roles2Array8').prop('checked')) {
        combineDisciplineRole.prop('checked', true).prop('disabled', true);
    }
    else if (!onload) {
        combineDisciplineRole.prop('checked', false).prop('disabled', false);
    }
}

function uploadComplete(evt) {
    if (evt.target.status === 200) {
        var url = $('#avatar').attr('src');
        var parts = url.split('?f=');
        $('#avatar').attr('src', parts[0] + '?f=' + Math.random());
    }
    reloadButtonText('#upload');
}

function uploadFailed() {
    $('#m').text('Upload fail, please try again!');
    reloadButtonText('#upload');
}

function upload() {
    var file = document.getElementById('file').files[0];
    if (file === undefined) {
        return;
    }
    if (file.size / 1024 > 5120) {
        alert('File is to big (' + file.size / 1024 + 'KB) to upload. Maximum = 5120KB');
        return;
    }

    var pass = /\.(gif|jpe?g|png)$/i.test(file.name);
    if (!pass) {
        alert('Please choose an image');
        return;
    }

    //notice
    actingButton('#upload', true);
    $('#m').text('');

    var fd = new FormData();
    fd.append('code', $('#Code').val());
    fd.append('file', file);

    var xhr = new XMLHttpRequest();
    xhr.addEventListener("load", uploadComplete, false);
    xhr.addEventListener("error", uploadFailed, false);
    xhr.open("POST", _global.root + "employee/uploadavatar");
    xhr.send(fd);
}

function resetPassword() {
    var id = $("#Id").val();
    var nickName = $("#NickName").val();
    confirm("Are you sure you want to reset " + nickName + "'s password?", function () {
        $.post(_global.root + 'Account/ResetPassword', { id: id }, function (response) {
            if (!response.IsSuccessful) {
                alert(response.Message);
                return;
            }
        });
    });
}

function checkAuthorizeToGrantAccess() {
    var canEditUserRole = $("#Authorize").val();
    if (canEditUserRole === "False") {
        $('#Roles2Array').unbind("click");
        $("#userRole label").attr('onclick', 'return false');
        $("#userRole label").last().hide();
    }
}

function showExpatApplicableHoliday() {
    var value = $("#IsLocal").val();
    if (value === "true") {
        $("#expatHoliday").hide();
    } else {
        $("#expatHoliday").show();
    }
}

$(document).ready(function () {
    $(".dropdown").chosen();
    $('#JobTitleId').chosen();
    var $uploadButton = $("#upload");

    $uploadButton.hide();

    bindSARole(true);
    $('#Roles2Array7').click(function () {
        bindSARole(false);
    });

    bindDisciplineRole(true);
    $('#Roles2Array8').click(function () {
        bindDisciplineRole(false);
    });

    $("#file").change(function () {
        var filePath = $(this).val();

        if (filePath) {
            $uploadButton.show();
        } else {
            $uploadButton.hide();
        }
    });

    //checkUserIsSystemAdmin
    checkAuthorizeToGrantAccess();

    //Birthday picker.
    $("#DOBPicker").birthdaypicker({
        futureDates: true,
        maxYear: new Date().getFullYear(),
        maxAge: 75,
        defaultDate: $('#DateOfBirth').val()
    });

    $("#postform").click(function () {
        var isCheckedTopManager = $("input[name='IsTopManager']").prop('checked');

        if (isCheckedTopManager === true) {
            $("#formEditUser")[0].submit();
        } else {
            var verifier = $("#lstVerifier").text(),
                approver = $("#lstApprover").text();
            if (verifier === null || verifier === "") {
                if (approver === null || approver === "") {
                    confirm("This user does not have any Verifier or Approver! Continue?", function () {
                        $("#formEditUser")[0].submit();
                    });
                } else {
                    confirm("This user does not have any Verifier! Continue?", function () {
                        $("#formEditUser")[0].submit();
                    });
                }
            } else {
                if (approver === null || approver === "") {
                    confirm("This user does not have any Approver! Continue?", function () {
                        $("#formEditUser")[0].submit();
                    });
                } else {
                    $("#formEditUser")[0].submit();
                }
            }
        }
    });

    $("#resetpassword").on("click", function () {
        resetPassword();
    });

    $("#month, #day, #year").change(function () {
        var year = $("#year").val();
        var month = $("#month").val();
        var day = $("#day").val();
        var date = new Date(year, month - 1, day);

        $("#DateOfBirth").val(date.format("dd mmm yyyy"));
    });

    $uploadButton.click(function () {
        upload();
    });

    $('#delete').click(function () {
        confirm(_messages.confirmDelete, function () {
            var resignedDate = $('#ResignedDate').val();
            if (resignedDate.length === 0) {
                alert(_messages.requireResignedDate);
                $('#ResignedDate').focus();
                return;
            }

            $.post(_global.root + 'Employee/Delete',
            { id: $('#Id').val(), resignedDate: resignedDate },
            function (response) {
                if (response.IsSuccessful) {
                    document.location = _global.root + 'employee/usermanagement';
                } else {
                    alert(response.Message);
                    return;
                }
            }).error(function () { alert(_messages.errorTryAgain); });
        });
    });

    var collapse = $(".panel > .collapse"),
        itemContent = collapse.next();
    itemContent.show();

    $(".panel > .collapse").click(function () {
        var $header = $(this);
        //getting the next element
        var $content = $header.next();
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
            var icon = $header.find('> span.fa');
            if ($(icon).hasClass('fa-chevron-down')) {
                $(icon).removeClass('fa-chevron-down').removeClass('fa-chevron-right').addClass('fa-chevron-right');
            } else if ($(icon).hasClass('fa-chevron-right')) {
                $(icon).removeClass('fa-chevron-right').removeClass('fa-chevron-down').addClass('fa-chevron-down');
            }
        });
    });

    $("#IsLocal").on("change", function () {
        showExpatApplicableHoliday();
    });
    showExpatApplicableHoliday();
});