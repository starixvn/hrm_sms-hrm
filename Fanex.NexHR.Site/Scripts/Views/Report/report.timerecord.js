'use strict';
/*global _global, _messages, fadeOutMessage, showMessage, floatThead*/

(function () {
    function initPagination() {
        var currentPage = $('table#data #pagination #CurrentPage').val(),
            itemsOnPage = $('table#data #pagination #ItemsPerPage').val(),
            totalItem = $('table#data #pagination #TotalItem').val();

        currentPage = currentPage ? parseInt(currentPage, 10) : 1;
        itemsOnPage = itemsOnPage ? parseInt(itemsOnPage, 10) : 1;
        totalItem = totalItem ? parseInt(totalItem, 10) : 1;

        $('table#data #pagination #numberlist').pagination({
            items: totalItem,
            itemsOnPage: itemsOnPage,
            currentPage: currentPage,
            displayedPages: 3,
            prevText: '<i class="fa fa-angle-left"></i>',
            nextText: '<i class="fa fa-angle-right"></i>',
            ellipsePageSet: false,
            cssStyle: 'compact-theme',
            onPageClick: function (pageNumber) {
                $('form.form-filter #CurrentPage').val(pageNumber);
                query();
            }
        });
    }

    function processInOut(inoutType, eventElement) {
        var recordTimeId = $(eventElement).attr('data-id');
        showMessage('Processing');

        $.post(_global.root + 'ajax/ProcessInOut',
        { inoutType: inoutType, recordTimeId: recordTimeId },
        function (response) {
            if (response.IsSuccessful) {
                query();
            } else {
                alert(response.Message);
                return;
            }
        });
        fadeOutMessage();
    }

    function bindMenu(element, menu) {
        $(element).contextMenu({
            menu: menu
        },
        function (command, eventElement) {
            switch (command) {
                case 'in':
                    confirm('Are you sure you change to in?', function () {
                        processInOut(0, eventElement);
                    });
                    break;
                case 'out':
                    confirm('Are you sure you change to out?', function () {
                        processInOut(1, eventElement);
                    });
                    break;
            }
        });
    }

    function query() {
        $.ajax({
            url: _global.root + 'Report/TimeRecordDetail',
            type: 'POST',
            dataType: 'html',
            data: $('form.form-filter').serialize(),
            success: function (data) {
                $('#TimeRecordContent').html(data);

                if ($('table#data #pagination').length > 0) {
                    bindMenu('.in-action', 'inMenu');
                    bindMenu('.out-action', 'outMenu');

                    if ($('table#data #pagination').length > 0) {
                        initPagination();

                        $('#ItemsPerPageOption').on('change', function () {
                            $('form.form-filter #ItemsPerPage').val($(this).val());
                            $('form.form-filter #CurrentPage').val(1);
                            query();
                        });
                    }
                    floatThead('table#data');
                }
            },
            error: function () {
                alert(_messages.errorTryAgain);
            }
        });
    }

    $(document).ready(function () {
        $('#TeamIds').multiselect();

        $('#TeamIds').on('change', function () {
            var teamIds = $(this).val() !== null ? $(this).val().join(',') : '';
            $('#TeamIds').val(teamIds);
        });

        $('#Employee').change(function () {
            if ($.trim($(this).val()).length === 0) {
                $(this).val('');
            }

            return false;
        });

        $('#btnSubmit').on('click', function () {
            $('form.form-filter #CurrentPage').val(1);
            query();
        });

        query();
    });
})();