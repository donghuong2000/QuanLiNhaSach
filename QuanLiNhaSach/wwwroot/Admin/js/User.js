$(document).ready(function () {
    $('#dataTable').DataTable({
        "ajax": {
            "url":"/Admin/user/getall"
        },
        "columns": [
            { "data":"username"},
            { "data":"name"},
            { "data": "email" },
            { "data": "role" },
            {
                "data": { islock:"islock",id:"id" },
                "render": function (data) {
                    if (data.islock) {
                        return `<div class="text-center">
                                    <a class="btn btn-danger" onclick=LockUnLock("${data.id}")><i class="fas fa-lock"></i></a>
                                </div> `
                    }
                    else {
                        return `<div class="text-center">
                                    <a class="btn btn-success" onclick=LockUnLock("${data.id}")><i class="fas fa-lock-open"></i></a>
                                </div> `
                    }
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/User/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i> 
                            </a>

                            <a  onclick=Delete("/Admin/User/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
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