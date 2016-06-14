'use strict';
/*global _global, _messages, actingButton, noInformation, reloadButtonText, getParam*/
/* exported index, resetPassword*/

var rowIndex = 1;
var enable = true;

function index() {
    return rowIndex++;
}

function query(jobTitleId) {
    if (jobTitleId === undefined || jobTitleId === '') {
        jobTitleId = $('#title').val();
    }

    var keyword = $('#keyword').val();
    actingButton('#query', true);
    enable = false;

    var isDelete = null;
    if ($("#IsDeleted").attr("checked")) {
        isDelete = true;
    }
    else {
        isDelete = null;
    }

    var groups = $("#group").val() !== null ? $("#group").val().join() : "";
    if (groups === "") {
        var groupid = $('#groupid').val();
        groups = groupid;
        $("#group option[value='" + groups + "']").prop("selected", 1);
        $("#group").multiselect("refresh");
    }

    var isLocal = $("#category").val();
    if (isLocal === "0") {
        isLocal = false;
    }
    if (isLocal === "1") {
        isLocal = true;
    }

    $.getJSON(_global.root + 'api/employee/getemployees',
    {
        query: keyword,
        groupIds: groups,
        enable: enable,
        isDelete: isDelete,
        titleId: jobTitleId,
        isLocal: isLocal
    },
    function (data) {
        if (data.length > 0) {
            rowIndex = 1;
            $('#data tbody').children().remove();
            $('#itemTemplate').tmpl(data).appendTo('#data');

            $('.nick').tooltip({
                track: true,
                delay: 0,
                showURL: false,
                bodyHandler: function () {
                    return '<img width=64px; height=64px; src="/employee/avatar/' + $(this).attr('data-code') + '" />';
                }
            });

            $("#title").trigger("liszt:updated");
        }
        else {
            $('#data tbody').children().remove();
            $(noInformation(12)).appendTo('#data');
        }

        reloadButtonText('#query');
    }).error(function () {
        reloadButtonText('#query');
        alert(_messages.errorTryAgain);
    });
    $('#keyword').focus().select();
}

$(document).ready(function () {
    var jobTitleId = $('#jobtitleid').val();
    $("#group").multiselect();
    $('#title').val(jobTitleId);
    $('#title').chosen();
    $("#title").trigger("liszt:updated");

    $('#keyword').keyup(function (event) {
        if (event.keyCode === 13) {
            query(jobTitleId);
        }
    });

    var q = getParam('query');
    if (q !== null) {
        $('#keyword').val(q);
    }

    //search onload
    query(jobTitleId);

    $('#query').click(function (evt) {
        evt.preventDefault();
        query(jobTitleId);
    });
});