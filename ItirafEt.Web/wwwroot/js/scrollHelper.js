window.scrollHelper = {
    getScrollY: () => window.pageYOffset,
    setScrollY: y => {
        setTimeout(() => {
            window.scrollTo(0, y);
        }, 50); // Short delay to allow rendering
    }
};