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
