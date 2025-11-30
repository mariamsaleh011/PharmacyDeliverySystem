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
const searchInput = document.getElementById('searchInput');
const langSelect = document.getElementById('langSelect');
const toTopBtn = document.getElementById('toTop');

// ===== Language system =====
let currentLang = localStorage.getItem(LANG_KEY) || 'en';

const translations = {
    en: {
        lang_ar: "AR",
        lang_en: "EN",

        nav_home: "Home",
        nav_drugs: "Drugs",
        nav_baby: "Baby Care",
        nav_men: "Men Care",
        nav_chat: "Chat",
        nav_privacy: "Privacy",
        nav_cart_label: "Cart",

        loginButton: "Login",
        themeToggle: "Theme",
        searchPlaceholder: "Search products & bundles...",

        footer_text: "© 2025 PharmacyMarts - All rights reserved",

        hero_title: "The all-in-one platform for all your pharmacy needs",
        hero_subtitle: "Browse and order what you need easily, and we deliver it quickly",
        hero_cta: "Order Now",

        categories_title: "Shop by Category",
        cat_drugs: "Drugs",
        cat_baby: "Baby Care",
        cat_men: "Men Care",

        offers_title: "Special Offers",
        btn_add_to_cart: "Add to Cart",

        // Home offers
        offer_nivea_cream_title: "Nivea Cream Offer",
        offer_nivea_cream_desc: "30% off Nivea moisturizing cream",

        offer_johnson_shampoo_title: "Johnson's Shampoo Offer",
        offer_johnson_shampoo_desc: "Buy 2 Johnson's shampoos and get the 3rd free",

        offer_pampers_title: "Pampers Diapers Discount",
        offer_pampers_desc: "20% discount on all Pampers sizes",

        offer_dove_title: "Dove Cream Offer",
        offer_dove_desc: "25% off Dove cream for skin brightening",

        offer_loreal_men_title: "L'Oréal Men Face Wash",
        offer_loreal_men_desc: "40% off L'Oréal men facial wash",

        offer_baby_bundle_title: "Baby Essentials Bundle",
        offer_baby_bundle_desc: "35% off baby care products bundle",

        offer_sebamed_title: "Sebamed Cleanser Offer",
        offer_sebamed_desc: "25% off Sebamed facial cleanser for all skin types",

        offer_colgate_title: "Colgate Toothpaste Offer",
        offer_colgate_desc: "Buy 2 Colgate and get the 3rd free",

        offer_nivea_deo_title: "Nivea Deodorant Offer",
        offer_nivea_deo_desc: "30% off Nivea deodorants for men and women",

        offer_loreal_sun_title: "L'Oréal Sunscreen Offer",
        offer_loreal_sun_desc: "40% off all L'Oréal sunscreen types",

        offer_johnson_oil_title: "Johnson's Baby Oil Offer",
        offer_johnson_oil_desc: "20% off Johnson's oil for all ages",

        offer_head_shoulders_title: "Head & Shoulders Shampoo Offer",
        offer_head_shoulders_desc: "30% off Head & Shoulders anti-dandruff shampoo",

        health_title: "💡 Health Tips",
        health_tip_cta: "Tip of the day",

        contact_title: "Contact Us",
        contact_subtitle: "You can reach us through any of the following channels:",
        contact_email: "Email",
        contact_facebook: "Facebook",
        contact_instagram: "Instagram",
        contact_whatsapp: "WhatsApp",

        // Drugs page
        drugs_title: "Available Drugs",

        drug_paracetamol_title: "Paracetamol 1000 mg",
        drug_paracetamol_desc: "Pain reliever for headache and body aches.",
        drug_paracetamol_price: "Price: 25 EGP",

        drug_congestal_title: "Congestal",
        drug_congestal_desc: "For cold and flu relief.",
        drug_congestal_price: "Price: 35 EGP",

        drug_augmentin_title: "Augmentin",
        drug_augmentin_desc: "Broad-spectrum antibiotic.",
        drug_augmentin_price: "Price: 80 EGP",

        drug_panadol_title: "Panadol",
        drug_panadol_desc: "Pain reliever and fever reducer.",
        drug_panadol_price: "Price: 30 EGP",

        drug_cetal_title: "Cetal Syrup",
        drug_cetal_desc: "Helps relieve fever and cold symptoms.",
        drug_cetal_price: "Price: 28 EGP",

        drug_flurest_title: "Flurest",
        drug_flurest_desc: "Treats nasal congestion and cold.",
        drug_flurest_price: "Price: 32 EGP",

        // Chat (New)
        chat_title: "Chat with the Pharmacist",
        chat_subtitle: "Ask about your medication dosage or any quick question.",
        chat_pharmacist_initial: "Hello! I'm the pharmacist 😊 How can I help you today?",
        chat_placeholder: "Type your message here...",
        chat_send_btn: "Send",
        chat_no_file: "No file selected",
        chat_typing: "Pharmacist is typing…",

        cart_title: "Shopping Cart",
        cart_total_label: "Total:",
        cart_checkout: "Checkout",
        cart_clear: "Clear Cart",
        cart_empty: "Your cart is empty",
        cart_empty_alert: "The cart is empty",
        cart_success_alert: "Your order has been placed. We will contact you shortly to confirm.",

        // BabyCare + MenCare
        babycare_title: "Baby Care Products",
        baby_johnsons_lotion_title: "Johnson's Baby Soft Lotion",
        baby_johnsons_lotion_desc: "Gentle moisturizing suitable for babies' sensitive skin.",
        baby_johnsons_lotion_price: "Price: 75 EGP",

        baby_prima_diapers_title: "Prima Baby Diapers",
        baby_prima_diapers_desc: "High-absorption diapers for sensitive baby skin.",
        baby_prima_diapers_price: "Price: 190 EGP",

        baby_sebamed_lotion_title: "Sebamed Baby Lotion",
        baby_sebamed_lotion_desc: "Gentle moisturizer free from harmful ingredients.",
        baby_sebamed_lotion_price: "Price: 140 EGP",

        baby_sudocrem_title: "Sudocrem",
        baby_sudocrem_desc: "Effective treatment for diaper rash and skin irritation.",
        baby_sudocrem_price: "Price: 115 EGP",

        baby_mustela_oil_title: "Mustela Baby Oil",
        baby_mustela_oil_desc: "Natural oil for baby skin care.",
        baby_mustela_oil_price: "Price: 160 EGP",

        baby_chicco_brush_title: "Chicco Baby Toothbrush",
        baby_chicco_brush_desc: "Safe and suitable toothbrush for babies.",
        baby_chicco_brush_price: "Price: 60 EGP",

        men_title: "Men Care Products",

        men_nivea_gel_title: "Nivea Men Shaving Gel",
        men_nivea_gel_desc: "Nivea shaving gel for smooth skin while shaving.",
        men_nivea_gel_price: "Price: 150 EGP",

        men_gillette_title: "Gillette Fusion5 Blades",
        men_gillette_desc: "Fusion5 razor blades for high precision and smoothness.",
        men_gillette_price: "Price: 220 EGP",

        men_loreal_oil_title: "L'Oréal Men Expert Barber Club Oil",
        men_loreal_oil_desc: "Beard and face oil from L'Oréal to nourish hair and skin.",
        men_loreal_oil_price: "Price: 250 EGP",

        men_xtend_oil_title: "Xtend Beard Oil",
        men_xtend_oil_desc: "Oil to soften and moisturize beard and prevent split ends.",
        men_xtend_oil_price: "Price: 180 EGP",

        men_oldspice_title: "Old Spice Original Deo Stick",
        men_oldspice_desc: "Old Spice deodorant with long-lasting strong scent.",
        men_oldspice_price: "Price: 130 EGP",

        men_head_shoulders_title: "Head & Shoulders Classic Clean",
        men_head_shoulders_desc: "Head & Shoulders anti-dandruff shampoo for scalp cleansing.",
        men_head_shoulders_price: "Price: 115 EGP",

        scroll_top_title: "Back to top"
    },
    ar: {
        lang_ar: "ع",
        lang_en: "EN",

        nav_home: "الرئيسية",
        nav_drugs: "الأدوية",
        nav_baby: "عناية الأطفال",
        nav_men: "عناية الرجال",
        nav_chat: "الدردشة",
        nav_privacy: "الخصوصية",
        nav_cart_label: "السلة",

        loginButton: "دخول لحسابك",
        themeToggle: "الوضع",
        searchPlaceholder: "ابحث في الباقات والمنتجات...",

        footer_text: "© 2025 صيدلية PharmacyMarts - جميع الحقوق محفوظة",

        hero_title: "المنصة الشاملة لتوريد كل احتياجات الصيدلية بتاعتك",
        hero_subtitle: "دور واطلب اللي محتاجه بسهولة، واحنا هنوصّلهولك بسرعة",
        hero_cta: "اطلب الآن",

        categories_title: "تسوق حسب الفئة",
        cat_drugs: "الأدوية",
        cat_baby: "عناية الأطفال",
        cat_men: "عناية الرجال",

        offers_title: "العروض الخاصة",
        btn_add_to_cart: "أضف للسلة",

        offer_nivea_cream_title: "عرض نيفيا كريم",
        offer_nivea_cream_desc: "خصم 30% على كريم نيفيا المرطب",

        offer_johnson_shampoo_title: "عرض شامبو جونسون",
        offer_johnson_shampoo_desc: "اشتري 2 واحصل على الثالث مجاناً",

        offer_pampers_title: "خصم على حفاضات بامبرز",
        offer_pampers_desc: "خصم 20% على جميع مقاسات بامبرز",

        offer_dove_title: "عرض كريم دوف",
        offer_dove_desc: "خصم 25% على كريم دوف لتفتيح البشرة",

        offer_loreal_men_title: "غسول لوريال للرجال",
        offer_loreal_men_desc: "خصم 40% على غسول الوجه لوريال للرجال",

        offer_baby_bundle_title: "مستلزمات الأطفال",
        offer_baby_bundle_desc: "خصم 35% على منتجات العناية بالأطفال",

        offer_sebamed_title: "عرض غسول سيباميد",
        offer_sebamed_desc: "خصم 25% على غسول الوجه من سيباميد لجميع أنواع البشرة",

        offer_colgate_title: "معجون كولجيت",
        offer_colgate_desc: "اشتري عبوتين واحصل على الثالثة مجاناً من كولجيت",

        offer_nivea_deo_title: "عرض ديودرنت نيفيا",
        offer_nivea_deo_desc: "خصم 30% على مزيلات العرق من نيفيا للرجال والنساء",

        offer_loreal_sun_title: "واقي الشمس لوريال",
        offer_loreal_sun_desc: "خصم 40% على جميع أنواع واقي الشمس من لوريال",

        offer_johnson_oil_title: "زيت الأطفال جونسون",
        offer_johnson_oil_desc: "خصم 20% على زيت جونسون لجميع الأعمار",

        offer_head_shoulders_title: "عرض شامبو هيد آند شولدرز",
        offer_head_shoulders_desc: "خصم 30% على شامبو القشرة من هيد آند شولدرز",

        health_title: "💡 نصائح صحية",
        health_tip_cta: "نصيحة اليوم",

        contact_title: "تواصل معنا",
        contact_subtitle: "يمكنك التواصل معنا عبر أي من القنوات التالية:",
        contact_email: "البريد الإلكتروني",
        contact_facebook: "فيسبوك",
        contact_instagram: "انستجرام",
        contact_whatsapp: "واتساب",

        // Drugs
        drugs_title: "الأدوية المتاحة",

        drug_paracetamol_title: "باراسيتامول 1000 مجم",
        drug_paracetamol_desc: "مسكن للصداع وآلام الجسم.",
        drug_paracetamol_price: "السعر: 25 جنيه",

        drug_congestal_title: "كونجستال",
        drug_congestal_desc: "لعلاج نزلات البرد والإنفلونزا.",
        drug_congestal_price: "السعر: 35 جنيه",

        drug_augmentin_title: "أوجمنتين",
        drug_augmentin_desc: "مضاد حيوي واسع المدى.",
        drug_augmentin_price: "السعر: 80 جنيه",

        drug_panadol_title: "بانادول",
        drug_panadol_desc: "مسكن وخافض للحرارة.",
        drug_panadol_price: "السعر: 30 جنيه",

        drug_cetal_title: "سيتال شراب",
        drug_cetal_desc: "لتخفيف آلام الحمى والبرد.",
        drug_cetal_price: "السعر: 28 جنيه",

        drug_flurest_title: "فلورست",
        drug_flurest_desc: "علاج احتقان الأنف ونزلات البرد.",
        drug_flurest_price: "السعر: 32 جنيه",

        // Chat
        chat_title: "تحدث مع الصيدلي",
        chat_subtitle: "اسأل عن الدواء، الجرعة أو أي استفسار صحي بسيط.",
        chat_pharmacist_initial: "مرحبًا! أنا الصيدلي 😊 كيف يمكنني مساعدتك اليوم؟",
        chat_placeholder: "اكتب رسالتك هنا...",
        chat_send_btn: "إرسال",
        chat_no_file: "لا يوجد ملف مرفوع",
        chat_typing: "الصيدلي يكتب الآن…",

        cart_title: "سلة المشتريات",
        cart_total_label: "الإجمالي:",
        cart_checkout: "إتمام الشراء",
        cart_clear: "تفريغ السلة",
        cart_empty: "السلة فاضية",
        cart_empty_alert: "السلة فاضية",
        cart_success_alert: "تم تسجيل طلبك وسيتم التواصل معك للتأكيد.",

        // BabyCare+MenCare Pages
        babycare_title: "منتجات عناية الأطفال",
        baby_johnsons_lotion_title: "Johnson's Baby Soft Lotion",
        baby_johnsons_lotion_desc: "ترطيب لطيف ومناسب لبشرة الأطفال الحساسة.",
        baby_johnsons_lotion_price: "السعر: 75 جنيه",

        baby_prima_diapers_title: "Prima Baby Diapers",
        baby_prima_diapers_desc: "حفاضات عالية الامتصاص لبشرة الطفل الحساسة.",
        baby_prima_diapers_price: "السعر: 190 جنيه",

        baby_sebamed_lotion_title: "Sebamed Baby Lotion",
        baby_sebamed_lotion_desc: "مرطب لطيف وخالي من المواد الضارة.",
        baby_sebamed_lotion_price: "السعر: 140 جنيه",

        baby_sudocrem_title: "Sudocrem",
        baby_sudocrem_desc: "علاج فعّال لالتهابات الحفاضات وتهيّجات البشرة.",
        baby_sudocrem_price: "السعر: 115 جنيه",

        baby_mustela_oil_title: "Mustela Baby Oil",
        baby_mustela_oil_desc: "زيت طبيعي للعناية ببشرة الأطفال.",
        baby_mustela_oil_price: "السعر: 160 جنيه",

        baby_chicco_brush_title: "Chicco Baby Toothbrush",
        baby_chicco_brush_desc: "فرشاة أسنان آمنة ومناسبة للأطفال.",
        baby_chicco_brush_price: "السعر: 60 جنيه",

        men_title: "منتجات العناية بالرجال",

        men_nivea_gel_title: "Nivea Men Shaving Gel",
        men_nivea_gel_desc: "جيل حلاقة نيفيا للرجال لنعومة البشرة أثناء الحلاقة.",
        men_nivea_gel_price: "السعر: 150 جنيه",

        men_gillette_title: "Gillette Fusion5 Blades",
        men_gillette_desc: "شفرات حلاقة جيليت فيوجن 5 لنعومة ودقة عالية.",
        men_gillette_price: "السعر: 220 جنيه",

        men_loreal_oil_title: "L'Oréal Men Expert Barber Club Oil",
        men_loreal_oil_desc: "زيت للّحية والوجه من لوريال لتغذية الشعر والبشرة.",
        men_loreal_oil_price: "السعر: 250 جنيه",

        men_xtend_oil_title: "Xtend Beard Oil",
        men_xtend_oil_desc: "زيت لتنعيم وترطيب اللحية ومنع التقصف.",
        men_xtend_oil_price: "السعر: 180 جنيه",

        men_oldspice_title: "Old Spice Original Deo Stick",
        men_oldspice_desc: "مزيل عرق أولد سبايس برائحة قوية تدوم طويلاً.",
        men_oldspice_price: "السعر: 130 جنيه",

        men_head_shoulders_title: "Head & Shoulders Classic Clean",
        men_head_shoulders_desc: "شامبو هيد أند شولدرز للقشرة وتنظيف فروة الرأس.",
        men_head_shoulders_price: "السعر: 115 جنيه",

        scroll_top_title: "أعلى الصفحة"
    }
};

function applyLanguage(lang) {
    currentLang = lang;
    const dict = translations[lang] || translations.en;
    window.translations = translations;
    window.currentLang = currentLang;


    document.querySelectorAll('[data-lang-key]').forEach(el => {
        const key = el.getAttribute('data-lang-key');
        const value = dict[key];
        if (!value) return;

        if (el.tagName === 'INPUT' || el.tagName === 'TEXTAREA') {
            el.placeholder = value;
        } else {
            el.textContent = value;
        }
    });

    document.documentElement.lang = lang === 'ar' ? 'ar' : 'en';
    document.documentElement.dir = lang === 'ar' ? 'rtl' : 'ltr';

    if (langSelect) {
        langSelect.value = lang;
    }

    if (toTopBtn) {
        toTopBtn.title = dict.scroll_top_title;
    }

    localStorage.setItem(LANG_KEY, lang);
}

if (!localStorage.getItem(LANG_KEY)) {
    currentLang = 'en';
}
applyLanguage(currentLang);

if (langSelect) {
    langSelect.addEventListener('change', (e) => {
        applyLanguage(e.target.value);
    });
}

// ===== Theme system =====
function setTheme(theme) {
    document.body.setAttribute('data-theme', theme);
    localStorage.setItem(THEME_KEY, theme);
    if (themeToggle) {
        themeToggle.textContent = theme === 'dark' ? '☀' : '☾';
        themeToggle.title = translations[currentLang]?.themeToggle || 'Theme';
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
    // نحاول نحوله لرقم، ولو طلع null/undefined/NaN نخليه 0
    const num = Number(v) || 0;
    return num.toFixed(2) + ' ج.م';
}


function renderCart() {
    const items = readCart();
    if (!cartList || !cartTotal || !cartCount) return;

    if (items.length === 0) {
        const emptyText = translations[currentLang]?.cart_empty || translations.en.cart_empty;
        cartList.innerHTML = `<p class="empty-cart-text">${emptyText}</p>`;
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
    const shouldOpen = (open === undefined) ? !drawer.classList.contains('open') : open;
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
async function checkout() {
    const items = readCart();
    if (!items.length) {
        const msg = translations[currentLang]?.cart_empty_alert || translations.en.cart_empty_alert;
        alert(msg);
        return;
    }

    try {
        // حول الـ items لصيغة مناسبة للـ backend
        const cartItems = items.map(item => ({
            ProductId: getProductIdByName(item.name), // محتاجين نجيب الـ ID
            Name: item.name,
            Price: item.price,
            Qty: item.qty
        }));

        // ابعت للـ backend
        const response = await fetch('/Product/Checkout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
            },
            body: JSON.stringify(cartItems)
        });

        const result = await response.json();

        if (result.success) {
            const okMsg = translations[currentLang]?.cart_success_alert || translations.en.cart_success_alert;
            alert(okMsg);
            clearCart();
            toggleCart(false);
        } else {
            alert(result.message || 'حدث خطأ أثناء الطلب');
        }
    } catch (error) {
        console.error('Checkout error:', error);
        alert('حدث خطأ في الاتصال بالسيرفر');
    }
}

// ===== Scroll to top =====
window.addEventListener('scroll', () => {
    if (!toTopBtn) return;
    if (window.scrollY > 0) {
        toTopBtn.style.display = 'block';
    } else {
        toTopBtn.style.display = 'none';
    }
});

function scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
}
window.scrollToTop = scrollToTop;

if (toTopBtn) {
    toTopBtn.addEventListener('click', scrollToTop);
}

// ===== Search (فلترة المنتجات) =====
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

// ===== زرار "اطلب الآن" في الهوم =====
function orderNow() {
    const section = document.getElementById('offers') || document.querySelector('.offers-section');
    if (section) {
        window.scrollTo({ top: section.offsetTop - 80, behavior: 'smooth' });
    }
}
window.orderNow = orderNow;

// ===== Add to cart buttons =====
// ===== Add to cart buttons (event delegation) =====
document.addEventListener('click', function (e) {
    const btn = e.target.closest('.add-to-cart');
    if (!btn) return;

    const card = btn.closest('.product');
    if (!card) return;

    const name = card.getAttribute('data-name') || 'Product';
    const productId = parseInt(card.getAttribute('data-product-id')) || 0; // 👈 جديد

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

    console.log('ADD TO CART:', { productId, name, price });
    addToCart(name, price, productId); // 👈 مررنا الـ productId
});


// ===== Health tips (هوم فقط) =====
// ===== Health tips (هوم فقط) =====
const healthTips = [
    { icon: "💧", text: "اشرب 8 أكواب من الماء يوميًا للحفاظ على رطوبة جسمك" },
    { icon: "🛌", text: "حافظ على نوم منتظم لا يقل عن 7 ساعات يوميًا" },
    { icon: "🥦", text: "تناول فواكه وخضروات طازجة يوميًا" },
    { icon: "🏃‍♂️", text: "مارس نشاط بدني مثل المشي 30 دقيقة يوميًا" },
    { icon: "😌", text: "خصص وقتًا للاسترخاء وإدارة التوتر" },
    { icon: "💊", text: "تناول الفيتامينات والمكملات حسب نصيحة طبيبك" },
    { icon: "🦠", text: "احرص على غسل اليدين والوقاية من الأمراض الموسمية" }
];

const dailyTipBtn = document.getElementById('dailyTipBtn');
const dailyTip = document.getElementById('dailyTip');

if (dailyTipBtn && dailyTip) {
    dailyTipBtn.addEventListener('click', () => {
        const tip = healthTips[Math.floor(Math.random() * healthTips.length)];
        dailyTip.textContent = tip.icon + " " + tip.text;
    });
}


// ===== Helper: جيب الـ product ID من الاسم =====
function getProductIdByName(name) {
    const card = Array.from(document.querySelectorAll('.product'))
        .find(p => p.getAttribute('data-name') === name);
    return card ? parseInt(card.getAttribute('data-product-id')) : 0;
}

// ===== Initial init =====
renderCart();
applyFilters();
// ===== Initial init =====
renderCart();
applyFilters();
