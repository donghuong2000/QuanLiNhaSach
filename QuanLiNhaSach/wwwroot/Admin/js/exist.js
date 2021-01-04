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
$('#button_create_exist').on('click', function () {
    $.ajax({
        url: "/admin/bookexist/ExistReport/",
        success: function (data) {
            if (data.success) {
                swalWithBootstrapButtons.fire(
                    'Đã tạo báo cáo thành công',
                    data.message,
                    'success'
                );
                $('#dataTable').DataTable().ajax.reload();
            }
            else {
                swalWithBootstrapButtons.fire(
                    'Không thể tạo báo cáo , vui lòng kiểm tra lại',
                    data.message,
                    'error'
                )
            }
        }

    })

});
$('#button_update_exist').on('click', function () {
    $.ajax({
        url: "/admin/bookexist/Update_Report/",
        success: function (data) {
            if (data.success) {
                swalWithBootstrapButtons.fire(
                    'Đã cập nhật báo cáo thành công',
                    data.message,
                    'success'
                );
                $('#dataTable').DataTable().ajax.reload();
            }
            else {
                swalWithBootstrapButtons.fire(
                    'Không thể cập nhật báo cáo , vui lòng kiểm tra lại',
                    data.message,
                    'error'
                )
            }
        }

    })

});