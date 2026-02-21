document.addEventListener('DOMContentLoaded', function () {
    const downloadForms = document.querySelectorAll('.download-form');
    const overlay = document.getElementById('overlay');
    const streamsCard = document.getElementById('streamsCard');

    downloadForms.forEach(form => {
        form.addEventListener('submit', async function (e) {
            e.preventDefault();

            overlay.style.display = 'block';
            downloadForms.forEach(f => {
                const btn = f.querySelector('button');
                if (btn) btn.disabled = true;
            });
            if (streamsCard) streamsCard.style.opacity = '0.5';

            try {
                const formData = new FormData(form);
                const response = await fetch(form.action, {
                    method: 'POST',
                    body: formData
                });

                if (!response.ok) {
                    const msg = await response.text();
                    alert('Erro no download: ' + msg);
                    return;
                }

                const blob = await response.blob();
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;

                const disposition = response.headers.get('Content-Disposition');
                const fileName = getFileNameFromDisposition(disposition);

                a.download = fileName;
                document.body.appendChild(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            } catch (err) {
                alert('Erro: ' + err.message);
            } finally {
                // Remove overlay
                overlay.style.display = 'none';
                downloadForms.forEach(f => {
                    const btn = f.querySelector('button');
                    if (btn) btn.disabled = false;
                });
                if (streamsCard) streamsCard.style.opacity = '1';
            }
        });
    });

    function getFileNameFromDisposition(disposition) {
        if (!disposition) return 'download';

        // Try UTF-8
        const utf8Match = disposition.match(/filename\*\=UTF-8''([^;]+)/);
        if (utf8Match) return decodeURIComponent(utf8Match[1]);

        // Fallback
        const asciiMatch = disposition.match(/filename="([^"]+)"/);
        if (asciiMatch) return asciiMatch[1];

        return 'download';
    }
});