var URLDelete = "";
$('#URLDelete')
    .keypress(function () {
        URLDelete = $(this).val();
    })
    .keypress();

function Contains(text_one, text_two) {
    if (text_one.indexOf(text_two) != -1)
        return true;
}
function deleteAlert(id, code) {
    swal({
        title: "Bạn chắn chắn muốn xóa ?",
        type: "warning",
        showCancelButton: true,
        closeOnConfirm: false,
        confirmButtonText: "Xác nhận",
        confirmButtonColor: "#ec6c62",
    },
        function () {
            var categorys = {};
            categorys.id = id;
            
            $.ajax({
                url: URLDelete,
                data: JSON.stringify(categorys),
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json'
            })
                .done(function (data) {
                    sweetAlert
                        ({
                            title: "Đã xóa!",
                            type: "success"
                        },
                            function () {
                                $("table tbody").find('input[name="record"]').each(function () {
                                    if (Contains($(this).val(), code)) {
                                        $(this).parents("tr").remove();
                                    }
                                });
                            })
                })
            /*   .error(function (data) {
                   swal("OOps", "Chúng tôi không thể kết nối đến server!", "error");
               })*/
        })
}


var URDEditStatus = "";
$('#URDEditStatus')
    .keypress(function () {
        URDEditStatus = $(this).val();
    })
    .keypress();
function EditStatus(id) {
    sweetAlert
        ({
            title: "Cập nhật trạng thái thành công!",
            type: "success"
        },
            function () {
                var categorys = {};
                categorys.id = id;
                $.ajax({
                    url: URDEditStatus,
                    data: JSON.stringify(categorys),
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json'
                })
            });

}

