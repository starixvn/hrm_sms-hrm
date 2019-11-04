'use strict';
/*global _global, autoIndex*/
/*exported deleteDepartment*/
function deleteDepartment(departmentId) {
    confirm('Are you sure you want to delete this division?', function () {
        $.post(_global.root + 'department/Delete', { departmentId: departmentId }, function (response) {
            if (response.IsSuccessful) {
                $('#row' + departmentId).remove();
                autoIndex();
            }
            else {
                alert(response.Message);
                return;
            }
        });
    });
}

function filterDepartment() {
    var status = $("#IsActive").val();
    var isActive = "";
    if (status == 0) {
        isActive = true;
    }
    else {
        isActive = false;
    }

    console.log(status);
    if (status != "") {
        window.location.href = _global.root + 'department/index?isActive=' + isActive;
    }
    else {
        window.location.href = _global.root + 'department/index?isActive=null';
    }
}

$(document).ready(function () {
    $('#submit').on('click', function () {
        filterDepartment();
    })
})