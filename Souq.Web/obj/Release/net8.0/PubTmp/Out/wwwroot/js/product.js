var dtable;
$(document).ready(function () {
    loaddata();
});

function loaddata() {
    dtable = $("#productstable").DataTable({
        "ajax" : {
            "url": "/Admin/Product/GetData"
        },
        "columns": [
            {"data": "name"},
            { "data": "description"},
            {"data": "category.name"},
            { "data": "price" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a class="btn btn-success" href="/Admin/Product/Edit/${data}">Edit</a>
                        <a class="btn btn-danger" href="/Admin/Product/Delete/${data}">Delete</a>
                    `
                }


            }
        ]
    });
}
