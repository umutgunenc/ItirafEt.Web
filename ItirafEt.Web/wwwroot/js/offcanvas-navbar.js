// Navbar toggle event listener'ları
function setupNavbarScroll() {
    const navbar = document.getElementById('navbarMain');

    navbar.addEventListener('show.bs.collapse', function () {
        toggleBodyScroll(true);
    });

    navbar.addEventListener('hide.bs.collapse', function () {
        toggleBodyScroll(false);
    });
}

// Scroll kontrol fonksiyonu
function toggleBodyScroll(isMenuOpen) {
    document.body.classList.toggle('navbar-open', isMenuOpen);
}

// Initialize
window.addEventListener('DOMContentLoaded', () => {
    setupNavbarScroll();
});







