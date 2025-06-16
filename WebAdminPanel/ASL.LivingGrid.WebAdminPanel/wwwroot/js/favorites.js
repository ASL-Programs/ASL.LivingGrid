window.getFavorites = function(){
  const json = localStorage.getItem('asl-favorites');
  return json ? JSON.parse(json) : [];
};
window.addFavorite = function(key){
  const favs = window.getFavorites();
  if(!favs.includes(key)){
    favs.push(key);
    localStorage.setItem('asl-favorites', JSON.stringify(favs));
  }
};
window.removeFavorite = function(key){
  let favs = window.getFavorites();
  favs = favs.filter(k => k !== key);
  localStorage.setItem('asl-favorites', JSON.stringify(favs));
};
