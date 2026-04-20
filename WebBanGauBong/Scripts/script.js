//Button back to top
let mybutton = document.getElementById("btn-back-to-top");


window.onscroll = function () {
    scrollFunction();
};

function scrollFunction() {
    if (
        document.body.scrollTop > 20 ||
        document.documentElement.scrollTop > 20
    ) {
        mybutton.style.display = "block";
    } else {
        mybutton.style.display = "none";
    }
}

mybutton.addEventListener("click", backToTop);

function backToTop() {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
}

/////////////////////////////////////////
// Nút size đổi giá
$(document).ready(function () {

    // Lắng nghe sự kiện click trên bất kỳ nút nào có class .btnSizeCard
    $('.btnSizeCard').on('click', function () {

        var $clickedButton = $(this);

        var $cardBody = $clickedButton.closest('.card-body');

        $cardBody.find('.btnSizeCard').removeClass('active');

        $clickedButton.addClass('active');

        var selectedSizeId = $clickedButton.data('id');
        $cardBody.find('input[name="productSizeID"]').val(selectedSizeId);

        //Xử lý Cập nhật Giá 
        var newPrice = $clickedButton.data('price');
        var rawTargetId = $clickedButton.data('target');
        var discount = $clickedButton.data('discount');
        
        var targetPriceAfterDiscount = '#after-' + rawTargetId;
       
        if (discount != 0) {
            var targetPrice = '#' + rawTargetId;
            var newPriceAfterDiscount = newPrice * (100.0 - discount) / 100.0;
            $(targetPrice).text(newPrice.toLocaleString('vi-VN') + 'đ');
            $(targetPriceAfterDiscount).text(newPriceAfterDiscount.toLocaleString('vi-VN') + 'đ');
        }
        else {
            $(targetPriceAfterDiscount).text(newPrice.toLocaleString('vi-VN') + 'đ');
        }
    });
});

//////////////////////////////////////////
// Lắng nghe thay đổi khi click checkbox của menu sidebar
$(document).ready(function () {
    $('.checkbox-round').on('change', function () {
        $('#filterForm').submit();
    });
});

$(document).ready(function () {
    $(".button-size.active").on("click", function () {
        $('#filterForm').submit();
    });
});

//////////////////////////////////////////
// Nút đổi giá  trang Detail
$(document).ready(function () {

    // Lắng nghe sự kiện click trên bất kỳ nút nào có class .btnSizeDetail
    $('.btnSizeDetail').on('click', function () {

        var $clickedButton = $(this);

        var $cardBody = $clickedButton.closest('.cardBodyDetail');

        $cardBody.find('.btnSizeDetail').removeClass('active');

        // Thêm class 'active' chỉ cho nút vừa được click
        $clickedButton.addClass('active');

        //Xử lý Cập nhật Giá 
        var productSizeID = $clickedButton.data('id');
        var newPrice = $clickedButton.data('price');
        var rawTargetId = $clickedButton.data('target');
        var discount = $clickedButton.data('discount');
        var size = $clickedButton.data('size');

        var targetPrice = '#' + rawTargetId;
        var targetPriceAfterDiscount = '#after-' + rawTargetId;
        var tagSizeDetail = '#tagSizeDetail';

        newPriceAfterDiscount = newPrice * (100 - discount) / 100;
        $(tagSizeDetail).text(size);
        $(targetPrice).text(newPrice.toLocaleString('vi-VN') + 'đ');
        $(targetPriceAfterDiscount).text(newPriceAfterDiscount.toLocaleString('vi-VN') + 'đ');
        $('#productSizeID').val(productSizeID);
    });
});
/////////////////////////////////
//Nút tăng giảm
$(document).ready(function () {

    // Xử lý nút GIẢM (-)
    $('.btn-decrease').on('click', function () {

        var $container = $(this).closest('.quantity-container');

        var $input = $container.find('.quantity-input');
        var $display = $container.find('.quantity-display');

        var currentVal = parseInt($input.val());

        if (currentVal > 1) {
            var newVal = currentVal - 1;
            $input.val(newVal);
            $display.text(newVal);
        }
    });

    // Xử lý nút TĂNG (+)
    $('.btn-increase').on('click', function () {
        var $container = $(this).closest('.quantity-container');
        var $input = $container.find('.quantity-input');
        var $display = $container.find('.quantity-display');

        var currentVal = parseInt($input.val());

        var newVal = currentVal + 1;

        $input.val(newVal);
        $display.text(newVal);
    });

});

////////////////////////////////////////////////
// Scroll aware navbar
let lastScrollTop = 0;
navbar = document.getElementById("nav-full");
window.addEventListener("scroll", function () {
    let scrollTop = window.pageYOffset || document.documentElement.scrollTop;
    if (scrollTop > lastScrollTop) {
        navbar.classList.add('active');
    } else {
        navbar.classList.remove('active');
    }
    lastScrollTop = scrollTop;
})
