/*global _global, reloadButtonText, _messages, floatThead */
"use strict";

var JobTitle = (function ($) {
    var me = {};
    var keyword, jobTitleTableBody, jobTitleTable, jobName, tips, allFields, dialogForm;
    var UPDATE_JOB = 1;
    var INSERT_JOB = 0;
    var urlRoot = "/";
    var messages = {};

    var checkLength = function (obj, objectName, min, max) {
        var valid = true;

        if (obj.val().length > max || obj.val().length < min) {
            obj.addClass("ui-state-error");
            tips.text("Length of " + objectName + " must be between " + min + " and " + max + ".");
            tips.addClass("error");
            valid = false;
        }

        return valid;
    };

    var exportExcel = function (keyword) {
        window.location.href = "/jobTitle/ExportExcel?keyword=" + keyword;
    };

    var getJobTitle = function (value) {
        $.post(urlRoot + "jobTitle/GetJobTitles", { jobTitle: value }, function (data) {
            jobTitleTableBody.html(data);
            floatThead(".js-job-title-table");
        }).error(function () {
            alert(messages.errorTryAgain);
        });

        reloadButtonText('#query');
    };

    var initEvent = function () {
        keyword.keyup(function (event) {
            if (event.keyCode === 13) {
                getJobTitle($(this).val());
            }
        });

        $("#btnNewJobTitle").click(function () {
            $("span.ui-dialog-title").text('Create an job title');
            $("#isUpdate").val(INSERT_JOB);
            $("#dialog-form").dialog("open");
        });

        $(".js-btn-submit").click(function () {
            getJobTitle(keyword.val());
        });

        $(".js-btn-exportExcel").on("click", function () {
            exportExcel(keyword.val());
        });

        jobTitleTableBody.on("click", ".ls-link-delete-job", function () {
            var jobTitleId = $(this).attr("data-id");

            confirm('Are you sure you want to delete?', function () {
                $.post(urlRoot + 'JobTitle/Delete',
                    { id: jobTitleId },
                    function (response) {
                        if (response.IsSuccessful) {
                            getJobTitle(keyword.val());
                        }
                        else {
                            alert(response.Message);
                        }
                    }).error(function () {
                        alert(messages.errorTryAgain);
                    });
            });
        });

        jobTitleTableBody.on("click", ".js-link-edit-job", function () {
            var $this = $(this);

            var jobTitleId = $this.attr("data-id");
            var categoryId = $this.attr("data-category-id");
            var jobTitle = $this.attr("data-job-title");

            $(".ui-dialog-title").text('Edit an job title');
            $("#isUpdate").val(UPDATE_JOB);
            $("#jobtitleId").val(jobTitleId);
            $("#category").val(categoryId);
            jobName.val($.trim(jobTitle));

            dialogForm.dialog("open");
        });

        jobTitleTableBody.on("click", ".js-link-redirect-use-management", function () {
            var $this = $(this);
            var jobId = $this.attr("data-id");

            document.location = urlRoot + "employee/usermanagement/?jobtitleid=" + jobId;
        });

        dialogForm.dialog({
            autoOpen: false,
            width: 400,
            modal: true,
            buttons: {
                "Save": function () {
                    var bValid = true;
                    allFields.removeClass("ui-state-error");
                    bValid = bValid && checkLength(jobName, "job title", 3, 100);

                    if (bValid) {
                        if (parseInt($("#isUpdate").val(), 10) === UPDATE_JOB) {
                            $.post(urlRoot + 'JobTitle/Update',
                                { id: $("#jobtitleId").val(), name: $("#name").val(), categoryId: $("#category").val() },
                                function (response) {
                                    if (response.IsSuccessful) {
                                        getJobTitle(keyword.val());
                                    }
                                    else {
                                        alert(response.Message);
                                    }
                                }).error(function () {
                                    alert(messages.errorTryAgain);
                                }).always(function () {
                                    dialogForm.dialog("close");
                                });
                        }
                        else {
                            $.post(urlRoot + 'JobTitle/Insert',
                                { name: $("#name").val(), categoryId: $("#category").val() },
                                function (response) {
                                    if (response.IsSuccessful) {
                                        getJobTitle(keyword.val());
                                    }
                                    else {
                                        alert(response.Message);
                                    }
                                }).error(function () {
                                    alert(messages.errorTryAgain);
                                }).always(function () {
                                    dialogForm.dialog("close");
                                });
                        }
                    }
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            },
            close: function () {
                tips.text("");
                tips.removeClass("error");
                allFields.val("").removeClass("ui-state-error");
            }
        });
    };

    var initDefaultValue = function () {
        keyword = $("#keyword");
        jobTitleTable = $(".js-job-title-table");
        jobTitleTableBody = $(".js-job-title-table-body");
        jobName = $("#name");
        tips = $(".js-validate-tips");
        allFields = $([]).add(jobName);
        dialogForm = $("#dialog-form");
    };

    me.init = function (globalRoot, messageList) {
        urlRoot = globalRoot;
        messages = messageList;

        initDefaultValue();
        initEvent();

        getJobTitle(keyword.val());
    };

    return me;
})(jQuery);

$(document).ready(function () {
    JobTitle.init(_global.root, _messages);
});