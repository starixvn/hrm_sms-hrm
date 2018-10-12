'use strict';
/*global _global, fadeOutMessage, showMessage*/
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

function floatingHeader() {
    var clonedHeaderRow;
    $(".persist-area").each(function () {
        clonedHeaderRow = $(".persist-header", this);
        clonedHeaderRow
            .before(clonedHeaderRow.clone())
            .css("width", clonedHeaderRow.width())
            .addClass("floatingHeader")
            .removeClass("persist-header");
    });

    $(".content_scroll")
        .scroll(updateTableHeaders)
        .trigger("scroll");
    $(window).resize(function () {
        updateTableHeaders();
    });
}

function confirm(message, callback, options) {
    var defaults = {
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

    var $confirm = $.getOrCreateDialog('confirm');
    // set message
    $("p", $confirm).html(message);
    // init dialog
    $confirm.dialog($.extend({}, defaults, options));
}

function updateTableHeaders() {
    if ($(".persist-area").val() !== undefined) {
        var leftspace = 191;
        var widthspace = $(".persist-area").width() + 18;
        //if ($.browser.safari) {
        //    leftspace = 192;
        //    widthspace = widthspace - 8;
        //    if (widthspace < 1060)
        //        widthspace = 1060;
        //}
        //else {
        widthspace = widthspace - 17;
        if (widthspace < 1051)
            widthspace = 1051;
        // }

        $('.floatingHeader').width(widthspace);
        var left = $('table.persist-area thead tr:first').position()["left"];

        var isShowLeftMenu = $("#toggle-left-navigation").hasClass("large-next");
        var widthLeftMenu = isShowLeftMenu ? 190 : 0;

        $(".persist-area").each(function () {
            var el = $(this),
                    offset = el.offset(),
                    scrollTop = $(window).scrollTop(),
                    floatingHeader = $(".floatingHeader", this);
            if ((scrollTop > offset.top - 60) && (scrollTop < offset.top + el.height())) {
                if (!isShowLeftMenu) {
                    floatingHeader.css({
                        "visibility": "visible",
                        "left": (leftspace + left)
                    });
                }
                else {
                    floatingHeader.css({
                        "visibility": "visible",
                        "left": left + 1
                    });
                }
            } else {
                floatingHeader.css({
                    "visibility": "hidden"
                });
            }
        });

        var i = 0;
        $("table .persist-header th").each(function () {
            var x = $(this).width();
            $($("table .floatingHeader th")[i]).width(x);
            i++;
        });
    }
}

////////////////////////////////////////////////////////////////////////////

$(function () {
    //Handle event when click search icon
    $("#searchIcon").on('click', function () {
        document.location = _global.root + 'employee/index?query=' + $("#searchbox").val();
    });

    $('#searchbox').keyup(function (event) {
        if (event.keyCode === 13) {
            $("#searchIcon").trigger('click');
        }
    });

    function processRequest(requestId, roleId, command, comment, ignoredDetails) {
        var model = {
            RequestId: requestId,
            RoleId: roleId,
            Command: command,
            Comment: comment,
            IgnoredDisciplineDetails: JSON.stringify(ignoredDetails)
        };

        $.ajax({
            type: 'POST',
            url: _global.root + 'request/ProcessRequest',
            data: model,
            dataType: 'json',
            beforeSend: function () {
                showMessage('Processing');
            },
            success: function (response) {
                if (response.Code !== 0) {
                    alert(response.Message);
                } else {
                    window.location.reload();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            },
            complete: function () {
                fadeOutMessage();
            }
        });
    }

    function initDialogProcessRequest(requestId, roleId, employeeId, command, title) {
        $('#dialogProcessRequest').dialog({
            title: title,
            height: 'auto',
            width: 'auto',
            modal: true,
            buttons: {
                'Confirm': function () {
                    var ignoredDisciplines = [],
                        comment = $(this).find('textarea#Comment').val(),
                        isValid = true;

                    $(this).find("table.table tbody tr").each(function () {
                        var chkIgnored = $(this).find('td.violationIgnored input:radio'),
                            chkNotIgnored = $(this).find('td.violationRecorded input:radio');

                        if (chkIgnored.length === 1) {
                            if (!chkIgnored.prop('checked') && !chkNotIgnored.prop('checked')) {
                                isValid = false;
                            } else {
                                ignoredDisciplines.push({
                                    WorkDate: chkIgnored.data('workdate'),
                                    EmployeeId: employeeId,
                                    DisciplineTypeId: chkIgnored.data('disciplinetypeid'),
                                    IsIgnored: chkIgnored.prop('checked')
                                });
                            }
                        }
                    });

                    if (!isValid) {
                        $(this).find('.validation-summary-valid').show()
                            .html('You must select <b>Violation Ignored</b> or <b>Violation Recorded</b> on each type of request.');
                    } else {
                        $(this).find('.validation-summary-valid').hide();
                        processRequest(requestId, roleId, command, comment, ignoredDisciplines);
                        $(this).dialog('close');
                    }
                },
                'Cancel': function () {
                    $(this).dialog('close');
                }
            }
        });
    }

    function renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title) {
        var model = {
            RequestId: requestId,
            RoleId: roleId,
            RequestTypeId: requestTypeId,
            EmployeeId: employeeId,
            Command: command
        };

        $.ajax({
            type: 'GET',
            url: _global.root + 'request/PartialViewProcessRequest',
            data: model,
            dataType: 'html',
            success: function (data) {
                $('#dialogProcessRequest').html(data);

                initDialogProcessRequest(requestId, roleId, employeeId, command, title);

                $('#dialogProcessRequest').find("table.table tbody tr").each(function () {
                    var chkIgnored = $(this).find('td.violationIgnored input:radio'),
                        chkNotIgnored = $(this).find('td.violationRecorded input:radio');

                    chkIgnored.on('click', function () {
                        if ($(this).prop('checked')) {
                            chkNotIgnored.prop('checked', false);
                        }
                    });

                    chkNotIgnored.on('click', function () {
                        if ($(this).prop('checked')) {
                            chkIgnored.prop('checked', false);
                        }
                    });
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    }

    function processCommand(command, options) {
        var eventElement = options.$trigger;
        var title,
            requestTypeId = parseInt((eventElement).attr('requestTypeId'), 10),
            requestId = parseInt((eventElement).attr('requestId'), 10),
            roleId = parseInt((eventElement).attr('roleId'), 10),
            employeeId = parseInt((eventElement).attr('employeeId'), 10);
        switch (command) {
            case 'Accept':
                title = 'Are you sure you want to verify?';
                renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title);
                break;
            case 'NotAccept':
                title = 'Are you sure you dont want to verify?';
                renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title);
                break;
            case 'Approve':
                title = 'Are you sure you want to approve?';
                renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title);
                break;
            case 'Reject':
                title = 'Are you sure you want to reject?';
                renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title);
                break;
            case 'Process':
                title = 'Are you sure you want to process?';
                renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title);
                break;
            case 'Void':
            case 'VoidBySA':
                title = 'Are you sure you want to void?';
                renderPartialViewProcessRequest(requestId, roleId, requestTypeId, employeeId, command, title);
                break;
            case 'Reset':
                confirm('Are you sure you want to reset?', function () {
                    processRequest(requestId, roleId, command);
                });
                break;
        }
    }

    function renderIconOk(opt, $itemElement, itemKey, item) {
        $itemElement.html('<span class="processed"><i class="glyphicon glyphicon-ok"></i>' + item.name + opt.selector + '</span>');
        return '';
    }

    function renderIconRemove(opt, $itemElement, itemKey, item) {
        $itemElement.html('<span class="void"><i class="glyphicon glyphicon-remove"></i>' + item.name + opt.selector + '</span>');
        return '';
    }

    function bindMenu(element, menu) {
        var listItem = {};
        switch (menu) {
            case 'hrMenu':
                listItem = {
                    "Process": { name: "Process", icon: renderIconOk },
                    "Void": { name: "Void", icon: renderIconRemove }
                }
                break;
            case 'verifierMenu':
                listItem = {
                    "Accept": { name: "Verified", icon: renderIconOk },
                    "NotAccept": { name: "Not Verified", icon: renderIconRemove }
                }
                break;
            case 'approverMenu':
                listItem = {
                    "Approve": { name: "Approve", icon: renderIconOk },
                    "Reject": { name: "Reject", icon: renderIconRemove }
                }
                break;
            case 'resetMenu':
                listItem = {
                    "Reset": { name: "Reset", icon: renderIconRemove }
                }
                break;
            case 'voidMenu':
                listItem = {
                    "VoidBySA": { name: "Void", icon: renderIconRemove },
                }
                break;
        }

        $.contextMenu({
            selector: element,
            trigger: 'left',
            callback: processCommand,
            items: listItem
        });
    }

    function initEventExportExcel() {
        $('#btnExcel').click(function () {
            var action = $('#ActionName').val() + 'WorkingTimeRequest',
                teams = $('#TeamId').val() !== null ? $('#TeamId').val().join() : '',
                requestType = $("input[type='radio'][name='RequestType']:checked").val(),
                isLocal = $('#IsLocal').val(),
                isFilterByAffectedDate = $('#IsFilterByAffectedDate').val();
            var url = _global.root + 'ExportRequest/' + action + '?DateOption=' + $('#Date').val() +
                '&TeamIds=' + teams + '&Employee=' + $('#Employee').val() +
                '&EmployeeId=' + $('#EmployeeId').val() +
                '&FromDate=' + $('#FromDate').val() +
                '&ToDate=' + $('#ToDate').val() +
                '&StatusId=' + $('#statusId').val() +
                '&RequestType=' + requestType +
                '&IsLocal=' + isLocal +
                '&IsFilterByAffectedDate=' + isFilterByAffectedDate;
            location.href = url;
        });
    }

    var dontBlock = false;

    $(document).ready(function () {
        $('.tooltip-tipsy').tipsy({ html: true, gravity: 'e' });

        $('#TeamId').multiselect();

        bindMenu('.hr-actions', 'hrMenu');
        bindMenu('.verifier-actions', 'verifierMenu');
        bindMenu('.approver-actions', 'approverMenu');
        bindMenu('.verifier-actions-reset', 'resetMenu');
        bindMenu('.approver-actions-reset', 'resetMenu');
        bindMenu('.hr-actions-reset', 'resetMenu');
        bindMenu('.sa-void', 'voidMenu');

        $('.viewhistory').click(function () {
            var url = $(this).attr('id');
            $.get(url, function (data) {
                $('#dialoghistorycontent').html(data);
                $('#dialoghistorycontent').dialog({
                    modal: true,
                    minHeight: 0,
                    width: 1100,
                    title: 'Request History'
                });
            });
        });

        $('form').submit(function (e) {
            e.preventDefault();
            var from = new Date($('#FromDate').val()),
                to = new Date($('#ToDate').val());
            if (from > to) {
                alert('From Date must be less than To Date');
                $('#submit').removeClass('disabled');
                $('#submit').val('Submit');
                e.preventDefault();
            } else {
                var action = $('#ActionName').val(),
                    teams = $('#TeamId').val() !== null ? $('#TeamId').val().join() : '',
                    requestType = $("input[type='radio'][name='RequestType']:checked").val(),
                    isLocal = $('#IsLocal').val(),
                    isFilterByAffectedDate = $('#IsFilterByAffectedDate').val();
                var url = _global.root + 'request/' + action + '?DateOption=' + $('#Date').val() +
                '&TeamIds=' + teams + '&Employee=' + $('#Employee').val() +
                '&EmployeeId=' + $('#EmployeeId').val() +
                '&FromDate=' + $('#FromDate').val() +
                '&ToDate=' + $('#ToDate').val() +
                '&StatusId=' + $('#statusId').val() +
                '&RequestType=' + requestType +
                '&IsLocal=' + isLocal +
                '&IsFilterByAffectedDate=' + isFilterByAffectedDate;

                location.href = url;
            }
        });

        floatingHeader();

        initEventExportExcel();
    }).ajaxStart(function () {
        if (!dontBlock) {
            $.blockUI();
        }
    }).ajaxStop($.unblockUI);
});