'use strict';
/*global _global, autoIndex*/

var Memo = (function ($) {
    var me = {};
    var urlRoot = "/";

    var initEvent = function () {
        $(".js-table-memo").on("click", ".js-link-delete-memo", function () {
            var $self = $(this);
            var memoId = $self.data("id");

            confirm('Are you sure you want to delete this memo?', function () {
                $.post(_global.root + 'Memo/deletememo', { id: memoId }, function (response) {
                    if (response.IsSuccessful) {
                        $('#row_' + memoId).remove();
                        autoIndex();
                    }
                    else {
                        alert(response.Message);
                    }
                });
            });
        });
    };

    me.init = function (globalUrl) {
        urlRoot = globalUrl;

        initEvent();
    };

    return me;
}(jQuery));

$(document).ready(function () {
    Memo.init();
});