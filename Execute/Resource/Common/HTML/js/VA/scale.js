function applySTMScale() {
    const stmWidth = 1280;
    const stmHeight = 1080;
    
    // Tính toán tỉ lệ scale phù hợp nhất để vừa màn hình
    const scaleX = window.innerWidth / stmWidth;
    const scaleY = window.innerHeight / stmHeight;
    const scale = Math.min(scaleX, scaleY);
    
    const layout = document.querySelector('.stm-layout');
    if (layout) {
        layout.style.transform = `scale(${scale})`;
    }
}

// Chạy hàm scale khi trang tải xong và khi thay đổi kích thước cửa sổ
window.addEventListener('resize', applySTMScale);
document.addEventListener('DOMContentLoaded', applySTMScale);
