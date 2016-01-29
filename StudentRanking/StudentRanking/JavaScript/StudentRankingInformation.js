function Enroll() {
    var answer = confirm("Сигурен ли сте, че искате да се запишете в специалността " + $("#programmes option:selected").text() );
    if (answer)
    {
        $.ajax({
                    
            type: 'post',
            url: "/studentrankinginformation/enrollstudent",
            data: {
                            
                programmename: $("#programmes option:selected").text()
            },
                success: function (result) {
                    window.location.href = result.Url;
                }

                });
    }
}