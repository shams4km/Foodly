async function j(url, opt){ const r = await fetch(url, opt); if(!r.ok) throw new Error(url); return r.json(); }
const el = s => document.querySelector(s);

function restItem(r){
  return `
  <div class="rest">
    <div class="rest-logo"><img src="${r.logo ?? '/img/placeholder-restaurant.png'}" alt=""></div>
    <div>
      <div class="rest-name">${r.name}</div>
      <div class="meta">⭐ ${r.rating.toFixed(1)} (${r.reviews}) · ${r.cuisine} · ${r.price} · ${r.distanceKm.toFixed(1)} km</div>
    </div>
  </div>`;
}

function dishCard(p){
  return `
  <article class="card">
    <img src="${p.photo ?? '/img/food/' + ((p.id%6)+1) + '.jpg'}" alt="">
    <div class="card-body">
      <div style="display:flex;justify-content:space-between;align-items:center">
        <h4 style="margin:0">${p.name}</h4>
        <span class="badge">${p.price.toFixed(2)}$</span>
      </div>
      <div class="meta">⭐ 4.${(p.id%9)} · ${p.category?.name ?? 'Burger'}</div>
    </div>
  </article>`;
}

(async () => {
  // Restaurants nearby
  const restaurants = await j('/Restaurants');
  el('#nearby').innerHTML = restaurants.slice(0,6).map(restItem).join('');

  // Popular orders — берём первые продукты из /Products
  const page = await j('/Products?page=1&pageSize=9');
  el('#popular').innerHTML = page.items.map(dishCard).join('');
})();
