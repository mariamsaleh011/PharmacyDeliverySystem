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
        closeConfirmBtn: document.getElementById('closeConfirmBtn'), // 👈 X بتاعة مودال الحذف

        toastContainer: document.getElementById('toastContainer'),

        // optional additional fields
        prodBarcode: document.getElementById('prodBarcode'),
        prodBrand: document.getElementById('prodBrand'),
        prodVat: document.getElementById('prodVat'),
        prodDosage: document.getElementById('prodDosage'),
        prodDrugType: document.getElementById('prodDrugType'),
        prodPharmId: document.getElementById('prodPharmId'),
    };

    const DEFAULT_IMG = '/images/image-placeholder.png';
    let itemToDeleteRow = null;

    function openModal(isEdit = false) {
        els.modalTitle.textContent = isEdit ? 'تعديل بيانات المنتج' : 'إضافة منتج جديد';
        els.modal.setAttribute('aria-hidden', 'false');
        document.body.style.overflow = 'hidden';

        if (els.form) {
            els.form.action = isEdit
                ? els.form.getAttribute('data-edit-url')
                : els.form.getAttribute('data-create-url');
        }
    }

    function closeModal() {
        els.modal.setAttribute('aria-hidden', 'true');
        document.body.style.overflow = '';
        setTimeout(() => {
            els.form.reset();
            els.id.value = '';
            resetImagePreview();
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
        els.id.value = '';
        openModal(false);
        resetImagePreview();
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

        // clear optional inputs
        if (els.prodBarcode) els.prodBarcode.value = '';
        if (els.prodBrand) els.prodBrand.value = '';
        if (els.prodVat) els.prodVat.value = '';
        if (els.prodDosage) els.prodDosage.value = '';
        if (els.prodDrugType) els.prodDrugType.value = '';
        if (els.prodPharmId) els.prodPharmId.value = '';

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
    els.closeConfirmBtn?.addEventListener('click', closeConfirmModal); // 👈 ربطنا الـ X

    els.confirmDeleteActionBtn?.addEventListener('click', () => {
        if (!els.deleteIdInput.value) return;
        els.confirmDeleteActionBtn.disabled = true;
        els.confirmDeleteActionBtn.textContent = 'جارٍ الحذف...';
        els.deleteForm.submit();
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

});
