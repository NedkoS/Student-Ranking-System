 $(document).ready(function () {
        $("#programmeName").prop("disabled", true);
        $("#faculty").change(function () {
            if ($("#faculty").val() != "Please select") {
                var options = {};
                options.url = "/ProgrammeRules/getprogrammes";
                options.type = "POST";
                options.data = JSON.stringify({ faculty: $("#faculty").val() });
                options.dataType = "json";
                options.contentType = "application/json";
                options.success = function (states) {
                    $("#programmeName").empty();
                    for (var i = 0; i < states.length; i++) {
                        $("#programmeName").append("<option>" + states[i] + "</option>");
                    }
                    $("#programmeName").prop("disabled", false);
                };
                $.ajax(options);
            }
            else {
                $("#programmeName").empty();
                $("#programmeName").prop("disabled", true);
            }
        });
    });

    function ChooseProgramme() {
        $.ajax({
            type: 'POST',
            url: "/ProgrammeRules/index",
            data: {
                faculty: $("#faculty option:selected").text(),
                programmeName: $("#programmeName option:selected").text()
            },
            success: function (data) {
                $('#resultTable').html(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            }

        });
    };

    function SaveClick() {
        $.ajax({
            type: 'POST',
            url: "/ProgrammeRules/saveCounts",
            data: {
                maleCount: $("#maleCount").val(),
                femaleCount: $("#femaleCount").val(),
                programmeName: $("#programmeName option:selected").text()
            }

        });
    };
