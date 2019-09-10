import $ from 'jquery/dist/jquery';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import axios from 'axios';

import { axiosError } from '../../shared/common.functions';

$(document).ready(function () {
    $('#generate').on('click', function () {
        axios(_global.root + 'employee/GenerateEmployeeCode')
            .then(response => {
                let data = response.data;

                $('#Code').val('X' + data);
            }).catch(axiosError);
    });
});