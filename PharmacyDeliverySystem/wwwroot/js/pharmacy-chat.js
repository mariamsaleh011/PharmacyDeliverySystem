document.addEventListener("DOMContentLoaded", function () {
    const chatBox = document.getElementById("chat-box");
    if (!chatBox) return;

    function scrollToBottom(scrollPageAlso = false) {
        // نزول جوّه صندوق الشات
        chatBox.scrollTop = chatBox.scrollHeight;

        // اختيارى: ننزل صفحة الويب كلها لآخرها عشان زرار Send يبان
        if (scrollPageAlso) {
            const fullHeight =
                document.documentElement.scrollHeight || document.body.scrollHeight;

            window.scrollTo({
                top: fullHeight,
                behavior: "smooth"
            });
        }
    }

    // أول ما الصفحة تفتح: انزل لآخر الرسائل + هات زرار Send في الصورة
    scrollToBottom(true);

    // Auto-scroll كل 2 ثانية (للرسائل الجديدة) جوّه الشات بس
    setInterval(() => {
        scrollToBottom(false);
    }, 2000);

    // Send message on Enter
    const input = document.querySelector(".chat-footer input[type='text']");
    const form = document.querySelector(".chat-footer form");
    if (input && form) {
        input.addEventListener("keypress", function (e) {
            if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                form.submit();
            }
        });
    }
});
