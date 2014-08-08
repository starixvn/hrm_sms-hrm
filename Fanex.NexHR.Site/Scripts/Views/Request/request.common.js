'use strict';
/*global _global*/

var RequestCommon = (function ($) {
    return {
        showErrorMsg: function (msg, htmlObject) {
            $('.validation-summary-valid').show();
            $('.validation-summary-valid').addClass('validation-summary-errors');
            var msgWrp = $('.validation-summary-valid').find('ul:first');
            msgWrp.append("<li>" + RequestCommon.htmlDecode(msg) + "</li>");
            htmlObject.addClass("input-validation-error");
        },
        removeErrorMsg: function (isReset) {
            if (isReset) {
                $(".validation-summary-errors").addClass("validation-summary-valid").removeClass("validation-summary-errors");
            }
            var msgWrp = $('.validation-summary-valid').removeClass('validation-summary-errors');
            msgWrp.find('ul:first').html("");
            $('.validation-summary-valid').hide();
        },
        showSuccessMsg: function (msg, url) {
            $('.validation-summary-valid').show();
            var msgWrp = $('.validation-summary-valid').addClass('success').find('ul:first');
            msgWrp.append("<li>" + RequestCommon.htmlDecode(msg) + "</li>");
            confirm('Successfull!\nPress OK to continue', function () {
                location.href = _global.root + url;
            });
        },
        htmlDecode: function (value) {
            return $('<div/>').html(value).text();
        }
    };
} (jQuery));
