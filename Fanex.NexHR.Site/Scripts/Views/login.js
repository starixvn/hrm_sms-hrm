'use strict';

$(document).ready(function () {
    $('form', '#formContainer').submit(function () {
        var $password = $('#Password');
        $password.val(btoa($password.val()));
    });
});
