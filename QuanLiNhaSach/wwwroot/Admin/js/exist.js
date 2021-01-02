
var value = ""

$(document).ready(function () {
    console.log(value)
    $('#dataTable').DataTable({
        "ajax": {
            "url": "/Admin/bookexist/getall/"
        },
        "columns": [
            { "data": "book" },
            { "data": "date" },
            { "data": "firstexist" },
            { "data": "incurredexist" },
            { "data": "lastexist" },

        ]
    })
})
const swalWithBootstrapButtons = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-success',
        cancelButton: 'btn btn-danger'
    },
    buttonsStyling: false
})

$('#time_exist').on('change', function () {
    value = $(this).val();
    console.log(value)
    $('#dataTable').DataTable().ajax.url("/Admin/bookexist/getall/" + value).load()
});
function LoadDatable(data) {
    $('#dataTable').DataTable({
        "ajax": {
            "url": "/Admin/bookexist/getall/" + data
        },
        "columns": [
            { "data": "book" },
            { "data": "date" },
            { "data": "firstexist" },
            { "data": "incurredexist" },
            { "data": "lastexist" },
        ]
    })

}






