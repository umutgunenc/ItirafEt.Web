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

//window.chatScrollHelper = {
//    scrollToBottom: function (element) {
//        if (element) {
//            element.scrollTop = element.scrollHeight;
//        }
//    }
//};


window.chatScrollHelper = {

    scrollToBottom: function (element) {
        if (element) {
            element.scrollTop = element.scrollHeight;
        }
    },


    scrollToBottomIfNeeded: function (element) {
        if (!element) return;

        const threshold = 300;
        // ScrollHeight ile karşılaştırmadan önce mevcut pozisyonu al
        const currentPosition = element.scrollTop + element.offsetHeight;
        const isNearBottom = element.scrollHeight - currentPosition  <= threshold;
        console.log(element.scrollHeight - currentPosition);

        // Eğer en altta değilse scroll etme
        if (isNearBottom) {
            console.log("scroll yapılıyor");
            // Raf kullanarak smooth scroll
            requestAnimationFrame(() => {
                element.scrollTop = element.scrollHeight;
            });
        }
    }
};


