// ==========================================
// 0. HÀM HIỂN THỊ THÔNG BÁO XỊN (TOAST)
// ==========================================
function showToast(message, type = 'info') {
    let container = document.getElementById('toast-container');
    // Nếu chưa có thùng chứa Toast thì tạo mới
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        document.body.appendChild(container);
    }

    const toast = document.createElement('div');
    toast.className = `toast-msg toast-${type}`;
    
    // Đổi icon theo loại thông báo
    let icon = 'fa-circle-info';
    if(type === 'success') icon = 'fa-circle-check';
    if(type === 'error') icon = 'fa-circle-xmark';

    toast.innerHTML = `<i class="fa-solid ${icon}"></i> <span>${message}</span>`;
    container.appendChild(toast);

    // Tự động xóa thông báo khỏi HTML sau 3s để khỏi nặng máy
    setTimeout(() => { toast.remove(); }, 3000);
}

// ==========================================
// 1. HÀM QUẢN LÝ TRẠNG THÁI ĐĂNG NHẬP (AUTH)
// ==========================================
function checkLoginState() {
    const token = localStorage.getItem('jwtToken');
    const authButton = document.getElementById('authButton');
    
    if (token) {
        if (authButton) {
            authButton.textContent = 'ĐĂNG XUẤT';
            authButton.href = '#'; 
            authButton.onclick = function(e) {
                e.preventDefault(); 
                localStorage.removeItem('jwtToken'); 
                // Thay alert bằng Toast Xanh lá
                showToast('Bạn đã đăng xuất thành công!', 'success');
                setTimeout(() => window.location.reload(), 1000); 
            };
        }
    } else {
        if (authButton) {
            authButton.textContent = 'ĐĂNG NHẬP';
        }
    }
}

// ==========================================
// 2 & 3. MENU DANH MỤC VÀ BỘ SƯU TẬP
// ==========================================
async function loadCategoriesToMenu() {
    const catalogMenu = document.getElementById('catalogMenu');
    if (!catalogMenu) return; 
    try {
        const response = await fetch('https://localhost:7282/api/Categories');
        if (response.ok) {
            const result = await response.json();
            if (result.success && result.data.length > 0) {
                catalogMenu.innerHTML = ''; 
                result.data.forEach(category => {
                    const aTag = document.createElement('a');
                    const isRoot = window.location.pathname.endsWith('TrangChu.html') || window.location.pathname.endsWith('/');
                    const basePath = isRoot ? './SanPham' : '../SanPham';
                    aTag.href = `${basePath}/SanPham.html?categoryId=${category.id}`;
                    aTag.textContent = category.name.toUpperCase();
                    catalogMenu.appendChild(aTag);
                });
            } else { catalogMenu.innerHTML = '<a href="#">Chưa có danh mục</a>'; }
        }
    } catch (error) { catalogMenu.innerHTML = '<a href="#">Lỗi tải dữ liệu</a>'; }
}

function loadCollectionsToMenu() {
    const collectionMenu = document.getElementById('collectionMenu');
    if (!collectionMenu) return;
    const collections = [ { id: 1, name: "SUMMER 2024", link: "../Summer2024/Summer2024.html" }, { id: 2, name: "WINTER 2023", link: "./Winter2023/Winter2023.html" } ];
    collectionMenu.innerHTML = ''; 
     collections.forEach(item => {
        const aTag = document.createElement('a');
        aTag.href = item.link;   // 👈 LẤY LINK Ở ĐÂY
        aTag.textContent = item.name;
        collectionMenu.appendChild(aTag);
    });
}

// ==========================================
// 4. KIỂM TRA ĐĂNG NHẬP KHI BẤM NÚT GIỎ HÀNG (+)
// ==========================================
function requireLoginForCart(detailUrl) {
    const token = localStorage.getItem('jwtToken');
    
    if (!token) {
        // Thay alert bằng Toast Đỏ báo lỗi
        showToast('Vui lòng ĐĂNG NHẬP để có thể thêm sản phẩm!', 'error');
    } else {
        // Thay alert bằng Toast Đen (Info) và delay 0.8s để đọc chữ rồi mới nhảy trang
        showToast('Đang chuyển đến trang chọn Size...', 'info');
        if (detailUrl) {
            setTimeout(() => { window.location.href = detailUrl; }, 800);
        }
    }
}

// ==========================================
// 5. ĐẾM SỐ LƯỢNG GIỎ HÀNG ĐỘNG
// ==========================================
async function updateCartCount() {
    const token = localStorage.getItem('jwtToken');
    const cartNavLinks = document.querySelectorAll('a[href*="GioHang"]');
    const floatingCartCount = document.querySelector('.cart-count');

    if (!token) {
        cartNavLinks.forEach(a => a.textContent = 'GIỎ HÀNG (0)');
        if (floatingCartCount) floatingCartCount.textContent = '0';
        return;
    }
    try {
        const response = await fetch('https://localhost:7282/api/Cart', {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        if (response.ok) {
            const result = await response.json();
            let totalItems = 0;
            if (result.data && result.data.items) {
                totalItems = result.data.items.reduce((sum, item) => sum + item.quantity, 0);
            }
            cartNavLinks.forEach(a => a.textContent = `GIỎ HÀNG (${totalItems})`);
            if (floatingCartCount) floatingCartCount.textContent = totalItems.toString();
        }
    } catch (error) {}
}

// ==========================================
// 6. KHỞI CHẠY
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    checkLoginState(); loadCategoriesToMenu(); loadCollectionsToMenu(); updateCartCount();
});

// ==========================================
// 0.5 HÀM HIỂN THỊ HỘP THOẠI XÁC NHẬN (QUESTION BOX / MODAL)
// ==========================================
function showModal(message, type = 'alert') {
    return new Promise((resolve) => {
        // Tạo lớp phủ mờ
        const overlay = document.createElement('div');
        overlay.className = 'custom-modal-overlay';

        // Tạo khung hộp thoại
        const box = document.createElement('div');
        box.className = 'custom-modal-box';

        // Render nút bấm tùy theo loại
        let buttonsHtml = '';
        if (type === 'confirm') {
            buttonsHtml = `
                <button class="modal-btn modal-cancel" id="modalCancelBtn">Hủy bỏ</button>
                <button class="modal-btn modal-ok" id="modalOkBtn">Đồng ý</button>
            `;
        } else {
            // Dạng 'alert' chỉ có 1 nút OK
            buttonsHtml = `
                <button class="modal-btn modal-ok" id="modalOkBtn" style="width: 100%;">OK</button>
            `;
        }

        box.innerHTML = `
            <div class="modal-message">${message}</div>
            <div class="modal-actions">
                ${buttonsHtml}
            </div>
        `;

        overlay.appendChild(box);
        document.body.appendChild(overlay);

        // Bắt sự kiện bấm nút
        const btnOk = document.getElementById('modalOkBtn');
        const btnCancel = document.getElementById('modalCancelBtn');

        if (btnOk) {
            btnOk.onclick = () => {
                document.body.removeChild(overlay);
                resolve(true); // Trả về true nếu chọn Đồng ý
            };
        }
        if (btnCancel) {
            btnCancel.onclick = () => {
                document.body.removeChild(overlay);
                resolve(false); // Trả về false nếu chọn Hủy
            };
        }
    });
}

// ==========================================
// 6. [TÍNH NĂNG MỚI] - XỬ LÝ THANH TÌM KIẾM (SEARCH)
// ==========================================
function setupSearch() {
    const searchInput = document.querySelector('.search-input');
    const searchBtn = document.querySelector('.search-btn');
    if (!searchInput || !searchBtn) return;

    const executeSearch = () => {
        const keyword = searchInput.value.trim();
        if (keyword) {
            // Xác định đường dẫn tương đối (đang ở Trang Chủ hay trang phụ)
            const isRoot = window.location.pathname.endsWith('TrangChu.html') || window.location.pathname.endsWith('/');
            const basePath = isRoot ? './SanPham' : '../SanPham';
            
            // Đá sang trang SanPham.html kèm theo từ khóa trên URL
            window.location.href = `${basePath}/SanPham.html?search=${encodeURIComponent(keyword)}`;
        }
    };

    searchBtn.addEventListener('click', executeSearch);
    searchInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') executeSearch();
    });
}

// ==========================================
// 7. KHỞI CHẠY ĐỒNG LOẠT
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    checkLoginState();       
    loadCategoriesToMenu();  
    loadCollectionsToMenu(); 
    updateCartCount();       
    setupSearch();           // Bật thanh tìm kiếm
});