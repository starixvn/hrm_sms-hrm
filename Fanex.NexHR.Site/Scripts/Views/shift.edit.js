'use strict';
/*global _global*/

var editShift = (function ($) {
    var me = {};
    var initEvent = function () {
        $(".js-link-delete-shift").click(function () {
            var $self = $(this);
            var shilfID = $self.data("id");
            confirm('Are you sure you want to delete this shift?', function () {
                $.post(_global.root + 'shift/delete', { shiftId: shilfID }, function (response) {
                    if (response.IsSuccessful) {
                        document.location = _global.root + "shift/index";
                    }
                    else {
                        alert(response.Message);
                    }
                });
            });
        });

        $('form').bind('submit', function () {
            $(this).find(':input').removeAttr('disabled');
        });
    };

    me.init = function () {
        if ($("#IsUsed").val() === "True") {
            $("#Name").attr("disabled", "disabled");
            $("#HourTimeIn").attr("disabled", "disabled");
            $("#MinuteTimeIn").attr("disabled", "disabled");
            $("#HourTimeOut").attr("disabled", "disabled");
            $("#MinuteTimeOut").attr("disabled", "disabled");
            $("#IsInAfterDay").attr("disabled", "disabled");
            $("#IsOutAfterDay").attr("disabled", "disabled");
        }

        initEvent();
    };

    return me;
}(jQuery));

$(document).ready(function () {
    editShift.init(_global.root);
});