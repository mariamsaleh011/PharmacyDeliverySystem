// product-admin.js — Classic postback version (no fetch / no JSON)
document.addEventListener('DOMContentLoaded', () => {

    const els = {
        btnAdd: document.getElementById('btnAddProduct'),
        modal: document.getElementById('productModal'),
        form: document.getElementById('productForm'),
        btnCancel: document.getElementById('cancelProductBtn'),
        btnSave: document.getElementById('saveProductBtn'),
        table: document.getElementById('productsTable'),
        search: document.getElementById('prodSearch'),
        id: document.getElementById('prodId'),
        name: document.getElementById('prodName'),
        desc: document.getElementById('prodDesc'),
        price: document.getElementById('prodPrice'),
        oldPrice: document.getElementById('prodOldPrice'),
        qty: document.getElementById('prodQty'),
        active: document.getElementById('prodActive'),
        imageInput: document.getElementById('prodImage'),
        imagePreview: document.getElementById('imagePreview'),
        modalTitle: document.getElementById('modalTitle'),

        // delete modal
        confirmModal: document.getElementById('confirmModal'),
        cancelConfirmBtn: document.getElementById('cancelConfirmBtn'),
        confirmDeleteActionBtn: document.getElementById('confirmDeleteActionBtn'),
        deleteItemName: document.getElementById('deleteItemName'),
        deleteForm: document.getElementById('deleteForm'),
        deleteIdInput: document.getElementById('deleteId'),
        closeConfirmBtn: document.getElementById('closeConfirmBtn'), // X بتاعة مودال الحذف

        toastContainer: document.getElementById('toastContainer'),

        // optional additional fields
        prodBarcode: document.getElementById('prodBarcode'),
        prodBrand: document.getElementById('prodBrand'),
        prodVat: document.getElementById('prodVat'),
        prodDosage: document.getElementById('prodDosage'),
        prodDrugType: document.getElementById('prodDrugType'), // hidden input
        prodPharmId: document.getElementById('prodPharmId'),
    };

    // ✅ Controls for DrugType custom select
    const drugTypeControls = {
        select: document.getElementById('drugTypeSelect'),
        trigger: document.getElementById('drugTypeTrigger'),
        optionsBox: document.getElementById('drugTypeOptions'),
        options: document.querySelectorAll('#drugTypeOptions .select-option')
    };

    // helpers for DrugType UI
    function resetDrugTypeSelect() {
        if (!drugTypeControls.trigger || !els.prodDrugType) return;

        drugTypeControls.trigger.innerHTML = `
            <span class="select-placeholder">-- Select drug type --</span>
            <span class="arrow">▲</span>
        `;
        els.prodDrugType.value = '';
        drugTypeControls.options.forEach(o => o.classList.remove('selected'));
    }

    function setDrugTypeFromValue(value) {
        if (!drugTypeControls.trigger || !els.prodDrugType) return;
        if (!value) {
            resetDrugTypeSelect();
            return;
        }

        const opt = document.querySelector(`#drugTypeOptions .select-option[data-value="${value}"]`);
        if (!opt) {
            resetDrugTypeSelect();
            return;
        }

        const icon = opt.querySelector('.option-icon')?.textContent || '';
        const mainText = opt.querySelector('.option-main')?.textContent || value;

        drugTypeControls.trigger.innerHTML = `
            <span style="display:flex;align-items:center;gap:10px;">
                <span style="font-size:20px;">${icon}</span>
                <span>${mainText}</span>
            </span>
            <span class="arrow">▲</span>
        `;

        els.prodDrugType.value = value;
        drugTypeControls.options.forEach(o => o.classList.remove('selected'));
        opt.classList.add('selected');
    }

    // الأكشن الافتراضي للفورم (/Admin/Create)
    const DEFAULT_ACTION = els.form ? els.form.getAttribute('action') : '';

    const DEFAULT_IMG = '/images/image-placeholder.png';
    let itemToDeleteRow = null;

    function openModal(isEdit = false) {
        els.modalTitle.textContent = isEdit ? 'تعديل بيانات المنتج' : 'إضافة منتج جديد';
        els.modal.setAttribute('aria-hidden', 'false');
        document.body.style.overflow = 'hidden';

        if (!els.form) return;

        if (isEdit) {
            // Edit → POST إلى /Admin/Edit
            els.form.action = els.form.getAttribute('data-edit-url');
        } else {
            // Create → رجّع الأكشن الافتراضي /Admin/Create
            els.form.action = DEFAULT_ACTION;
            if (els.id) els.id.value = '';
            resetDrugTypeSelect(); // new product → reset drug type UI
        }
    }

    function closeModal() {
        els.modal.setAttribute('aria-hidden', 'true');
        document.body.style.overflow = '';
        setTimeout(() => {
            els.form.reset();
            els.id.value = '';
            resetImagePreview();
            resetDrugTypeSelect(); // reset UI + hidden input
            // رجّع زرار الحفظ لحالته الطبيعية
            if (els.btnSave) {
                els.btnSave.disabled = false;
                els.btnSave.textContent = 'Save changes';
            }
        }, 200);
    }

    function resetImagePreview() {
        if (els.imagePreview) {
            els.imagePreview.src = DEFAULT_IMG;
            els.imagePreview.style.display = 'block';
        }
    }

    // preview image
    els.imageInput?.addEventListener('change', function () {
        const preview = els.imagePreview;
        if (!preview) return;

        if (this.files && this.files[0]) {
            const reader = new FileReader();
            reader.onload = (e) => {
                preview.src = e.target.result;
                preview.style.display = 'block';
            };
            reader.readAsDataURL(this.files[0]);
        } else {
            resetImagePreview();
        }
    });

    // open create
    els.btnAdd?.addEventListener('click', () => {
        if (els.id) els.id.value = '';
        openModal(false);
        resetImagePreview();
        resetDrugTypeSelect();
    });

    els.btnCancel?.addEventListener('click', closeModal);
    document.getElementById('closeModalBtn')?.addEventListener('click', closeModal);

    window.addEventListener('click', (e) => {
        if (e.target === els.modal) closeModal();
        if (e.target === els.confirmModal) closeConfirmModal();
    });

    function populateForm(row) {
        els.id.value = row.getAttribute('data-id') || '';
        els.name.value = row.querySelector('.prod-name')?.textContent?.trim() || '';
        els.desc.value = row.querySelector('.prod-desc-full')?.textContent?.trim() || '';

        const priceTxt = row.querySelector('.current-price')?.textContent || '0';
        els.price.value = parseFloat(priceTxt.replace(/[^\d.]/g, '')) || 0;

        const oldPriceTxt = row.querySelector('.old-price')?.textContent;
        els.oldPrice.value = oldPriceTxt
            ? (parseFloat(oldPriceTxt.replace(/[^\d.]/g, '')) || '')
            : '';

        els.qty.value = row.querySelector('.prod-qty')?.textContent?.trim() || 0;

        const active = row.getAttribute('data-active');
        if (els.active && active !== null) {
            els.active.value = active;
        }

        const img = row.querySelector('.prod-thumb');
        if (img && img.getAttribute('src')) {
            els.imagePreview.src = img.getAttribute('src');
            els.imagePreview.style.display = 'block';
        } else {
            resetImagePreview();
        }

        // optional inputs (لسه فاضية لحد ما تزودها من الداتا لو حابب)
        if (els.prodBarcode) els.prodBarcode.value = '';
        if (els.prodBrand) els.prodBrand.value = '';
        if (els.prodVat) els.prodVat.value = '';
        if (els.prodDosage) els.prodDosage.value = '';
        if (els.prodPharmId) els.prodPharmId.value = '';

        // ✅ DrugType من الـ data-attribute لو موجود
        const drugTypeValue = row.getAttribute('data-drugtype') || '';
        setDrugTypeFromValue(drugTypeValue);

        // Edit → POST إلى /Admin/Edit
        els.form.action = els.form.getAttribute('data-edit-url');
    }

    els.table?.addEventListener('click', (e) => {
        const btnEdit = e.target.closest('.edit-btn');
        const btnDelete = e.target.closest('.delete-btn');

        if (btnEdit) {
            const row = btnEdit.closest('tr');
            populateForm(row);
            openModal(true);
        }

        if (btnDelete) {
            const row = btnDelete.closest('tr');
            const id = row.getAttribute('data-id');
            const name = row.querySelector('.prod-name')?.textContent || '';
            els.deleteItemName.textContent = name;
            els.deleteIdInput.value = id;
            itemToDeleteRow = row;
            els.confirmModal.setAttribute('aria-hidden', 'false');
        }
    });

    function closeConfirmModal() {
        els.confirmModal.setAttribute('aria-hidden', 'true');
        els.deleteIdInput.value = '';
        itemToDeleteRow = null;
        els.confirmDeleteActionBtn.disabled = false;
        els.confirmDeleteActionBtn.textContent = 'Yes, delete';
    }

    els.cancelConfirmBtn?.addEventListener('click', closeConfirmModal);
    els.closeConfirmBtn?.addEventListener('click', closeConfirmModal); // X

    els.confirmDeleteActionBtn?.addEventListener('click', () => {
        if (!els.deleteIdInput.value) return;
        els.confirmDeleteActionBtn.disabled = true;
        els.confirmDeleteActionBtn.textContent = 'جارٍ الحذف...';
        els.deleteForm.submit(); // POST Admin/Delete
    });

    // disable double submit
    els.form?.addEventListener('submit', () => {
        if (els.btnSave) {
            els.btnSave.disabled = true;
            els.btnSave.textContent = 'جارٍ الحفظ...';
        }
    });

    // client-side search
    els.search?.addEventListener('input', (e) => {
        const q = e.target.value.toLowerCase().trim();
        document.querySelectorAll('#productsTable tbody tr').forEach(row => {
            if (row.classList.contains('empty-state')) return;
            const text = row.textContent.toLowerCase();
            row.style.display = text.includes(q) ? '' : 'none';
        });
    });

    function showToast(msg, type = 'info') {
        if (!els.toastContainer) return;
        const t = document.createElement('div');
        t.className = `toast ${type}`;
        t.textContent = msg;
        els.toastContainer.appendChild(t);
        setTimeout(() => t.remove(), 4000);
    }

    // ✅ DrugType select events
    if (drugTypeControls.trigger && drugTypeControls.optionsBox) {
        drugTypeControls.trigger.addEventListener('click', (e) => {
            e.stopPropagation();
            drugTypeControls.optionsBox.classList.toggle('show');
            drugTypeControls.trigger.classList.toggle('active');
        });

        drugTypeControls.options.forEach(opt => {
            opt.addEventListener('click', (e) => {
                e.stopPropagation();
                const value = opt.getAttribute('data-value');
                setDrugTypeFromValue(value);
                drugTypeControls.optionsBox.classList.remove('show');
                drugTypeControls.trigger.classList.remove('active');
            });
        });

        // close when clicking outside
        document.addEventListener('click', (e) => {
            if (!e.target.closest('#drugTypeSelect')) {
                drugTypeControls.optionsBox.classList.remove('show');
                drugTypeControls.trigger.classList.remove('active');
            }
        });
    }

});
