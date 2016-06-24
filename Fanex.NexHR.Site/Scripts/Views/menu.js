'use strict';
/*global _global*/
function deleteMenu(id) {
    confirm('Are you sure you want to delete this menu?', function () {
        $.post(_global.root + 'Menu/delete', { id: id }, function (response) {
            if (response.IsSuccessful) {
                $('#row_' + id).remove();

            }
            else {
                alert(response.Message);
            }
        });

    });
}

$(document).ready(function () {

});