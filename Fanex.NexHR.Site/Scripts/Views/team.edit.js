'use strict';
/*global _global, autoIndex*/
/*exported deleteTeam*/

function deleteTeam(id) {
    confirm('Are you sure you want to delete this team?', function () {
        $.post(_global.root + 'Team/Delete', { id: id }, function (error) {
            if (error.length > 0) {
                alert(error);
                return;
            }
            $('#row' + id).remove();
            autoIndex();
        });
    });
}