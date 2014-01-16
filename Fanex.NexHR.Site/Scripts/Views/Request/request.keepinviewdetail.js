$(document).ready(function () {
    $("th.halfday").width(100);
    $(".tdhalfday").hide();
    $(".ddlfromtime").hide();
    $(".ddltotime").hide();
    $(".chkhalfday").change(function () {
        var currentObject = $(this).parent().parent();
        if (currentObject.find(".chkhalfday").attr("checked")) {
            currentObject.find(".addhalfday").first().html("+ Add a half");
            currentObject.find(".ddlfromtime").show();
            currentObject.find(".ddltotime").show();
            currentObject.find(".tdnumberleaves").html("0.5");
        }
        else {
            currentObject.find(".addhalfday").first().html("");
            currentObject.find(".ddlfromtime").hide();
            currentObject.find(".ddltotime").hide();
            currentObject.find(".tdnumberleaves").html("1");
        }
        var isShow = false;
        var objectTr = $(".data tbody tr");
        for (var i = 0; i < objectTr.length; i++) {
            var object = objectTr[i];
            if ($(object).find(".chkhalfday").first().attr("checked")) {
                isShow = true;
                break;
            }
        }
        if (isShow) {
            $(".tdhalfday").show();
        }
        else {
            $(".tdhalfday").hide();
        }
    });

    $(".addhalfday").on("click", function () {
        var object = $(this);
        var trObject = object.parent().parent();
        var trCopy = trObject.clone();
        var halfdayObject = trCopy.find(".addhalfday").first().html("- Remove a half").removeClass("addhalfday").addClass("removehaflday");
        var leaveTypeObject = trCopy.find(".tdleavetype").first();

        trCopy.find(".chkhalfday").attr("checked", "checked").attr("disabled", "disabled");
        trObject.find(".chkhalfday").attr("checked", "checked").attr("disabled", "disabled");

        halfdayObject.click(function () {
            var objectRemove = $(this).parent().parent().remove();
            trObject.find(".addhalfday").first().html("+ Add a half");
            trObject.find(".chkhalfday").removeAttr("disabled");
        });


        trObject.after(trCopy);
        trObject.find(".addhalfday").first().html("");

    });

});