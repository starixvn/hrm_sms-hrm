'use strict';
/*global _global, _messages, floatThead*/

(function () {
    function initPagination() {
        var currentPage = $('table#data #pagination #CurrentPage').val(),
            itemsOnPage = $('table#data #pagination #ItemsPerPage').val(),
            totalItem = $('table#data #pagination #TotalItem').val();

        currentPage = currentPage ? parseInt(currentPage) : 1;
        itemsOnPage = itemsOnPage ? parseInt(itemsOnPage) : 1;
        totalItem = totalItem ? parseInt(totalItem) : 1;

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

    function query() {
        $.ajax({
            url: _global.root + 'employee/ContactList',
            type: 'POST',
            dataType: 'html',
            data: $('form.form-filter').serialize(),
            success: function (response) {
                $('#contact-list').html(response);

                $('.nick').tooltip({
                    track: true,
                    delay: 0,
                    showURL: false,
                    bodyHandler: function () {
                        var path = _global.root + 'employee/avatar/' + $(this).attr('data-code');
                        return '<img width=64px; height=64px; src="' + path + '" />';
                    }
                });

                if ($('table#data #pagination').length > 0) {
                    initPagination();

                    $('#ItemsPerPageOption').on('change', function () {
                        $('form.form-filter #ItemsPerPage').val($(this).val());
                        $('form.form-filter #CurrentPage').val(1);
                        query();
                    });
                }

                floatThead('table#data');
            },
            error: function () {
                alert(_messages.errorTryAgain);
            }
        });
    }

    function exportExcel(object) {
        var jobTitleId = $('#title').val(),
            groups = $("#group").val() !== null ? $("#group").val().join() : "",
            keyword = $('#Keyword').val(),
            url = null;

        var controllerPath;
        if (object.attr("id") === "basicExcel") {
            controllerPath = "export/EmployeeBasicInfo?query=";
        }
        if (object.attr("id") === "detailsExcel") {
            controllerPath = "export/EmployeeDetailsInfo?query=";
        }

        url = _global.root + controllerPath + keyword + "&groupIds=" + groups +
            "&enable=" + enable + "&isDelete=" + isDelete + "&titleId=" + jobTitleId + "&isLocal=" + isLocal;

        location.href = url;
    }

    $(document).ready(function () {
        $("#group").multiselect();

        $('#group').on('change', function () {
            var groupIds = $(this).val() !== null ? $(this).val().join(',') : '';
            $('#GroupIds').val(groupIds);
        });

        $('#category').on('change', function () {
            switch ($(this).val()) {
                case '0':
                    $('#IsLocal').val(false);
                    break;
                case '1':
                    $('#IsLocal').val(true);
                    break;
                default:
                    $('#IsLocal').val(null);
                    break;
            }
        });

        query();

        $('#query').on("click", function (evt) {
            evt.preventDefault();
            $('form.form-filter #CurrentPage').val('1');
            query();
        });
    });
})();