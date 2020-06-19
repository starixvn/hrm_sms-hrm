import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import '../../shared/multiselect-tree-view';
import '../../../../sources/libs/SimplePagination/jquery.simplePagination.js';
import '../../shared/employee.search.in.team';
import { blockUI, floatThead, unblockUI, alert, getParam, resolveClientUrl } from '../../shared/common.functions';

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
                $('form.filter #CurrentPage').val(pageNumber);
                query();
            }
        });
    }

    function query() {
        blockUI();
        $.ajax({
            url: resolveClientUrl('employee/ContactList'),
            type: 'POST',
            dataType: 'html',
            data: $('form.filter').serialize(),
            success: function (response) {
                unblockUI();
                $('#contact-list').html(response);
                initTipsyTooltip();
                if ($('table#data #pagination').length > 0) {
                    initPagination();
                    $('#ItemsPerPageOption').on('change', function () {
                        $('form.filter #ItemsPerPage').val($(this).val());
                        $('form.filter #CurrentPage').val(1);
                        query();
                    });
                }

                floatThead('table#data');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                unblockUI();
                alert(textStatus + ': ' + errorThrown);
            }
        });
    }

    function initTipsyTooltip() {
        $('.nick').tipsy({
            html: true,
            fade: false,
            gravity: 'w',
            className: 'nick-tooltip',
            live: false,
            title: function () {
                var path = resolveClientUrl('employee/avatar/' + $(this).attr('data-code'));
                return '<img width=80px; height=80px; src="' + path + '" />';
            }
        });

        $('.tooltip-tipsy').tipsy({
            html: true,
            gravity: 'w'
        });
    }

    $(document).ready(function () {
        $.ajax({
            url: resolveClientUrl('Section/GetOrgChart'),
            success: function (data) {
                $('#Team').multiSelectTreeView({
                    data: data,
                    isSelectedAll: true
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
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

        var q = getParam('query');
        if (q !== null) {
            var decode = decodeURIComponent(q);
            $('#Keyword').val(decode);
        }

        $('#Keyword').keydown(function (event) {
            // enter
            if (event.keyCode === 13) {
                query();
            }
        });

        query();

        $('form').on("submit", function (evt) {
            evt.preventDefault();
            $('form.filter #CurrentPage').val('1');
            $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
            query();
        });
    });
})();