﻿$(document).ready(function () {
    $('#dataTable').DataTable({
        "ajax": {
            "url":"/Admin/book/getall"
        },
        "columns": [
            {
                "data": "imgUrl",
                "render": function (data) {
                    return `
                            <img src="${data}" height = 50 width = 40 class="rounded"/>`
                }
            },

            { "data":"name"},
            { "data": "author" },
            { "data": "category" },
            { "data": "decription" },
            { "data": "quantity" },
            { "data": "datePublish" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/book/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i> 
                            </a>

                            <a  onclick=Delete("/Admin/book/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                                <i class="fas fa-trash-alt"></i> 
                            </a>

                        </div>
                    `}
            },

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
function Delete(url) {
    swalWithBootstrapButtons.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, cancel!',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        swalWithBootstrapButtons.fire(
                            'Deleted!',
                            data.message,
                            'success'
                        );
                        $('#dataTable').DataTable().ajax.reload();
                    }
                    else {
                        swalWithBootstrapButtons.fire(
                            'Error',
                            data.message,
                            'error'
                        )
                    }
                }

            }).catch(function (data) {
                swalWithBootstrapButtons.fire(
                    'Cancelled',
                    data.statusText + ' ' + data.status,
                    'error'
                )
                
            })

        }
        else if (result.dismiss === Swal.DismissReason.cancel) {
            swalWithBootstrapButtons.fire(
                'Cancelled',
                'Your record is safe :)',
                'error'
            )
        }
    })
}
function LockUnLock(data) {
    $.ajax({
        url: '/admin/user/lockunlock/',
        data: { id: data },
        success: function (result) {
            if (result.success) {
                toastr.success(result.message);
                $('#dataTable').DataTable().ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    })

}