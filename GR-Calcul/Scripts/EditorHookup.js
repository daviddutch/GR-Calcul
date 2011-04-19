/// <reference path="jquery-1.4.4.js" />
/// <reference path="jquery-ui.js" />

$(document).ready(function () {    
    $('.date').each(function () {        
        $(this).datepicker({
            dateFormat: "mm/dd/yy"
        });
    });
});