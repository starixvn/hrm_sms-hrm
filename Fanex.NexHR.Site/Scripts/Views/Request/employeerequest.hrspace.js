$(document).ready(function () {
    // menu
    $("#TeamId").multiselect();

    bindMenu('.hr-actions', 'hrMenu');
    bindMenu('.hr-actions-reset', 'resetMenu');

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

    $("form").submit(function (e) {
        e.preventDefault();
        var action = $("#ActionRequest").val();
        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var requestType = $("input[type='radio'][name='RequestType']:checked").val();
        var isLocal = $("#IsLocal").val();

        var url = _global.root + "request/" + action + "?DateOption=" + $("#Date").val()
            + "&TeamIds=" + teams + "&Employee=" + $("#Employee").val()
            + "&EmployeeId=" + $("#EmployeeId").val()
            + "&FromDate=" + $("#FromDate").val()
            + "&ToDate=" + $("#ToDate").val()
            + "&StatusId=" + $("#statusId").val()
            + "&RequestType=" + requestType
            + "&IsLocal=" + isLocal;

        location.href = url;
    });

    $("#btnExcel").click(function () {
        var action = $("#ActionRequest").val() + "WorkingTimeRequest";
        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var requestType = $("input[type='radio'][name='RequestType']:checked").val();
        var isLocal = $("#IsLocal").val();

        var url = _global.root + "export/" + action + "?DateOption=" + $("#Date").val()
            + "&TeamIds=" + teams + "&Employee=" + $("#Employee").val()
            + "&EmployeeId=" + $("#EmployeeId").val()
            + "&FromDate=" + $("#FromDate").val()
            + "&ToDate=" + $("#ToDate").val()
            + "&StatusId=" + $("#statusId").val()
            + "&RequestType=" + requestType
            + "&IsLocal=" + isLocal;

        location.href = url;
    });
});

function processRequest(command, eventElement) {
    var requestId = $(eventElement).attr('requestId');
    var roleId = $(eventElement).attr('roleId');
    showMessage('Processing');
    $.post(_global.root + 'ajax/processrequest',
	{ command: command, requestId: requestId, roleId: roleId },
		function (response) {
		    fadeOutMessage();
		    if (response != true) {
		        alert(response);
		        return;
		    }
		    else {
		        window.location.reload();
		    }
		});

}

function bindMenu(element, menu) {
    $(element).contextMenu({
        menu: menu
    },
	function (command, eventElement) {
	    switch (command) {
	        case 'Process':
	            confirm('Are you sure you want to process?', function () {
	                processRequest(command, eventElement);
	            });
	            break;
	        case 'Void':
	            confirm('Are you sure you want to void?', function () {
	                processRequest(command, eventElement);
	            });
	            break;
	        case 'Reset':
	            confirm('Are you sure you want to reset?', function () {
	                processRequest(command, eventElement);
	            });
	            break;
	    }
	});
}