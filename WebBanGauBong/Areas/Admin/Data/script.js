///////////////////////////////
// 1. PHẦN ADD PRODUCT (GIỮ NGUYÊN HOẶC SỬA CHO ĐỒNG BỘ)
// Nếu Modal Add có sẵn trên trang thì giữ nguyên, nếu load AJAX thì sửa giống bên dưới
$(document).ready(function () {
    // ... Code phần Add Product cũ của bạn ...
    // (Giữ nguyên đoạn code xử lý cho .upload-area-add nếu nó hoạt động ổn)
    $(".upload-area-add").click(function () {
        $('#upload-input-add').trigger('click');
    });

    $('#upload-input-add').change(event => {
        // ... (Giữ nguyên logic cũ của bạn) ...
        if (event.target.files) {
            let filesAmount = event.target.files.length;
            $('.upload-img-add').html("");
            for (let i = 0; i < filesAmount; i++) {
                let reader = new FileReader();
                reader.onload = function (event) {
                    let html = `
                        <div class = "uploaded-img">
                            <img src = "${event.target.result}">
                            <button type = "button" class = "remove-btn">
                                <i class = "fas fa-times"></i>
                            </button>
                        </div>
                    `;
                    $(".upload-img-add").append(html); // Sửa class selector cho đúng
                }
                reader.readAsDataURL(event.target.files[i]);
            }
            $('.upload-info-value-add').text(filesAmount);
            $('.upload-img-add').css('padding', "20px");
        }
    });

    // Xóa ảnh (Logic chung)
    $(document).on('click', '.remove-btn', function () {
        $(this).parent().remove();
        // Cần thêm logic cập nhật lại số lượng file text nếu muốn hoàn hảo
    });
});


///////////////////////////////
// 2. PHẦN UPDATE PRODUCT (SỬA LẠI BẰNG EVENT DELEGATION)

// Không cần bọc trong $(document).ready cũng được, nhưng để an toàn cứ dùng
$(document).ready(function () {

    // SỬA 1: Dùng $(document).on('click', 'selector', function)
    // Để bắt sự kiện cho phần tử được load bằng AJAX
    $(document).on('click', '.upload-area-edit', function () {
        $('#upload-input-edit').trigger('click');
    });

    // SỬA 2: Bắt sự kiện change của input file AJAX
    $(document).on('change', '#upload-input-edit', function (event) {
        if (event.target.files) {
            let filesAmount = event.target.files.length;

            // Nếu muốn xóa ảnh cũ khi chọn ảnh mới thì uncomment dòng dưới
            // $(".upload-img-edit").html(""); 

            for (let i = 0; i < filesAmount; i++) {
                let reader = new FileReader();
                reader.onload = function (event) {
                    let html = `
                        <div class="uploaded-img">
                            <img src="${event.target.result}">
                            <button type="button" class="remove-btn">
                                <i class="fas fa-times"></i>
                            </button>
                        </div>
                    `;
                    // Append vào class container ảnh sửa
                    $(".upload-img-edit").append(html);
                }
                reader.readAsDataURL(event.target.files[i]);
            }

            // Cập nhật số lượng file
            $('.upload-info-value-edit').text(filesAmount);
            $('.upload-img-edit').css('padding', "20px");
        }
    });

    // SỬA 3: Nút xóa ảnh trong Modal Sửa (cũng cần Delegation)
    $(document).on('click', '.upload-img-edit .remove-btn', function () {
        $(this).parent().remove();
    });
});