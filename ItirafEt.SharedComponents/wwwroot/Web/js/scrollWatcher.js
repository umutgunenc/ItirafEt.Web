window.addScrollListener = (dotNetObjRef) => {
    window.onscroll = () => {

        if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 100) {
            dotNetObjRef.invokeMethodAsync('OnScrollToBottom');
        }
    };
};


