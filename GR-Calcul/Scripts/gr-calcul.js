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

function reserve(radio, reserve) {
    //"/GR-Calcul/SlotRange/Reserve/"
    $.getJSON($("#reserveLink").val() + $(radio).val() + "?reserve=" + reserve, null, function (data) {
        if (data.Success) {
            if (reserve) {
                $(radio).parent().parent().find("td").removeClass("reserved");
                $(radio).parent().addClass("reserved");
            } else {
                $(radio).parent().parent().find("td").removeClass("reserved");
            }
        } else {
            alert(data.Message + "\nVeuillez recharger la page pour ressayer de faire votre réservation.");
        }
    });
}

$(document).ready(function () {

    $(".slotRangeRow input:radio").change(function () {
        if ($(this).is(":checked")) {
            reserve(this, true);
        } else {
            reserve(this, false);
        }
    });

    $(".slotRangeRow input:radio").click(function (e) {
        e.stopPropagation();
        if ($(this).parent().hasClass("reserved")) {
            $(this).attr("checked", false).trigger("change");
        } else {
            $(this).attr("checked", true);
        }
    });

    $(".slotRangeRow input:radio").parent().click(function () {
        if ($(this).find("input:radio").is(":checked")) {
            $(this).find("input:radio").attr("checked", false).trigger("change");
        } else {
            $(this).find("input:radio").attr("checked", true).trigger("change");
        }
    });


    $("input:submit, .button").button();
});