﻿/// <reference path="jquery-1.4.4.js" />
/// <reference path="jquery-ui.js" />

$(document).ready(reloadDatePickers);

function reloadDatePickers() {
    $('.DateFormat').each(function () {
        $(this).datepicker({
            dateFormat: "dd.mm.yy"
        });
    });
}