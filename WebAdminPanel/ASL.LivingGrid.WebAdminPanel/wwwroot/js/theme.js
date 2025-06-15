(function(){
  var theme = localStorage.getItem('asl-theme') || 'light';
  document.body.setAttribute('data-theme', theme);
})();
