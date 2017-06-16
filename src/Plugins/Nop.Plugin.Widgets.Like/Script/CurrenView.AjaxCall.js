var CurrentView = function (productId,content) {
    var ProductId;
    (function () {
        ProductId = productId;
        //console.log(ProductId);
        getCount();
        myVar = setInterval(getCount, 60000);
    }());
      function addData() {
        $.ajax({
            type: "Get",
            url: "/WidgetsCurrentView/AddDataToCurrentViewer/"+ProductId,
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                //$(content).html(response.Number+ " person currently see this product");
                //console.log(response.Number);
            },
            failure: function (response) {
                
            }
        });
    }
    function getCount() {
        $.ajax({
            type: "Get",
            url: "/WidgetsCurrentView/getCurrentView/"+ ProductId,
            contentType: "application/json; charset=utf-8",
            success: function (response) {
               // $("#Content").text(response.d);
                $(content).html(response.Number);
                console.log(response.Number);
                addData();
            },
            failure: function (response) {

            }
        });
    }
    //return {
    //    Add: addData,
    //    Count : getCount
    //}
}