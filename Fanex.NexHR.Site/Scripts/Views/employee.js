$(document).ready(function () {

    $("#group").multiselect();

    var jobTitleId = $('#jobtitleid').val();

    $('#title').val(jobTitleId);
    $('#title').chosen();
    $("#title").trigger("liszt:updated");

    $('#keyword').keyup(function (event) {
        if (event.keyCode == 13) {
            $("#userdata").dataTable().fnDestroy();
            query(jobTitleId);
        }
    });

    var q = getParam('query');
    if (q != null) {
        $('#keyword').val(q);
    }

    //search onload
    query(jobTitleId);

    $('#query').click(function (evt) {
        evt.preventDefault();
        $("#userdata").dataTable().fnDestroy();
        query(jobTitleId);
    });

    $("#basicExcel").click(function () {
        exportExcel($(this));
    });

    $("#detailsExcel").click(function () {
        exportExcel($(this));
    });

});

function query(jobTitleId) {

    if (jobTitleId == undefined || jobTitleId == '') {
        jobTitleId = $('#title').val();
    }

    var query = $('#keyword').val();
    actingLoading('#query', true);

    if ($("#enable").attr("checked")) {
        enable = true;
    }
    else {
        if ($("#active").attr("checked")) {
            enable = false;
        }
        else {
            enable = null;
        }
    }

    var isDelete = null;
    if ($("#IsDeleted").attr("checked")) {
        isDelete = true;
    }
    else {
        isDelete = null;
    }

    var groups = $("#group").val() != null ? $("#group").val().join() : "";
    if (groups == "") {
        var groupid = $('#groupid').val();
        groups = groupid;
        //$("#group").multiselect("widget").find(":checkbox[value='" + groups + "']").attr("checked", "checked");
        $("#group option[value='" + groups + "']").prop("selected", 1);
        $("#group").multiselect("refresh");
    }

    var isLocal = $("#category").val();
    if (isLocal == "0") {
        isLocal = false;
    }
    if (isLocal == "1") {
        isLocal = true;
    }

    $.getJSON(
	_global.root + 'api/employee/getemployees'
	, { query: query, groupIds: groups, enable: enable, isDelete: isDelete, titleId: jobTitleId, isLocal: isLocal }
	, function (data) {
	    if (data.length > 0) {
	        rowIndex = 1;
	        $('#userdata tbody').children().remove();
	        $('#itemTemplate').tmpl(data).appendTo('#userdata');

	        $("#userdata").dataTable({
	            "sScrollY": "580px",
	            "sScrollX": "100%",
	            "sScrollXInner": "100%",
	            "bScrollCollapse": true,
	            "bFilter": true,
	            "bSort": true,
	            "bSortClasses": false,
	            "bPaginate": true,
	            "sPaginationType": "full_numbers",
	            "bInfo": true,
	            "bJQueryUI": true,
	            'bLengthChange': false,
                //"aLengthMenu": [[25, 50, 100, -1], [25, 50, 100, "All"]],
	            'iDisplayLength': 50,
	            "aoColumnDefs": [{ 'bSortable': false, 'aTargets': [6, 7, 8]}]
	        });

	        $('.nick').tooltip({
	            track: true,
	            delay: 0,
	            showURL: false,
	            bodyHandler: function () {
	                return '<img src="/employee/avatar/' + $(this).attr('data-code') + '" />';
	            }
	        });
	        $("#title").trigger("liszt:updated");
	    }
	    else {
	        $('#userdata tbody').children().remove();
	        $(noInformation(12)).appendTo('#userdata');
	    }
	    reloadButtonText('#query');
	}).error(function () {
	    reloadButtonText('#query');
	    alert(_messages.errorTryAgain);
	});
    $('#keyword').focus().select();
}

var rowIndex = 1;
function index() {
    return rowIndex++;
}


function resetPassword(id, nickName) {
    confirm("Are you sure you want to reset " + nickName + "'s password?", function () {
        $.post(_global.root + 'ajax/ResetPassword', { id: id }, function (data) {
        });
    });
}

function exportExcel(object) {

    var jobTitleId = $('#title').val();
    var groups = $("#group").val() != null ? $("#group").val().join() : "";
    var query = $('#keyword').val();
    var url = null;

    if ($("#enable").attr("checked")) {
        enable = true;
    }
    else {
        if ($("#active").attr("checked")) {
            enable = false;
        }
        else {
            enable = null;
        }
    }

    var isDelete = null;
    if ($("#IsDeleted").attr("checked")) {
        isDelete = true;
    }
    else {
        isDelete = null;
    }

    if (object.attr("id") == "basicExcel") {
        url = _global.root + "export/EmployeeBasicInfo?query=" + query + "&groupIds=" + groups
                               + "&enable=" + enable + "&isDelete=" + isDelete + "&titleId=" + jobTitleId;
    }
    if (object.attr("id") == "detailsExcel") {
        url = _global.root + "export/EmployeeDetailsInfo?query=" + query + "&groupIds=" + groups
                               + "&enable=" + enable + "&isDelete=" + isDelete + "&titleId=" + jobTitleId;
    }

    location.href = url;

}

function actingLoading(element, disabled) {
    $(element).attr("origin-value", $(element).attr("value"));
    $(element).attr("value", 'Processing');
    $(element).attr("disabled", "disabled");
}

function reloadButtonText(element) {
    $(element).attr("value", $(element).attr("origin-value"));
    $(element).removeAttr("disabled");
    $("#blocker").hide();
}