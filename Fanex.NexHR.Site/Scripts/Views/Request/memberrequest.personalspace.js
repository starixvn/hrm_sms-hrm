$(document).ready(function () {
    $("#TeamId").multiselect();
    bindMenu('.verifier-actions', 'verifierMenu');
    bindMenu('.approver-actions', 'approverMenu');
    bindMenu('.verifier-actions-reset', 'resetMenu');
    bindMenu('.approver-actions-reset', 'resetMenu');

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
        var employee = $("#Employee").val();
        var employeeId = employee == "" ? "" : $("#EmployeeId").val();
        var isLocal = $("#isLocal").val();

        var url = _global.root + "request/" + action + "?DateOption=" + $("#Date").val()
                + "&TeamIds=" + teams + "&Employee=" + employee
                + "&EmployeeId=" + employeeId
                + "&FromDate=" + $("#FromDate").val()
                + "&ToDate=" + $("#ToDate").val()
                + "&StatusId=" + $("#statusId").val()
                + "&RequestType=" + requestType
                + "&isLocal=" + isLocal;

        location.href = url;
    });
    $("#btnExcel").click(function () {
        var action = $("#ActionRequest").val() + "WorkingTimeRequest";
        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var requestType = $("input[type='radio'][name='RequestType']:checked").val();
        var employee = $("#Employee").val();
        var employeeId = employee == "" ? "" : $("#EmployeeId").val();
        var isLocal = $("#isLocal").val();

        var url = _global.root + "export/" + action + "?DateOption=" + $("#Date").val()
                + "&TeamIds=" + teams + "&Employee=" + employee
                + "&EmployeeId=" + employeeId
                + "&FromDate=" + $("#FromDate").val()
                + "&ToDate=" + $("#ToDate").val()
                + "&StatusId=" + $("#statusId").val()
                + "&RequestType=" + requestType
                + "&isLocal=" + isLocal;

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
		        $(eventElement).removeClass().destroyContextMenu();
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
	        case 'Accept':
	            confirm('Are you sure you want to accept?', function () {
	                processRequest(command, eventElement);
	            });
	            break;
	        case 'NotAccept':
	            confirm('Are you sure you dont want to accept?', function () {
	                processRequest(command, eventElement);
	            });
	            break;
	        case 'Approve':
	            confirm('Are you sure you want to approve?', function () {
	                processRequest(command, eventElement);
	            });
	            break;
	        case 'Reject':
	            confirm('Are you sure you want to reject?', function () {
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
