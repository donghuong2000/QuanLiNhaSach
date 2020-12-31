$(document).ready(function () {
    $('#dataTable').DataTable({
        "ajax": {
            "url":"/Admin/bookentryticket/getall"
        },
        "columns": [
            { "data": "id" },
            { "data": "date" },
            {
                "data": "id",
                "render": function (data) {
                    console.log(data);
                    return `<div class="text-center">
                            <a href="/Admin/bookentryticket/Detail/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-info-circle"></i>
                            </a>
                        </div>
                    `}
            },
        ]
    })
})

