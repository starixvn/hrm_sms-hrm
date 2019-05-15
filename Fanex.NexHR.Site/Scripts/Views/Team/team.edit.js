'use strict';
/*global _global*/

(function () {
    function initSearchLeader() {
        var leaderIds = $('#LeaderIds').val();
        if (leaderIds) {
            var arrValues = leaderIds.split(",");
            $('#Leaders').val(arrValues);
        }
        $("#Leaders").chosen();

        $("#Leaders").rules("add", {
            required: true,
            messages: { required: "The Leader field is required" }
        });

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
            console.log(arrValues);
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
        //Validation Leader field
        $("#LeaderIds").rules("add", {
            required: true,
            messages: { required: "The Leader field is required" }
        });

        initSearchLeader();
        initSearchAssistantLeader();
    });
})();