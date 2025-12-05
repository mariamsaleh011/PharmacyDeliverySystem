// ===== Constants =====
const CART_KEY = 'pharmacy_cart_v1';
const THEME_KEY = 'pharmacy_theme_v1';
const LANG_KEY = 'pharmacy_lang_v1';

// ===== DOM Elements =====
const cartBtn = document.getElementById('cartBtn');
const cartCount = document.getElementById('cartCount');
const cartList = document.getElementById('cartList');
const cartTotal = document.getElementById('cartTotal');
const overlay = document.getElementById('overlay');
const drawer = document.getElementById('drawer');
const themeToggle = document.getElementById('themeToggle');
const headerSearchInput = document.getElementById('searchInput'); // header search
const toTopBtn = document.getElementById('toTop');
const checkoutBtn = document.getElementById('checkoutBtn');

// عناصر للنصوص (للترجمة)
const navHomeEl = document.getElementById('navHome');
const navChatEl = document.getElementById('navChat');
const loginTextEl = document.getElementById('loginText');
const logoutTextEl = document.getElementById('logoutText');
const cartTextEl = document.getElementById('cartText');
const backBtnTextEl = document.getElementById('backBtnText');
const footerTextEl = document.getElementById('footerText');
const privacyLinkTextEl = document.getElementById('privacyLinkText');
const cartTitleEl = document.getElementById('cartTitle');
const totalLabelEl = document.getElementById('totalLabel');
const clearCartTextEl = document.getElementById('clearCartText');
const currentLangEl = document.getElementById('currentLang');

// ===== Translations =====
const translations = {
    en: {
        navHome: 'Home',
        navChat: 'Chat with Pharmacy',
        loginText: 'Login',
        logoutText: 'Logout',
        cartText: 'Cart',
        backBtn: 'Back',
        footerText: '© 2025 - PharmacyDeliverySystem - All rights reserved',
        privacyText: 'Privacy',
        cartTitle: 'Shopping Cart',
        totalLabel: 'Total Amount',
        checkoutBtn: 'Checkout',
        clearCartText: 'Clear cart',
        searchPlaceholder: 'Search in items and products...',
        emptyCartTitle: 'Your cart is empty',
        emptyCartSubtitle: 'Start adding items to get started!',
        toTopTitle: 'Back to top'
    },
    ar: {
        navHome: 'الرئيسية',
        navChat: 'Chat مع الصيدلي',
        loginText: 'تسجيل الدخول',
        logoutText: 'تسجيل الخروج',
        cartText: 'السلة',
        backBtn: 'رجوع',
        footerText: '© 2025 - نظام توصيل الصيدليات - جميع الحقوق محفوظة',
        privacyText: 'الخصوصية',
        cartTitle: 'سلة المشتريات',
        totalLabel: 'الإجمالي',
        checkoutBtn: 'إتمام الشراء',
        clearCartText: 'تفريغ السلة',
        searchPlaceholder: 'ابحث في الباقات والمنتجات...',
        emptyCartTitle: 'سلتك فارغة',
        emptyCartSubtitle: 'ابدأ بإضافة المنتجات الآن!',
        toTopTitle: 'أعلى الصفحة'
    }
};

function getCurrentLang() {
    const saved = localStorage.getItem(LANG_KEY);
    return saved === 'ar' ? 'ar' : 'en';
}

function applyLanguage(lang) {
    const safeLang = lang === 'ar' ? 'ar' : 'en';
    localStorage.setItem(LANG_KEY, safeLang);

    const t = translations[safeLang];

    // تحديث lang و dir على الـ html
    if (document.documentElement) {
        document.documentElement.lang = safeLang;
        document.documentElement.dir = safeLang === 'ar' ? 'rtl' : 'ltr';
    }

    if (currentLangEl) currentLangEl.textContent = safeLang.toUpperCase();

    if (navHomeEl && t.navHome) navHomeEl.textContent = t.navHome;
    if (navChatEl && t.navChat) navChatEl.textContent = t.navChat;

    if (loginTextEl && t.loginText) loginTextEl.textContent = t.loginText;
    if (logoutTextEl && t.logoutText) logoutTextEl.textContent = t.logoutText;

    if (cartTextEl && t.cartText) cartTextEl.textContent = t.cartText;
    if (backBtnTextEl && t.backBtn) backBtnTextEl.textContent = t.backBtn;

    if (footerTextEl && t.footerText) footerTextEl.textContent = t.footerText;
    if (privacyLinkTextEl && t.privacyText) privacyLinkTextEl.textContent = t.privacyText;

    if (cartTitleEl && t.cartTitle) cartTitleEl.textContent = t.cartTitle;
    if (totalLabelEl && t.totalLabel) totalLabelEl.textContent = t.totalLabel;

    if (checkoutBtn && t.checkoutBtn) checkoutBtn.textContent = t.checkoutBtn;
    if (clearCartTextEl && t.clearCartText) clearCartTextEl.textContent = t.clearCartText;

    if (headerSearchInput && t.searchPlaceholder) {
        headerSearchInput.placeholder = t.searchPlaceholder;
    }

    if (toTopBtn && t.toTopTitle) {
        toTopBtn.title = t.toTopTitle;
    }

    // إعادة رسم السلة (عشان نص الرسالة الفارغة يتغير)
    renderCart();
}

// تطبيق اللغة المحفوظة أول ما الصفحة تفتح
applyLanguage(getCurrentLang());

// ===== Theme system =====
function setTheme(theme) {
    document.body.setAttribute('data-theme', theme);
    localStorage.setItem(THEME_KEY, theme);

    if (themeToggle) {
        themeToggle.textContent = theme === 'dark' ? '☀' : '☾';
        themeToggle.title = 'Toggle theme';
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
    return num.toFixed(2) + ' EGP';
}

function renderCart() {
    const items = readCart();
    if (!cartList || !cartTotal || !cartCount) return;

    const lang = getCurrentLang();
    const t = translations[lang] || translations.en;

    if (items.length === 0) {
        cartList.innerHTML = `
            <div class="empty-cart">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <circle cx="9" cy="21" r="1"></circle>
                    <circle cx="20" cy="21" r="1"></circle>
                    <path d="M1 1h4l2.68 13.39a2 2 0 002 1.61h9.72a2 2 0 002-1.61L23 6H6"></path>
                </svg>
                <p>${t.emptyCartTitle}<br />${t.emptyCartSubtitle}</p>
            </div>
        `;
        cartTotal.textContent = '0 EGP';
        cartCount.textContent = '0';
        return;
    }

    const totalQty = items.reduce((s, i) => s + i.qty, 0);
    const total = items.reduce((s, i) => s + i.price * i.qty, 0);

    cartList.innerHTML = items.map(i => `
        <div class="cart-item">
            <div class="cart-item-image">
                <svg viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2">
                    <rect x="3" y="3" width="18" height="18" rx="2"></rect>
                    <circle cx="8.5" cy="8.5" r="1.5"></circle>
                    <path d="M21 15l-5-5L5 21"></path>
                </svg>
            </div>
            <div class="cart-item-details">
                <div class="cart-item-name">${i.name}</div>
                <div class="cart-item-price">${formatPrice(i.price)}</div>
                <div class="cart-item-qty">Qty: ${i.qty}</div>
            </div>
            <button type="button" class="cart-item-remove" onclick="removeFromCart('${i.name}')">
                <span>&times;</span>
            </button>
        </div>
    `).join('');

    cartTotal.textContent = formatPrice(total);
    cartCount.textContent = totalQty;
}

function toggleCart(open) {
    if (!drawer || !overlay) return;
    const shouldOpen = (open === undefined)
        ? !drawer.classList.contains('open')
        : open;

    if (shouldOpen) {
        drawer.classList.add('open');
        overlay.classList.add('open');
        document.body.classList.add('cart-open');
    } else {
        drawer.classList.remove('open');
        overlay.classList.remove('open');
        document.body.classList.remove('cart-open');
    }
}

if (cartBtn) {
    cartBtn.addEventListener('click', () => toggleCart());
}

// ===== Checkout =====
async function checkout() {
    const items = readCart();
    const lang = getCurrentLang();
    const t = translations[lang] || translations.en;

    if (!items.length) {
        alert(lang === 'ar' ? 'سلتك فارغة' : 'Your cart is empty');
        return;
    }

    let isAuth = true;
    let loginUrl = '/CustomerAuth/Login';

    if (checkoutBtn) {
        isAuth = checkoutBtn.dataset.isAuthenticated === 'true';
        if (checkoutBtn.dataset.loginUrl) {
            loginUrl = checkoutBtn.dataset.loginUrl;
        }
    }

    if (!isAuth) {
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
        } catch { }

        if (!response.ok || !result || result.success !== true) {
            const msg = (result && result.message) || (lang === 'ar'
                ? 'حدث خطأ أثناء تسجيل الطلب.'
                : 'An error occurred while placing your order.');
            alert(msg);
            return;
        }

        alert(lang === 'ar'
            ? 'تم تسجيل طلبك، سوف نتواصل معك للتأكيد.'
            : 'Your order has been placed. We will contact you to confirm.');

        clearCart();
        toggleCart(false);

        if (result.redirectUrl) {
            window.location.href = result.redirectUrl;
        }
    } catch (error) {
        console.error('Checkout error:', error);
        alert(lang === 'ar'
            ? 'حدث خطأ في الاتصال. حاول مرة أخرى.'
            : 'A network error occurred. Please try again.');
    }
}

// ===== Scroll to top =====
window.addEventListener('scroll', () => {
    if (!toTopBtn) return;
    if (window.scrollY > 0) {
        toTopBtn.classList.add('show');
    } else {
        toTopBtn.classList.remove('show');
    }
});

function scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
}
window.scrollToTop = scrollToTop;

if (toTopBtn) {
    toTopBtn.addEventListener('click', scrollToTop);
}

// ===== Global header search (works on all products on the page) =====
const allProductCards = Array.from(document.querySelectorAll('.product'));

function normalize(str) {
    return (str || '').toString().toLowerCase();
}

function applyGlobalHeaderFilter() {
    if (!headerSearchInput) return;
    const term = normalize(headerSearchInput.value);
    allProductCards.forEach(card => {
        const name = normalize(card.getAttribute('data-name'));
        card.style.display = (!term || name.includes(term)) ? '' : 'none';
    });
}

if (headerSearchInput) {
    headerSearchInput.addEventListener('input', applyGlobalHeaderFilter);
}

// ===== Per-category search (Drugs / Baby / Men Care) =====
document.querySelectorAll('.products .category-search').forEach(function (wrapper) {
    const searchInput = wrapper.querySelector('.category-search-input');
    if (!searchInput) return;

    const productsSection = wrapper.closest('.products');
    if (!productsSection) return;

    const cards = productsSection.querySelectorAll('.product-grid .product');
    if (!cards.length) return;

    const countLabel = productsSection.querySelector('.category-count');
    const total = cards.length;

    function applyCategoryFilter() {
        const q = searchInput.value.toLowerCase().trim();
        let shown = 0;

        cards.forEach(card => {
            const text = card.textContent.toLowerCase();
            const match = text.includes(q);
            card.style.display = match ? '' : 'none';
            if (match) shown++;
        });

        if (countLabel) {
            if (!q) {
                countLabel.textContent = `${total} items`;
            } else {
                countLabel.textContent = `${shown} / ${total} items`;
            }
        }
    }

    searchInput.addEventListener('input', applyCategoryFilter);
});

// ===== Order Now (scroll to offers section) =====
function orderNow() {
    const section = document.getElementById('offers') || document.querySelector('.offers-section');
    if (section) {
        window.scrollTo({ top: section.offsetTop - 80, behavior: 'smooth' });
    }
}
window.orderNow = orderNow;

// ===== Add to cart (delegation) =====
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
    { icon: '💧', text: 'Drink at least 8 cups of water daily to stay hydrated.' },
    { icon: '🛌', text: 'Try to get at least 7 hours of sleep every night.' },
    { icon: '🥦', text: 'Include fresh fruits and vegetables in your daily meals.' },
    { icon: '🏃‍♂️', text: 'Do some physical activity like walking for 30 minutes a day.' },
    { icon: '😌', text: 'Take time to relax and manage your stress levels.' },
    { icon: '💊', text: 'Take vitamins and supplements only as advised by your doctor.' }
];

const dailyTipBtn = document.getElementById('dailyTipBtn');
const dailyTip = document.getElementById('dailyTip');

if (dailyTipBtn && dailyTip) {
    dailyTipBtn.addEventListener('click', () => {
        const tip = healthTips[Math.floor(Math.random() * healthTips.length)];
        dailyTip.textContent = tip.icon + ' ' + tip.text;
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
applyGlobalHeaderFilter();


// ===== Categories horizontal scroll (Home categories strip) =====
const catStrip = document.getElementById('catStrip');
const catPrevBtn = document.querySelector('.cat-scroll-prev');
const catNextBtn = document.querySelector('.cat-scroll-next');

if (catStrip) {
    // خطوة الاسكرول = عرض كارت تقريباً
    let step = 260;
    const firstCard = catStrip.querySelector('.cat-card');
    if (firstCard) {
        const rect = firstCard.getBoundingClientRect();
        step = rect.width + 16; // عرض الكارت + الجاب
    }

    if (catNextBtn) {
        catNextBtn.addEventListener('click', () => {
            catStrip.scrollBy({
                left: step,
                behavior: 'smooth'
            });
        });
    }

    if (catPrevBtn) {
        catPrevBtn.addEventListener('click', () => {
            catStrip.scrollBy({
                left: -step,
                behavior: 'smooth'
            });
        });
    }
}


// ===== Language dropdown (EN / AR) =====
(function () {
    const toggle = document.getElementById('langToggle');
    const menu = document.getElementById('langMenu');

    if (!toggle || !menu) return;

    // فتح/قفل القائمة
    toggle.addEventListener('click', function (e) {
        e.stopPropagation();
        const isOpen = menu.classList.toggle('show');
        toggle.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
    });

    // اختيار اللغة
    menu.addEventListener('click', function (e) {
        const btn = e.target.closest('.lang-option');
        if (!btn) return;

        const lang = btn.dataset.lang; // "en" أو "ar"
        applyLanguage(lang);

        menu.classList.remove('show');
        toggle.setAttribute('aria-expanded', 'false');
    });

    // قفل القائمة عند الضغط برة
    document.addEventListener('click', function () {
        if (menu.classList.contains('show')) {
            menu.classList.remove('show');
            toggle.setAttribute('aria-expanded', 'false');
        }
    });
})();
