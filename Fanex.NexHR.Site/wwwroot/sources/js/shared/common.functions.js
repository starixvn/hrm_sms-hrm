function updateTableHeaders() {
    if ($('.persist-area').val() !== undefined) {
        var leftspace = 191;
        var widthspace = $('.persist-area').width() + 18;
        widthspace = widthspace - 17;
        if (widthspace < 1051)
            widthspace = 1051;

        $('.floatingHeader').width(widthspace);
        var left = $('table.persist-area thead tr:first').position()['left'];

        var isShowLeftMenu = $('#toggle-left-navigation').hasClass('large-next');

        $('.persist-area').each(function () {
            var el = $(this),
                offset = el.offset(),
                scrollTop = $(window).scrollTop(),
                floatingHeader = $('.floatingHeader', this);
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

        var i = 0;
        $('table .persist-header th').each(function () {
            var x = $(this).width();
            $($('table .floatingHeader th')[i]).width(x);
            i++;
        });
    }
}

function floatingHeader() {
    var clonedHeaderRow;
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

export { updateTableHeaders, floatingHeader };