// wwwroot/js/chat.js

document.addEventListener("DOMContentLoaded", () => {
    const chatBox = document.getElementById("chat-box");

    // أول ما الصفحة تفتح انزل لآخر الرسائل
    if (chatBox) {
        chatBox.scrollTop = chatBox.scrollHeight;
    }

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
});
