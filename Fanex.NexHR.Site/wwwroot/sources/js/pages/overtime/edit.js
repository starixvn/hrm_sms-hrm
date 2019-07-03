import $ from 'jquery/dist/jquery';
import 'jquery.are-you-sure/jquery.are-you-sure';
import axios from 'axios';
import { stringify } from 'qs';
import toastr from 'toastr';

import { axiosError, toastOptions } from '../../shared/common.functions';

$(document).ready(function () {
    let rangeTextBox = function (textbox, min, max) {
        const value = parseFloat(textbox.val());
        if (value % 0.5 === 0) {
            if (value < min || value > max) {
                textbox.val(0);
            }
        } else {
            textbox.val(0);
        }

        textbox.keyup(function (event) {
            if ((event.which !== 46 || $(this).val().indexOf('.') !== -1) && (event.which < 48 || event.which > 57)) {
                event.preventDefault();
            }
        });

        textbox.blur(function () {
            if (textbox.val() === '') {
                textbox.val(0);
            }
        });

        return false;
    };

    let calculateTotalOTHours = function (selector) {
        let totalHours = 0;
        selector.each(function () {
            const value = $(this).val().trim();
            if (value !== '') {
                if (!isNaN(value) && value.lenght !== 0) {
                    totalHours += parseFloat(value);
                }
            }
        });

        return totalHours;
    };

    let calculateOTHours = function () {
        let totalHours = calculateTotalOTHours($('.cell-overtime input.normalHours'));
        $('#sumnormal').text(totalHours.toFixed(2));

        totalHours = calculateTotalOTHours($('.cell-overtime input.offHours'));
        $('#sumoff').text(totalHours.toFixed(2));

        totalHours = calculateTotalOTHours($('.cell-overtime input.publicHours'));
        $('#sumpublic').text(totalHours.toFixed(2));
    };

    let validation = function () {
        const verifierId = $('#VerifierId').val();
        const approverId = $('#ApproverId').val();

        if (verifierId === '' && approverId === '') {
            return 3;
        }

        if (verifierId === null || verifierId === '') {
            return 1;
        }

        if (approverId === null || approverId === '') {
            return 2;
        }

        return 0;
    };

    $('form').areYouSure();

    $('.cell-overtime input').on('keyup', function () {
        let $this = $(this);
        let row = $this.closest('tr');
        const workingHours = parseFloat($('#WorkingHours').val());
        const totalHours = parseFloat(row.find('.totalHours').html().trim());

        if ($this.hasClass('normalHours')) {
            if (totalHours >= workingHours) {
                const limit = parseFloat((totalHours - workingHours + 0.09).toFixed(2));
                rangeTextBox($this, 0, limit);
            } else {
                $this.val(0);
            }
        } else {
            const limit = parseFloat((totalHours + 0.09).toFixed(2));
            rangeTextBox($this, 0, limit);
        }

        calculateOTHours();
    });

    $('#btnSave').on('click', function () {
        const validationResult = validation();

        if (validationResult === 0) {
            let model = {
                employeeId: $('#EmployeeId').val(),
                verifierId: $('#VerifierId').val(),
                approverId: $('#ApproverId').val(),
                dailyOvertimes: []
            };

            $('input.remark').each(function () {
                let row = $(this).closest('tr');
                model.dailyOvertimes.push({
                    workDate: row.find('.workdate').data('workdate'),
                    shiftId: row.find('.shift').data('shiftid'),
                    timeIn: row.find('.timeIn').html().trim(),
                    timeOut: row.find('.timeOut').html().trim(),
                    actualWorkingHours: row.find('.totalHours').html().trim(),
                    normalHours: row.find('input.normalHours').val(),
                    offHours: row.find('input.offHours').val(),
                    publicHours: row.find('input.publicHours').val(),
                    remark: $(this).val()
                });
            });

            axios({
                url: _global.root + 'overtime/Edit',
                method: 'POST',
                data: stringify({ model }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    $('form').removeClass('dirty');
                    toastr.success('Updated Successfully!', '', toastOptions());
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        } else {
            switch (validationResult) {
                case 1:
                    toastr.error('Please select a Verifier.', '', toastOptions());
                    break;
                case 2:
                    toastr.error('Please select an Approver.', '', toastOptions());
                    break;
                default:
                    toastr.error('Please select Verifier and Approver.', '', toastOptions());
                    break;
            }
        }
    });
});