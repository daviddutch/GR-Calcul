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
            $(".slotRangeRow input:radio[name=\"" + $(this).name + "\"]:not(:checked)").removeClass("RadioSelected");
            $(".RadioSelected:not(:checked)").removeClass("RadioSelected");
            $(this).next("label").addClass("RadioSelected");
        }
    });
});