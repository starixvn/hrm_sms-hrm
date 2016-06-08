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