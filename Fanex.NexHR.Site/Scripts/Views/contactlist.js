$(document).ready(function () {
    $("#group").multiselect();

    var jobTitleId = $('#jobtitleid').val();

    $('#title').val(jobTitleId);
    $('#title').chosen();
    $("#title").trigger("liszt:updated");

    $('#keyword').keyup(function (event) {
        if (event.keyCode == 13) {
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
    actingButton('#query', true);
    enable = false;
    
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
	    //console.log(data);
	    if (data.length > 0) {
	        rowIndex = 1;

	        $('#data tbody').children().remove();
	        $('#itemTemplate').tmpl(data).appendTo('#data');
	        $('.nick').tooltip({
	            track: true,
	            delay: 0,
	            showURL: false,
	            bodyHandler: function () {
	                return '<img width=64px; height=64px; src="/employee/avatar/' + $(this).attr('data-code') + '" />';
	            }
	        });
	        $("#title").trigger("liszt:updated");
	    }
	    else {
	        $('#data tbody').children().remove();
	        $(noInformation(12)).appendTo('#data');
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