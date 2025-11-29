// wwwroot/js/chat.js

document.addEventListener("DOMContentLoaded", () => {
    const chatBox = document.getElementById("chat-box");
    const userInput = document.getElementById("userInput");
    const fileInput = document.getElementById("fileUpload");
    const fileNameSpan = document.getElementById("fileName");
    const attachBtn = document.getElementById("attachBtn");
    const sendBtn = document.getElementById("sendBtn");
    const typingIndicator = document.getElementById("typingIndicator");

    // لو الصفحة مش صفحة الشات (مفيهاش العناصر دي) اطلع
    if (!chatBox || !userInput || !fileInput || !fileNameSpan || !attachBtn || !sendBtn) {
        return;
    }

    function getDict() {
        // نحاول نجيب الترجمة من السكربت التاني لو موجود
        const tr = window.translations || {};
        const lang = window.currentLang || "en";
        return tr[lang] || tr.en || {};
    }

    // فتح اختيار الملف من زر الأيقونة
    attachBtn.addEventListener("click", () => fileInput.click());

    // عرض اسم الملف
    fileInput.addEventListener("change", () => {
        if (fileInput.files.length > 0) {
            fileNameSpan.textContent = fileInput.files[0].name;
        } else {
            const dict = getDict();
            fileNameSpan.textContent = dict.chat_no_file || "No file selected";
        }
    });

    function scrollToBottom() {
        chatBox.scrollTop = chatBox.scrollHeight;
    }

    function createUserMessage(text) {
        const row = document.createElement("div");
        row.className = "msg-row user";

        const bubble = document.createElement("div");
        bubble.className = "message user";
        bubble.textContent = text;

        row.appendChild(bubble);
        chatBox.appendChild(row);
    }

    function createUserFileMessage(file) {
        const row = document.createElement("div");
        row.className = "msg-row user";

        const bubble = document.createElement("div");
        bubble.className = "message user";

        if (file.type && file.type.startsWith("image/")) {
            const img = document.createElement("img");
            img.src = URL.createObjectURL(file);
            img.style.maxWidth = "220px";
            img.style.borderRadius = "10px";
            bubble.appendChild(img);
        } else {
            bubble.textContent = "📄 " + file.name;
        }

        row.appendChild(bubble);
        chatBox.appendChild(row);
    }

    function createPharmacistMessage(text) {
        const row = document.createElement("div");
        row.className = "msg-row pharmacist";

        const avatar = document.createElement("div");
        avatar.className = "msg-avatar";
        avatar.textContent = "💊";

        const bubble = document.createElement("div");
        bubble.className = "message pharmacist";
        bubble.textContent = text;

        row.appendChild(avatar);
        row.appendChild(bubble);
        chatBox.appendChild(row);
    }

    function showTyping() {
        if (typingIndicator) {
            typingIndicator.style.display = "flex";
        }
    }

    function hideTyping() {
        if (typingIndicator) {
            typingIndicator.style.display = "none";
        }
    }

    function sendMessage() {
        const text = userInput.value.trim();
        const hasFile = fileInput.files.length > 0;

        // لو مفيش لا رسالة ولا ملف ما تبعتش حاجة
        if (!text && !hasFile) return;

        // رسالة اليوزر
        if (text) {
            createUserMessage(text);
        }

        // ملف من اليوزر
        if (hasFile) {
            const file = fileInput.files[0];
            createUserFileMessage(file);
            fileInput.value = "";

            const dict = getDict();
            fileNameSpan.textContent = dict.chat_no_file || "No file selected";
        }

        userInput.value = "";
        scrollToBottom();

        // فقاعات typing + رد تجريبي
        showTyping();

        setTimeout(() => {
            hideTyping();

            const dict = getDict();
            const reply =
                dict.chat_auto_reply ||
                "Thank you for your message. The pharmacist will review it shortly.";

            createPharmacistMessage(reply);
            scrollToBottom();
        }, 900);
    }

    // زرار Send
    sendBtn.addEventListener("click", sendMessage);

    // Enter من الكيبورد
    userInput.addEventListener("keydown", (e) => {
        if (e.key === "Enter") {
            e.preventDefault();
            sendMessage();
        }
    });
});
