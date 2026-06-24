window.initCaseTypeDependentSelect = function (config) {
    const sourceSelect = document.getElementById(config.sourceSelectId);
    const targetSelect = document.getElementById(config.targetSelectId);
    const endpoint = config.endpoint;
    const placeholder = config.placeholder || 'اختر';
    const paramName = config.paramName || 'caseTypeId';

    if (!targetSelect || !endpoint) {
        return;
    }

    function setOptions(items, selectedValue) {
        let html = `<option value="">${placeholder}</option>`;
        for (const item of items || []) {
            const selected = String(item.value) === String(selectedValue) ? ' selected' : '';
            html += `<option value="${item.value}"${selected}>${item.text}</option>`;
        }
        targetSelect.innerHTML = html;
    }

    async function load() {
        const current = targetSelect.value;
        const url = new URL(endpoint, window.location.origin);
        if (sourceSelect && sourceSelect.value) {
            url.searchParams.set(paramName, sourceSelect.value);
        }

        try {
            const response = await fetch(url.toString(), { headers: { 'Accept': 'application/json' } });
            if (!response.ok) {
                return;
            }
            const items = await response.json();
            setOptions(items, current);
        } catch {
            // silent fallback
        }
    }

    if (sourceSelect) {
        sourceSelect.addEventListener('change', load);
    }

    load();
};
