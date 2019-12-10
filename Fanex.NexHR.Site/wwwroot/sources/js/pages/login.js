import 'jquery/dist/jquery';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';

$(document).ready(function () {
    $('form', '#formContainer').submit(function () {
        let $password = $('#Password');
        $password.val(btoa($password.val()));
    });
});