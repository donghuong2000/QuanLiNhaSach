$(document).ready(function () {
	calc();
	var optionSelected = $('#select-customer').find(optionSelected)
	var valueSelected = optionSelected.val();
	if (valueSelected == "") {
		document.getElementById("check_debit").disabled = true
		console.log("true")
	}
	//document.getElementById("check_debit").disabled = true
	//document.getElementById("check_debit").checked = false
	
	

	$('#tab_logic tbody').on('keyup change', function () {
		calc();
	});
	$('#tax').on('keyup change', function () {
		calc_total();
	});


});
var i = $('#mytable tr').length - 1;
$("#add_row").click(function () {
	b = i - 1;
	console.log(b)
	var html = $('#addr0');
	console.log(html)
	$('#addr' + i).html($('#addr' + b).html()).find('td:first-child').html(i + 1);
	$('#tab_logic').append('<tr id="addr' + (i + 1) + '"></tr>');
	i++;
	console.log(i)
});
$("#delete_row").click(function () {
	if (i > 1) {
		$("#addr" + (i - 1)).html('');
		i--;
	}
	calc();
	console.log(i)
});

function calc() {
	$('#tab_logic tbody tr').each(function (i, element) {
		var html = $(this).html();
		if (html != '') {
			var qty = $(this).find('.qty').val();
			var price = $(this).find('.price').val();
			$(this).find('.total').val(qty * price);

			calc_total();
		}
	});
}

function calc_total() {
	total = 0;
	$('.total').each(function () {
		total += parseInt($(this).val());
	});
	$('#sub_total').val(total.toFixed(2));
	tax_sum = total / 100 * $('#tax').val();
	$('#tax_amount').val(tax_sum.toFixed(2));
	$('#total_amount').val((tax_sum + total).toFixed(2));
}

$('#mytable').on('change','.select-book',function () {
	var optionSelected = $(this).find("option:selected");
	var parent = $(this).parent().parent();
	var valueSelected = optionSelected.val();
	var price = parent.find('.price');
	var category = parent.find('.category')
	$.ajax({
		method: 'get',
		url: '/admin/bill/getbookprice/' + valueSelected,
		success: function (data) {
			if (data.success) { 
				price.val(data.price);
				category.val(data.category);
			}
			else {
				price.val(data.price);
				category.val(data.category);
			}
			calc();
        }
    })
	
});


$('#mycustomer').on('change', '#select-customer', function () {
	var optionSelected = $(this).find("option:selected");
	var parent = $(this).parent().parent();
	var valueSelected = optionSelected.val();
	console.log(valueSelected)
	if (valueSelected == "") {
		console.log("readonly")
		document.getElementById("check_debit").disabled = true
		document.getElementById("check_debit").checked = false
    }
		
	else
	{
		console.log("not-readonly")
		document.getElementById("check_debit").disabled = false
    }
		

});
