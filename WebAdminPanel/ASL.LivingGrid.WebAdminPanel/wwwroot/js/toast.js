window.toast = {
  show: function(message) {
    var container = document.getElementById('toastContainer');
    if (!container) {
      container = document.createElement('div');
      container.id = 'toastContainer';
      container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
      document.body.appendChild(container);
    }
    var wrapper = document.createElement('div');
    wrapper.className = 'toast align-items-center text-bg-primary border-0';
    wrapper.innerHTML = '<div class="d-flex"><div class="toast-body">' + message + '</div>' +
      '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button></div>';
    container.appendChild(wrapper);
    var t = new bootstrap.Toast(wrapper);
    t.show();
  }
};
