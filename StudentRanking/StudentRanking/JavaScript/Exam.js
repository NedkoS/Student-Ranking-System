function showExams() {
    $.ajax({
        type: 'POST',
        url: "/Exam/Index",
        data: {
            egn: $("#egn").val()
        },
        success: function (data) {
            $('#resultTable').html(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        }

    });
}