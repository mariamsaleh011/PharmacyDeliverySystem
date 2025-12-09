// wwwroot/js/chat.js

(function () {
    const chatBox = document.getElementById("chat-box");

    function scrollToBottom(scrollPageAlso = false) {
        if (!chatBox) return;

        // نأخرها شوية عشان الـ layout يكون اتحسب وارتفاع الـ chat-box يكون نهائي
        setTimeout(() => {
            // نزول جوّه صندوق الرسائل نفسه
            chatBox.scrollTop = chatBox.scrollHeight;

            // كمان نزّل صفحة الويب كلها لآخرها عشان الفوتر وزرار Send يبانوا
            if (scrollPageAlso) {
                const fullHeight =
                    document.documentElement.scrollHeight || document.body.scrollHeight;

                window.scrollTo({
                    top: fullHeight,
                    behavior: "smooth"
                });
            }
        }, 0);
    }

    // أول ما الصفحة تفتح / تتعمل Reload:
    // ننزل لآخر الرسائل + نخلي الفوتر في الصورة
    scrollToBottom(true);

    // إرسال الرسالة عند الضغط على Enter (بدون Shift)
    const input = document.querySelector(".chat-input-row input[name='message']");
    const form = document.querySelector(".chat-form");

    if (input && form) {
        input.addEventListener("keypress", function (e) {
            if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                form.submit();
            }
        });
    }

    // عرض اسم الملف المختار
    const fileInput = document.getElementById("fileUpload");
    const fileNameSpan = document.getElementById("fileName");

    if (fileInput && fileNameSpan) {
        fileInput.addEventListener("change", () => {
            if (fileInput.files.length > 0) {
                fileNameSpan.textContent = fileInput.files[0].name;
            } else {
                fileNameSpan.textContent = "No file selected";
            }
        });
    }
})();
