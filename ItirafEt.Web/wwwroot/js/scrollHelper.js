//window.scrollHelper = {
//    getScrollY: () => window.pageYOffset,
//    setScrollY: y => {
//        setTimeout(() => {
//            document.documentElement.style.scrollBehavior = 'auto';

//            window.scrollTo({
//                top: y,
//                left: 0
//            });

//        }, 50);
//    }
//};

window.scrollHelper = {
    getScrollY: () => window.pageYOffset,
    setScrollY: y => {
        document.documentElement.style.scrollBehavior = 'auto';
        requestAnimationFrame(() => {
            window.scrollTo({
                top: y,
                left: 0
            });
        });
    }
};




