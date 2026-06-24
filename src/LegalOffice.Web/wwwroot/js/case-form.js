window.initCaseAssignmentForm = function (config) {
    const countInput = document.getElementById(config.countInputId);
    const caseTypeSelect = document.getElementById(config.caseTypeSelectId);
    const container = document.getElementById(config.containerId);
    const accessLevels = config.accessLevels || [];
    const initialRows = config.initialRows || [];
    const lawyerSelectPlaceholder = config.lawyerSelectPlaceholder || 'اختر المحامي';
    const eligibleLawyersUrl = config.eligibleLawyersUrl;

    let lawyerOptions = config.lawyers || [];
    let rowState = initialRows.slice();

    function optionHtml(items, selectedValue, placeholder) {
        let html = placeholder ? `<option value="">${placeholder}</option>` : '';
        for (const item of items) {
            const selected = String(item.value) === String(selectedValue) ? ' selected' : '';
            html += `<option value="${item.value}"${selected}>${item.text}</option>`;
        }
        return html;
    }

    function captureRows() {
        const rows = [];
        const count = Math.max(1, parseInt(countInput.value || '1', 10));
        for (let i = 0; i < count; i++) {
            const lawyerSelect = container.querySelector(`select[name="LawyerAssignments[${i}].LawyerId"]`);
            const accessSelect = container.querySelector(`select[name="LawyerAssignments[${i}].AccessLevelId"]`);
            rows.push({
                lawyerId: lawyerSelect ? lawyerSelect.value : '',
                accessLevelId: accessSelect ? accessSelect.value : accessLevels[0]?.value || ''
            });
        }
        return rows;
    }

    function render() {
        const count = Math.max(1, parseInt(countInput.value || '1', 10));
        const rows = [];
        for (let i = 0; i < count; i++) {
            const row = rowState[i] || {};
            rows.push(`
                <div class="assignment-row">
                    <div>
                        <label class="form-label fw-semibold">المحامي رقم ${i + 1}</label>
                        <select name="LawyerAssignments[${i}].LawyerId" class="form-select" required>
                            ${optionHtml(lawyerOptions, row.lawyerId, lawyerSelectPlaceholder)}
                        </select>
                    </div>
                    <div>
                        <label class="form-label fw-semibold">الصلاحية</label>
                        <select name="LawyerAssignments[${i}].AccessLevelId" class="form-select" required>
                            ${optionHtml(accessLevels, row.accessLevelId || accessLevels[0]?.value, null)}
                        </select>
                    </div>
                    <div class="d-flex align-items-end justify-content-end h-100">
                        <span class="badge text-bg-light border">#${i + 1}</span>
                    </div>
                </div>`);
        }
        container.innerHTML = rows.join('');
    }

    async function loadLawyers(caseTypeId) {
        if (!eligibleLawyersUrl) {
            return;
        }

        const url = new URL(eligibleLawyersUrl, window.location.origin);
        if (caseTypeId) {
            url.searchParams.set('caseTypeId', caseTypeId);
        }

        const response = await fetch(url.toString(), { headers: { 'Accept': 'application/json' } });
        if (!response.ok) {
            return;
        }
        lawyerOptions = await response.json();
    }

    async function refreshLawyers() {
        rowState = captureRows();
        await loadLawyers(caseTypeSelect ? caseTypeSelect.value : null);
        render();
    }

    countInput.addEventListener('change', async () => {
        rowState = captureRows();
        render();
    });

    if (caseTypeSelect) {
        caseTypeSelect.addEventListener('change', refreshLawyers);
    }

    render();
    loadLawyers(caseTypeSelect ? caseTypeSelect.value : null).then(render);
};
