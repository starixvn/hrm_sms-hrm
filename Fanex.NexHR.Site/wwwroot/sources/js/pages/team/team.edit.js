import $ from 'jquery/dist/jquery';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import 'chosen-js/chosen.jquery';

(function () {
    function initSearchLeader() {
        var leaderIds = $('#LeaderIds').val();
        if (leaderIds) {
            var arrValues = leaderIds.split(",");
            $('#Leaders').val(arrValues);
        }
        $("#Leaders").chosen();

        $('#Leaders').on("change", function () {
            var values = $(this).val();
            var leaderIds = values ? values.join(',') : '';
            $('#LeaderIds').val(leaderIds);
        });
    }

    function initSearchAssistantLeader() {
        var assistIds = $('#AssistantLeaderIds').val();
        if (assistIds) {
            var arrValues = assistIds.split(",");
            $('#AssistantLeaders').val(arrValues);
        }
        $("#AssistantLeaders").chosen();
        $('#AssistantLeaders').on("change", function () {
            var values = $(this).val();
            var assistLeaderIds = values ? values.join(',') : '';
            $('#AssistantLeaderIds').val(assistLeaderIds);
        });
    }

    $(document).ready(function () {
        $("#LeaderIds").rules("add", {
            required: true,
            messages: { required: "The Leader field is required" }
        });

        initSearchLeader();
        initSearchAssistantLeader();
    });
})();