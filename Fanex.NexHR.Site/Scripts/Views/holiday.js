'use strict';
/*global _global, autoIndex*/

var Holiday = (function ($) {
    var me = {};
    var $year;
    var urlRoot = "/";

    var initEvent = function () {
        $(".js-link-delete-holiday").click(function () {
            var $self = $(this);
            var holidayId = $self.data("id");

            confirm('Are you sure you want to delete this holiday?', function () {
                $.post(urlRoot + 'Holiday/Delete', { id: holidayId }, function (response) {
                    if (response.IsSuccessful) {
                        $('#row' + holidayId).remove();
                        autoIndex();
                    }
                    else {
                        alert(response.Message);
                    }
                });
            });
        });

        $('#submit').click(function () {
            location.href = urlRoot + "Holiday/Index?year=" + $year.val();
        });
    };

    me.init = function (globalRoot) {
        $year = $("#year");
        urlRoot = globalRoot;

        initEvent();
    };

    return me;
}(jQuery));

$(document).ready(function () {
    Holiday.init(_global.root);
});