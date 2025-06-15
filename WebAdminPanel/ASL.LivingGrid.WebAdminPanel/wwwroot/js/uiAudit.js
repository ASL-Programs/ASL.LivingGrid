window.uiAudit = {
  runAudit: function (dotnetObj) {
    const report = [];
    document.querySelectorAll('img').forEach(img => {
      if (!img.alt || img.alt.trim() === '') {
        report.push({ type: 'Image', message: 'Image missing alt text', element: img.outerHTML });
      }
    });
    document.querySelectorAll('input, textarea, select').forEach(field => {
      if (field.type === 'hidden') return;
      if (!field.id) return;
      if (!document.querySelector('label[for="' + field.id + '"]')) {
        report.push({ type: 'Input', message: 'Field without label', element: field.outerHTML });
      }
    });
    dotnetObj.invokeMethodAsync('OnAuditCompleted', report);
  }
};
