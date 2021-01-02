var i = $('#mytable tr').length - 1;
$(document).ready(function () {
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
		console.log(i)
	});

});




$('#mytable').on('change','.select-book',function () {
	var optionSelected = $(this).find("option:selected");
	var parent = $(this).parent().parent();
	var valueSelected = optionSelected.val();
	var category = parent.find('.category');
	var author = parent.find('.author');
	$.ajax({
		method: 'get',
		url: '/admin/bookentryticket/getbookinfo/' + valueSelected,
		success: function (data) {
			if (data.success) {
				category.val(data.category);
				author.val(data.author);
			}
			else
				category.val(data.category);
				author.val(data.author);
        }
    })
	
});