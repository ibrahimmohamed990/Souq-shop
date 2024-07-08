var dtable;
$(document).ready(function () {
    loaddata();
});

function loaddata() {
    dtable = $("#orderstable").DataTable({
        "ajax" : {
            "url": "/Admin/Order/GetData"
        },
        "columns": [
            { "data": "orderHeader.id"},
            { "data": "applicationUser.name"},
            { "data": "orderHeader.phoneNumber"},
            {"data": "applicationUser.email"},
            { "data": "orderHeader.orderStatus" },
            { "data": "orderHeader.paymentStatus" },
            { "data": "orderHeader.totalPrice" },
            {
                "data": "orderHeader.id",
                "render": function (data) {
                    return `
                        <a class="btn btn-warning" href="/Admin/Order/Details?orderid=${data}">Details</a>
                    `
                }


            }
        ]
    });
}
