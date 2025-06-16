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
    },
    setWidgetError: function(id, msg){
        const el = document.querySelector(`[data-widget-id="${id}"] #err-${id}`);
        if(el){
            el.textContent = 'Missing: ' + msg;
            el.classList.add('text-danger');
        }
    },
    setWidgetUsage: function(id, count){
        const badge = document.querySelector(`[data-widget-id="${id}"] .badge`);
        if(badge){
            badge.textContent = count;
        }
    }
};
