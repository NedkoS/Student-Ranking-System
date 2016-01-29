$(document).ready(function () {
    $("#programmeName").prop("disabled", true);
    $("#faculty").change(function () {
        if ($("#faculty").val() != "Please select") {
            var options = {};
            options.url = "/ProgrammeRankList/getprogrammes";
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
            options.error = function () { alert("Error retrieving states!"); };
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
        url: "/ProgrammeRankList/index",
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

function PublishRanking() {
    DisplayLoadingDiv();
    $.ajax({

        type: 'post',
        url: "/ProgrammeRankList/algoStart",
        success: function (result) {
            window.location.href = result.Url;
        }

    });

};

function DisplayLoadingDiv() {
    document.getElementById("img-container").style.display = '';
    var arr = document.getElementsByClassName("publish-btn-container");
    for (i = 0; i < arr.length; i++) {
        arr[i].style.visibility = 'hidden';
    }
    var loadingImage = document.getElementById("loading-img");
    var attr = document.createAttribute("src");
    attr.value = "../../img/loading.gif";
    loadingImage.setAttributeNode(attr);
}