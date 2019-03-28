import $ from 'jquery/dist/jquery';

$(document).ready(function () {
    function toggleLeftNavigation(isShow) {
        var currentElement = $("#toggle-left-navigation");

        if (isShow) {
            currentElement.removeClass("large-next");
            currentElement.addClass("large-back");
            currentElement.attr({ title: 'Hide Left Navigation' });
            $("#left-navigation").show();
            $("#content-wrapper").removeClass("expand");
        }
        else {
            currentElement.removeClass("large-back");
            currentElement.addClass("large-next");
            currentElement.attr({ title: 'Show Left Navigation' });
            $("#left-navigation").hide();
            $("#content-wrapper").addClass("expand");
        }
    }

    function updateLeftNavigationSetting(isShow) {
        $.ajax({
            type: "POST",
            url: '/Employee/ToogleLeftNavigation',
            data: { isShow: isShow }
        });
    }

    if (_global.isShowLeftNavigation === "True") {
        toggleLeftNavigation(true);
    }
    else {
        toggleLeftNavigation(false);
    }

    $("#toggle-left-navigation").bind("click", function () {
        var currentElement = $(this);
        if (currentElement.hasClass("large-back")) {
            toggleLeftNavigation(false);
            updateLeftNavigationSetting(false);
        }
        else {
            toggleLeftNavigation(true);
            updateLeftNavigationSetting(true);
        }

        $(".content_scroll").scroll(updateTableHeaders).trigger("scroll");
        $(window).resize(function () {
            updateTableHeaders();
        });
    });

    var selectNavItem = $("#" + $('#CurrentUrl').val());
    if (selectNavItem !== undefined) {
        selectNavItem.addClass("selected");
    }

    //show/hide navigation secion
    $("div.togglable_section").bind("click", function () {
        var currentSection = $(this);
        var divItems = currentSection.next("div.navItem");
        if (currentSection.hasClass("folded")) {
            currentSection.removeClass("folded");
            divItems.show(200);
        }
        else {
            currentSection.addClass("folded");
            divItems.hide(200);
        }
    });

    //add style for first item section
    $("div.togglable_section:first-child").children(":first").addClass("first");

    $('.el[data-hashovermenu=True], .el[data-hashovermenu=data-hashovermenu]').hover(
        function () {
            $(this).find('.hoveraction').fadeIn(400);
        }, function () {
            $(this).find('.hoveraction').hide();
        });
});