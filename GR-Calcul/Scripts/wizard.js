/// <reference path="jquery-1.4.4.js" />
/// <reference path="jquery-ui.js" />
/// <reference path="date.js" />
/// <reference path="EditorHookup.js" />

var slotRowIdPrefix = "slotRow";
Date.format = "dd.mm.yyyy";

function enableWizard(buildLastStep) {
    $(function () {

        $(".wizard-step:first").fadeIn(); // show first step
        if ($(".wizard-step").length == 1) //if there's only one step
            $("#next-step").val("Terminer");


        // attach backStep button handler
        // hide on first step
        $("#back-step").hide().click(function () {
            var $step = $(".wizard-step:visible"); // get current step
            if ($step.prev().hasClass("wizard-step")) { // is there any previous step?
                $step.hide().prev().fadeIn();  // show it and hide current step

                // disable backstep button?
                if (!$step.prev().prev().hasClass("wizard-step")) {
                    $("#back-step").hide();
                }

            }
        });

        $("#back-step").click(function () {
            var $step = $(".wizard-step:visible");
            if ($step.next().hasClass("wizard-step")) {
                $("#next-step").val("Suivant");
            }
        });

        // attach nextStep button handler       
        $("#next-step").click(function () {

            var $step = $(".wizard-step:visible"); // get current step


            // enhance wizard by preventing user from navigating to next step if there are errors in validation            
            var validator = $("form").validate(); // obtain validator
            var anyError = false;
            $step.find("input").each(function () {
                /*
                * There are a few things I do not want jquery to validate such as all the checkboxes because it won't be able to find a validator for them.
                * Also the slot times should not be validated, etc.
                */
                var attr = $(this).attr('novalidation');
                //if the element has the 'novalidation' attribute then exit from here
                if (typeof attr !== 'undefined' && attr !== false) {
                    return;
                }

                if (!validator.element(this)) { // validate every input element inside this step
                    anyError = true;
                }

            });

            if (anyError)
                return false; // exit if any error found


            if (!$step.next().next().hasClass("wizard-step")) {
                $("#next-step").val("Terminer");
            }
            if (buildLastStep) {
                if (!$step.next().next().hasClass("wizard-step") && $step.next().hasClass("wizard-step")) {
                    createLastStep();
                }
            }
            if ($step.next().hasClass("wizard-step")) { // is there any next step?
                $step.hide().next().fadeIn();  // show it and hide current step
                $("#back-step").show();   // recall to show backStep button
            } else { // this is last step, submit form
                $("form").submit();
            }
        });

    });
}

function enableRoomBoxes() {
    $(document).ready(function () {

        $(".roomBox").click(function () {
            var checked_status = this.checked;
            var lookUpId = "inroom-" + this.id;
            $("input[room=" + lookUpId + "]").each(function () {
                this.checked = checked_status;
            });
        });
    });
}

function createRow(rowId, i, suffix) {
    var duration = $("#SlotDuration").val()
    if (!duration || duration == "undefined") {
        duration = 24 / (($(".slotRow:first td").length - 1) / 2);
    }
    var cols = 24 / duration;
    var nslots = $("#NumberOfSlots").val();

    if (nslots < cols)
        cols = nslots;

    var rows = Math.ceil(nslots / cols);
    var nOnLastRow = cols - (rows * cols - nslots);

    var slotDateField = document.getElementById("Slotdate");
    var startDate = "";
    var beginning = false;
    if (slotDateField) {
        slotDateField = $(".Slotdate:last");
        startDate = slotDateField.val();
    } else {
        slotDateField = document.getElementById("SlotdateAdded");
        if (slotDateField) {
            slotDateField = $(".Slotdate:last");
            startDate = slotDateField.val();
        } else {
            startDate = $("#Beginning").val();
            beginning = true;
        }
    }

    var day = startDate.substring(0, startDate.indexOf("."));
    var month = startDate.substring(startDate.indexOf(".") + 1, startDate.indexOf(".", startDate.indexOf(".") + 1));
    var year = startDate.substring(startDate.indexOf(".", startDate.indexOf(".") + 1) + 1, startDate.length);
    var currentDate = new Date(year, parseInt(month - 1, 10), parseInt(day, 10));
    if (!beginning) {
        currentDate.addDays(1);
    }
    var html = "";
    html += "<tr id=\"" + rowId + "\" class=\"" + slotRowIdPrefix + "\">";
    html += "<td colspan=\"2\">&nbsp;<input novalidation=\"true\" type=\"text\" class=\"lollipop Slotdate\" name=\"Slotdate" + suffix + "\" id=\"Slotdate" + suffix + "\" value=\"" + currentDate.asString() + "\" size=\"8\" /><a href=\"javascript:deleteRow('" + rowId + "');\" class=\"delete delRow\"></a></td>";
    var max = (i == rows - 1 ? nOnLastRow : cols);

    for (var j = 0; j < max; j++) {
        var dura = j * duration;
        var dura2 = (j + 1) * duration;

        var min = ":00";
        var min2 = ":00";
        if (dura == 24) {
            //this shouldn't happen in theory
            dura = 23;
            min = ":59";
        }
        if (dura2 == 24) {
            dura2 = 23;
            min2 = ":59";
        }

        html += "<td><input novalidation=\"true\" type=\"text\" value=\"" + dura + "" + min + "\" name=\"Startz" + suffix + "\" size=\"4\" /></td>";
        html += "<td><input novalidation=\"true\" type=\"text\" value=\"" + dura2 + "" + min2 + "\" name=\"Endz" + suffix + "\" size=\"4\" /></td>";
    }
    html += "</tr>";

    return html;
}

function createLastStep() {
    //build slot selection view
    var duration = $("#SlotDuration").val()
    var cols = 24 / duration;
    var nslots = $("#NumberOfSlots").val();
    if (nslots < cols)
        cols = nslots;
    var rows = Math.ceil(nslots / cols);
    var nOnLastRow = cols - (rows * cols - nslots);
    var startDate = $("#Beginning").val();
    //header
    var html = "<thead><tr><th style='text-align:center;' colspan='2'>&nbsp;</th>";
    for (var i = 0; i < cols; i++) {
        html += "<th colspan='2'>Slot</th>";
    }
    html += "</tr>";
    html += "<tr class='subTitle'>";
    html += "<th colspan='2'>&nbsp;</th>";
    for (var i = 0; i < cols; i++) {
        html += "<th>Heure début</th><th>Heure fin</th>";
    }
    html += "</tr></thead>";
    //body
    html += "<tbody id=\"slotBody\">";

    //clear current table content otherwise we might be appending the content several times
    $("#slotTable").html("");
    $("#slotTable").append(html);
    html = "";
    for (var i = 0; i < rows; i++) {
        var rowId = slotRowIdPrefix + i;
        html = createRow(rowId, i, "");
        $("#slotTable").append(html);
    }

    html = "</tbody>";
    html += "<tfoot><tr><td colspan=\"7\"><button type=\"button\" onclick=\"addRow()\">+</button></td></tr></tfoot>";

    $("#slotTable").append(html);

    //load datepickers
    reloadDatePickers();
}

function addRow() {
    var old = $("." + slotRowIdPrefix + ":last").attr("id");
    if (!old || old == "undefined") {
        $("#slotBody").append(createRow(slotRowIdPrefix+"0", 0, "Added"));
    } else {
        var num = parseInt(old.substring(slotRowIdPrefix.length, old.length), 10);
        num++;
        var nextId = slotRowIdPrefix + num;
        $("#slotBody").append(createRow(nextId, num, "Added"));
    }
    //load datepickers
    reloadDatePickers();
}

function deleteRow(rowId) {
    $("#" + rowId).remove();
}
