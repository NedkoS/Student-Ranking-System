$(function () {
    $("#orderID").CascadingDropDown("#customerID", '/ProgrammeRules/AsyncOrders')
       //{ postData: function () {
       //    return { customerID: $('#customerID').val() };
       //}
       //{
       //    postData: function () {
       //        return { customerID: $('#customerID').val() };
       //    }
       //})
});

//var dataPost = { title: 'title' };
//jQuery.ajax({
//    type: "POST",
//    url: "ProgrammeRules/AsyncOrders",
//    data: dataPost,
//    dataType: "json",
//    success: function (result) {
//        alert("Data Returned: ");
//    }
//});