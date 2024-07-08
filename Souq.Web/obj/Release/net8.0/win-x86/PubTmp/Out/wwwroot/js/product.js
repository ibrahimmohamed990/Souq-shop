var dtable;
$(document).ready(function () {
    loaddata();
});

function truncateText(text, maxLength) {
    return text.length > maxLength ? text.substr(0, maxLength) + '...' : text;
}

function loaddata() {
      dtable = $("#productstable").DataTable({
                "ajax": {
                    "url": "/Admin/Product/GetData"
                },
                "columns": [
                    {
                        "data": "name",
                        "render": function (data, type, row) {
                            const maxLength = 30; // Set the max length for name
                            const truncated = truncateText(data, maxLength);
                            return `<span title="${data}">${truncated}</span>`;
                        }
                    },
                    {
                        "data": "description",
                        "render": function (data, type, row) {
                            const maxLength = 100; // Set the max length for description
                            const truncated = truncateText(data, maxLength);
                            return `<span title="${data}">${truncated}</span>`;
                        }
                    },
                    { "data": "category.name" },
                    { "data": "price" },
                    {
                        "data": "id",
                        "render": function (data) {
                            return `
                                    <a class="btn btn-info btn-sm mr-1" href="/customer/home/details/${data}" title="Details">
                                        <i class="fas fa-eye"></i>
                                    </a>
                                    <a class="btn btn-primary btn-sm mr-1" href="/Admin/Product/Edit/${data}" title="Edit">
                                        <i class="fas fa-edit"></i>
                                    </a>
                                    <a class="btn btn-danger btn-sm" href="/Admin/Product/Delete/${data}" title="Delete">
                                        <i class="fas fa-trash"></i>
                                    </a>   
                            `;
                        }
                    }
                ],
                "responsive": true,
                "paging": true,
                "searching": true,
                "ordering": true,
                "info": true,
                "autoWidth": false,
                "lengthChange": true,
                "pageLength": 10,
                "language": {
                    "emptyTable": "No products available"
                }
            });
}
