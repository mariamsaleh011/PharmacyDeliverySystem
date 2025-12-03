document.addEventListener("DOMContentLoaded", function () {
    const chatBox = document.getElementById("chat-box");
    if (!chatBox) return;

    // Scroll to bottom on load
    chatBox.scrollTop = chatBox.scrollHeight;

    // Auto-scroll every 2 seconds
    setInterval(() => {
        chatBox.scrollTop = chatBox.scrollHeight;
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
