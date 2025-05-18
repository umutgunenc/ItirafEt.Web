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

window.chatScrollHelper = {
    scrollToBottom: function (element) {
        if (element) {
            element.scrollTop = element.scrollHeight;
        }
    }
};


window.chatScrollHelper = {
    scrollToBottom: function (element) {
        if (!element) return;

        // threshold: alttan kaç px uzaklıktayken hala "en altta" kabul edelim
        const threshold = 0;

        const scrollPosition = element.scrollTop + element.clientHeight;
        const scrollHeight = element.scrollHeight;

        // Eğer en altta isek (veya threshold içinde kaldıysak), otomatik scroll yap
        if (scrollHeight - scrollPosition <= threshold) {
            element.scrollTop = scrollHeight;
        }
    }
};


