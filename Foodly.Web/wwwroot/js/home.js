(async function () {
  // helpers
  const qs = (s) => document.querySelector(s);
  const el = (h) => {
    const t = document.createElement('template');
    t.innerHTML = h.trim();
    return t.content.firstElementChild;
  };

  // 1) Категории
  try {
    const cats = await fetch('/api/home/categories').then(r => r.json());
    const wrap = qs('#cat-list');
    const colorBy = ['orange','violet','gold','red','blue','yellow'];
    const iconSvg = {
      flame: "<svg viewBox='0 0 24 24'><path d='M13 3s3 3 3 6a4 4 0 1 1-8 0c0-2 2-5 2-5S5 6 5 11a7 7 0 0 0 14 0c0-5-6-8-6-8Z' fill='currentColor'/></svg>",
      flash: "<svg viewBox='0 0 24 24'><path d='M13 2 3 14h7l-1 8 11-14h-7l1-6z' fill='currentColor'/></svg>",
      vip: "<svg viewBox='0 0 24 24'><path d='M4 12 12 4l8 8-8 8-8-8Z' fill='currentColor' opacity='.2'/><path d='M12 2l10 10-10 10L2 12 12 2Z' fill='none' stroke='currentColor' stroke-width='2'/></svg>",
      dine: "<svg viewBox='0 0 24 24'><path d='M6 3v9a3 3 0 0 0 3 3h0V3m6 0v12m0 0h3V3' stroke='currentColor' stroke-width='2' fill='none'/></svg>",
      pickup:"<svg viewBox='0 0 24 24'><path d='M3 7h13l5 6v7H3V7Z' fill='none' stroke='currentColor' stroke-width='2'/><circle cx='8' cy='19' r='1.5' fill='currentColor'/><circle cx='18' cy='19' r='1.5' fill='currentColor'/></svg>",
      map: "<svg viewBox='0 0 24 24'><path d='M9 3 3 6v15l6-3 6 3 6-3V3l-6 3-6-3Z' fill='none' stroke='currentColor' stroke-width='2'/></svg>",
    };

    cats.forEach((c, i) => {
      const color = colorBy[i % colorBy.length];
      wrap.appendChild(el(`
        <div class="cat">
          <div class="ico ${color}">${iconSvg[c.icon] || iconSvg.flame}</div>
          <b>${c.name}</b>
          <small>${(200 + i*10)}+ options</small>
        </div>
      `));
    });
  } catch { /* ignore */ }

  // 2) Featured restaurants
  try {
    const rows = await fetch('/api/home/featured-restaurants').then(r => r.json());
    const grid = qs('#rest-grid');
    rows.forEach(r => {
      const logo = r.logo || '/img/brand-placeholder.png';
      grid.appendChild(el(`
        <div class="rest">
          <img src="${logo}" alt="">
          <div class="meta">
            <strong>${r.name}</strong>
            <div class="sub">
              <span>⭐ ${r.rating.toFixed(1)}</span>
              <span class="pill">Free delivery</span>
              <span>🕒</span>
              <span>${r.km.toFixed(1)} km</span>
            </div>
          </div>
        </div>
      `));
    });
  } catch {}

  // 3) Asian food collection
  try {
    const items = await fetch('/api/home/collection').then(r => r.json());
    const row = qs('#asian-row');
    const imgs = [
      '/img/food1.jpg',
      '/img/food2.jpg',
      '/img/food3.jpg'
    ];
    items.forEach((p, i) => {
      row.appendChild(el(`
        <div class="card">
          <img src="${imgs[i % imgs.length]}" alt="">
          <div class="cap">
            <h4>${p.name}</h4>
            <span class="pill">${Math.random()>0.33 ? 'Free delivery' : '$1.99 Delivery'}</span>
          </div>
        </div>
      `));
    });
  } catch {}
})();
