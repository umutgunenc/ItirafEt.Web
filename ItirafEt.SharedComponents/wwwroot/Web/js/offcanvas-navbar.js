

//window.collapseNavbar = () => {
//    const navbar = document.getElementById('navbarMain');
//    const bsCollapse = bootstrap.Collapse.getInstance(navbar);
//    if (bsCollapse) {
//        bsCollapse.hide();
//    }
//};


window.closeOffcanvas = (id) => {
    const el = document.getElementById(id);
    if (el) {
        const offcanvas = bootstrap.Offcanvas.getInstance(el);
        if (offcanvas) {
            offcanvas.hide();
        }
    }
};




