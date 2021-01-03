
var value = ""

$(document).ready(function () {
    console.log(value)
    $('#dataTable').DataTable({
        "ajax": {
            "url": "/Admin/Debit/getall/"
        },
        "columns": [
            { "data": "customer" },
            { "data": "date" },
            { "data": "firstdebit" },
            { "data": "incurreddebit" },
            { "data": "lastdebit" },

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

$('#time_debit').on('change', function () {
    value = $(this).val();
    console.log(value)
    $('#dataTable').DataTable().ajax.url("/Admin/Debit/getall/" + value).load()
});
function LoadDatable(data) {
    $('#dataTable').DataTable({
        "ajax": {
            "url": "/Admin/Debit/getall/" + data
        },
        "columns": [
            { "data": "customer" },
            { "data": "date" },
            { "data": "firstdebit" },
            { "data": "incurreddebit" },
            { "data": "lastdebit" },
        ]
    })
}
$('#button_create_debit').on('click', function () {
    var time = $('#time_debit').val();
    console.log(time)
    $.ajax({
        url: "/admin/debit/Create_List_Debit/" + time,
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

$('#button_update_debit').on('click', function () {
    var time = $('#time_debit').val();
    console.log(time)
    $.ajax({
        url: "/admin/debit/Update_List_Debit/" + time,
        success: function (data) {
            if (data.success) {
                swalWithBootstrapButtons.fire(
                    'Đã cập báo cáo thành công',
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





