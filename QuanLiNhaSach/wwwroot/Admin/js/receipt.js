$(document).ready(function () {
    $('#dataTable').DataTable({
        "ajax": {
            "url": "/Admin/Receipt/getall"
        },
        "columns": [
            { "data": "id" },
            { "data": "customer" },
            { "data": "datecreate" },
            { "data": "proceed" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/admin/Receipt/Detail/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-info-circle"></i>
                            </a>
                        </div>
                    `}
            },

        ]
    })
})
$('#select-customer').on('change',function () {
	var optionSelected = $(this).val(); 
	var address = $('#address');
	var phone_number = $('#phone_number');
	var email = $('#email');
	$.ajax({
		method: 'get',
		url: '/admin/receipt/GetInfoUser/' + optionSelected,
		success: function (data) {
			console.log(data)
			console.log(data.phonenumber)
			console.log(phone_number)
			if (data.success) {
				address.val(data.address);
				phone_number.val(data.phonenumber);
				email.val(data.email);
			}
			else {
				address.val(data.address);
				phone_number.val(data.phone_number);
				email.val(data.email);
			}
        }
    })
	
});


