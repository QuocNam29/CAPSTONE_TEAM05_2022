
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
//----------------------Add, Update group, category-------------------

$('#URL_List')
    .keypress(function () {
        URL_List = $(this).val();
    })
    .keypress();

function GetList_CategoryAndGroup() {
    $.ajax({
        url: URL_List,
        data: {}
    }).done(function (result) {
        $('#dataContainer').html(result);
        $('#example').DataTable()

    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
    });
}
$(document).ready(function () {
    var forms = document.getElementsByClassName('needs-validation');
    var validation = Array.prototype.filter.call(forms, function (form) {     
               
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
            $('#history_btn').hide();

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

//---------------------load customer sale----------------------------
var URLGetSearchValue = "";
$('#URLGetSearchValue')
    .keypress(function () {
        URLGetSearchValue = $(this).val();
    })
    .keypress();
var URLGetSearch_phoneValue = "";
$('#URLGetSearch_phoneValue')
    .keypress(function () {
        URLGetSearch_phoneValue = $(this).val();
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
                    $("#history_btn").show();
                    if ($("#total").val() != undefined) {
                        $("#payment_btn").show();
                        $("#order_btn").show();
                    } else {
                        $("#payment_btn").hide();
                        $("#order_btn").hide();
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
$(function () {
    $("#customer_phone").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: URLGetSearch_phoneValue,
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
                    $('#customer_name').val(response.name);
                    $('#customer_code').val(response.code);
                    $('#customer_phone').val(response.phone);
                    $('#customer_type').val(response.note);
                    $('#count_sale').val(response.status);
                    $('#debit_sum').val(response.type.toLocaleString());
                    if ($("#total").val() != undefined) {
                        $("#payment_btn").show();
                        $("#order_btn").show();
                    } else {
                        $("#payment_btn").hide();
                        $("#order_btn").hide();
                    }
                }
            })
        },
        minLength: 0
    })/*.focus(function () {
        if ($(this).autocomplete("widget").is(":visible")) {
            return;
        }
        $(this).data("autocomplete").search($(this).val());
    });*/
});
//--------------------------Add, Update Product----------------------------------
$('.ProductForm').submit(function (e) {
    var form = $(this);

    // Check if form is valid then submit ajax
    if (form[0].checkValidity()) {
        e.preventDefault();
        var url = form.attr('action');
        console.log();
        $.ajax({
            url: url,
            type: 'POST',
            data: form.serialize(),
            success: function (data) {
                // Hide bootstrap modal to prevent conflict
                $('.modal').modal('hide');

                if (data.status) {
                    // Refresh table data
                    var group_id = $("#filter_GroupProduct").val();
                    var category_id = $("#filter_Category").val();
                    GetList(group_id, category_id)
                    Swal.fire('Thành công!', data.message, 'success')                  
                    form[0].reset();
                    form.removeClass('was-validated');
                } else {
                    Swal.fire('Lỗi !', data.message , 'error');
                }
            }
        });
    }
});
//-------------------------------ADD CUSTOMER SALE--------------------------------

$('.ProductSaleForm').submit(function (e) {
    var form = $(this);

    // Check if form is valid then submit ajax
    if (form[0].checkValidity()) {
        e.preventDefault();
        var url = form.attr('action');

        $.ajax({
            url: url,
            type: 'POST',
            data: form.serialize(),
            success: function (data) {
                // Hide bootstrap modal to prevent conflict
                $('.modal').modal('hide');

                if (data.status) {
                    // Refresh table data
                    Swal.fire('Thành công !', data.message, 'success')
                    form[0].reset();
                    form.removeClass('was-validated');
                    $('#product_id').val(data.id);
                    LoadDataProduct(data.id);
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: data.message,
                    }).then((result) => {
                        if (result.isConfirmed) {
                            $('#AddProduct').modal('show');
                        }
                    });// Show bootstrap modal again                           
                }
            }
        });
    }
});

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
    Swal.fire({
        title: 'Bạn có chắc chắn muốn xóa',
        text: "Bạn không thể hoàn nguyên nếu có sai xót!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Xác nhận'
    }).then((result) => {
        if (result.isConfirmed) {
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
                        Swal.fire({
                            icon: 'success',
                            title: 'Thành công',
                            text: data.message,
                        }).then((result) => {
                            if (result.isConfirmed) {
                                $("table tbody").find('input[name="record"]').each(function () {
                                    if (Contains($(this).val(), code)) {
                                        $(this).parents("tr").remove();
                                    }
                                });
                            }
                        })
                     
                    }
                    else {
                        Swal.fire('Lỗi !', data.message, 'error');
                    }
                })
            /*   .error(function (data) {
               swal("OOps", "Chúng tôi không thể kết nối đến server!", "error");
           })*/        
        }
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
    var categorys = {};
    categorys.id = id;
    $.ajax({
        url: URDEditStatus,
        data: JSON.stringify(categorys),
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    }).done(function (data) {
        Swal.fire('Cập nhật trạng thái thành công!', '', 'success')
    })
}

//-----------------------Định dạng giá-------------------------------------

$('.Price').keydown(function (e) {
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
var URLgetSupplier = "";
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
$('#URLgetSupplier')
    .keypress(function () {
        URLgetSupplier = $(this).val();
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
$.ajax({
    type: "GET",
    url: URLgetSupplier,
    data: "{}",
    success: function (data) {
        var s = '<option value="" disabled="disabled" selected="selected">Chọn nhà cung cấp</option>';
        for (var i = 0; i < data.length; i++) {
            s += '<option value="' + data[i].supplierID + '">' + data[i].supplierName + '</option>';
        }
        $(".SupplierDropdown").html(s);
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
        Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
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

//-------------------------Get Group Product -------------------------------

$('#URLFindGroupProduct')
    .keypress(function () {
        URLFindGroupProduct = $(this).val();
    })
    .keypress();
function GetGroupProduct(ele, id, name) {
    row = $(ele).closest('tr');
   
            $('#edit_id').val(id);
            $('#Edit_name').val(name);

            $('#Edit_Modal .close').css('display', 'none');
            $('#Edit_Modal').modal('show');
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
    }
})

$('#btnExportExcel_Inventory').click(function () {
    var date_start = $("#filter_DateStart").val();
    var date_end = $("#filter_DateEnd").val();    
        var URLExportExcel1 = URLExportExcel + "?date_start=" + date_start + "&date_end=" + date_end;
        window.location.href = URLExportExcel1;
})
//-----------------------Export Excel Revenue-----------------------------------------

$('#btnExportExcel_profit').click(function () { 
    $('#URLExportExcel_profit')
        .keypress(function () {
            URLExportExcel_profit = $(this).val();
        })
        .keypress();
    window.location.href = URLExportExcel_profit;
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
        Swal.fire('Lỗi!', 'Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !', 'error')     
    }
});
$("#filter_DateEnd").change(function () {
    var date_start = $("#filter_DateStart").val();
    var date_end = $("#filter_DateEnd").val();
    if (date_start <= date_end) {
        GetList_Inventory(date_start, date_end);
    } else {
        Swal.fire('Lỗi!', 'Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !', 'error')     
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
        Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
    });
}
//----------------------FILTER CusomerForm------------------------------------------------
$('#URLCustomerList')
    .keypress(function () {
        URLCustomerList = $(this).val();
    })
    .keypress();
$("#filter_type").change(function () {
    var type = $("#filter_type").val();
    GetList_Customer(type);
    TestPrint();
});

function GetList_Customer(type) {
    $.ajax({
        url: URLCustomerList,
        data: {
            type,

        }
    }).done(function (result) {

        $('#dataContainer').html(result);
        $('#example').DataTable();
    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
    });
}

$('#URLTestPrint')
    .keypress(function () {
        URLTestPrint = $(this).val();
    })
    .keypress();
function PrintOrder(id) {
        $.ajax({
            url: URLTestPrint,
            data: {
                id: id
            }, 
            async: false,
        }).done(function (result) {
            var WinPrint = window.open('', '', 'width=1200,height=800');
            WinPrint.document.write('<html><head>');
            WinPrint.document.write('</head><body onload="print();">');
            WinPrint.document.write(result);
            WinPrint.document.write('</body></html>');
            WinPrint.document.close();
            WinPrint.focus();        
        }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus)
            console.log(errorThrown)
            Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
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
            $('#edit_customers_birth').val(response.birthday);
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
//----------------------LOAD FORM EDIT USER---------------------------------------

$('#URLFindUser')
    .keypress(function () {
        URLFindUser = $(this).val();
    })
    .keypress();

function GetUser(ele, id) {
    row = $(ele).closest('tr');
    $.ajax({
        type: 'POST',
        url: URLFindUser,
        data: { "user_id": id },
        success: function (response) {
            $('#edit_id').val(response.id);
            $('#edit_name_user').val(response.name);
            $('#edit_phone_user').val(response.phone);
            $('#edit_email_user').val(response.email);
            $('#edit_role_user').val(response.remember_token);
            $('#edit_address_user').val(response.address);

            $('#EditUser .close').css('display', 'none');
            $('#EditUser').modal('show');
        }
    })
}
//-------------------------------ADD CUSTOMER SALE--------------------------------

$('.AddCustomerSaleFrom').submit(function (e) {
    var form = $(this);

    // Check if form is valid then submit ajax
    if (form[0].checkValidity()) {
        e.preventDefault();
        var url = form.attr('action');
        $.ajax({
            url: url,
            type: 'POST',
            data: form.serialize(),
            success: function (data) {
                // Hide bootstrap modal to prevent conflict
                $('.modal').modal('hide');

                if (data.status) {
                    // Refresh table data
                    Swal.fire('Thành công !', data.message, 'success')     
                    form[0].reset();
                    form.removeClass('was-validated');
                    $('#customer_name').val(data.customer.name);
                    $('#customer_id').val(data.customer.id);
                    $('#customer_code').val(data.customer.code);
                    $('#customer_phone').val(data.customer.phone);
                    $('#count_sale').val(0);
                    $('#debit_sum').val(0);
                    if (data.customer.type == 1) {
                        $('#customer_type').val("Khách mua lẻ");
                    } else if (data.customer.type == 2) {
                        $('#customer_type').val("Công ty (Khách mua sĩ)");
                    } else if (data.customer.type == 3) {
                        $('#customer_type').val("Nhà cung cấp");
                    }
                    if ($("#total").val() != undefined) {
                        $("#payment_btn").show();
                        $("#order_btn").show();
                    } else {
                        $("#payment_btn").hide();
                        $("#order_btn").hide();
                    }
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: data.message,
                    }).then((result) => {
                        if (result.isConfirmed) {
                            $('#AddCustomer').modal('show');
                        }
                    });// Show bootstrap modal again                           
                }
            }
        });
    }
});

//-------------------------------KHÔNG CHO NHẬP KÍ TỰ ĐẶC BIỆC--------------------------------

/*$('input').keypress(function (event) {
    var character = String.fromCharCode(event.keyCode);
    return isValid(character);
});

function isValid(str) {
    return !/[~`!#$%\^&*+=\\[\]\\';,/{}|\\":<>\?]/g.test(str);
}*/

//-------------------------------CHECK Người dùng đã có trong hệ thống--------------------------------



/*//------------------------Import Fail Add-----------------------------
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
}*/

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
    cart_create.note = $('#cart_note').val();   
    cart_create.unit = $('#unit_product_swap').val();   
    console.log(cart_create);
        $.ajax({
            url: URLCreateCart,
            type: "Post",
            data: JSON.stringify(cart_create),
            contentType: "application/json; charset=UTF-8",
            dataType: "json",
            success: function (response) {
                if (response.message == "Record Saved Successfully") {
                    GetList_Cart($('#customer_id').val());
                    $("#payment_btn").show();
                    LoadDataProduct($('#product_id').val());
                    Swal.fire('Thành công !', 'Thêm giỏ hàng thành công !', 'success');
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: "Số lượng sản phẩm chỉ còn: " + response.message + " !",
                    })
                }        
                
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (textStatus == 'error') {
                    Swal.fire('Lỗi !', 'Vui lòng kiểm tra lại thông tin !', 'error');     
                }
                else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: "Trạng thái: " + textStatus + ", Lỗi: " + errorThrown +" !",
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
        Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
    });
}
$("#product_quantity").change(function () {
    var sum_price = Number($('#product_price').val().replace(/\,/g, '').replace(/\./g, '')) * Number($('#product_quantity').val());
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
    Swal.fire({
        title: "Cảnh báo !",
        text: "Bạn chắc chắn muốn thêm dữ liệu!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Xác nhận'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: URLImportFail_continues,
                data: { id: id },

            })
                .done(function (data) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Thành công !',
                        text: 'Đã thêm thành công!',
                    }).then((result) => {
                        if (result.isConfirmed) {
                            $("table tbody").find('input[name="record"]').each(function () {
                                if (Contains($(this).val(), code)) {
                                    $(this).parents("tr").remove();
                                }
                            });
                            GetList(-1, -1);
                        }
                    });
                })
            /*   .error(function (data) {
               swal("OOps", "Chúng tôi không thể kết nối đến server!", "error");
           })*/
        }
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
    Swal.fire({
        title: "Cảnh báo !",
        text: "Bạn chắc chắn muốn thêm tất cả dữ liệu!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Xác nhận'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: ImportFail_continues_ALL,
                data: {},

            })
                .done(function (data) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Thành công !',
                        text: 'Đã thêm thành công!',
                    }).then((result) => {
                        if (result.isConfirmed) {
                            /*  $("#FailFormat_table tr").remove(); */
                            $('#table_body_failimport').empty()
                            $("#btn_ImportFail_continues_ALL").prop("disabled", true);

                            GetList(-1, -1);
                        }
                    });
                })
            /*   .error(function (data) {
               swal("OOps", "Chúng tôi không thể kết nối đến server!", "error");
           })*/
        }
    })
}



function ButtonDebit() {
    var cart_Prepay1 = $('#cart_Prepay').val();
    
    if (cart_Prepay1 != undefined && cart_Prepay1 != '0') {
        $("#btn_debit").show();
        $("#payment_btn").hide();
    } else {
        $("#btn_debit").hide();
        $("#payment_btn").show();
    }
}



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
            console.log(response);
            $('#product_name').val(response.name);
            $('#product_code').val(response.code);
            $('#product_unit').val(response.note);
            $('#product_price').val(response.name_category);
            var sum_price = Number($('#product_price').val().replace(/\,/g, '').replace(/\./g, '')) * Number($('#product_quantity').val()) ;
            $('#sum_price').val(sum_price.toLocaleString());
            
            var s = ' <option value="' + response.unit + '" data-type="other" selected>' + response.unit + '</option>';
            if (response.unit_swap != null) {          
                s += ' <option value="' + response.unit_swap + '" data-type="other">' + response.unit_swap + '</option>';
                $('#product_price_swap').val(response.name_group);
            }
            $("#unit_product_swap").html(s);
        }
    })
}
$("#unit_product_swap").change(function () {
    var product_price = $('#product_price').val();
    var product_price_swap = $('#product_price_swap').val();
    $('#product_price').val(product_price_swap);
    $('#product_price_swap').val(product_price);
    var sum_price = Number($('#product_price').val().replace(/\,/g, '').replace(/\./g, '')) * Number($('#product_quantity').val());
    $('#sum_price').val(sum_price.toLocaleString());

});
//------------------------Payment / Debit-------------------------------------
var URLCreateSale = "";
$('#URLCreateSale')
    .keypress(function () {
        URLCreateSale = $(this).val();
    })
    .keypress();
function Payment_order() {
    Swal.fire({
        title: 'Thanh toán?',
        text: 'Bạn có chắc chắn muốn thanh toán',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'xác nhận',
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            var sale = {};
            sale.customer_id = $('#customer_id').val();
            sale.total = Number($('#total').val().replace(/\,/g, '').replace(/\./g, ''));
            sale.note = $('#cart_note').val();
            sale.method = Number($('#cart_Prepay').val().replace(/\,/g, '').replace(/\./g, ''));
            $.ajax({
                url: URLCreateSale,
                async: false,
                type: "Post",
                data: JSON.stringify(sale),
                contentType: "application/json; charset=UTF-8",
                dataType: "json",
                success: function (response) {
                    if (response.status) {
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
                            $("#payment_btn").hide();
                            $("#order_btn").hide();
                        
                        Swal.fire({
                            title: 'Thành công!',
                            text: 'Bạn có muốn in hóa đơn ?',
                            icon: 'success',
                            showCancelButton: true,
                            confirmButtonText: 'In hóa đơn',
                        }).then((result) => {
                            /* Read more about isConfirmed, isDenied below */
                            if (result.isConfirmed) {
                                PrintOrder(response.sale_id);                            }
                        })
                    } else {
                        Swal.fire('Lỗi !', response.message, 'error');                 
                    }

                }
            });
        }
    })

    
}

function FillProduct_cart(id, masp, tensp, soluong, unit) {
    $('#product_name').val(tensp);
    $('#product_quantity').val(soluong);
    $('#product_id').val(masp);
    $('#cart_id').val(id);

    document.querySelector("#customer_name").disabled = true;
    document.querySelector("#customer_phone").disabled = true;
    document.querySelector("#product_name").disabled = true;

    $("#submit_addCart").hide();
    $("#refresh_cart").hide();
    $("#submit_updateCart").show();
    $("#Cancel_Cart").show();
    LoadDataProduct_editCart(masp, unit);
   
    window.scrollTo({
        top: 0,
        behavior: `smooth`
    })
}
function LoadDataProduct_editCart(id,unit) {
    $.ajax({
        type: 'POST',
        url: URLFindProduct_name,
        data: { "product_id": id },
        success: function (response) {        
            if (unit == response.unit) {
                $('#product_code').val(response.code);
                $('#product_unit').val(response.note);
                $('#product_price').val(response.name_category);
                var sum_price = Number($('#product_price').val().replace(/\,/g, '').replace(/\./g, '')) * Number($('#product_quantity').val());
                $('#sum_price').val(sum_price.toLocaleString());
                var s = ' <option value="' + response.unit + '" data-type="other" selected>' + response.unit + '</option>';
                if (response.unit_swap != null) {
                    s += ' <option value="' + response.unit_swap + '" data-type="other">' + response.unit_swap + '</option>';
                    $('#product_price_swap').val(response.name_group);
                }
            } else {
                $('#product_code').val(response.code);
                $('#product_unit').val(response.note);
                $('#product_price').val(response.name_group);
                var sum_price = Number($('#product_price').val().replace(/\,/g, '').replace(/\./g, '')) * Number($('#product_quantity').val());
                $('#sum_price').val(sum_price.toLocaleString());
                var s = ' <option value="' + response.unit + '" data-type="other" >' + response.unit + '</option>';
                if (response.unit_swap != null) {
                    s += ' <option value="' + response.unit_swap + '" data-type="other" selected>' + response.unit_swap + '</option>';
                    $('#product_price_swap').val(response.name_category);
                }
            }
            $("#unit_product_swap").html(s);
        }
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
    cart_create.note = $('#cart_note').val();
    cart_create.unit = $('#unit_product_swap').val();   

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

                Swal.fire('Thành công !', 'Cập nhật đơn hàng thành công !', 'success');

            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: "Số lượng sản phẩm chỉ còn: " + response.message + " !",
                })
              
            }

        }
    });

}

//----------------------FILTER INVENTORY PRODUCT------------------------------------------------
/*$('#URL_RevenueList_Date')
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
        Swal.fire('Lỗi', 'Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !', 'error')     
    }
    
});
$("#revenue_DateEnd").change(function () {
    var date_start = $("#revenue_DateStart").val();
    var date_end = $("#revenue_DateEnd").val();
    if (date_start <= date_end) {
        GetList_RevenueListDate(date_start, date_end);
    } else {
        Swal.fire('Lỗi', 'Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !', 'error')     
    }
});
$("#revenue_DateStart_Month").change(function () {
    var date_start = $("#revenue_DateStart_Month").val();
    var date_end = $("#revenue_DateEnd_Month").val();
    if (date_start <= date_end) {
        GetList_RevenueListMonth(date_start, date_end);
    } else {
        Swal.fire('Lỗi', 'Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !', 'error')     
    }

});
$("#revenue_DateEnd_Month").change(function () {
    var date_start = $("#revenue_DateStart_Month").val();
    var date_end = $("#revenue_DateEnd_Month").val();
    if (date_start <= date_end) {
        GetList_RevenueListMonth(date_start, date_end);
    } else {
        Swal.fire('Lỗi', 'Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !', 'error')     
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
        $('#example').DataTable()
    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
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
        Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
    });
}*/

$("#sale_DateStart").change(function () {
    var date_start = $("#sale_DateStart").val();
    var date_end = $("#sale_DateEnd").val();
    if (date_start <= date_end) {
        GetList_OrderList(date_start, date_end);
    } else {
        Swal.fire('Lỗi', 'Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !', 'error')     
    }

});
$("#sale_DateEnd").change(function () {
    var date_start = $("#sale_DateStart").val();
    var date_end = $("#sale_DateEnd").val();
    if (date_start <= date_end) {
        GetList_OrderList(date_start, date_end);
    } else {
        Swal.fire('Lỗi', 'Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !', 'error')     
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
        Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
    });
}
//----------------------Xem trước hóa đơn------------------------------------------------
$('#URLgetOrderProduct')
    .keypress(function () {
        URLgetOrderProduct = $(this).val();
    })
    .keypress();
$('#URLTemplateInvoicePreview')
    .keypress(function () {
        URLTemplateInvoicePreview = $(this).val();
    })
    .keypress();
function openWin() {
    var id = $('#customer_id').val();
    $.ajax({
        url: URLTemplateInvoicePreview,
        data: {
            id: id
        },
        async: false,
    }).done(function (result) {
        var WinPrint = window.open('', '', 'width=1200,height=800');
        WinPrint.document.write('<html><head>');
        WinPrint.document.write('</head><body onload="print();">');
        WinPrint.document.write(result);
        WinPrint.document.write('</body></html>');
        WinPrint.document.close();
        WinPrint.focus();
    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
    });

}

//---------------------------flatpickr---------------
flatpickr(".flatpickr", {});

//---------------------------User form---------------
$('#URL_UserList')
    .keypress(function () {
        URL_UserList = $(this).val();
    })
    .keypress();
$('.UserForm').submit(function (e) {
    var form = $(this);

    // Check if form is valid then submit ajax
    if (form[0].checkValidity()) {
        e.preventDefault();
        var url = form.attr('action');
        $.ajax({
            url: url,
            type: 'POST',
            data: form.serialize(),
            success: function (data) {
                // Hide bootstrap modal to prevent conflict
                $('.modal').modal('hide');

                if (data.status) {
                    // Refresh table data
                    GetList_UserList()
                    Swal.fire('Thành công !', data.message, 'success')     
                    form[0].reset();
                    form.removeClass('was-validated');
                } else {
                    Swal.fire('Lỗi !', data.message, 'error');
                }
            }
        });
    }
});
function GetList_UserList() {
    $.ajax({
        url: URL_UserList,
        data: {         
        }
    }).done(function (result) {
        $('#dataContainer').html(result);
        $('#example').DataTable();
    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
    });
}
//----------------------Add  DebtForm------------------------------------------------
$('#URLCustomerDebtsList')
    .keypress(function () {
        URLCustomerDebtsList = $(this).val();
    })
    .keypress();
$('#URLSupplierDebtsList')
    .keypress(function () {
        URLSupplierDebtsList = $(this).val();
    })
    .keypress();
$('.DebtsForm').submit(function (e) {
    var form = $(this);
    // Check if form is valid then submit ajax
    if (form[0].checkValidity()) {
        e.preventDefault();
        var url = form.attr('action');
        $.ajax({
            url: url,
            type: 'POST',
            data: form.serialize(),
            success: function (data) {
                // Hide bootstrap modal to prevent conflict
                $('.modal').modal('hide');

                if (data.status) {
                    // Refresh table data
                    var date_start = $("#debt_DateStart").val();
                    var date_end = $("#debt_DateEnd").val();
                    GetList_Debt(date_start, date_end);
                    GetList_Debt2(date_start, date_end);
                    Swal.fire('Thành công !', data.message, 'success');
                    form[0].reset();
                    form.removeClass('was-validated');
                } else {
                    Swal.fire('Lỗi !', data.message, 'error');
                }
            }
        });
    }
});
function GetList_Debt(date_start, date_end) {
    $.ajax({
        url: URLCustomerDebtsList,
        data: {
            date_Start: date_start,
            date_End: date_end,
        }
    }).done(function (result) {
        $('#dataContainer').html(result);
        $('#example').DataTable();        
    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
    });
}
function GetList_Debt2(date_start, date_end) {
    $.ajax({
        url: URLSupplierDebtsList,
        data: {
            date_Start: date_start,
            date_End: date_end,
        }
    }).done(function (result) {       
        $('#dataContainer1').html(result);
        $('#example1').DataTable();
    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        Swal.fire('Lỗi !', 'Đã xảy ra lỗi, hãy thử lại sau !', 'error');
    });
}
$('#URLFindDebt')
    .keypress(function () {
        URLFindDebt = $(this).val();
    })
    .keypress();
function DebtCollection(ele, id, method) {
    row = $(ele).closest('tr');
    $.ajax({
        type: 'POST',
        url: URLFindDebt,
        data: {
            id: id,
            method: method
        },
        success: function (response) {
            $('#customer_id').val(response.id);
            $('#order_total').val(response.total.toLocaleString());
            $('#debts_total').val(response.prepayment.toLocaleString());
            $('#Debt_conlai').val((response.total - response.prepayment).toLocaleString());
            $('#MKH').val(response.code);
            $('#Name_KH').val(response.note);
            $('#phone_KH').val(response.created_by);
            $('#method').val(method);
          
            $('#DebtCollectionModal .close').css('display', 'none');
            $('#DebtCollectionModal').modal('show');
        }
    })
}
//----------------------------Filter Debt----------------------------------
$("#debt_DateStart").change(function () {
    var date_start = $("#debt_DateStart").val();
    var date_end = $("#debt_DateEnd").val();
    if (date_start <= date_end) {
        GetList_Debt(date_start, date_end);
    } else {
        Swal.fire('Lỗi', 'Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !', 'error')     
    }

});
$("#debt_DateEnd").change(function () {
    var date_start = $("#debt_DateStart").val();
    var date_end = $("#debt_DateEnd").val();
    if (date_start <= date_end) {
        GetList_Debt(date_start, date_end);
    } else {
        Swal.fire('Lỗi', 'Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc !', 'error')     
    }
});
//--------------------------------
$('.ReturnForm').submit(function (e) {
    var form = $(this);
    

    // Check if form is valid then submit ajax
    if (form[0].checkValidity()) {
        e.preventDefault();
        var url = form.attr('action');
        console.log(form.serialize());
        $.ajax({
            url: url,
            type: 'POST',
            data: form.serialize(),
            success: function (data) {
                // Hide bootstrap modal to prevent conflict
                $('.modal').modal('hide');

                if (data.status) {
                    // Refresh table data
                    Swal.fire('Thành công !', data.message, 'success');
                    form[0].reset();
                    form.removeClass('was-validated');
                } else {
                    Swal.fire('Lỗi !', data.message, 'error');
                }
            }
        });
    }
});
//---------------------------------Trả sản phẩm cho nhà cung cấp------------------
function Get_exchangeSupplier(id, product_name, quantity_stock, supplier, price_product_stock) {
    $("#id_inventory").val(id);
    $("#productName_inventory").val(product_name);
    $("#quantity_stock").val(quantity_stock);
    $("#name_supplier").val(supplier);
    $("#price_product_stock").val(price_product_stock.toLocaleString()); 
    var quantity = $("#quantity").val();
    var price_product = price_product_stock.replace(/\,/g, '').replace(/\./g, '');
    var total = quantity * price_product;
    $("#cost_return").val(total.toLocaleString());

    $('#productSelectModal.close').css('display', 'none');
    $('#productSelectModal').modal('hide');

    $('#exchangeSupplierModal .close').css('display', 'none');
    $('#exchangeSupplierModal').modal('show');
       
}
//-------------------Định dạng số điện thoại (phone)------------------
function phoneMask() {
    var num = $(this).val().replace(/\D/g, '');
    if (num != "") {
        $(this).val(num.substring(0, 3) + ' ' + num.substring(3, 10));
    }  
}
$('[type="tel"]').keyup(phoneMask);
