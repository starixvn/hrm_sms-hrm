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
        title: 'SMS HRM',
        modal: true,
        resizable: false,
        buttons: [
            {
                text: 'OK',
                class: 'btn btn-primary',
                click: function () {
                    $(this).dialog('close');
                    if (callback !== null) {
                        callback();
                    }

                    return true;
                }
            },
            {
                text: 'Cancel',
                class: 'btn btn-default',
                click: function () {
                    $(this).dialog('close');
                    return false;
                }
            }
        ],
        minWidth: 300,
        minHeight: 100,
        dialogClass: 'modal-shadow'
    };

    let $confirm = $.getOrCreateDialog('confirm');
    // set message
    $('p', $confirm).html(message);
    // init dialog
    $confirm.dialog($.extend({}, defaults, options));
}

function alert(message, callback) {
    const defaults = {
        title: 'SMSHRM',
        modal: true,
        resizable: false,
        buttons: {
            OK: function () {
                $(this).dialog('close');
                if (callback) callback();
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

function reorder(items, startIndex, endIndex) {
    const [removed] = items.splice(startIndex, 1);
    items.splice(endIndex, 0, removed);

    return [...items];
}

function floatThead(el) {
    $(el).floatThead({
        scrollContainer: function ($table) {
            return $table.closest('.content_scroll');
        },
        position: 'fixed',
        top: 72
    });

    $('#toggle-left-navigation').on('click', function () {
        $(el).floatThead('reflow');
    });
}

function initFromDateOnChange($fromDate, $toDate) {
    if ($fromDate && $toDate) {
        $fromDate.on("change", function () {
            let fromDateValue = $(this).val();
            $toDate.val(fromDateValue);
        });
    }
}

function initDatepicker() {
    let checkBadDates = function (mydate) {
        let isEnable = true;
        let disabledDates = $('.datepicker').attr('disabledDate');
        let myBadDates = disabledDates.split(",");

        let checkdate = new Date(mydate).format('dd mmm, yyyy');
        for (let i = 0; i < myBadDates.length; i++) {
            let date = new Date(myBadDates[i]).format('dd mmm, yyyy');
            if (date === checkdate) {
                isEnable = false;
            }
        }

        return [isEnable];
    };
    // get minDate, maxDate, disabledDates
    let mindate = $('.datepicker').attr('mindate');
    let maxdate = $('.datepicker').attr('maxdate');
    let disabledDates = $('.datepicker').attr('disabledDate');

    if (disabledDates.length > 0) {
        $('.datepicker').datepicker({
            dateFormat: 'dd M, yy',
            beforeShowDay: checkBadDates,
            changeMonth: true,
            changeYear: true
        });
    }
    else {
        $('.datepicker').datepicker({
            dateFormat: 'dd M, yy',
            changeMonth: true,
            changeYear: true
        });
    }

    if (mindate !== 'null') {
        mindate = mindate.split('/');
        $('.datepicker').datepicker('option', 'minDate', new Date(mindate[0], mindate[1] - 1, mindate[2]));
    }

    if (maxdate !== 'null') {
        maxdate = maxdate.split('/');
        $('.datepicker').datepicker('option', 'maxDate', new Date(maxdate[0], maxdate[1] - 1, maxdate[2]));
    }

    $('.datepicker').attr('readonly', 'readonly');
    $('.datepicker').keydown(function (e) {
        const tab = 9, enter = 13;
        if (e.keyCode === tab || e.keyCode === enter) {
            return true;
        }

        return false;
    });

    $('.icon-calendar').on('click', function () {
        let $el = $(this).closest('.calendar-form').find('.datepicker');

        if ($el.datepicker('widget').is(':visible')) {
            $el.datepicker('hide');
        } else {
            $el.datepicker('show');
        }
    });
}

function setDateRange(selectedIndex) {
    let fromDate = new Date($('#FromDate').val()).toString('dd MMM, yyyy');
    let toDate = new Date($('#ToDate').val()).toString('dd MMM, yyyy');
    let now = $('#Today').val();

    switch (parseInt(selectedIndex, 10)) {
        case 0: //default
            break;

        case 1: //today
            toDate = fromDate = Date.parse(now).toString('dd MMM, yyyy');
            break;

        case 2: // yesterday
            toDate = fromDate = Date.parse(now).add(-1).days().toString('dd MMM, yyyy');
            break;

        case 3: //current week
            fromDate = Date.parse(now).moveToDayOfWeek(0, -1).toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;

        case 4: //last week
            fromDate = Date.parse(now).moveToDayOfWeek(0, -1).add(-7).days().toString('dd MMM, yyyy');
            toDate = Date.parse(now).moveToDayOfWeek(0, -1).add(-1).days().toString('dd MMM, yyyy');
            break;

        case 5: //current month
            fromDate = Date.parse(now).moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;

        case 6: //last month
            fromDate = Date.parse(now).add(-1).months().moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).add(-1).months().moveToLastDayOfMonth().toString('dd MMM, yyyy');
            break;

        case 7: //since last month
            fromDate = Date.parse(now).add(-1).months().moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
    }

    $('#FromDate').val(fromDate);
    $('#ToDate').val(toDate);
}

export {
    showMessage,
    fadeOutMessage,
    confirm,
    updateTableHeaders,
    floatingHeader,
    blockUI,
    alert,
    unblockUI,
    toastOptions,
    axiosError,
    reorder,
    floatThead,
    initDatepicker,
    initFromDateOnChange,
    setDateRange
};