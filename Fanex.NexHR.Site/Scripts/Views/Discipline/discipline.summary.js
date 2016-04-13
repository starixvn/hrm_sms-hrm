(function () {
    'use strict';
    /*global _global*/

    function initAddOtherViolationDialog() {
        $('#dialog-toeic').dialog({
            modal: true,
            width: 'auto',
            buttons: [
                {
                    text: 'Close',
                    click: function () {
                        $(this).dialog('close');
                    }
                },
                {
                    text: 'Submit',
                    click: function () {
                        $.ajax({
                            type: 'POST',
                            url: _global.root + 'toeic/updatetoeic',
                            dataType: 'json',
                            data: $('#frmUpdateTOEIC').serialize(),
                            success: function () {
                            }
                        });
                    }
                }
            ],
            show: {
                effect: 'fade',
                duration: 300
            },
            hide: {
                effect: 'fade',
                duration: 200
            },
            dialogClass: 'modal-shadow'
        });
    }

    function query() {
        $.ajax({
            url: _global.root + 'discipline/listdisciplinesummary',
            data: $('form.form-filter').serialize(),
            success: function (data) {
                $('#discipline-summary').html(data);

                $('#btnAddOtherViolation').on('click', function (event) {
                    event.preventDefault();

                    $('#add-discipline-dialog').dialog();
                });
            }
        });
    }

    $(document).ready(function () {
        $('#submit').on('click', function (event) {
            event.preventDefault();
            query();
        });

        query();
    });
})();