window.dashboardDesigner = {
    init: function () {
        const widgets = document.querySelectorAll('#widget-list div');
        const canvas = document.getElementById('canvas');
        widgets.forEach(w => {
            w.addEventListener('dragstart', e => {
                e.dataTransfer.setData('text/plain', w.innerText);
            });
        });
        canvas.addEventListener('dragover', e => e.preventDefault());
        canvas.addEventListener('drop', e => {
            e.preventDefault();
            const text = e.dataTransfer.getData('text/plain');
            const div = document.createElement('div');
            div.textContent = text;
            canvas.appendChild(div);
        });
    }
};
