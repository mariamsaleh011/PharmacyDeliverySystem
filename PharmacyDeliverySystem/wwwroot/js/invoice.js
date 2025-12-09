// wwwroot/js/invoice.js

// ================== Print / Save Invoice ==================
function openInvoiceWindow() {
    const card = document.getElementById('invoiceCard');
    if (!card) return null;

    // نعمل نسخة من الكارت عشان ما نلعبش في النسخة الأصلية
    const clone = card.cloneNode(true);

    const w = window.open('', '_blank');
    if (!w) return null;

    // لو عندك عنصر جوه الفاتورة عليه data-order-id
    const orderIdHolder = clone.querySelector('[data-order-id]');
    const orderId = orderIdHolder ? (orderIdHolder.dataset.orderId || '') : '';

    w.document.write(`<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<title>Invoice #${orderId}</title>
<link rel="stylesheet" href="/lib/bootstrap/dist/css/bootstrap.min.css" />
<style>
    body {
        background: #fff;
        padding: 20px;
        font-family: Arial, sans-serif;
    }
    .card {
        box-shadow: none !important;
    }
    @media print {
        .btn,
        button,
        .no-print {
            display: none !important;
        }
    }
</style>
</head>
<body></body>
</html>`);

    w.document.close();

    // نضيف نسخة الفاتورة لجسم الصفحة الجديدة
    w.document.body.appendChild(clone);

    // ننتظر تحميل الصور قبل ما نعمل focus/print
    const images = w.document.images;
    let loaded = 0;

    if (images.length === 0) {
        w.focus();
        return w;
    }

    for (let img of images) {
        img.onload = img.onerror = () => {
            loaded++;
            if (loaded === images.length) {
                w.focus();
            }
        };
    }

    return w;
}

function printInvoice() {
    const w = openInvoiceWindow();
    if (!w) return;

    // delay بسيط لضمان تحميل كل حاجة
    setTimeout(() => {
        w.print();
        w.close();
    }, 300);
}

// Save Invoice = Print Dialog → Save as PDF من المتصفح
function saveInvoice() {
    printInvoice();
}

// ================== Download QR Code ==================
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

// ================== QR Scan + Star Rating ==================
document.addEventListener('DOMContentLoaded', function () {
    // ---------- QR Upload Scan ----------
    const qrInput = document.getElementById('qrUpload');           // file input for QR
    const scanResult = document.getElementById('scanResult');      // span/paragraph for messages
    const orderStatus = document.getElementById('orderStatus');    // span to show Delivered
    const ratingSection = document.getElementById('ratingSection'); // div with stars section

    if (qrInput) {
        qrInput.addEventListener('change', function () {
            const file = this.files[0];
            if (!file) return;

            const reader = new FileReader();
            reader.onload = function () {
                const img = new Image();
                img.src = reader.result;

                img.onload = function () {
                    const canvas = document.createElement('canvas');
                    const ctx = canvas.getContext('2d');
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
                                if (data && data.success) {
                                    if (orderStatus) orderStatus.innerText = 'Delivered';
                                    if (ratingSection) ratingSection.style.display = 'block';
                                    if (scanResult) scanResult.innerText = '✅ Order delivered!';
                                } else {
                                    if (scanResult) scanResult.innerText = '❌ Failed to deliver order';
                                }
                            })
                            .catch(() => {
                                if (scanResult) scanResult.innerText = '❌ Error scanning QR';
                            });
                    } else {
                        if (scanResult) scanResult.innerText = '❌ No QR detected';
                    }
                };
            };
            reader.readAsDataURL(file);
        });
    }

    // ---------- Star Rating (لون قديم + منطق جديد منظم) ----------
    const stars = document.querySelectorAll('.star');
    const ratingValue = document.getElementById('ratingValue'); // hidden input

    if (stars.length && ratingValue) {
        stars.forEach(star => {
            star.addEventListener('click', function () {
                const value = parseInt(this.dataset.value);

                // نحفظ القيمة في الـ hidden
                ratingValue.value = value;

                // نلوّن النجوم زي القديم (#facc15)
                stars.forEach(s => {
                    const v = parseInt(s.dataset.value);
                    s.style.color = v <= value ? '#facc15' : '#ccc';
                });
            });
        });
    }
});

// ================== Submit Rating ==================
function submitRating(orderId) {
    const ratingValue = document.getElementById('ratingValue');
    if (!ratingValue) {
        alert('Rating control not found');
        return;
    }

    const rating = parseInt(ratingValue.value);
    if (!rating || rating < 1) {
        alert('Please select a rating');
        return;
    }

    // Anti-forgery token زي القديم
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenInput ? tokenInput.value : '';

    fetch('/QrConfirmation/SubmitRating', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        // نخلي اسم الخاصية Stars زي النسخة القديمة عشان الموديل في الـ Backend
        body: JSON.stringify({ OrderId: orderId, Stars: rating })
    })
        .then(res => res.json())
        .then(data => {
            if (data && data.success) {
                alert('Thank you for your rating!');

                // نقفل النجوم بعد التقييم
                document.querySelectorAll('.star').forEach(s => {
                    s.style.pointerEvents = 'none';
                });

                const btn = document.getElementById('submitRating');
                if (btn) btn.disabled = true;

                // زيادة صغيرة من الجديد: نعمل Refresh بعد شوية لو حابب تحدث الصفحة
                setTimeout(() => {
                    try {
                        location.reload();
                    } catch (e) {
                        // لو في أي مشكلة، نتجاهلها بهدوء
                    }
                }, 600);
            } else {
                alert('Error submitting rating');
            }
        })
        .catch(() => {
            alert('Error submitting rating');
        });
}
