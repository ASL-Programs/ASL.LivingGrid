(function(){
  var theme = localStorage.getItem('asl-theme') || 'light';
  applyTheme(theme);
})();

function applyTheme(theme){
  document.body.setAttribute('data-theme', theme);
  var link = document.getElementById('theme-link');
  if(link){
    link.href = `themes/${theme}/theme.css`;
  }
}

window.setAslTheme = function(theme){
  localStorage.setItem('asl-theme', theme);
  applyTheme(theme);
}
