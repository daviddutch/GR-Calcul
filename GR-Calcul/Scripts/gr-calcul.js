/// <reference path="jquery-1.4.4.js" />
/// <reference path="jquery-ui.js" />


function loadTooltips(selector) {
    $(document).ready(function () {

        // select all desired input fields and attach tooltips to them
        $(selector).tooltip({

            // place tooltip on the right edge
            position: "center right",

            // a little tweaking of the position
            offset: [-2, 10],

            // use the built-in fadeIn/fadeOut effect
            effect: "fade",

            // custom opacity setting
            opacity: 0.7

        });
    });	
}

$(document).ready(function () {

    $(".slotRangeRow input:radio").change(function () {
        if ($(this).is(":checked")) {
            $(this).parent().parent().find("td").removeClass("reserved");
            $(this).parent().addClass("reserved");
        } else {
            $(this).parent().parent().find("td").removeClass("reserved");
        }
    });

    $(".slotRangeRow input:radio").click(function (e) {
        e.stopPropagation();
        if ($(this).parent().hasClass("reserved")) {
            $(this).attr("checked", false).trigger("change");
        } else {
            $(this).attr("checked", true).trigger("change");
        }
    });

    $(".slotRangeRow input:radio").parent().click(function () {
        if ($(this).find("input:radio").is(":checked")) {
            $(this).find("input:radio").attr("checked", false).trigger("change");
        } else {
            $(this).find("input:radio").attr("checked", true).trigger("change");
        }
    });


    $("input:submit, a.button").button();
});