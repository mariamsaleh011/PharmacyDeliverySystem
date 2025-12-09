// ===== Constants =====
const CART_KEY = 'pharmacy_cart_v1';
const THEME_KEY = 'pharmacy_theme_v1';
const LANG_KEY = 'pharmacy_lang_v1';

// منع تكرار الـ checkout
let isCheckoutInProgress = false;

// ===== DOM Elements =====
const cartBtn = document.getElementById('cartBtn');
const cartCount = document.getElementById('cartCount');
const cartList = document.getElementById('cartList');
const cartTotal = document.getElementById('cartTotal');
const overlay = document.getElementById('overlay');
const drawer = document.getElementById('drawer');
const themeToggle = document.getElementById('themeToggle');
const headerSearchInput = document.getElementById('searchInput'); // header search
const headerSearchBar = document.getElementById('headerSearchBar'); // container بتاع السيرش
const toTopBtn = document.getElementById('toTop');
const checkoutBtn = document.getElementById('checkoutBtn');

// ===== Back to top progress helpers =====
let toTopProgress = null;
if (toTopBtn) {
    toTopProgress = toTopBtn.querySelector('.to-top-progress');
}

function updateToTopProgress() {
    if (!toTopBtn || !toTopProgress) return;

    const scrollTop =
        window.scrollY ??
        document.documentElement.scrollTop ??
        0;

    const docHeight = document.documentElement.scrollHeight - window.innerHeight;
    if (docHeight <= 0) {
        toTopProgress.style.background = 'none';
        return;
    }

    const progress = Math.min((scrollTop / docHeight) * 100, 100);
    const deg = progress * 3.6;
    toTopProgress.style.background =
        `conic-gradient(#00e6ff ${deg}deg, transparent ${deg}deg)`;
}

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
        searchPlaceholder: 'Search medicines, healthcare products...',
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

// addToCart مع imageUrl
function addToCart(name, price, productId, imageUrl) {
    const items = readCart();
    const idx = items.findIndex(i => i.productId === productId);
    if (idx >= 0) {
        items[idx].qty += 1;
    } else {
        items.push({ productId, name, price, qty: 1, imageUrl: imageUrl || 'fallback.png' });
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

    // ✅ هنا بنحاول نجيب صورة مؤكدة:
    cartList.innerHTML = items.map(i => {
        // لو عندي imageUrl → استخدمها
        let imgSrc = i.imageUrl || '';

        // لو مفيش، حاول أجيبها من كارت المنتج على الصفحة (لو موجود)
        if (!imgSrc && i.productId) {
            const card = document.querySelector(`.product[data-product-id="${i.productId}"] img`);
            if (card) {
                imgSrc = card.getAttribute('src');
            }
        }

        // لو لسه مفيش → استخدم لوجو السيستم
        if (!imgSrc) {
            imgSrc = '/images/icons/medicine-logo.svg';
        }

        return `
        <div class="cart-item">
            <div class="cart-item-image">
                <img src="${imgSrc}"
                     alt="${i.name}"
                     onerror="this.onerror=null;this.src='/images/icons/medicine-logo.svg';" />
            </div>
            <div class="cart-item-details">
                <div class="cart-item-name">${i.name}</div>
                <div class="cart-item-price">${formatPrice(i.price)}</div>
                <div class="cart-item-qty-controls">
                    <button type="button" class="qty-btn minus-btn" onclick="changeQty('${i.name}', -1)">-</button>
                    <span class="qty-display">${i.qty}</span>
                    <button type="button" class="qty-btn plus-btn" onclick="changeQty('${i.name}', 1)">+</button>
                </div>
            </div>
            <button type="button" class="cart-item-remove" onclick="removeFromCart('${i.name}')">
                <span>&times;</span>
            </button>
        </div>
        `;
    }).join('');

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
    cartBtn.addEventListener('click', (e) => {
        const mode = cartBtn.dataset.mode || 'drawer';

        if (mode === 'page') {
            e.preventDefault();
            const url = cartBtn.dataset.cartUrl || '/Home/Cart';
            window.location.href = url;
        } else {
            toggleCart();
        }
    });
}

// ===== Checkout =====
async function checkout() {
    if (isCheckoutInProgress) {
        return;
    }
    isCheckoutInProgress = true;
    if (checkoutBtn) checkoutBtn.disabled = true;

    const items = readCart();
    const lang = getCurrentLang();

    if (!items.length) {
        alert(lang === 'ar' ? 'سلتك فارغة' : 'Your cart is empty');
        isCheckoutInProgress = false;
        if (checkoutBtn) checkoutBtn.disabled = false;
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
        isCheckoutInProgress = false;
        if (checkoutBtn) checkoutBtn.disabled = false;
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
    } finally {
        isCheckoutInProgress = false;
        if (checkoutBtn) checkoutBtn.disabled = false;
    }
}

// ===== Scroll to top (show / hide + progress) =====
window.addEventListener('scroll', () => {
    if (!toTopBtn) return;

    const scrollTop =
        window.scrollY ??
        document.documentElement.scrollTop ??
        0;

    if (scrollTop > 300) {
        toTopBtn.classList.add('show');
    } else {
        toTopBtn.classList.remove('show');
    }

    updateToTopProgress();
});

function scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
}
window.scrollToTop = scrollToTop;

if (toTopBtn) {
    toTopBtn.addEventListener('click', scrollToTop);
}

// ===== Enhanced global header search (dropdown + autocomplete + recent) =====

// كل كروت المنتجات اللي على الصفحة
const allProductCards = Array.from(document.querySelectorAll('.product'));

function normalize(str) {
    return (str || '').toString().toLowerCase();
}

function escapeHtml(str) {
    return (str || '')
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

// recent searches في localStorage
function getRecentSearches() {
    const defaults = ['Panadol', 'Vitamins', 'Face Mask'];
    try {
        const stored = JSON.parse(localStorage.getItem('recentSearches')) || [];
        return stored.length ? stored : defaults;
    } catch {
        return defaults;
    }
}

function saveSearch(query) {
    if (!query) return;
    let searches = [];
    try {
        searches = JSON.parse(localStorage.getItem('recentSearches')) || [];
    } catch {
        searches = [];
    }

    searches = searches.filter(x => x !== query);
    searches.unshift(query);
    searches = searches.slice(0, 5);

    localStorage.setItem('recentSearches', JSON.stringify(searches));
}

// فلترة المنتجات الموجودة على الصفحة فقط
function applyGlobalHeaderFilter(term) {
    const t = normalize(term);
    allProductCards.forEach(card => {
        const name = normalize(card.getAttribute('data-name') || card.textContent);
        card.style.display = (!t || name.includes(t)) ? '' : 'none';
    });
}

function initHeaderSearch() {
    if (!headerSearchInput || !headerSearchBar) return;

    const dropdown = document.getElementById('searchDropdown');
    const tips = document.getElementById('searchTips');
    const spinner = document.getElementById('searchSpinner');
    const clearBtn = document.getElementById('searchClearBtn');

    if (!dropdown || !tips || !spinner || !clearBtn) return;

    const trendingSearches = ['Pain Relief', 'Cold Medicine', 'Baby Care'];

    // نبني داتا بسيطة من الكروت الموجودة على الصفحة (لو موجودة)
    const headerProducts = allProductCards.map(card => {
        const name = card.getAttribute('data-name') ||
            (card.querySelector('h3') && card.querySelector('h3').textContent) ||
            'Product';

        const descEl = card.querySelector('p');
        const desc = descEl ? descEl.textContent : '';

        let price = 0;
        const dp = card.getAttribute('data-price');
        if (dp) {
            price = parseFloat(dp);
        } else {
            const priceText = card.querySelector('.price')?.textContent || '';
            const m = priceText.replace(',', '.').match(/[\d.]+/);
            if (m) price = parseFloat(m[0]);
        }

        let oldPrice = null;
        const oldEl = card.querySelector('.old-price');
        if (oldEl) {
            const m2 = oldEl.textContent.replace(',', '.').match(/[\d.]+/);
            if (m2) oldPrice = parseFloat(m2[0]);
        }

        const imgEl = card.querySelector('img');
        const imageHtml = imgEl
            ? `<img src="${imgEl.getAttribute('src')}" alt="${escapeHtml(imgEl.getAttribute('alt') || name)}" />`
            : '💊';

        const id = card.getAttribute('data-product-id') || name;
        const inStock = !card.classList.contains('out-of-stock');

        const detailsAnchor = card.querySelector('a[href*="/Products/Details"]');
        const detailsUrl = detailsAnchor ? detailsAnchor.getAttribute('href') : null;

        return { id, name, description: desc, price, oldPrice, imageHtml, inStock, card, detailsUrl };
    });

    const hasProductsOnPage = headerProducts.length > 0;

    let query = '';
    let isFocused = false;
    let isLoading = false;
    let showResults = false;
    let selectedIndex = -1;
    let filtered = [];
    let timer = null;

    function setLoading(val) {
        isLoading = val;
        spinner.style.display = isLoading ? 'block' : 'none';
    }

    function setShowResults(val) {
        showResults = val;
        dropdown.style.display = showResults ? 'block' : 'none';
    }

    function updateClearBtn() {
        clearBtn.style.display = query ? 'block' : 'none';
    }

    function updateFocusStyles() {
        if (isFocused) {
            headerSearchBar.classList.add('searchbar-focused');
            tips.style.display = 'block';
        } else {
            headerSearchBar.classList.remove('searchbar-focused');
            tips.style.display = 'none';
        }
    }

    // ===== فلترة محلية + استدعاء SearchJson لو مفيش نتيجة =====
    async function filterProductsAsync() {
        const q = normalize(query.trim());
        if (!q) {
            filtered = [];
            return;
        }

        // 1) نجرب الأول من المنتجات الموجودة على الصفحة
        let local = [];
        if (hasProductsOnPage) {
            local = headerProducts.filter(p =>
                normalize(p.name).includes(q) ||
                normalize(p.description).includes(q)
            );
        }

        if (local.length > 0) {
            filtered = local.slice(0, 10);
            return;
        }

        // 2) لو مفيش نتيجة محلية → نجيب من الـ API /Home/SearchJson
        try {
            const resp = await fetch(`/Home/SearchJson?query=${encodeURIComponent(query)}`);
            if (!resp.ok) {
                filtered = [];
                return;
            }
            const data = await resp.json();
            filtered = data.map(p => {
                const card = allProductCards.find(c =>
                    c.getAttribute('data-product-id') === String(p.id));
                return {
                    id: p.id,
                    name: p.name,
                    description: p.description || '',
                    price: p.price,
                    oldPrice: p.oldPrice,
                    imageHtml: p.imageUrl
                        ? `<img src="${p.imageUrl}" alt="${escapeHtml(p.name)}" />`
                        : '💊',
                    inStock: true,
                    card: card || null,
                    detailsUrl: p.detailsUrl
                };
            }).slice(0, 10);
        } catch (e) {
            console.error('SearchJson error', e);
            filtered = [];
        }
    }

    function renderDropdown() {
        if (!showResults) {
            dropdown.innerHTML = '';
            dropdown.style.display = 'none';
            return;
        }

        // لا يوجد query → نعرض recent + trending
        if (!query.trim()) {
            const recent = getRecentSearches();
            dropdown.innerHTML = `
                <div class="search-suggestions">
                    <div class="search-suggestions-block">
                        <div class="search-suggestions-title">
                            <span>⏱</span>
                            <span>Recent Searches</span>
                        </div>
                        <div class="search-suggestions-chips">
                            ${recent.map(term => `
                                <button type="button"
                                            class="search-chip"
                                            data-term="${escapeHtml(term)}">
                                    ${escapeHtml(term)}
                                </button>
                            `).join('')}
                        </div>
                    </div>

                    <div class="search-suggestions-block">
                        <div class="search-suggestions-title">
                            <span>📈</span>
                            <span>Trending Now</span>
                        </div>
                        <div class="search-suggestions-chips">
                            ${trendingSearches.map(term => `
                                <button type="button"
                                            class="search-chip search-chip-trending"
                                            data-term="${escapeHtml(term)}">
                                    ${escapeHtml(term)}
                                </button>
                            `).join('')}
                        </div>
                    </div>
                </div>
            `;
            return;
        }

        if (isLoading) {
            dropdown.innerHTML = `
                <div class="search-dropdown-header">
                    Searching for "<strong>${escapeHtml(query)}</strong>"...
                </div>
            `;
            return;
        }

        if (filtered.length === 0) {
            dropdown.innerHTML = `
                <div class="search-results-empty">
                    <div class="search-results-empty-icon">🔍</div>
                    <div class="search-results-empty-title">No products found</div>
                    <div class="search-results-empty-text">Try searching for something else</div>
                </div>
            `;
            return;
        }

        const itemsHtml = filtered.map((p, index) => `
            <button type="button"
                    class="search-result-item ${index === selectedIndex ? 'search-result-selected' : ''}"
                    data-id="${escapeHtml(String(p.id))}"
                    data-index="${index}">
                <div class="search-result-image">
                    ${p.imageHtml}
                </div>
                <div class="search-result-main">
                    <div class="search-result-title-row">
                        <span class="search-result-name">${escapeHtml(p.name)}</span>
                        ${!p.inStock ? '<span class="search-result-badge">Out of stock</span>' : ''}
                    </div>
                    <div class="search-result-desc">
                        ${escapeHtml(p.description)}
                    </div>
                    <div class="search-result-price-row">
                        <span class="search-result-price">${formatPrice(p.price)}</span>
                        ${p.oldPrice != null ? `<span class="search-result-price-old">${formatPrice(p.oldPrice)}</span>` : ''}
                    </div>
                </div>
            </button>
        `).join('');

        dropdown.innerHTML = `
            <div class="search-dropdown-header">
                Found ${filtered.length} products
            </div>
            ${itemsHtml}
        `;
    }

    async function handleInput(val) {
        query = val || '';
        updateClearBtn();
        applyGlobalHeaderFilter(query);

        if (timer) clearTimeout(timer);

        if (!query.trim()) {
            setLoading(false);
            filtered = [];
            selectedIndex = -1;
            setShowResults(isFocused);
            renderDropdown();
            return;
        }

        setLoading(true);
        setShowResults(true);
        renderDropdown();

        timer = setTimeout(async function () {
            await filterProductsAsync();
            setLoading(false);
            selectedIndex = filtered.length ? 0 : -1;
            renderDropdown();
        }, 300);
    }

    function handleSelect(product) {
        if (!product) return;

        saveSearch(product.name);
        query = product.name;
        headerSearchInput.value = product.name;
        updateClearBtn();

        // لو في كارت ظاهر على الصفحة → فلتر + هايلايت + scroll
        if (product.card) {
            applyGlobalHeaderFilter(product.name);

            product.card.classList.add('search-highlight');
            const rect = product.card.getBoundingClientRect();
            const offset = window.scrollY + rect.top - 120;
            window.scrollTo({ top: offset, behavior: 'smooth' });
            setTimeout(() => {
                product.card.classList.remove('search-highlight');
            }, 1500);

            setShowResults(false);
            return;
        }

        // لو جاي من السيرفر ومفيش card في الصفحة → روح لصفحة الـ Details
        if (product.detailsUrl) {
            window.location.href = product.detailsUrl;
        } else if (product.id) {
            window.location.href = '/Products/Details/' + product.id;
        } else {
            setShowResults(false);
        }
    }

    headerSearchInput.addEventListener('input', function (e) {
        handleInput(e.target.value);
    });

    headerSearchInput.addEventListener('focus', function () {
        isFocused = true;
        updateFocusStyles();
        setShowResults(true);
        renderDropdown();
    });

    headerSearchInput.addEventListener('keydown', function (e) {
        if (!showResults) return;

        if (e.key === 'ArrowDown') {
            if (filtered.length === 0) return;
            e.preventDefault();
            e.stopImmediatePropagation();
            if (selectedIndex < filtered.length - 1) {
                selectedIndex++;
                renderDropdown();
            }
        } else if (e.key === 'ArrowUp') {
            if (filtered.length === 0) return;
            e.preventDefault();
            e.stopImmediatePropagation();
            if (selectedIndex > 0) {
                selectedIndex--;
                renderDropdown();
            } else {
                selectedIndex = -1;
                renderDropdown();
            }
        } else if (e.key === 'Enter') {
            if (selectedIndex >= 0 && filtered[selectedIndex]) {
                e.preventDefault();
                e.stopImmediatePropagation();
                handleSelect(filtered[selectedIndex]);
            }
        } else if (e.key === 'Escape') {
            e.preventDefault();
            e.stopImmediatePropagation();
            setShowResults(false);
            isFocused = false;
            updateFocusStyles();
        }
    });

    clearBtn.addEventListener('click', function () {
        query = '';
        headerSearchInput.value = '';
        updateClearBtn();
        applyGlobalHeaderFilter('');
        filtered = [];
        selectedIndex = -1;
        setShowResults(false);
    });

    dropdown.addEventListener('click', function (e) {
        const chip = e.target.closest('.search-chip');
        if (chip && chip.dataset.term) {
            const term = chip.dataset.term;
            headerSearchInput.value = term;
            headerSearchInput.focus();
            handleInput(term);
            return;
        }

        const item = e.target.closest('.search-result-item');
        if (item && item.dataset.index != null) {
            const idx = parseInt(item.dataset.index);
            const product = filtered[idx];
            handleSelect(product);
        }
    });

    document.addEventListener('mousedown', function (e) {
        if (headerSearchBar && !headerSearchBar.contains(e.target)) {
            isFocused = false;
            updateFocusStyles();
            setShowResults(false);
        }
    });

    // أول تحميل → نعرض كل المنتجات الموجودة على الصفحة (لو فيه)
    applyGlobalHeaderFilter('');
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

    const imgEl = card.querySelector('img');
    const imageUrl = imgEl ? imgEl.getAttribute('src') : 'fallback.png';

    addToCart(name, price, productId, imageUrl);
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
initHeaderSearch();

// ===== Categories horizontal scroll (Home categories strip) =====
const catStrip = document.getElementById('catStrip');
const catPrevBtn = document.querySelector('.cat-scroll-prev');
const catNextBtn = document.querySelector('.cat-scroll-next');

if (catStrip) {
    let step = 260;
    const firstCard = catStrip.querySelector('.cat-card');
    if (firstCard) {
        const rect = firstCard.getBoundingClientRect();
        step = rect.width + 16;
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

    // اختيار اللغة من العناصر جوه القائمة
    menu.addEventListener('click', function (e) {
        const btn = e.target.closest('.lang-option');
        if (!btn || !btn.dataset.lang) return;

        const lang = btn.dataset.lang;
        applyLanguage(lang);

        menu.classList.remove('show');
        toggle.setAttribute('aria-expanded', 'false');
    });

    // إغلاق القائمة عند الضغط خارجها
    document.addEventListener('click', function (e) {
        if (menu.classList.contains('show') &&
            !toggle.contains(e.target) &&
            !menu.contains(e.target)) {
            menu.classList.remove('show');
            toggle.setAttribute('aria-expanded', 'false');
        }
    });
})();
