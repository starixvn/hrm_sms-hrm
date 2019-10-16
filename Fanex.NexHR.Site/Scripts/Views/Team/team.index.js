'use strict';
/*global _global, autoIndex, floatThead*/

(function () {
    function deleteTeam(id) {
        confirm('Are you sure you want to delete this team?', function () {
            $.post(_global.root + 'Team/Delete', { id: id },
            function (response) {
                if (response.IsSuccessful) {
                    $('#row' + id).remove();
                    autoIndex();
                } else {
                    alert(response.Message);
                }
            });
        });
    }

    $(document).ready(function () {
        floatThead('table#data');
        $('.tooltip-tipsy').tipsy({
                    html: true,
                    gravity: 'e'
                });
        $('table#data a.deleteTeam').on('click', function () {
            var teamId = $(this).data('team-id');
            deleteTeam(teamId);
        });
    });
})();