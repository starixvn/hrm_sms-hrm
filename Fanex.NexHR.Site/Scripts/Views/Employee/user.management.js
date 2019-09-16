'use strict';
/*global _global, _messages, floatThead, getParam*/

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

    function checkDisabledUser() {
        if ($("#disabled").prop("checked")) {
            $('table#data .resignedDate').show();
        } else {
            $('table#data .resignedDate').hide();
        }
    }

    function query() {
        $('#GroupIds').val($('#Section').multiSelectTreeView('getSelectedItems'));

        $.ajax({
            url: _global.root + 'employee/ListEmployee',
            type: 'POST',
            dataType: 'html',
            data: $('form.form-filter').serialize(),
            success: function (response) {
                $('#user-management').html(response);
                checkDisabledUser();

                $('.nick').tooltip({
                    track: true,
                    delay: 0,
                    showURL: false,
                    bodyHandler: function () {
                        var path = _global.root + 'employee/avatar/' + $(this).attr('data-code');
                        return '<img width=80px; height=80px; src="' + path + '" />';
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
        var url = null,
            controllerPath;

        if (object.attr("id") === "basicExcel") {
            controllerPath = "ExportEmployee/EmployeeBasicInfo?";
        }
        if (object.attr("id") === "detailsExcel") {
            controllerPath = "ExportEmployee/EmployeeDetailsInfo?";
        }

        $('#GroupIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        url = _global.root + controllerPath + $('form.form-filter').serialize();
        location.href = url;
    }

    $(document).ready(function () {
        $.ajax({
            url: _global.root + 'Department/GetHierarchy',
            success: function (data) {
                $('#Section').multiSelectTreeView({
                    data: data,
                    selectedItems: $('#GroupIds').val()
                });

                query();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });

        var jobTitleId = $('#JobTitleId').val();
        var dropDownTittle = $('#title');

        dropDownTittle.val(jobTitleId);
        dropDownTittle.chosen();
        dropDownTittle.trigger("liszt:updated");

        $('#Keyword').keyup(function (event) {
            if (event.keyCode === 13) {
                query();
            }
        });

        var q = getParam('query');
        if (q !== null) {
            var decode = decodeURIComponent(q);
            $('#Keyword').val(decode);
        }

        $('#title').on('change', function () {
            $('#JobTitleId').val($(this).val());
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

        $('#enabled').on('click', function () {
            $("#IsEnable").val(true);
            $("#IsDelete").val(null);
            $('#deleted').prop('checked', false);
            $('#disabled').prop('checked', false);
        });

        $('#disabled').on('click', function () {
            $("#IsEnable").val(false);
            $("#IsDelete").val(null);
            $('#deleted').prop('checked', false);
            $('#enabled').prop('checked', false);
        });

        $('#deleted').on('click', function () {
            $("#IsDelete").val(true);
            $("#IsEnable").val(null);
            $('#enabled').prop('checked', false);
            $('#disabled').prop('checked', false);
        });

        $('#query').on("click", function (evt) {
            evt.preventDefault();
            $('form.form-filter #CurrentPage').val('1');
            query();
        });

        $("#basicExcel").on('click', function () {
            exportExcel($(this));
        });

        $("#detailsExcel").on('click', function () {
            exportExcel($(this));
        });
    });
})();