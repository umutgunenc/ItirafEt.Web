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
        return new Promise((resolve, reject) => {
            try {
                if (element) {
                    element.scrollTop = element.scrollHeight;

                    setTimeout(() => {
                        resolve();
                    }, 1000);
                } else {
                    resolve();
                }
            } catch (e) {
                reject(e);
            }
        });
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
    },


    setupInfiniteScroll: function (elementId, dotNetObjRef, threshold) {
        const el = document.getElementById(elementId);
        if (!el) return;
        el.addEventListener("scroll", () => {
            const distanceFromTop = el.scrollHeight + el.scrollTop - el.clientHeight;
            if (distanceFromTop <= threshold) {
                dotNetObjRef.invokeMethodAsync("OnScrollToTopAsync");
            }
        });
    },

};
