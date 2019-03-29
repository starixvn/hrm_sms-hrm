import $ from 'jquery/dist/jquery';

$.extend({
    getOrCreateDialog: function (id) {
        var $box = $('#' + id);
        if (!$box.length) {
            $box = $('<div id="' + id + '"><p></p></div>').hide().appendTo('body');
        }

        return $box;
    }
});

function showMessage(message) {
    $('#message').html(message + ' <img src="/images/ajax-loader.gif" />').css('left', $(window).width() / 2);
    $('#message').slideDown();
}

function fadeOutMessage() {
    $('#message').fadeOut(500);
}

function confirm(message, callback, options) {
    const defaults = {
        title: 'SMSHRM',
        modal: true,
        resizable: false,
        buttons: {
            OK: function () {
                $(this).dialog('close');
                if (callback !== null) {
                    callback();
                }

                return true;
            },
            Cancel: function () {
                $(this).dialog('close');
                return false;
            }
        },
        minHeight: 50,
        dialogClass: 'modal-shadow'
    };

    let $confirm = $.getOrCreateDialog('confirm');
    // set message
    $('p', $confirm).html(message);
    // init dialog
    $confirm.dialog($.extend({}, defaults, options));
}

function updateTableHeaders() {
    if ($('.persist-area').val() !== undefined) {
        let widthspace = $('.persist-area').width() + 18;
        widthspace = widthspace - 17;
        if (widthspace < 1051) {
            widthspace = 1051;
        }

        $('.floatingHeader').width(widthspace);

        const isShowLeftMenu = $('#toggle-left-navigation').hasClass('large-next');

        $('.persist-area').each(function () {
            const el = $(this);
            const offset = el.offset();
            const scrollTop = $(window).scrollTop();
            let floatingHeader = $('.floatingHeader', this);
            if ((scrollTop > offset.top - 60) && (scrollTop < offset.top + el.height())) {
                if (!isShowLeftMenu) {
                    floatingHeader.css({
                        'visibility': 'visible'
                    });
                } else {
                    floatingHeader.css({
                        'visibility': 'visible'
                    });
                }
            } else {
                floatingHeader.css({
                    'visibility': 'hidden'
                });
            }
        });

        let i = 0;
        $('table .persist-header th').each(function () {
            let x = $(this).width();
            $($('table .floatingHeader th')[i]).width(x);
            i++;
        });
    }
}

function floatingHeader() {
    let clonedHeaderRow;
    $('.persist-area').each(function () {
        clonedHeaderRow = $('.persist-header', this);
        clonedHeaderRow
            .before(clonedHeaderRow.clone())
            .css('width', clonedHeaderRow.width())
            .addClass('floatingHeader')
            .removeClass('persist-header');
    });

    $('.content_scroll')
        .scroll(updateTableHeaders)
        .trigger('scroll');
    $(window).resize(function () {
        updateTableHeaders();
    });
}

export { showMessage, fadeOutMessage, confirm, updateTableHeaders, floatingHeader };