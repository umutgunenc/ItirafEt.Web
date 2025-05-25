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


window.chatScrollHelper = {

    scrollToBottom: function (element) {
        if (element) {
            element.scrollTop = element.scrollHeight;
        }
    },


    scrollToBottomIfNeeded: function (element) {
        if (!element) return;

        const threshold = 300;
        const currentPosition = element.scrollTop + element.offsetHeight;
        const isNearBottom = element.scrollHeight - currentPosition  <= threshold;

        if (isNearBottom) {
            requestAnimationFrame(() => {
                element.scrollTop = element.scrollHeight;
            });
        }
    }
};
