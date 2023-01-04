
// Example starter JavaScript for disabling form submissions if there are invalid fields
(function () {
    'use strict';
    window.addEventListener('load', function () {
        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.getElementsByClassName('needs-validation');
        // Loop over them and prevent submission
        var validation = Array.prototype.filter.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                    form.classList.add('was-validated');
                } else {
                    form.classList.remove('was-validated');

                }
               
               
            }, false);
        });

    }, false);
})();

$(document).ready(function () {
    var forms = document.getElementsByClassName('needs-validation');
    var validation = Array.prototype.filter.call(forms, function (form) {
    $('#submit_edit').on('click', function () {
        if (form.checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
            form.classList.add('was-validated');
            
        } else {
           
            Update();
            form.classList.remove('was-validated');

        }
    })
        $('#submit_edit_product').on('click', function () {
            

            if (form.checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                form.classList.add('was-validated');
            } else {               
                console.log("hihi");
                Update_Product();
                form.classList.remove('was-validated');
            }
        })
        $('#submit_edit_customer').on('click', function () {
            if (form.checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                form.classList.add('was-validated');
            } else {
                Update_Customer();
                form.classList.remove('was-validated');

            }
        })
        $('#add_customer_sale').on('click', function () {
            if (form.checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                form.classList.add('was-validated');
            } else {
                AddCustomerSale();
                form.classList.remove('was-validated');

            }
            
        })
        $('#submit_addCart').on('click', function () {
            if (form.checkValidity() === false) {
                
                event.preventDefault();
                event.stopPropagation();
                form.classList.add('was-validated');
            } else {
                
                Create_Cart();
                form.classList.remove('was-validated');
            }
           
        })
        $('#submit_updateCart').on('click', function () {
            if (form.checkValidity() === false) {

                event.preventDefault();
                event.stopPropagation();
                form.classList.add('was-validated');
            } else {

                Update_Cart();
                form.classList.remove('was-validated');
            }

        })
//-------------------------Làm mới cart--------------------------------
        $('#refresh_cart').on('click', function () {
            GetList_Cart(-1);
            $('#customer_code').val('');
            $('#customer_phone').val('');
            $('#customer_type').val('');
            $('#count_sale').val('');
            $('#debit_sum').val('');
            $('#customer_name').val('');
            $('#product_code').val('');
            $('#product_unit').val('');
            $('#product_price').val('');
            $('#product_name').val('');
            $('#sum_price').val('');
            $('#cart_note').val('');
            form.classList.remove('was-validated');
        })
        $('#Cancel_Cart').on('click', function () {
            $('#product_code').val('');
            $('#product_unit').val('');
            $('#product_price').val('');
            $('#product_name').val('');
            $('#sum_price').val('');
            $('#cart_note').val('');
            $('#product_quantity').val(1);
            document.querySelector("#customer_name").disabled = false;
            document.querySelector("#product_name").disabled = false;

            $("#submit_addCart").show();
            $("#refresh_cart").show();
            $("#submit_updateCart").hide();
            $("#Cancel_Cart").hide();
            form.classList.remove('was-validated');
        })
       

    }, false);
})

//-------------------------Delete Group, Category, Product-----------------------
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
        allowOutsideClick: true,

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
                    if (data.status) {
                        sweetAlert
                            ({
                                title: data.message,
                                type: "success",
                                allowOutsideClick: true,

                            },
                                function () {
                                    $("table tbody").find('input[name="record"]').each(function () {
                                        if (Contains($(this).val(), code)) {
                                            $(this).parents("tr").remove();
                                        }
                                    });
                                })
                    }
                    else {
                        sweetAlert
                            ({
                                title: data.message,
                                type: "error",
                                allowOutsideClick: true,

                            })
                    }
                 
                })
            /*   .error(function (data) {
                   swal("OOps", "Chúng tôi không thể kết nối đến server!", "error");
               })*/
        })
}

//-----------------------Edit status------------------------------------------
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
            type: "success",
            allowOutsideClick: true,

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

//-----------------------Định dạng giá-------------------------------------
$('#product-price-sold').keydown(function (e) {
    setTimeout(() => {
        let parts = $(this).val().split(".");
        let v = parts[0].replace(/\D/g, ""),
            dec = parts[1]
        let calc_num = Number((dec !== undefined ? v + "." + dec : v));
        // use this for numeric calculations
        // console.log('number for calculations: ', calc_num);
        let n = new Intl.NumberFormat('en-EN').format(v);
        n = dec !== undefined ? n + "." + dec : n;
        $(this).val(n);
    })
})
$('#product-price-buy').keydown(function (e) {
    setTimeout(() => {
        let parts = $(this).val().split(".");
        let v = parts[0].replace(/\D/g, ""),
            dec = parts[1]
        let calc_num = Number((dec !== undefined ? v + "." + dec : v));
        // use this for numeric calculations
        // console.log('number for calculations: ', calc_num);
        let n = new Intl.NumberFormat('en-EN').format(v);
        n = dec !== undefined ? n + "." + dec : n;
        $(this).val(n);
    })
})
$('#edit_sell_price').keydown(function (e) {
    setTimeout(() => {
        let parts = $(this).val().split(".");
        let v = parts[0].replace(/\D/g, ""),
            dec = parts[1]
        let calc_num = Number((dec !== undefined ? v + "." + dec : v));
        // use this for numeric calculations
        // console.log('number for calculations: ', calc_num);
        let n = new Intl.NumberFormat('en-EN').format(v);
        n = dec !== undefined ? n + "." + dec : n;
        $(this).val(n);
    })
})
$('#edit_purchase_price').keydown(function (e) {
    setTimeout(() => {
        let parts = $(this).val().split(".");
        let v = parts[0].replace(/\D/g, ""),
            dec = parts[1]
        let calc_num = Number((dec !== undefined ? v + "." + dec : v));
        // use this for numeric calculations
        // console.log('number for calculations: ', calc_num);
        let n = new Intl.NumberFormat('en-EN').format(v);
        n = dec !== undefined ? n + "." + dec : n;
        $(this).val(n);
    })
})


//------------ Load dropdown form add product----------------------------------

var URLgetGroupProduct = "";
var URLgetCategory = "";
$('#URLgetGroupProduct')
    .keypress(function () {
        URLgetGroupProduct = $(this).val();
    })
    .keypress();
$('#URLgetCategory')
    .keypress(function () {
        URLgetCategory = $(this).val();
    })
    .keypress();

$.ajax({
    type: "GET",
    url: URLgetGroupProduct,
    data: "{}",
    success: function (data) {
        var s = '<option value="" disabled="disabled" selected="selected">Chọn nhóm hàng</option>';
        for (var i = 0; i < data.length; i++) {
            s += '<option value="' + data[i].groupID + '">' + data[i].groupName + '</option>';
        }
        $("#GroupProductDropdown").html(s);
    }
});
$.ajax({
    type: "GET",
    url: URLgetCategory,
    data: "{}",
    success: function (data) {
        var s = '<option value="" disabled="disabled" selected="selected">Chọn danh mục</option>';
        for (var i = 0; i < data.length; i++) {
            s += '<option value="' + data[i].categoryID + '">' + data[i].categoryName + '</option>';
        }
        $("#CategoryDropdown").html(s);
    }
});

//------------ Load dropdown FILTER add product----------------------------------
$.ajax({
    type: "GET",
    url: URLgetGroupProduct,
    data: "{}",
    success: function (data) {
        var s = '<option value="-1" >Xem tất cả nhóm hàng</option>';
        for (var i = 0; i < data.length; i++) {
            s += '<option value="' + data[i].groupID + '">' + data[i].groupName + '</option>';
        }
        $("#filter_GroupProduct").html(s);
    }
});
$.ajax({
    type: "GET",
    url: URLgetCategory,
    data: "{}",
    success: function (data) {
        var s = '<option value="-1" >Xem tất cả danh mục</option>';
        for (var i = 0; i < data.length; i++) {
            s += '<option value="' + data[i].categoryID + '">' + data[i].categoryName + '</option>';
        }
        $("#filter_Category").html(s);
    }
});

//----------------------FILTER PRODUCT------------------------------------------------
$('#URLProductList')
    .keypress(function () {
        URLProductList = $(this).val();
    })
    .keypress();

$("#filter_GroupProduct").change(function () {
    var group_id = $("#filter_GroupProduct").val();
    var category_id = $("#filter_Category").val();
    GetList(group_id, category_id)
});
$("#filter_Category").change(function () {
    var group_id = $("#filter_GroupProduct").val();
    var category_id = $("#filter_Category").val();
    GetList(group_id, category_id)
});

function GetList(group_id, category_id) {
    $.ajax({
        url: URLProductList,
        data: {
            group_id: group_id,
            category_id: category_id,
        }
    }).done(function (result) {
        $('#dataContainer').html(result);
        $('#example').DataTable()
      
    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        alert("Something Went Wrong, Try Later");
    });
}

//----------------------LOAD FORM EDIT PRODUCT---------------------------------------

$('#URLFindProduct')
    .keypress(function () {
        URLFindProduct = $(this).val();
    })
    .keypress();

function GetProduct(ele, id) {
    row = $(ele).closest('tr');
    $.ajax({
        type: 'POST',
        url: URLFindProduct,
        data: { "Product_id": id },
        success: function (response) {
            $('#edit_id').val(response.id);
            $('#edit_unit').val(response.unit);
            $('#edit_quantity').val(response.quantity);
            $('#edit_sell_price').val(response.sell_price);
            $('#edit_purchase_price').val(response.purchase_price);
            $('#edit_name').val(response.name);
            var group_id = response.group_id;
            var category_id = response.category_id;
            $.ajax({
                type: "GET",
                url: URLgetGroupProduct,
                data: "{}",
                success: function (data) {
                    var s = '<option value="" disabled="disabled" >Chọn nhóm hàng</option>';
                    for (var i = 0; i < data.length; i++) {
                        if (group_id == data[i].groupID) {
                            s += '<option value="' + data[i].groupID + '" selected="selected">' + data[i].groupName + '</option>';
                        } else {
                            s += '<option value="' + data[i].groupID + '">' + data[i].groupName + '</option>';
                        }
                    }
                    $("#edit_GroupProduct").html(s);
                }
            });
            $.ajax({
                type: "GET",
                url: URLgetCategory,
                data: "{}",
                success: function (data) {
                    var s = '<option value="" disabled="disabled">Chọn danh mục</option>';
                    for (var i = 0; i < data.length; i++) {
                        if (category_id == data[i].categoryID) {
                            s += '<option value="' + data[i].categoryID + '"  selected="selected">' + data[i].categoryName + '</option>';
                        } else {
                            s += '<option value="' + data[i].categoryID + '" >' + data[i].categoryName + '</option>';

                        }
                    }
                    $("#edit_Category").html(s);
                }
            });
            $('#EditProduct .close').css('display', 'none');
            $('#EditProduct').modal('show');
        }
    })
}

function CopyProduct(ele, id) {
    row = $(ele).closest('tr');
    $.ajax({
        type: 'POST',
        url: URLFindProduct,
        data: { "Product_id": id },
        success: function (response) {
            $('#add_unit').val(response.unit);
            $('#add_quantity').val(response.quantity);
            $('#product-price-sold').val(response.sell_price);
            $('#product-price-buy').val(response.purchase_price);
            $('#add_name').val(response.name);
            var group_id = response.group_id;
            var category_id = response.category_id;
            $.ajax({
                type: "GET",
                url: URLgetGroupProduct,
                data: "{}",
                success: function (data) {
                    var s = '<option value="" disabled="disabled" >Chọn nhóm hàng</option>';
                    for (var i = 0; i < data.length; i++) {
                        if (group_id == data[i].groupID) {
                            s += '<option value="' + data[i].groupID + '" selected="selected">' + data[i].groupName + '</option>';
                        } else {
                            s += '<option value="' + data[i].groupID + '">' + data[i].groupName + '</option>';
                        }
                    }
                    $("#GroupProductDropdown").html(s);
                }
            });
            $.ajax({
                type: "GET",
                url: URLgetCategory,
                data: "{}",
                success: function (data) {
                    var s = '<option value="" disabled="disabled">Chọn danh mục</option>';
                    for (var i = 0; i < data.length; i++) {
                        if (category_id == data[i].categoryID) {
                            s += '<option value="' + data[i].categoryID + '"  selected="selected">' + data[i].categoryName + '</option>';
                        } else {
                            s += '<option value="' + data[i].categoryID + '" >' + data[i].categoryName + '</option>';

                        }
                    }
                    $("#CategoryDropdown").html(s);
                }
            });
            $('#AddProduct .close').css('display', 'none');
            $('#AddProduct').modal('show');
        }
    })
}


//-------------------------------UPDATE PRODUCT--------------------------------

$('#URLUpdateProduct')
    .keypress(function () {
        URLUpdateProduct = $(this).val();
    })
    .keypress();

function Update_Product() {
    var table = $('#example').DataTable();
    var product = {};
    product.id = $('#edit_id').val();
    product.name = $('#edit_name').val();
    product.unit = $('#edit_unit').val();
    product.quantity = $('#edit_quantity').val();
    product.sell_price = Number($('#edit_sell_price').val().replace(/,/g, ''));
    product.purchase_price = Number($('#edit_purchase_price').val().replace(/,/g, ''));
    product.group_id = $('#edit_GroupProduct').val();
    product.category_id = $('#edit_Category').val();
  
    $.ajax({
        url: URLUpdateProduct,
        async: false,
        type: "Post",
        data: JSON.stringify(product),
        contentType: "application/json; charset=UTF-8",
        dataType: "json",
        success: function (response) {
            var group_id = $("#filter_GroupProduct").val();
            var category_id = $("#filter_Category").val();
            $.ajax({
                url: URLProductList,
                data: {
                    group_id: group_id,
                    category_id: category_id,
                }
            }).done(function (result) {
                $('#dataContainer').html(result);
                $('#example').DataTable()
                $('#EditProduct .close').css('display', 'none');
                $('#EditProduct').modal('hide');

                sweetAlert
                    ({
                        title: "Cập nhật thành công !",
                        type: "success",
                        allowOutsideClick: true,

                    })
            }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus)
                console.log(errorThrown)
                alert("Something Went Wrong, Try Later");
            });
        }
    });
}

//-------------------------Get Group Product -------------------------------

$('#URLFindGroupProduct')
    .keypress(function () {
        URLFindGroupProduct = $(this).val();
    })
    .keypress();
function GetGroupProduct(ele, id) {
    row = $(ele).closest('tr');
    $.ajax({
        type: 'POST',
        url: URLFindGroupProduct,
        data: { "GroupProduct_id": id },
        success: function (response) {
            $('#edit_id').val(response.id);
            $('#Edit_name').val(response.name);

            $('#Edit_Modal .close').css('display', 'none');
            $('#Edit_Modal').modal('show');
        }
    })
}

//------------------------UPDATE GROUP PRODUCT-----------------------------
$('#URLUpdateGroupProduct')
    .keypress(function () {
        URLUpdateGroupProduct = $(this).val();
    })
    .keypress();

function Update() {
    var table = $('#example').DataTable();
    var group = {};
    group.id = $('#edit_id').val();
    group.name = $('#Edit_name').val();

    $.ajax({
        url: URLUpdateGroupProduct,
        type: "Post",
        data: JSON.stringify(group),
        contentType: "application/json; charset=UTF-8",
        dataType: "json",
        success: function (response) {
            $('#Edit_Modal .close').css('display', 'none');
            $('#Edit_Modal').modal('hide');
            table.cell(row, 2).data($('#Edit_name').val());
            table.draw();
            sweetAlert
                ({
                    title: "Cập nhật thành công !",
                    type: "success",
                    allowOutsideClick: true,

                })
        }
    });
}

//---------------------Export Excel-------------------------
$('#URLExportExcel')
    .keypress(function () {
        URLExportExcel = $(this).val();
    })
    .keypress();

$('#btnExportExcel').click(function () {
    group_id = $("#filter_GroupProduct").val();
    category_id = $("#filter_Category").val();
    if (group_id == null || category_id == null) {
       
    } else {
       var URLExportExcel1 = URLExportExcel + "?group_id=" + group_id + "&category_id=" + category_id;
        window.location.href = URLExportExcel1;
        /*sweetAlert
            ({
                title: "Xuất sản phẩm thành công !",
                type: "success"
            })*/
    }
    
})

$('#btnExportExcel_Inventory').click(function () {
    var date_start = $("#filter_DateStart").val();
    var date_end = $("#filter_DateEnd").val();    
        var URLExportExcel1 = URLExportExcel + "?date_start=" + date_start + "&date_end=" + date_end;
        window.location.href = URLExportExcel1;
})
//----------------------FILTER INVENTORY PRODUCT------------------------------------------------
$('#URLInventoryList')
    .keypress(function () {
        URLInventoryList = $(this).val();
    })
    .keypress();

$("#filter_DateStart").change(function () {
    var date_start = $("#filter_DateStart").val();
    var date_end = $("#filter_DateEnd").val();
   
    if (date_start <= date_end) {
        GetList_Inventory(date_start, date_end);
    } else {
        sweetAlert
            ({
                title: "Lỗi",
                text: "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !",
                type: "error",
                allowOutsideClick: true,

            })
    }
});
$("#filter_DateEnd").change(function () {
    var date_start = $("#filter_DateStart").val();
    var date_end = $("#filter_DateEnd").val();
    if (date_start <= date_end) {
        GetList_Inventory(date_start, date_end);
    } else {
        sweetAlert
            ({
                title: "Lỗi",
                text: "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !",
                type: "error",
                allowOutsideClick: true,

            })
    }
});

function GetList_Inventory(date_start, date_end) {
    $.ajax({
        url: URLInventoryList,
        data: {
            date_start: date_start,
            date_end: date_end,
        }
    }).done(function (result) {
        $('#dataContainer').html(result);
        $('#example').DataTable();
        $('#example1').DataTable();
        $('#example2').DataTable();

    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        alert("Something Went Wrong, Try Later");
    });
}
//----------------------FILTER CUSTOMER------------------------------------------------
$('#URLCustomerList')
    .keypress(function () {
        URLCustomerList = $(this).val();
    })
    .keypress();

$("#filter_type").change(function () {
    var type = $("#filter_type").val();
    GetList_Customer(type)
});
function GetList_Customer(type) {
    $.ajax({
        url: URLCustomerList,
        data: {
            type: type,
          
        }
    }).done(function (result) {
        $('#dataContainer').html(result);
        $('#example').DataTable();
    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        alert("Something Went Wrong, Try Later");
    });
}
//----------------------LOAD FORM EDIT CUSTOMER---------------------------------------

$('#URLFindCustomer')
    .keypress(function () {
        URLFindCustomer = $(this).val();
    })
    .keypress();

function GetCustomer(ele, id) {
    row = $(ele).closest('tr');
    $.ajax({
        type: 'POST',
        url: URLFindCustomer,
        data: { "customer_id": id },
        success: function (response) {
            $('#edit_id').val(response.id);
            $('#edit_customer_name').val(response.name);
            $('#edit_customer_phone').val(response.phone);
            $('#edit_customer_email').val(response.email);
            $('#customers_birth').val(response.birthday);
            $('#edit_customer_account').val(response.account_number);
            $('#edit_customer_bank').val(response.bank);
            $('#edit_customer_type').val(response.type);
            $('textarea#edit_customer_address').html(response.address);
            $('textarea#edit_customer_note').html(response.note);
            
            $('#EditCustomer .close').css('display', 'none');
            $('#EditCustomer').modal('show');
        }
    })
}

//-------------------------------UPDATE PRODUCT--------------------------------

$('#URLUpdateCustomer')
    .keypress(function () {
        URLUpdateCustomer = $(this).val();
    })
    .keypress();

function Update_Customer() {
    var table = $('#example').DataTable();
    var customer = {};
    customer.id = $('#edit_id').val();
    customer.name = $('#edit_customer_name').val();
    customer.phone = $('#edit_customer_phone').val();
    customer.email = $('#edit_customer_email').val();
    customer.birthday = $('#customers_birth').val();
    customer.account_number = $('#edit_customer_account').val();
    customer.bank = $('#edit_customer_bank').val();
    customer.type = $('#edit_customer_type').val();
    customer.address = $('#edit_customer_address').val();
    customer.note = $('#edit_customer_note').val();

    console.log(customer);
    $.ajax({
        url: URLUpdateCustomer,
        type: "Post",
        data: JSON.stringify(customer),
        contentType: "application/json; charset=UTF-8",
        dataType: "json",
        success: function (response) {
            var type = $("#filter_type").val();          
            $.ajax({
                url: URLCustomerList,
                data: {
                    type: type,
                }
            }).done(function (result) {
                $('#dataContainer').html(result);
                $('#example').DataTable()
                $('#EditCustomer .close').css('display', 'none');
                $('#EditCustomer').modal('hide');

                sweetAlert
                    ({
                        title: "Cập nhật thành công !",
                        type: "success",
                        allowOutsideClick: true,

                    })
            }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus)
                console.log(errorThrown)
                alert("Something Went Wrong, Try Later");
            });
        }
    });
}
function AddCustomerSale() {
    var URLAddCustomerSale = "";
    $('#URLAddCustomerSale')
        .keypress(function () {
            URLAddCustomerSale = $(this).val();
        })
        .keypress();
   
    var customer = {};
    customer.name = $('#create_customer_name').val();
    customer.phone = $('#sale_customer_phone').val();
    customer.email = $('#customer_email').val();
    customer.birthday = $('#customers_birth').val();
    customer.account_number = $('#customer_account').val();
    customer.bank = $('#customer_bank').val();
    customer.type = $('#sale_customer_type').val();
    customer.address = $('#customer_address').val();
    customer.note = $('#customer_note').val();

    console.log(URLAddCustomerSale);
    $.ajax({
        url: URLAddCustomerSale,
        type: "Post",
        data: JSON.stringify(customer),
        contentType: "application/json; charset=UTF-8",
        dataType: "json",
        success: function (response) {
            $('#customer_code').val(response.code);
            $('#customer_phone').val(response.phone);
            $('#count_sale').val(0);
            $('#debit_sum').val(0);
            if (response.type == 1 ) {
                $('#customer_type').val("Khách mua lẻ");
            } else if (response.type ==2 ) {
                $('#customer_type').val("Công ty (Khách mua sĩ)");
            } else if (response.type == 3) {
                $('#customer_type').val("Nhà cung cấp");
            }
            if ($("#cart_total").val() != undefined) {
                console.log($("#cart_total").val());
                $("#payment_btn").prop("disabled", false);
                $("#order_btn").prop("disabled", false);
            }
            $('#AddCustomer .close').css('display', 'none');
            $('#AddCustomer').modal('hide');         
        }
    });
}
//-------------------------------KHÔNG CHO NHẬP KÍ TỰ ĐẶC BIỆC--------------------------------

$('input').keypress(function (event) {
    var character = String.fromCharCode(event.keyCode);
    return isValid(character);
});

function isValid(str) {
    return !/[~`!#$%\^&*+=\\[\]\\';,/{}|\\":<>\?]/g.test(str);
}

//-------------------------------CHECK DỮ LIỆU TRÙNG--------------------------------

$('#URLChecknameAvailability')
    .keypress(function () {
        URLChecknameAvailability = $(this).val();
    })
    .keypress();


function UserCheck() {
    console.log(URLChecknameAvailability);
    $.post(URLChecknameAvailability,
        {
            categorydata: $("#name_category").val()
        },
        function (data) {
            if (data == 0) {
                $("#Status").html('<font color="Red" hidden>That name is taken.Try Another.</font>');
                $("#name_GroupProduct").css("border-color", "#adb5bd");
                document.querySelector("#btn_submit").disabled = false;


            }
            else if (data == 1) {
                $("#Status").html('<font color="Red">Tên nhóm hàng đã tồn tại, vui lòng nhập tên khác.</font>');
                $("#name_GroupProduct").css("border-color", "#e74c3c");
                document.querySelector("#btn_submit").disabled = true;

            }
        });
    $.post(URLChecknameAvailability,
        {
            categorydata: $("#name_GroupProduct").val()
        },
        function (data) {
            if (data == 0) {
                $("#Status").html('<font color="Red" hidden>That name is taken.Try Another.</font>');             
                $("#name_GroupProduct").css("border-color", "#adb5bd");
                document.querySelector("#btn_submit").disabled = false;


            }
            else if (data == 1) {
                $("#Status").html('<font color="Red">Tên danh mục đã tồn tại, vui lòng nhập tên khác.</font>');       
                $("#name_GroupProduct").css("border-color", "#e74c3c");
                document.querySelector("#btn_submit").disabled = true;

            }
        });
}

function UserCustomer() {
    var customer_name = $("#customer_name").val();
    if (customer_name != "" && customer_name != undefined) {
        var URLCheckCustomernameAvailability = "";
        $('#URLCheckCustomernameAvailability')
            .keypress(function () {
                URLCheckCustomernameAvailability = $(this).val();
            })
            .keypress();
        $.post(URLCheckCustomernameAvailability,
            {
                customer_name: customer_name
            },
            function (data) {
                if (data == 0) {
                    $('#customer_id').val('');
                    $('#customer_code').val('');
                    $('#customer_phone').val('');
                    $('#customer_type').val('');
                    $('#count_sale').val('');
                    $('#debit_sum').val('');
                    $('#debit_sum').val('');

                    swal({
                        title: "Khách hàng không có trong hệ thống, bạn có muốn thêm mới ?",
                        type: "warning",
                        showCancelButton: true,
                        closeOnConfirm: true,
                        confirmButtonText: "Xác nhận",
                        confirmButtonColor: "#ec6c62",
                        allowOutsideClick: true,
                    },
                        function (isConfirm) {
                            if (isConfirm) {
                                $('#AddCustomer .close').css('display', 'none');
                                $('#AddCustomer').modal('show');
                                $('#create_customer_name').val($('#customer_name').val());
                                document.querySelector("#create_customer_name").disabled = true;

                            }
                        })

                }

            });
    }
  
  
}
//------------------------Import Fail Add-----------------------------
$('#URLUpdateGroupProduct')
    .keypress(function () {
        URLUpdateGroupProduct = $(this).val();
    })
    .keypress();

function ImportFail_continue() {
    var table = $('#example').DataTable();
    var group = {};
    group.id = $('#edit_id').val();
    group.name = $('#Edit_name').val();

    $.ajax({
        url: URLUpdateGroupProduct,
        type: "Post",
        data: JSON.stringify(group),
        contentType: "application/json; charset=UTF-8",
        dataType: "json",
        success: function (response) {
          
            table.cell(row, 2).data($('#Edit_name').val());
            table.draw();
            sweetAlert
                ({
                    title: "Cập nhật thành công !",
                    type: "success",
                    allowOutsideClick: true,

                }, function () {
                    $("table tbody").find('input[name="record"]').each(function () {
                        if (Contains($(this).val(), code)) {
                            $(this).parents("tr").remove();
                        }
                    });
                })
        }
    });
}

//-------------------------------CREATE CART - SALE DETAIL --------------------------------

$('#URLCreateCart')
    .keypress(function () {
        URLCreateCart = $(this).val();
    })
    .keypress();
$('#URLCartList')
    .keypress(function () {
        URLCartList = $(this).val();
    })
    .keypress();
function Create_Cart() {
   
    var cart_create = {};
    cart_create.product_id = $('#product_id').val();
    cart_create.customer_id = $('#customer_id').val();
    cart_create.quantity = $('#product_quantity').val();
    cart_create.price = Number($('#sum_price').val().replace(/\,/g, '').replace(/\./g, ''));
    cart_create.discount = $('#product_discount').val();
    cart_create.note = $('#cart_note').val();   
    console.log(Number($('#sum_price').val().replace(/\,/g, '').replace(/\./g, '')));
        $.ajax({
            url: URLCreateCart,
            type: "Post",
            data: JSON.stringify(cart_create),
            contentType: "application/json; charset=UTF-8",
            dataType: "json",
            success: function (response) {
                if (response.message == "Record Saved Successfully") {
                    GetList_Cart($('#customer_id').val());
                    $("#payment_btn").prop("disabled", false);
                    LoadDataProduct($('#product_id').val());
                    sweetAlert
                        ({
                            title: "Thêm giỏ hàng thành công !",
                            type: "success",
                            allowOutsideClick: true,

                        })
                } else {
                    sweetAlert
                        ({
                            title: "Số lượng sản phẩm chỉ còn: " + response.message + " sản phẩm !",
                            type: "error",
                            allowOutsideClick: true,

                        })
                }        
                
            }
        });   
   
}

function GetList_Cart(customer_id) {
    $.ajax({
        url: URLCartList,
        data: {
            customer_id: customer_id,

        }
    }).done(function (result) {
        $('#dataContainer').html(result);
        $('#example').DataTable()

       
    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        alert("Something Went Wrong, Try Later");
    });
}
$("#product_quantity").change(function () {
    var sum_price = Number($('#product_price').val().replace(/\,/g, '').replace(/\./g, '')) * Number($('#product_quantity').val()) * (1 - (parseFloat($('#product_discount').val()) / 100));
    $('#sum_price').val(sum_price.toLocaleString());
});
$("#product_discount").change(function () {
    var sum_price = Number($('#product_price').val().replace(/\,/g, '').replace(/\./g, ''))* Number($('#product_quantity').val()) * (1 - (parseFloat($('#product_discount').val()) / 100));
    $('#sum_price').val(sum_price.toLocaleString());
});

//-------------------------Import data fail one-----------------------
var URLImportFail_continues = "";
$('#URLImportFail_continues')
    .keypress(function () {
        URLImportFail_continues = $(this).val();
    })
    .keypress();

function ImportFail_continues(id, code) {
    swal({
        title: "Bạn chắn chắn muốn thêm dữ liệu ?",
        type: "warning",
        showCancelButton: true,
        closeOnConfirm: false,
        confirmButtonText: "Xác nhận",
        confirmButtonColor: "#ec6c62",
        allowOutsideClick: true,

    },
        function () {

            $.ajax({
                url: URLImportFail_continues,
                data: { id: id },

            })
                .done(function (data) {
                    sweetAlert
                        ({
                            title: "Đã thêm thành công!",
                            type: "success",
                            allowOutsideClick: true,

                        },
                            function () {
                                $("table tbody").find('input[name="record"]').each(function () {
                                    if (Contains($(this).val(), code)) {
                                        $(this).parents("tr").remove();
                                    }
                                });
                                GetList(-1, -1);
                            })
                })
            /*   .error(function (data) {
                   swal("OOps", "Chúng tôi không thể kết nối đến server!", "error");
               })*/
        })
}
//-------------------------Import data fail all-----------------------
var ImportFail_continues_ALL = "";
$('#ImportFail_continues_ALL')
    .keypress(function () {
        ImportFail_continues_ALL = $(this).val();
    })
    .keypress();

function ImportFail_continuesAll() {
    swal({
        title: "Bạn chắn chắn muốn thêm tất cả dữ liệu ?",
        type: "warning",
        showCancelButton: true,
        closeOnConfirm: false,
        confirmButtonText: "Xác nhận",
        confirmButtonColor: "#ec6c62",
        allowOutsideClick: true,

    },
        function () {

            $.ajax({
                url: ImportFail_continues_ALL,
                data: {},

            })
                .done(function (data) {
                    sweetAlert
                        ({
                            title: "Đã thêm thành công !",
                            type: "success",
                            allowOutsideClick: true,

                        },
                            function () {                              
                              /*  $("#FailFormat_table tr").remove(); */
                                $('#table_body_failimport').empty()
                                $("#btn_ImportFail_continues_ALL").prop("disabled", true);

                                GetList(-1, -1);
                            })
                })
            /*   .error(function (data) {
                   swal("OOps", "Chúng tôi không thể kết nối đến server!", "error");
               })*/
        })
}



function ButtonDebit() {
    var cart_Prepay1 = $('#cart_Prepay').val();
    
    if (cart_Prepay1 != undefined && cart_Prepay1 != '0') {
        $("#btn_debit").prop("disabled", false);
        $("#payment_btn").prop("disabled", true);
    } else {
        $("#btn_debit").prop("disabled", true);
        $("#payment_btn").prop("disabled", false);
    }
}


//---------------------load customer sale----------------------------
var URLGetSearchValue = "";
$('#URLGetSearchValue')
    .keypress(function () {
        URLGetSearchValue = $(this).val();
    })
    .keypress();
var URLFindCustomer_name = "";
$('#URLFindCustomer_name')
    .keypress(function () {
        URLFindCustomer_name = $(this).val();
    })
    .keypress();
$(function () {
    $("#customer_name").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: URLGetSearchValue,
                data: "{ 'search': '" + request.term + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;

                    }))

                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            $('#customer_id').val(i.item.val);
            GetList_Cart(i.item.val);
            $.ajax({
                type: 'POST',
                url: URLFindCustomer_name,
                data: { "customer_id": i.item.val },
                success: function (response) {
                    $('#customer_code').val(response.code);
                    $('#customer_phone').val(response.phone);
                    $('#customer_type').val(response.note);
                    $('#count_sale').val(response.status);
                    $('#debit_sum').val(response.type.toLocaleString());
                    if ($("#cart_total").val() != undefined) {
                        console.log($("#cart_total").val());
                        $("#payment_btn").prop("disabled", false);
                        $("#order_btn").prop("disabled", false);
                    }
                }
            })
        },
        minLength: 0
    }).focus(function () {
        if ($(this).autocomplete("widget").is(":visible")) {
            return;
        }
        $(this).data("autocomplete").search($(this).val());
    });
});

//---------------------load product sale----------------------------

var URLGetSearchValue_product = "";
$('#URLGetSearchValue_product')
    .keypress(function () {
        URLGetSearchValue_product = $(this).val();
    })
    .keypress();
var URLFindProduct_name = "";
$('#URLFindProduct_name')
    .keypress(function () {
        URLFindProduct_name = $(this).val();
    })
    .keypress();
$(function () {
    $("#product_name").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: URLGetSearchValue_product,
                data: "{ 'search': '" + request.term + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;

                    }))

                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            $('#product_id').val(i.item.val);
            LoadDataProduct(i.item.val);
        },
        minLength: 0
    }).focus(function () {
        if ($(this).autocomplete("widget").is(":visible")) {
            return;
        }
        $(this).data("autocomplete").search($(this).val());
    });
});
function LoadDataProduct(id) {
    $.ajax({
        type: 'POST',
        url: URLFindProduct_name,
        data: { "product_id": id },
        success: function (response) {
            $('#product_code').val(response.code);
            $('#product_unit').val(response.note);
            $('#product_price').val(response.name);
            var sum_price = Number($('#product_price').val().replace(/\,/g, '').replace(/\./g, '')) * Number($('#product_quantity').val()) * (1 - (parseFloat($('#product_discount').val()) / 100));
            $('#sum_price').val(sum_price.toLocaleString());

        }
    })
}
//------------------------Payment / Debit-------------------------------------
var URLCreateSale = "";
$('#URLCreateSale')
    .keypress(function () {
        URLCreateSale = $(this).val();
    })
    .keypress();
function Payment_order() {
    var sale = {};
    sale.customer_id = $('#customer_id').val();
    sale.total = Number($('#total').val().replace(/\,/g, '').replace(/\./g, ''));
    sale.discount = $('#discount').val();
    sale.vat = $('#vat').val();
    sale.note = $('#cart_note').val();
    sale.method = Number($('#cart_Prepay').val().replace(/\,/g, '').replace(/\./g, ''));
    console.log(URLCreateSale);
    $.ajax({
        url: URLCreateSale,
        async: false,
        type: "Post",
        data: JSON.stringify(sale),
        contentType: "application/json; charset=UTF-8",
        dataType: "json",
        success: function (response) {

            GetList_Cart($('#customer_id').val());
            sweetAlert
                ({
                    title: "Thanh toán thành công !",
                    type: "success"
                })
        }
    });
}

function FillProduct_cart(id, masp, tensp, soluong) {
    $('#product_name').val(tensp);
    $('#product_quantity').val(soluong);
    $('#cart_id').val(id);
    $('#product_id').val(masp);

    document.querySelector("#customer_name").disabled = true;
    document.querySelector("#product_name").disabled = true;

    $("#submit_addCart").hide();
    $("#refresh_cart").hide();
    $("#submit_updateCart").show();
    $("#Cancel_Cart").show();
    LoadDataProduct(masp);
   
    window.scrollTo({
        top: 0,
        behavior: `smooth`
    })
}
//-------------------Update cart-------------------------------

function Update_Cart() {
    var URLUpdateCart = "";
    $('#URLUpdateCart')
        .keypress(function () {
            URLUpdateCart = $(this).val();
        })
        .keypress();
  
    var cart_create = {};
    cart_create.id = $('#cart_id').val();;
    cart_create.product_id = $('#product_id').val();
    cart_create.customer_id = $('#customer_id').val();
    cart_create.quantity = $('#product_quantity').val();
    cart_create.price = Number($('#sum_price').val().replace(/\,/g, '').replace(/\./g, ''));
    cart_create.discount = $('#product_discount').val();
    cart_create.note = $('#cart_note').val();

    console.log(cart_create);
    $.ajax({
        url: URLUpdateCart,
        type: "Post",
        data: JSON.stringify(cart_create),
        contentType: "application/json; charset=UTF-8",
        dataType: "json",
        success: function (response) {
            if (response.message == "Record Saved Successfully") {
                GetList_Cart($('#customer_id').val());
                document.querySelector("#customer_name").disabled = false;
                document.querySelector("#product_name").disabled = false;
                $('#product_code').val('');
                $('#product_unit').val('');
                $('#product_price').val('');
                $('#product_name').val('');
                $('#sum_price').val('');
                $('#product_quantity').val(1);
                $("#submit_addCart").show();
                $("#refresh_cart").show();
                $("#submit_updateCart").hide();
                $("#Cancel_Cart").hide();

                sweetAlert
                    ({
                        title: "Cập nhật đơn hàng thành công !",
                        type: "success"
                    })
            } else {
                sweetAlert
                    ({
                        title: "Số lượng sản phẩm chỉ còn: " + response.message + " sản phẩm !",
                        type: "error"
                    })
            }

        }
    });

}

//----------------------FILTER INVENTORY PRODUCT------------------------------------------------
$('#URL_RevenueList_Date')
    .keypress(function () {
        URL_RevenueList_Date = $(this).val();
    })
    .keypress();
$('#URL_RevenueList_Month')
    .keypress(function () {
        URL_RevenueList_Month = $(this).val();
    })
    .keypress();

$("#revenue_DateStart").change(function () {
    var date_start = $("#revenue_DateStart").val();
    var date_end = $("#revenue_DateEnd").val();
    if (date_start <= date_end) {
        GetList_RevenueListDate(date_start, date_end);
    } else {
        sweetAlert
            ({
                title: "Lỗi",
                text: "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !",
                type: "error",
                allowOutsideClick: true,

            })
    }
    
});
$("#revenue_DateEnd").change(function () {
    var date_start = $("#revenue_DateStart").val();
    var date_end = $("#revenue_DateEnd").val();
    if (date_start <= date_end) {
        GetList_RevenueListDate(date_start, date_end);
    } else {
        sweetAlert
            ({
                title: "Lỗi",
                text: "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !",
                type: "error",
                allowOutsideClick: true,

            })
    }
});
$("#revenue_DateStart_Month").change(function () {
    var date_start = $("#revenue_DateStart_Month").val();
    var date_end = $("#revenue_DateEnd_Month").val();
    if (date_start <= date_end) {
        GetList_RevenueListMonth(date_start, date_end);
    } else {
        sweetAlert
            ({
                title: "Lỗi",
                text: "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !",
                type: "error",
                allowOutsideClick: true,

            })
    }

});
$("#revenue_DateEnd_Month").change(function () {
    var date_start = $("#revenue_DateStart_Month").val();
    var date_end = $("#revenue_DateEnd_Month").val();
    if (date_start <= date_end) {
        GetList_RevenueListMonth(date_start, date_end);
    } else {
        sweetAlert
            ({
                title: "Lỗi",
                text: "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !",
                type: "error",
                allowOutsideClick: true,

            })
    }
});

function GetList_RevenueListDate(date_start, date_end) {
    $.ajax({
        url: URL_RevenueList_Date,
        data: {
            date_Start: date_start,
            date_End: date_end,
        }
    }).done(function (result) {
        $('#dataContainer').html(result);

    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        alert("Something Went Wrong, Try Later");
    });
}

function GetList_RevenueListMonth(date_start, date_end) {
    $.ajax({
        url: URL_RevenueList_Month,
        data: {
            date_Start: date_start,
            date_End: date_end,
        }
    }).done(function (result) {
        $('#dataContainer1').html(result);

    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        alert("Something Went Wrong, Try Later");
    });
}

$("#sale_DateStart").change(function () {
    var date_start = $("#sale_DateStart").val();
    var date_end = $("#sale_DateEnd").val();
    console.log("hihi");
    if (date_start <= date_end) {
        GetList_OrderList(date_start, date_end);
    } else {
        sweetAlert
            ({
                title: "Lỗi",
                text: "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !",
                type: "error",
                allowOutsideClick: true,

            })
    }

});
$("#sale_DateEnd").change(function () {
    var date_start = $("#sale_DateStart").val();
    var date_end = $("#sale_DateEnd").val();
    if (date_start <= date_end) {
        GetList_OrderList(date_start, date_end);
    } else {
        sweetAlert
            ({
                title: "Lỗi",
                text: "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !",
                type: "error",
                allowOutsideClick: true,

            })
    }
});
var URL_OrderList = "";
$('#URL_OrderList')
    .keypress(function () {
        URL_OrderList = $(this).val();
    })
    .keypress();
function GetList_OrderList(date_start, date_end) {
   
    $.ajax({
        url: URL_OrderList,
        data: {
            date_Start: date_start,
            date_End: date_end,
        }
    }).done(function (result) {
        $('#dataContainer3').html(result);
        $('#example').DataTable()
    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        alert("Something Went Wrong, Try Later");
    });
}
//----------------------Xem trước hóa đơn------------------------------------------------
$('#URLgetCartProduct')
    .keypress(function () {
        URLgetCartProduct = $(this).val();
    })
    .keypress();
function openWin() {
    var curDate = new Date();
    // Ngày hiện tại
    var curDay = curDate.getDate();
    // Tháng hiện tại
    var curMonth = curDate.getMonth() + 1;
    // Năm hiện tại
    var curYear = curDate.getFullYear();
    // giờ hiện tại
    var curTime = curDate.getHours() + ":" + curDate.getMinutes() + ":" + curDate.getSeconds();
    var customer_id = $('#customer_id').val();
    var customer_code = $('#customer_code').val();
    var cart_total = $('#cart_total').val();
    var total = $('#total').val();
    var cart_vat = $('#cart_vat').val();
    var cart_discount = $('#cart_discount').val();
    var s = "";
    $.ajax({
        type: "GET",
        async: false,
        url: URLgetCartProduct,
        data: { id: customer_id },
        success: function (data) {

            for (var i = 0; i < data.length; i++) {
                var note = "";
                if (data[i].cartNote != null) {
                    note = data[i].cartNote;
                }
                s += `<tr>
                        <td class="text-center">`+ (i + 1) + `</td>
                        <td>`+ data[i].cartCode + `</td>
                        <td>`+ data[i].cartName + `</td>
                        <td class="text-center">`+ data[i].cartUnit + `</td>
                        <td class="text-center">`+ data[i].cartQuantity + `</td>
                        <td class="text-center">`+ data[i].cartDiscount + `%</td>
                        <td class="text-center">`+ data[i].cartPrice.toLocaleString() + `</td>
                        <td class="text-center">`+ data[i].cartTotal.toLocaleString() + `</td>
                        <td>`+ note + `</td>
                    </tr>`
            }
        }
    });
    var myWindow = window.open('', '', 'width=1200,height=800');
  /*  myWindow.document.write('<link href="/Template/assets/css/style.css" rel="stylesheet" />'); */
    myWindow.document.write('<link href="/CP25Team05/Template/assets/css/style.css" rel="stylesheet" />');
    myWindow.document.write(`<div class="col-md-12">
    <div class="card">
        <div class="card-header">
            <h4 class="text-center">DOANH NGHIỆP TẤN THÀNH</h4>
            <h6 class="text-center">Địa chỉ: 11/43 Ấp Tân Trung B, xã Tân Hưng, Huyện Tân Châu, Tỉnh Tây Ninh</h6>
            <h6 class="text-center">SDT: 0382399026</h6>
            <hr>
            <h4 class="text-center">BẢN XEM TRƯỚC HOÁ ĐƠN</h4>
        </div>
        <div class="card-body table-border-style">
            <div class="table-responsive">
                <div class="row">
                    <div class="col-md-6">
                        <p>Ngày: ` + curDay + ' tháng ' + curMonth + ' năm ' + curYear + ', Thời gian: ' + curTime + `</p>
                    </div>
                    <div class="col-md-6">
                        <p class="text-right">Mã khách hàng: `+ customer_code + `</p>
                    </div>
                </div>
                <table class="table table-hover">
                    <thead>
                        <tr>
                            <th class="text-center">STT</th>
                            <th>Mã hàng</th>
                            <th>Tên hàng hàng</th>
                            <th class="text-center">Đơn vị</th>
                            <th class="text-center">Số lượng</th>
                            <th class="text-center">Chiết khấu</th>
                            <th class="text-center">Đơn giá</th>
                            <th class="text-center">Thành tiền</th>
                            <th>Ghi chú</th>
                        </tr>
                    </thead>
                        <tbody>` + s + `</tbody >
                    <tfoot>
                        <tr>
                            <td colspan="7" class="text-right">
                                <h5>Tạm tính</h5>
                            </td>
                            <td colspan="2" class="text-left">
                                <h5>`+ cart_total + `₫</h5>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7" class="text-right">
                                <h5>VAT (%)</h5>
                            </td>
                            <td colspan="2" class="text-left">
                                <h5>`+ cart_vat + `%</h5>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7" class="text-right">
                                <h5>Chiết khấu (%)</h5>
                            </td>
                            <td colspan="2" class="text-left">
                                <h5>`+ cart_discount + `%</h5>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7" class="text-right">
                                <h5>Hình thức</h5>
                            </td>
                            <td colspan="2" class="text-left">
                                Xem trước (Chưa thanh toán)
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7" class="text-right">
                                <h5>Thành tiền</h5>
                            </td>
                            <td colspan="2" class="text-left">
                                <h5>
                                    `+ total + `₫
                                </h5>
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </div>
        </div>
    </div>
</div>`);

    myWindow.document.close();
    myWindow.focus();
}

function PrintOrder(id, code, method, total, discount, vat, create_at, customer_code) {
    
   
    var s = "";
    $.ajax({
        type: "GET",
        async: false,
        url: URLgetCartProduct,
        data: { id: id },
        success: function (data) {

            for (var i = 0; i < data.length; i++) {
                if (data[i].cartNote != null) {
                    note = data[i].cartNote;
                }
                s += `<tr>
                        <td class="text-center">`+ (i + 1) + `</td>
                        <td>`+ data[i].cartCode + `</td>
                        <td>`+ data[i].cartName + `</td>
                        <td class="text-center">`+ data[i].cartUnit + `</td>
                        <td class="text-center">`+ data[i].cartQuantity + `</td>
                        <td class="text-center">`+ data[i].cartDiscount + `%</td>
                        <td class="text-center">`+ data[i].cartPrice.toLocaleString() + `</td>
                        <td class="text-center">`+ data[i].cartTotal.toLocaleString() + `</td>
                    </tr>`
            }
        }
    });
    if (method == 1) {
        var form_method = "Đã thanh toán";
    } else if (method == 1) {
        var form_method = "Ghi nợ";
    }

    var myWindow = window.open('', '', 'width=1200,height=800');
/*      myWindow.document.write('<link href="/Template/assets/css/style.css" rel="stylesheet" />'); 
*/    myWindow.document.write('<link href="/CP25Team05/Template/assets/css/style.css" rel="stylesheet" />');
    myWindow.document.write(`<div class="col-md-12">
    <div class="card">
        <div class="card-header">
            <h4 class="text-center">DOANH NGHIỆP TẤN THÀNH</h4>
            <h6 class="text-center">Địa chỉ: 11/43 Ấp Tân Trung B, xã Tân Hưng, Huyện Tân Châu, Tỉnh Tây Ninh</h6>
            <h6 class="text-center">SDT: 0382399026</h6>
            <hr>
         <h4 class="text-center">HOÁ ĐƠN BÁN HÀNG</h4>
        </div>
        <div class="card-body table-border-style">
            <div class="table-responsive">
                <div class="row">
                    <div class="col-md-6">
                        <p>Ngày tạo đơn: ` + create_at + `</p>
                    </div>
                    <div class="col-md-6">
                        <p class="text-right">Mã hóa đơn: `+ code + `</p>
                        <p class="text-right">Mã khách hàng: `+ customer_code + `</p>
                    </div>
                </div>
                <table class="table table-hover">
                    <thead>
                        <tr>
                            <th class="text-center">STT</th>
                            <th>Mã hàng</th>
                            <th>Tên hàng hàng</th>
                            <th class="text-center">Đơn vị</th>
                            <th class="text-center">Số lượng</th>
                            <th class="text-center">Chiết khấu</th>
                            <th class="text-center">Đơn giá</th>
                            <th class="text-center">Thành tiền</th>
                        </tr>
                    </thead>
                        <tbody>` + s + `</tbody >
                    <tfoot>
                        <tr>
                            <td colspan="7" class="text-right">
                                <h5>Tạm tính</h5>
                            </td>
                            <td colspan="2" class="text-left">
                                <h5>`+ (total / (1 + vat / 100 - discount / 100)).toLocaleString() + `₫</h5>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7" class="text-right">
                                <h5>VAT (%)</h5>
                            </td>
                            <td colspan="2" class="text-left">
                                <h5>`+ vat + `%</h5>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7" class="text-right">
                                <h5>Chiết khấu (%)</h5>
                            </td>
                            <td colspan="2" class="text-left">
                                <h5>`+ discount + `%</h5>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7" class="text-right">
                                <h5>Hình thức</h5>
                            </td>
                            <td colspan="2" class="text-left">
                               `+ form_method +`
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7" class="text-right">
                                <h5>Thành tiền</h5>
                            </td>
                            <td colspan="2" class="text-left">
                                <h5>
                                    `+ total.toLocaleString() + `₫
                                </h5>
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </div>
        </div>
    </div>
</div>`);
    
    myWindow.print();
    myWindow.document.close();
    myWindow.focus();
}
//---------------------------click---------------
