'use strict';
/*global _global, autoIndex*/

function cancelRequest(id) {
    confirm('Are you sure you want to cancel this request?', function () {
        $.post(_global.root + 'request/delete' + "/" + id, function (response) {
            if (response.IsSuccessful) {
                $('#row' + id).remove();
                autoIndex();
            }
            else {
                alert(response.Message);
                return;
            }
        });
    });
}

function processRequest(command, eventElement) {
    var requestId = $(eventElement).attr('requestId');
    var requestType = $("#RequestType").val();

    //$(eventElement).removeClass().destroyContextMenu();
    switch (command) {
        case 'Edit':
            location.href = _global.root + requestType + "/Update/" + requestId;
            break;
        case 'Cancel':
            cancelRequest(requestId);
            break;
    }
}

function bindMenu(element, menu) {
    $(element).contextMenu({
        menu: menu
    }, function (command, eventElement) {
        processRequest(command, eventElement);
    });
}

$(document).ready(function() {
	// menu
    bindMenu('.Pending', 'pendingMenu');
    $(".viewhistory").click(function () {
        var url = $(this).attr("id");
        $.get(url, function (data) {
            $('#dialoghistorycontent').html(data);
            $("#dialoghistorycontent").dialog({
                modal: true,
                minHeight: 0,
                width: 1100,
                title: 'Request History'
            });
        });
    });
});
