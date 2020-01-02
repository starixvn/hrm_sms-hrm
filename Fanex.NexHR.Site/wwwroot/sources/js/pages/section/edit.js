import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import 'chosen-js/chosen.jquery';

(function () {
    function initSearchLeader() {
        const leaderIds = $('#LeaderIds').val();
        if (leaderIds) {
            $('#Leaders').val(leaderIds.split(','));
        }
        $('#Leaders').chosen();

        $('#Leaders').on('change', function () {
            const values = $(this).val(),
                leaderIds = values ? values.join(',') : '';
            $('#LeaderIds').val(leaderIds);
        });
    }

    function initSearchAssistantLeader() {
        const assistIds = $('#AssistantLeaderIds').val();
        if (assistIds) {
            $('#AssistantLeaders').val(assistIds.split(','));
        }
        $('#AssistantLeaders').chosen();
        $('#AssistantLeaders').on('change', function () {
            const values = $(this).val(),
                assistLeaderIds = values ? values.join(',') : '';
            $('#AssistantLeaderIds').val(assistLeaderIds);
        });
    }

    $(document).ready(function () {
        initSearchLeader();
        initSearchAssistantLeader();
    });
})();