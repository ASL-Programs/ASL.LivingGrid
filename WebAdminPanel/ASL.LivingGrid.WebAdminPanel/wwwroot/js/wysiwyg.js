window.wysiwyg = {
    init: function() {
        const editor = document.getElementById('editor');
        const preview = document.getElementById('preview');
        if (!editor || !preview) return;
        const update = () => {
            preview.contentDocument.body.innerHTML = editor.innerHTML;
        };
        editor.addEventListener('input', update);
        update();
    },
    command: function(cmd) {
        document.execCommand(cmd, false, null);
    }
};
