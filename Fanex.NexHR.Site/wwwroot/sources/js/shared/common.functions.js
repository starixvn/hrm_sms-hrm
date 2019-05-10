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

function alert(message) {
    const defaults = {
        title: 'SMSHRM',
        modal: true,
        resizable: false,
        buttons: {
            OK: function () {
                $(this).dialog('close');
                return false;
            }
        },
        minHeight: 50,
        dialogClass: 'modal-shadow'
    };

    let $alert = $.getOrCreateDialog('alert');
    // set message
    $('p', $alert).html(message);
    // init dialog
    $alert.dialog($.extend({}, defaults));
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
        $('table.persist-area thead th').each(function () {
            let x = $(this).width();
            $($('table.persist-area .floatingHeader th')[i]).width(x);
            i++;
        });
    }
}

function floatingHeader() {
    let clonedHeaderRow;
    $('.persist-area').each(function () {
        clonedHeaderRow = $('thead', this);
        clonedHeaderRow
            .before(clonedHeaderRow.clone())
            .css('width', clonedHeaderRow.width())
            .addClass('floatingHeader');
    });

    $('.content_scroll')
        .scroll(updateTableHeaders)
        .trigger('scroll');
    $(window).resize(function () {
        updateTableHeaders();
    });
}

function blockUI() {
    document.getElementById('loadingProcess').style.display = 'block';
}

function unblockUI() {
    document.getElementById('loadingProcess').style.display = 'none';
}

function toastOptions() {
    return {
        tapToDismiss: false,
        closeButton: true,
        progressBar: true,
        positionClass: 'toast-top-center',
        showDuration: 0,
        hideDuration: 0,
        timeOut: 3000,
        extendedTimeOut: 0,
        closeOnHover: false,
        preventDuplicates: true,
        closeHtml: '<button type="button">x</button>'
    };
}

function axiosError(error) {
    if (error.response) {
        let url;

        switch (error.response.status) {
            case 401:
                url = 'auth/login';
                break;

            case 404:
                url = 'error/PageNotFound';
                break;

            case 405:
                url = 'error/AccessDenied';
                break;

            case 500:
                url = 'error/InternalError';
                break;
        }

        window.location = _global.root + url;
    }
}

export { showMessage, fadeOutMessage, confirm, alert, updateTableHeaders, floatingHeader, blockUI, unblockUI, toastOptions, axiosError };