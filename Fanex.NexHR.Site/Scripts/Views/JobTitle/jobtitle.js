/// <reference path="../Libs/jquery.min.js" />
/*global actingButton, _global, noInformation, reloadButtonText, _messages, getParam*/
/*exported deleteJobTitle, index, editJobTile, canNotUpdate*/
"use strict";

var rowIndex = 1;
function canNotUpdate(id) {
    $('#txtJobTile_' + id).val($('#spanJobTitle_' + id).text());
}

function index() {
    return rowIndex++;
}

function query() {
    var value = $('#keyword').val();
    actingButton('#query', true);

    $.getJSON(
        _global.root + 'jobtitle/getjobtitle',
        { query: value },
        function (data) {
            if (data.length > 0) {
                rowIndex = 1;

                $('#data tbody').children().remove();
                $('#itemTemplate').tmpl(data).appendTo('#data');
                floatThead('table#data');
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
}

function deleteJobTitle(id) {
    confirm('Are you sure you want to delete?', function () {
        $.post(_global.root + 'JobTitle/Delete',
        { id: id },
        function (response) {
            if (response.IsSuccessful) {
                document.location = '/jobtitle/jobtitlemanagement';
            }
            else {
                alert(response.Message);
            }
        }).error(function () {
            alert(_messages.errorTryAgain);
        });
    });
}

$(function () {
    var name = $("#name"),
        allFields = $([]).add(name),
        tips = $(".validateTips");

    function updateTips(t) {
        tips.text(t);
    }

    function checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            o.addClass("ui-state-error");
            updateTips("Length of " + n + " must be between " +
        min + " and " + max + ".");
            return false;
        } else {
            return true;
        }
    }

    $("#dialog-form").dialog({
        autoOpen: false,
        height: 220,
        width: 400,
        modal: true,
        buttons: {
            "Save": function () {
                var bValid = true;
                allFields.removeClass("ui-state-error");
                bValid = bValid && checkLength(name, "job title", 3, 100);

                if (bValid) {
                    if ($("#isUpdate").val() === "1") {
                        $.post(_global.root + 'JobTitle/Update',
                        { id: $("#jobtitleId").val(), name: $("#name").val(), categoryId: $("#category").val() },
                        function (response) {
                            if (response.IsSuccessful) {
                                document.location = '/jobtitle/jobtitlemanagement';
                            }
                            else {
                                alert(response.Message);
                            }
                        }).error(function () {
                            alert(_messages.errorTryAgain);
                        });
                    }
                    else {
                        $.post(_global.root + 'JobTitle/Insert',
                        { name: $("#name").val(), categoryId: $("#category").val() },
                        function (response) {
                            if (response.IsSuccessful) {
                                document.location = '/jobtitle/jobtitlemanagement';
                            }
                            else {
                                alert(response.Message);
                            }
                        }).error(function () {
                            alert(_messages.errorTryAgain);
                        });
                    }
                }
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        },
        close: function () {
            updateTips('');
            allFields.val("").removeClass("ui-state-error");
        }
    });

    $("#btnNewJobTitle").click(function () {
        $("span.ui-dialog-title").text('Create an job title');
        $("#isUpdate").val(0);
        $("#dialog-form").dialog("open");
    });
});

$(document).ready(function () {
    $('#keyword').keyup(function (event) {
        if (event.keyCode === 13) {
            query();
        }
    });

    var q = getParam('query');
    if (q !== null) {
        $('#keyword').val(q);
    }

    //search onload
    query();

    $('#query').click(function (evt) {
        evt.preventDefault();
        query();
    });

    $('#keyword').keypress(function () {
        if ($(this).val().length >= 2) {
            query();
        }
    });

    $("#data").on("click", ".lnkEdit", function () {
        var jobTitleId = $(this).attr("data-id");
        var categoryId = $(this).attr("data-categoryId");
        $("span.ui-dialog-title").text('Edit an job title');
        $("#isUpdate").val(1);
        $("#jobtitleId").val(jobTitleId);
        $("#category").val(categoryId);
        $("#name").val($.trim($('#txtJobTile_' + jobTitleId).val()));
        $("#dialog-form").dialog("open");
    });

    $("#data").on("click", ".lnkDelete", function () {
        var jobTitleId = $(this).attr("data-id");
        deleteJobTitle(jobTitleId);
    });
});