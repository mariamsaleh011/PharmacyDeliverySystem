// invoice.js

function openInvoiceWindow() {
    const card = document.getElementById('invoiceCard');
    if (!card) return null;

    const invoiceHtml = card.outerHTML;
    const win = window.open('', '_blank');

    win.document.write('<html><head><title>Invoice</title>');
    win.document.write('<link rel="stylesheet" href="/lib/bootstrap/dist/css/bootstrap.min.css" />');
    win.document.write('<style>body{background:#fff!important;padding:20px;}</style>');
    win.document.write('</head><body>');
    win.document.write(invoiceHtml);
    win.document.write('</body></html>');
    win.document.close();
    win.focus();
    return win;
}

function printInvoice() {
    const w = openInvoiceWindow();
    if (w) {
        w.print();
    }
}

// Save Invoice = Print Dialog → Save as PDF من المتصفح
function saveInvoice() {
    const w = openInvoiceWindow();
    if (w) {
        w.print();
    }
}

// ==================== Download QR Code ====================
function downloadQRCode() {
    const img = document.getElementById('qrCodeImage');
    if (!img) return;

    // جاي من الـ data-order-id في الـ View
    const orderId = img.dataset.orderId || 'Unknown';

    const link = document.createElement('a');
    link.href = img.src;
    link.download = 'QRCode_Order_' + orderId + '.png';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

// ==================== QR Upload Scan ====================
const qrInput = document.getElementById("qrUpload");         // file input for QR
const scanResult = document.getElementById("scanResult");   // span/paragraph for messages
const orderStatus = document.getElementById("orderStatus"); // span to show Delivered
const ratingSection = document.getElementById("ratingSection"); // div with stars

if (qrInput) {
    qrInput.addEventListener("change", function () {
        const file = this.files[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = function () {
            const img = new Image();
            img.src = reader.result;

            img.onload = function () {
                const canvas = document.createElement("canvas");
                const ctx = canvas.getContext("2d");
                canvas.width = img.width;
                canvas.height = img.height;
                ctx.drawImage(img, 0, 0);

                const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);

                // لازم تكون محمّل jsQR في الـ View:
                // <script src="https://cdn.jsdelivr.net/npm/jsqr/dist/jsQR.js"></script>
                const code = jsQR(imageData.data, canvas.width, canvas.height);

                if (code) {
                    fetch('/QrConfirmation/ConfirmDelivery?qrData=' + encodeURIComponent(code.data))
                        .then(res => res.json())
                        .then(data => {
                            if (data.success) {
                                if (orderStatus) orderStatus.innerText = "Delivered";
                                if (ratingSection) ratingSection.style.display = "block";
                                if (scanResult) scanResult.innerText = "✅ Order delivered!";
                            } else {
                                if (scanResult) scanResult.innerText = "❌ Failed to deliver order";
                            }
                        })
                        .catch(() => {
                            if (scanResult) scanResult.innerText = "❌ Error scanning QR";
                        });
                } else {
                    if (scanResult) scanResult.innerText = "❌ No QR detected";
                }
            };
        };
        reader.readAsDataURL(file);
    });
}

// ==================== Star Rating ====================
const stars = document.querySelectorAll(".star");
const ratingValue = document.getElementById("ratingValue");

if (stars && ratingValue) {
    stars.forEach(star => {
        star.addEventListener("click", function () {
            const value = parseInt(this.dataset.value);
            ratingValue.value = value;
            stars.forEach(s => s.style.color = s.dataset.value <= value ? "#facc15" : "#ccc");
        });
    });
}

// ==================== Submit Rating ====================
function submitRating(orderId) {
    if (!ratingValue) {
        alert("Rating control not found");
        return;
    }

    const rating = parseInt(ratingValue.value);
    if (!rating || rating < 1) {
        alert("Please select a rating");
        return;
    }

    fetch('/QrConfirmation/SubmitRating', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': (document.querySelector('input[name="__RequestVerificationToken"]') || {}).value
        },
        body: JSON.stringify({ OrderId: orderId, Stars: rating })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                alert("Thank you for your rating!");
                if (stars) {
                    stars.forEach(s => s.style.pointerEvents = "none");
                }
                const btn = document.getElementById("submitRating");
                if (btn) btn.disabled = true;
            } else {
                alert("Error submitting rating");
            }
        })
        .catch(() => alert("Error submitting rating"));
}
