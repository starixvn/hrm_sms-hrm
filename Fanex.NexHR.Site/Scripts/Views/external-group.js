/*global _global, _messages*/
"use strict";

var ExternalGroup = (function ($) {
    var UPDATE_GROUP_STATUS = 1;
    var ADD_GROUP_STATUS = 0;
    var me = {};
    var urlRoot = "/";
    var $externalGroupTable, $dialog, $tips, $inputGroupName;
    var modifiedGroupInfo = {
        Id: 0,
        Status: 0
    };

    var checkLength = function (obj, objectName, min, max) {
        var valid = true;

        if (obj.val().length > max || obj.val().length < min) {
            obj.addClass("ui-state-error");
            $tips.text("Length of " + objectName + " must be between " + min + " and " + max + ".");
            $tips.addClass("error");
            valid = false;
        }

        return valid;
    };

    var initEvent = function () {
        $externalGroupTable.on("click", ".js-link-delete-group", function () {
            var $self = $(this);
            var teamId = $self.data("id");

            confirm('Are you sure you want to delete?', function () {
                $.post(urlRoot + 'ExtensionNo/DeleteExternalGroup', { groupId: teamId }, function (response) {
                    if (response.IsSuccessful) {
                        document.location = '/ExtensionNo/ManageExternalGroup';
                    }
                    else {
                        alert(response.Message);
                    }
                }).error(function () {
                    alert(_messages.errorTryAgain);
                });
            });
        });

        $externalGroupTable.on("click", ".js-link-edit-group", function () {
            var $self = $(this);
            var teamId = parseInt($self.data("id"), 10);
            var teamName = $self.data("name");
            
            modifiedGroupInfo.Id = teamId,
            modifiedGroupInfo.Status = UPDATE_GROUP_STATUS;

            $inputGroupName.val(teamName);

            $dialog.dialog("open");
        });

        $("#btnNewExternalGroup").click(function () {
            modifiedGroupInfo.Status = ADD_GROUP_STATUS;

            $dialog.dialog("open");
        });
    };

    var initValue = function () {
        $externalGroupTable = $(".js-external-group-table");
        $dialog = $("#dialog-form");
        $inputGroupName = $("#GroupName");
        $tips = $(".validateTips");

        $dialog.dialog({
            autoOpen: false,
            width: 400,
            modal: true,
            buttons: {
                "Save": function () {
                    var bValid = true;
                    var params = {};
                    var dialogUrl = "";
                    $inputGroupName.removeClass("ui-state-error");
                    bValid = bValid && checkLength($inputGroupName, "external group", 3, 100);

                    if (bValid) {
                        if (modifiedGroupInfo.Status === UPDATE_GROUP_STATUS) {
                            dialogUrl = urlRoot + "ExtensionNo/EditExternalGroup";
                            params.GroupName = $inputGroupName.val();
                            params.GroupId = modifiedGroupInfo.Id;
                        } else {
                            dialogUrl = urlRoot + "ExtensionNo/AddNewExternalGroup";
                            params.GroupName = $inputGroupName.val();
                        }

                        $.post(dialogUrl, params, function (response) {
                            if (response.IsSuccessful) {
                                document.location = '/ExtensionNo/ManageExternalGroup';
                            }
                            else {
                                alert(response.Message);
                            }
                        }).error(function () {
                            alert(_messages.errorTryAgain);
                        });
                    }
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            },
            close: function () {
                $tips.text("");
                $tips.removeClass("error");
                $inputGroupName.val("").removeClass("ui-state-error");
            }
        });
    };

    me.init = function (globalRoot) {
        urlRoot = globalRoot;

        initValue();
        initEvent();
    };

    return me;
})(jQuery);

$(document).ready(function () {
    ExternalGroup.init(_global.root);
});