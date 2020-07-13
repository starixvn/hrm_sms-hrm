import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import 'bootstrap/dist/js/bootstrap';
import axios from 'axios';

import { axiosError, floatThead, confirm, resolveClientUrl, alert, autoIndex } from '../../shared/common.functions';


function deleteTeam(id) {
    confirm('Are you sure you want to delete this team ?', function () {
        axios({
            url: resolveClientUrl('team/delete' + '/' + id),
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;
            if (data.isSuccessful) {
                alert('Delete successfully!');
                $('#row' + id).remove();
                autoIndex();
            } else {
                alert(response.data.Message);
            }
            }).catch(axiosError);
    });
}

function sortTable() {
    $('th.col-sorter').on('click', function () {
        var $this = $(this).find('div.fa');
        if ($this.hasClass('fa-sort-desc')) {
            $this.removeClass('fa-sort-desc').addClass('fa-sort-asc');
        } else if ($this.hasClass('fa-sort-asc')) {
            $this.removeClass('fa-sort-asc').addClass('fa-sort-desc');
        } else {
            refreshSorting();
            $this.addClass('fa-sort-desc');
        }

        var table = $(this).parents('table').eq(0);
        var rows = table.find('tr:gt(0)').toArray().sort(comparer($(this).index()));
        this.asc = !this.asc;
        if (!this.asc) {
            rows = rows.reverse()
        };

        for (var i = 0; i < rows.length; i++) {
            table.append(rows[i]);
        };
        $("tbody tr").each(function (index) {
            var initIndex = index + 1;
            $(this).find("td:eq(0)").html(initIndex++);
        });

    })
}

function comparer(index) {
    return function (a, b) {
        var valA = getCellValue(a, index), valB = getCellValue(b, index);
        return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.toString().localeCompare(valB);
    }
}

function getCellValue(row, index) {
    return $(row).children('td').eq(index).text()
}

function refreshSorting() {
    $('th.col-sorter').each(function () {
        $(this).find('div.fa').removeClass('fa-sort-asc').removeClass('fa-sort-desc');
    });
}

$(document).ready(function () {
    sortTable();
    floatThead('table#data');
    $('table#data a.deleteTeam').on('click', function () {
        const teamId = $(this).data('team-id');
        deleteTeam(teamId);
    });
});