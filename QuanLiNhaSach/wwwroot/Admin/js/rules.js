




$('#updateqd-1').click(function () {
    var useThisRule = $('#usethisrule1')[0].checked;
    // nhập ít nhất
    var nin = $('#min-1').val()

    // tồn ít nhất
    var tin = $('#max-1').val()


    if (nin < 0 || tin < 0) {
        toastr.error("Giá trị nhập vào phải lớn hơn 0")
        return false
    }
        
   
    $.ajax({
        url:'/Admin/rule/update',
        method: 'post',
        data: {id:'QD1', utr: useThisRule, min: nin, max: tin },
        success: function (data) {
            if (data.success)
                toastr.success(data.message)
            else
                toastr.error(data.message)
        }
    }).catch(function (data) {
    console.log(data)
        toastr.error(data.statusText + ' ' + data.status)
    })
})
$('#updateqd-2').click(function () {
    var useThisRule = $('#usethisrule2')[0].checked;
    // nhập ít nhất
    var nin = $('#min-2').val()
    // tồn ít nhất
    var tin = $('#max-2').val()

    if (nin < 0 || tin < 0) {
        toastr.error("Giá trị nhập vào phải lớn hơn 0")
        return false
    }
   
   
    $.ajax({
        url: '/Admin/rule/update',
        method: 'post',
        data: { id: 'QD2', utr: useThisRule, min: nin, max: tin },
        success: function (data) {
            if (data.success)
                toastr.success(data.message)
            else
                toastr.error(data.message)
        }
    }).catch(function (data) {
        toastr.error(data.statusText + ' ' + data.status)
    })
})
$('#updateqd-3').click(function () {
    var useThisRule = $('#usethisrule3')[0].checked;
    // nhập ít nhất
    var nin = $('#min-1').val()
    // tồn ít nhất
    var tin = $('#max-1').val()
    $.ajax({
        url: '/Admin/rule/update',
        method: 'post',
        data: { id: 'QD3', utr: useThisRule, min: nin, max: tin },
        success: function (data) {
            if (data.success)
                toastr.success(data.message)
            else
                toastr.error(data.message)
        }
    }).catch(function (data) {
        toastr.error(data.statusText + ' ' + data.status)
    })
})