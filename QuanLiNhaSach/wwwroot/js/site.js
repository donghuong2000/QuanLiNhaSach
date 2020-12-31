// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {

    $('#searchbook').select2({
            ajax: {
                url: "/Home/SearchBook",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        q: params.term, // search term
                        page: params.page
                    };
                },
                processResults: function (data, params) {
                    
                    // parse the results into the format expected by Select2
                    // since we are using custom formatting functions we do not need to
                    // alter the remote JSON data, except to indicate that infinite
                    // scrolling can be used
                    params.page = params.page || 1;

                    return {
                        results: data.items,
                        pagination: {
                            more: (params.page * 30) < data.total_count
                        }
                    };
                },
                cache: true
            },
            placeholder: 'Tìm Kiếm sách',
            minimumInputLength: 1,
            templateResult: formatRepo,
            templateSelection: formatRepoSelection
        });
        
        })


        function formatRepo(repo) {
            if (repo.loading) {
                return repo.text;
            }

            var $container = $(
                
                //"<div class='select2-result-repository clearfix'>" +
                //"<div class='select2-result-repository__avatar'><img src='" + repo.owner.avatar_url + "' height = 50 width=50 /></div>" +
                //"<div class='select2-result-repository__meta'>" +
                //"<div class='select2-result-repository__title'></div>" +
                //"<div class='select2-result-repository__description'></div>" +
                //"<div class='select2-result-repository__statistics'>" +
                //"<div class='select2-result-repository__forks'><i class='fa fa-flash'></i> </div>" +
                //"<div class='select2-result-repository__stargazers'><i class='fa fa-star'></i> </div>" +
                //"<div class='select2-result-repository__watchers'><i class='fa fa-eye'></i> </div>" +
                //"</div>" +
                //"</div>" +
                //"</div>"
                "<div class='row'>"+
                   
                "<img class='col-3' src='" + repo.imgUrl+"'  />"+
                   
                    "<div class='col-9'>"+
                        "<h5>"+repo.name+"</h5>"+
                        "<p>Mô tả:</p>"+
                        "<p>" + repo.decription.substring(0,100) +"...</p>"+
                        "<div class='row'>"+
                        "<p class='col-6'>Danh mục:" + repo.category.name +"</p>"+
                        "<p class='col-6'>ngày xuất bản:" + repo.datePublish +"</p>"+
                         "<p class='col-6'>Tác giả:" + repo.author +"</p>"+
                        "<p class='col-6'>giá tiền:" + repo.price +"</p>"+
                        "</div>"+
                    "</div>"+
                "</div>"
                
            );

          

            return $container;
        }

function formatRepoSelection(repo) {
    if (repo.id.length > 2) {
        console.log(2)
        window.location.href = '/Books/' + repo.id;
        
    }
   
    return repo.name || repo.text;
}

