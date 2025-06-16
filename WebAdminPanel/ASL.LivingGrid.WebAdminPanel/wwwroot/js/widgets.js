window.aslWidgets = {
  getWidgets: function(companyId, userId){
    const key = `asl-widgets-${companyId}-${userId}`;
    return localStorage.getItem(key);
  },
  saveWidgets: function(companyId, userId, json){
    const key = `asl-widgets-${companyId}-${userId}`;
    localStorage.setItem(key, json);
  },
  incrementUsage: function(id){
    const key = 'asl-widget-usage';
    const raw = localStorage.getItem(key);
    const stats = raw ? JSON.parse(raw) : {};
    stats[id] = (stats[id] || 0) + 1;
    localStorage.setItem(key, JSON.stringify(stats));
  },
  getUsage: function(id){
    const key = 'asl-widget-usage';
    const raw = localStorage.getItem(key);
    if(!raw) return 0;
    const stats = JSON.parse(raw);
    return stats[id] || 0;
  }
};
