// ==========================================
// 1. HÀM QUẢN LÝ TRẠNG THÁI ĐĂNG NHẬP (AUTH)
// ==========================================
function checkLoginState() {
    const token = localStorage.getItem('jwtToken');
    const authButton = document.getElementById('authButton');
    
    if (token) {
        // Nếu có thẻ Token -> Đã đăng nhập: Đổi chữ thành ĐĂNG XUẤT
        if (authButton) {
            authButton.textContent = 'ĐĂNG XUẤT';
            authButton.href = '#'; // Hủy link sang trang đăng nhập
            authButton.onclick = function(e) {
                e.preventDefault(); // Ngăn chặn load lại trang ngay lập tức
                localStorage.removeItem('jwtToken'); // Xé vé (Xóa token)
                alert('Bạn đã đăng xuất thành công!');
                window.location.reload(); // F5 lại trang để cập nhật giao diện
            };
        }
    } else {
        // Chưa đăng nhập: Giữ nguyên là ĐĂNG NHẬP
        if (authButton) {
            authButton.textContent = 'ĐĂNG NHẬP';
        }
    }
}

// ==========================================
// 2. HÀM ĐỔ DỮ LIỆU DANH MỤC LÊN THANH MENU (TỪ SQL SERVER)
// ==========================================
async function loadCategoriesToMenu() {
    const catalogMenu = document.getElementById('catalogMenu');
    if (!catalogMenu) return; // Nếu đang ở trang không có menu thì bỏ qua, chống báo lỗi

    try {
        const response = await fetch('https://localhost:7282/api/Categories');
        if (response.ok) {
            const result = await response.json();
            
            if (result.success && result.data.length > 0) {
                catalogMenu.innerHTML = ''; // Quét sạch chữ "Đang tải danh mục..."
                
                result.data.forEach(category => {
                    const aTag = document.createElement('a');
                    
                    // Tuyệt chiêu xử lý đường dẫn lùi thư mục:
                    // Xem thử trình duyệt đang ở Trang Chủ hay đang ở trong thư mục con (VD: thư mục GioHang)
                    const isRoot = window.location.pathname.endsWith('TrangChu.html') || window.location.pathname.endsWith('/');
                    const basePath = isRoot ? './SanPham' : '../SanPham';
                    
                    // Gắn link trỏ thẳng về Siêu File SanPham.html kèm theo ID
                    aTag.href = `${basePath}/SanPham.html?categoryId=${category.id}`;
                    aTag.textContent = category.name.toUpperCase();
                    
                    catalogMenu.appendChild(aTag);
                });
            } else {
                catalogMenu.innerHTML = '<a href="#">Chưa có danh mục</a>';
            }
        }
    } catch (error) {
        console.error("Lỗi tải Menu Catalog:", error);
        catalogMenu.innerHTML = '<a href="#">Lỗi tải dữ liệu</a>';
    }
}

// ==========================================
// 3. HÀM ĐỔ DỮ LIỆU BỘ SƯU TẬP (COLLECTION) - MOCK DATA TẠM THỜI
// ==========================================
function loadCollectionsToMenu() {
    const collectionMenu = document.getElementById('collectionMenu');
    if (!collectionMenu) return;

    // Tạm thời dùng mảng vì SQL chưa có bảng Collections
    const collections = [
        { id: 1, name: "SUMMER 2024" },
        { id: 2, name: "WINTER 2023" }
    ];

    collectionMenu.innerHTML = ''; // Xóa chữ "Đang tải..."
    
    collections.forEach(item => {
        const aTag = document.createElement('a');
        
        // Vì đã xóa các folder rác nên tạm thời để href="#" cho đẹp giao diện
        // Sau này làm trang chi tiết bộ sưu tập thì thay link vào đây
        aTag.href = "#"; 
        aTag.textContent = item.name;
        
        collectionMenu.appendChild(aTag);
    });
}

// ==========================================
// 4. KÍCH HOẠT ĐỒNG LOẠT KHI TẢI TRANG XONG
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    checkLoginState();       // Chạy check Auth
    loadCategoriesToMenu();  // Chạy load Catalog
    loadCollectionsToMenu(); // Chạy load Collection
});
// ==========================================
// 5. KIỂM TRA ĐĂNG NHẬP KHI BẤM NÚT GIỎ HÀNG (+)
// ==========================================
function requireLoginForCart(detailUrl) {
    const token = localStorage.getItem('jwtToken');
    
    if (!token) {
        // Chưa đăng nhập thì chặn lại
        alert('Vui lòng ĐĂNG NHẬP để có thể thêm sản phẩm vào giỏ hàng nhé!');
    } else {
        // Đã đăng nhập thì nhắc chọn Size và tự động chuyển sang trang Chi Tiết
        alert('Hãy vào trang chi tiết để chọn Size và Màu sắc trước khi chốt đơn nha!');
        if (detailUrl) {
            window.location.href = detailUrl; 
        }
    }
}