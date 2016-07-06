'use strict';
/*global _global, _messages*/
/*exported*/

$(document).ready(function () {
    $("#deleteExt").live("click", function () {
        var id = $(this).closest('tr').attr('id');
        confirm('Are you sure you want to delete?', function () {
            $.post(_global.root + 'ExtensionNo/DeleteExtensionNumber', { extId: id }, function (response) {
                if (response.IsSuccessful) {
                    document.location = '/extensionno/index';
                }
                else {
                    alert(response.Message);
                }
            }).error(function () {
                alert(_messages.errorTryAgain);
            });
        });
    });
});