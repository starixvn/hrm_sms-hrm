'use strict';
/*global _global, autoIndex*/
/*exported deleteDepartment*/
function deleteDepartment(departmentId) {
    confirm('Are you sure you want to delete this division?', function () {
        $.post(_global.root + 'department/Delete', { departmentId: departmentId }, function (error) {
            if (error.length > 0) {
                alert(error);
                return;
            }
            $('#row' + departmentId).remove();
            autoIndex();
        });
    });
}