// ===== Constants =====
const CART_KEY = 'pharmacy_cart_v1';
const THEME_KEY = 'pharmacy_theme_v1';

// ===== DOM Elements =====
const cartBtn = document.getElementById('cartBtn');
const cartCount = document.getElementById('cartCount');
const cartList = document.getElementById('cartList');
const cartTotal = document.getElementById('cartTotal');
const overlay = document.getElementById('overlay');
const drawer = document.getElementById('drawer');
const themeToggle = document.getElementById('themeToggle');
const searchInput = document.getElementById('searchInput');
const toTopBtn = document.getElementById('toTop');
const checkoutBtn = document.getElementById('checkoutBtn'); // لو مش موجود مش هيبوظ حاجة

// ===== Theme system =====
function setTheme(theme) {
    document.body.setAttribute('data-theme', theme);
    localStorage.setItem(THEME_KEY, theme);

    if (themeToggle) {
        themeToggle.textContent = theme === 'dark' ? '☀' : '☾';
        themeToggle.title = 'تغيير وضع العرض';
    }
}

const savedTheme =
    localStorage.getItem(THEME_KEY) ||
    (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');

setTheme(savedTheme);

if (themeToggle) {
    themeToggle.addEventListener('click', () => {
        const next = document.body.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
        setTheme(next);
    });
}

// ===== Cart helpers =====
function readCart() {
    try {
        return JSON.parse(localStorage.getItem(CART_KEY) || '[]');
    } catch {
        return [];
    }
}

function writeCart(items) {
    localStorage.setItem(CART_KEY, JSON.stringify(items));
    renderCart();
}

function addToCart(name, price, productId) {
    const items = readCart();
    const idx = items.findIndex(i => i.productId === productId);
    if (idx >= 0) {
        items[idx].qty += 1;
    } else {
        items.push({ productId, name, price, qty: 1 });
    }
    writeCart(items);
}

function removeFromCart(name) {
    const items = readCart().filter(i => i.name !== name);
    writeCart(items);
}

function changeQty(name, delta) {
    const items = readCart();
    const it = items.find(i => i.name === name);
    if (!it) return;
    it.qty += delta;
    if (it.qty <= 0) {
        writeCart(items.filter(i => i.name !== name));
    } else {
        writeCart(items);
    }
}

function clearCart() {
    writeCart([]);
}

function formatPrice(v) {
    const num = Number(v) || 0;
    return num.toFixed(2) + ' ج.م';
}

function renderCart() {
    const items = readCart();
    if (!cartList || !cartTotal || !cartCount) return;

    if (items.length === 0) {
        cartList.innerHTML = `<p class="empty-cart-text">السلة فاضية</p>`;
        cartTotal.textContent = '0 ج.م';
        cartCount.textContent = '0';
        return;
    }

    cartList.innerHTML = items.map(i => `
        <div class="cart-item">
            <div>
                <div class="cart-item-name">${i.name}</div>
                <div class="cart-item-meta">${formatPrice(i.price)} × ${i.qty}</div>
            </div>
            <div class="qty-controls">
                <button type="button" onclick="changeQty('${i.name}', 1)">+</button>
                <button type="button" onclick="changeQty('${i.name}', -1)">-</button>
                <button type="button" onclick="removeFromCart('${i.name}')" style="background:#ef4444">حذف</button>
            </div>
        </div>
    `).join('');

    const total = items.reduce((s, i) => s + i.price * i.qty, 0);
    cartTotal.textContent = formatPrice(total);
    cartCount.textContent = items.reduce((s, i) => s + i.qty, 0);
}

function toggleCart(open) {
    if (!drawer || !overlay) return;
    const shouldOpen = (open === undefined)
        ? !drawer.classList.contains('open')
        : open;

    if (shouldOpen) {
        drawer.classList.add('open');
        overlay.style.display = 'block';
    } else {
        drawer.classList.remove('open');
        overlay.style.display = 'none';
    }
}

if (cartBtn) {
    cartBtn.addEventListener('click', () => toggleCart());
}

// ===== Checkout =====
async function checkout() {
    const items = readCart();
    if (!items.length) {
        alert('السلة فاضية');
        return;
    }

    // 🔐 Check authentication لو عندنا زرار بـ data-*
    let isAuth = true;
    let loginUrl = '/CustomerAuth/Login';

    if (checkoutBtn) {
        isAuth = checkoutBtn.dataset.isAuthenticated === 'true';
        if (checkoutBtn.dataset.loginUrl) {
            loginUrl = checkoutBtn.dataset.loginUrl;
        }
    }

    if (!isAuth) {
        // ممكن تزودي returnUrl لو حابة
        window.location.href = loginUrl;
        return;
    }

    try {
        const model = {
            items: items.map(item => ({
                productId: item.productId || getProductIdByName(item.name),
                productName: item.name,
                quantity: item.qty,
                price: item.price
            }))
        };


        const response = await fetch('/Order/Checkout', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(model)
        });

        let result = null;
        try {
            result = await response.json();
        } catch {
            // لو السيرفر رجّع نص مش JSON
        }

        if (!response.ok || !result || result.success !== true) {
            const msg = (result && result.message) || 'حدث خطأ أثناء إتمام الطلب';
            alert(msg);
            return;
        }

        alert('تم تسجيل طلبك وسيتم التواصل معك للتأكيد.');
        clearCart();
        toggleCart(false);

        if (result.redirectUrl) {
            window.location.href = result.redirectUrl;
        }
    } catch (error) {
        console.error('Checkout error:', error);
        alert('حدث خطأ في الاتصال بالسيرفر');
    }
}

// ===== Scroll to top =====
window.addEventListener('scroll', () => {
    if (!toTopBtn) return;
    toTopBtn.style.display = window.scrollY > 0 ? 'block' : 'none';
});

function scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
}
window.scrollToTop = scrollToTop;

if (toTopBtn) {
    toTopBtn.title = 'أعلى الصفحة';
    toTopBtn.addEventListener('click', scrollToTop);
}

// ===== Search =====
const products = Array.from(document.querySelectorAll('.product'));

function normalize(str) {
    return (str || '').toString().toLowerCase();
}

function applyFilters() {
    if (!searchInput) return;
    const term = normalize(searchInput.value);
    products.forEach(card => {
        const name = normalize(card.getAttribute('data-name'));
        card.style.display = (!term || name.includes(term)) ? '' : 'none';
    });
}

if (searchInput) {
    searchInput.addEventListener('input', applyFilters);
}

// ===== Order Now =====
function orderNow() {
    const section = document.getElementById('offers') || document.querySelector('.offers-section');
    if (section) {
        window.scrollTo({ top: section.offsetTop - 80, behavior: 'smooth' });
    }
}
window.orderNow = orderNow;

// ===== Add to cart =====
document.addEventListener('click', function (e) {
    const btn = e.target.closest('.add-to-cart');
    if (!btn) return;

    const card = btn.closest('.product');
    if (!card) return;

    const name = card.getAttribute('data-name') || 'Product';
    const productId = parseInt(card.getAttribute('data-product-id')) || 0;

    let price = 0;
    const dataPrice = card.getAttribute('data-price');
    if (dataPrice) {
        price = parseFloat(dataPrice);
    } else {
        const priceText = card.querySelector('.price')?.textContent || '';
        const match = priceText.replace(',', '.').match(/[\d.]+/);
        if (match) {
            price = parseFloat(match[0]);
        }
    }

    addToCart(name, price, productId);
});

// ===== Health Tips =====
const healthTips = [
    { icon: "💧", text: "اشرب 8 أكواب من الماء يوميًا للحفاظ على رطوبة جسمك" },
    { icon: "🛌", text: "حافظ على نوم منتظم لا يقل عن 7 ساعات يوميًا" },
    { icon: "🥦", text: "تناول فواكه وخضروات طازجة يوميًا" },
    { icon: "🏃‍♂️", text: "مارس نشاط بدني مثل المشي 30 دقيقة يوميًا" },
    { icon: "😌", text: "خصص وقتًا للاسترخاء وإدارة التوتر" },
    { icon: "💊", text: "تناول الفيتامينات والمكملات حسب نصيحة طبيبك" }
];

const dailyTipBtn = document.getElementById('dailyTipBtn');
const dailyTip = document.getElementById('dailyTip');

if (dailyTipBtn && dailyTip) {
    dailyTipBtn.addEventListener('click', () => {
        const tip = healthTips[Math.floor(Math.random() * healthTips.length)];
        dailyTip.textContent = tip.icon + " " + tip.text;
    });
}

// ===== Helper =====
function getProductIdByName(name) {
    const card = Array.from(document.querySelectorAll('.product'))
        .find(p => p.getAttribute('data-name') === name);
    return card ? parseInt(card.getAttribute('data-product-id')) : 0;
}

// ===== Init =====
renderCart();
applyFilters();
