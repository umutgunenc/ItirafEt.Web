window.chatInput = {
    init: function (id, isMobile, dotNetRef) {

        const element = document.getElementById(id);
        if (!element) {
            setTimeout(() => this.init(id, isMobile, dotNetRef), 200);
            return;
        }

        element.addEventListener("keydown", (e) => {

            if (e.key === "Enter") {

                let caret = element.selectionStart;
                let content = element.value;
                if (isMobile) {
                    e.preventDefault();
                    element.value = content.substring(0, caret) + "\n" + content.substring(caret);
                    element.selectionStart = element.selectionEnd = caret + 1;
                    this.resize(id);

                } else {
                    if (e.shiftKey) {

                        e.preventDefault();
                        element.value = content.substring(0, caret) + "\n" + content.substring(caret);
                        element.selectionStart = element.selectionEnd = caret + 1;
                        this.resize(id);

                    } else {
                        e.preventDefault();
                        // Blazor tarafını çağır
                        //DotNet.invokeMethodAsync("ItirafEt.SharedComponents", "SendMessageFromJs");
                        try {
                            dotNetRef.invokeMethodAsync("SendMessageFromJs", element.value);
                        } catch (error) {
                            console.error("Blazor metodu çağrılırken hata oluştu:", error);
                        }

                    }
                }
            }


        });
    },

    resize: function (id) {
        const element = document.getElementById(id);
        if (!element) return;
        element.style.height = "auto";
        let newHeight = element.scrollHeight;
        let lineHeight = parseInt(getComputedStyle(element).lineHeight);
        let maxHeight = lineHeight * 4;
        element.style.height = Math.min(newHeight, maxHeight) + "px";
    },

    reset: function (id) {
        const element = document.getElementById(id);
        if (!element) return;
        element.style.height = "auto";
    }
};

window.chatInputWithPhoto = {
    init: function (id, isMobile, dotNetRef) {

        const element = document.getElementById(id);
        if (!element) {
            setTimeout(() => this.init(id, isMobile, dotNetRef), 200);
            return;
        }

        element.addEventListener("keydown", (e) => {

            if (e.key === "Enter") {

                let caret = element.selectionStart;
                let content = element.value;
                if (isMobile) {
                    e.preventDefault();
                    element.value = content.substring(0, caret) + "\n" + content.substring(caret);
                    element.selectionStart = element.selectionEnd = caret + 1;
                    this.resize(id);

                } else {
                    if (e.shiftKey) {

                        e.preventDefault();
                        element.value = content.substring(0, caret) + "\n" + content.substring(caret);
                        element.selectionStart = element.selectionEnd = caret + 1;
                        this.resize(id);

                    } else {
                        e.preventDefault();
                        // Blazor tarafını çağır
                        //DotNet.invokeMethodAsync("ItirafEt.SharedComponents", "SendMessageFromJs");
                        try {
                            dotNetRef.invokeMethodAsync("SendMessagemWithPhotoFromJs", element.value);
                        } catch (error) {
                            console.error("Blazor metodu çağrılırken hata oluştu:", error);
                        }

                    }
                }
            }


        });
    },

    resize: function (id) {
        const element = document.getElementById(id);
        if (!element) return;
        element.style.height = "auto";
        let newHeight = element.scrollHeight;
        let lineHeight = parseInt(getComputedStyle(element).lineHeight);
        let maxHeight = lineHeight * 4;
        element.style.height = Math.min(newHeight, maxHeight) + "px";
    },

    reset: function (id) {
        const element = document.getElementById(id);
        if (!element) return;
        element.style.height = "auto";
    }
};






