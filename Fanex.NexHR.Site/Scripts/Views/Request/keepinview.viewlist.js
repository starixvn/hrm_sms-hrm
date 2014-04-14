$(document).ready(function () {
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

function cancelRequest(id) {
    confirm('Are you sure you want to cancel this request?', function () {
        $.post(_global.root + 'request/deleteworkingtimerequest' + "/" + id, function (data) {
            $('#row' + id).remove();
            autoIndex();
        });
    });
}

function processRequest(command, eventElement) {
    var requestId = $(eventElement).attr('requestId');

    switch (command) {
        case 'Edit':
            location.href = _global.root + "KeepInView/Update/" + requestId;
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