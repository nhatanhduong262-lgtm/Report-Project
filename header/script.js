/* 
  FILE: script.js
  Mô tả: Chứa các kịch bản JavaScript để xử lý các tương tác trên giao diện người dùng (UI).
*/

// Lấy các phần tử cần thiết từ DOM
const header = document.getElementById('main-header');
const menuOpenBtn = document.getElementById('menu-open');
const menuCloseBtn = document.getElementById('menu-close');
const mobileMenuContainer = document.getElementById('mobile-menu');
const menuOverlay = document.getElementById('menu-overlay');
const menuContent = document.getElementById('menu-content');

/**
 * Xử lý hiệu ứng cuộn trang (Scroll Effect)
 * Khi người dùng cuộn xuống, Header sẽ thu nhỏ lại.
 */
window.addEventListener('scroll', () => {
  if (window.scrollY > 50) {
    header?.classList.add('py-1.5', 'shadow-2xl', 'bg-black/98');
    header?.classList.remove('py-2');
  } else {
    header?.classList.remove('py-1.5', 'shadow-2xl', 'bg-black/98');
    header?.classList.add('py-2');
  }
});

/**
 * Hàm mở Menu trên di động
 */
const openMobileMenu = () => {
  mobileMenuContainer?.classList.remove('hidden');
  setTimeout(() => {
    menuContent?.classList.remove('-translate-x-full');
  }, 10);
};

/**
 * Hàm đóng Menu trên di động
 */
const closeMobileMenu = () => {
  menuContent?.classList.add('-translate-x-full');
  setTimeout(() => {
    mobileMenuContainer?.classList.add('hidden');
  }, 300);
};

// Gán sự kiện cho các nút bấm
menuOpenBtn?.addEventListener('click', openMobileMenu);
menuCloseBtn?.addEventListener('click', closeMobileMenu);
menuOverlay?.addEventListener('click', closeMobileMenu);
