import $ from 'jquery/dist/jquery';
import { updateTableHeaders } from './shared/common.functions';

$(document).ready(function () {
    function toggleLeftNavigation(isShow) {
        let currentElement = $('#toggle-left-navigation');

        if (isShow) {
            currentElement.removeClass('large-next');
            currentElement.addClass('large-back');
            currentElement.attr({ title: 'Hide Left Navigation' });
            $('#left-navigation').show();
            $('#content-wrapper').removeClass('expand');
        } else {
            currentElement.removeClass('large-back');
            currentElement.addClass('large-next');
            currentElement.attr({ title: 'Show Left Navigation' });
            $('#left-navigation').hide();
            $('#content-wrapper').addClass('expand');
        }
    }

    function updateLeftNavigationSetting(isShow) {
        $.ajax({
            type: 'POST',
            url: _global.root + 'Employee/ToogleLeftNavigation',
            data: { isShow: isShow }
        });
    }

    if (_global.isShowLeftNavigation === 'True') {
        toggleLeftNavigation(true);
    } else {
        toggleLeftNavigation(false);
    }

    $('#searchIcon').on('click', function () {
        document.location = _global.root + 'employee/index?query=' + $('#searchbox').val();
    });

    $('#searchbox').keyup(function (event) {
        if (event.keyCode === 13) {
            $('#searchIcon').trigger('click');
        }
    });

    $('#toggle-left-navigation').bind('click', function () {
        let currentElement = $(this);
        if (currentElement.hasClass('large-back')) {
            toggleLeftNavigation(false);
            updateLeftNavigationSetting(false);
        } else {
            toggleLeftNavigation(true);
            updateLeftNavigationSetting(true);
        }

        $('.content_scroll').scroll(updateTableHeaders).trigger('scroll');
        $(window).resize(function () {
            updateTableHeaders();
        });
    });

    const currentUrlSelected = $('#CurrentUrlSelected').val();
    if (currentUrlSelected) {
        $('#' + currentUrlSelected).addClass('selected');
    }

    //show/hide navigation section
    $('div.togglable_section').bind('click', function () {
        let currentSection = $(this);
        let divItems = currentSection.next('div.navItem');
        if (currentSection.hasClass('folded')) {
            currentSection.removeClass('folded');
            divItems.show(200);
        } else {
            currentSection.addClass('folded');
            divItems.hide(200);
        }
    });

    // add style for first item section
    $('div.togglable_section:first-child').children(':first').addClass('first');

    $('.el[data-hashovermenu=True], .el[data-hashovermenu=data-hashovermenu]').hover(
        function () {
            $(this).find('.hoveraction').fadeIn(400);
        }, function () {
            $(this).find('.hoveraction').hide();
        }
    );
});