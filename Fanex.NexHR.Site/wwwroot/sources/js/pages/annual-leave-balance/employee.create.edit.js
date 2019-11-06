import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import { initDatepicker } from '../../shared/common.functions';
import '../../date';

$(document).ready(function() {
    initDatepicker();
    setInterval(function() {$('.success').fadeOut("slow");}, 3000)
})