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

    $(".slotRangeRow input:radio").click(function () {
        if ($(this).is(":checked")) {
            $(this).parent().parent().find("td").removeClass("reserved");
            $(this).parent().addClass("reserved");
        } else {
            alert("unsubscribe");
        }
    });

    $(".slotRangeRow input:radio").parent().click(function () {
        if ($(this).find("input:radio").is(":checked")) {
            $(this).find("input:radio").attr("checked", false).trigger("click");
        } else {
            $(this).find("input:radio").attr("checked", true).trigger("click");
        }
    });


    $("input:submit, a.button").button();
});