'use strict';
/*global _global*/

var modifiedExtensionNo = (function ($) {
    var me = {};
    var urlRoot = "/";
    var $checkboxInternal, $ddlTeam, $fullName, $nickName, $employeeField, $empId, $employee;

    var checkInternal = function () {
        if ($checkboxInternal.is(":checked")) {
            $fullName.prop("readonly", true);
            $nickName.prop("readonly", true);
            $employeeField.show();

            if ($empId.val() > 0) {
                var employeeName = $nickName.val() + ' (' + $fullName.val() + ')';
                $employee.val(employeeName);
            }
        }
        else {
            $fullName.prop("readonly", false);
            $nickName.prop("readonly", false);
            $employeeField.hide();
        }
    };

    var bindDataToGroupDropdown = function () {
        var isInternal = $checkboxInternal.is(":checked");

        $.getJSON(urlRoot + 'extensionno/GetGroupExtension', { isInternal: isInternal, groupId: null }, function (data) {
            if (data[0] !== null) {
                $ddlTeam.empty();

                var options = "";
                for (var i = 0; i < data.length; i++) {
                    options += "<option value='" + data[i].Value + "'>" + data[i].Text + "</option>";
                }

                $ddlTeam.html(options);
            }
        });
    };

    var initEmployeeAutoComplete = function () {
        var nickName = "";
        $("#Employee").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: urlRoot + 'api/employee/getemployees',
                    data: {
                        query: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.NickName + ' (' + item.FullName + ')',
                                value: item.Id,
                                fullname: item.FullName,
                                nickname: item.NickName
                            };
                        }));
                    }
                });
            },
            minLength: 1,
            select: function (event, ui) {
                nickName = ui.item.label;
                $employee.val(nickName);
                $empId.val(ui.item.value);
                $fullName.val(ui.item.fullname);
                $nickName.val(ui.item.nickname);
            },
            focus: function (event, ui) {
                nickName = ui.item.label;
                $employee.val(nickName);
                $empId.val(ui.item !== null ? ui.item.value : "");
                $fullName.val(ui.item.fullname);
                $nickName.val(ui.item.nickname);
                event.preventDefault();
            },
            change: function (event, ui) {
                nickName = ui.item !== null ? ui.item.label : "";
                $employee.val(nickName);
                $empId.val(ui.item !== null ? ui.item.value : "");
                $fullName.val(ui.item.fullname);
                $nickName.val(ui.item.nickname);
            },
            close: function () {
                $employee.val(nickName);
            }
        });
    };

    var initValue = function () {
        $checkboxInternal = $("#IsInternal");
        $ddlTeam = $("#TeamId");
        $fullName = $("#FullName");
        $nickName = $("#NickName");
        $employeeField = $("#EmployeeField");
        $employee = $("#Employee");
        $empId = $('#EmpId');
    };

    var initEvent = function () {
        $checkboxInternal.change(function () {
            bindDataToGroupDropdown();
            checkInternal();
        });
    };

    me.init = function (globalRoot) {
        urlRoot = globalRoot;

        initValue();
        initEvent();

        if ($("#IsAddForm").val() === "True") {
            bindDataToGroupDropdown();
        }

        checkInternal();
        initEmployeeAutoComplete();
    };

    return me;
})(jQuery);

$(document).ready(function () {
    modifiedExtensionNo.init(_global.root);
});