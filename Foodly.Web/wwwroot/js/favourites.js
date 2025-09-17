async function j(url, opt){ const r = await fetch(url, opt); if(!r.ok) throw new Error(url); return r.json(); }
const $ = s => document.querySelector(s);

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
  const favBtn = `<button class="chip" data-id="${p.id}" data-act="toggle">♥</button>`;
  return `
  <article class="card">
    <img src="${p.photo ?? '/img/food/' + ((p.id%6)+1) + '.jpg'}" alt="">
    <div class="card-body">
      <div style="display:flex;justify-content:space-between;align-items:center">
        <h4 style="margin:0">${p.name}</h4>
        <span class="badge">${p.price.toFixed(2)}$</span>
      </div>
      <div class="meta" style="display:flex;justify-content:space-between;align-items:center;margin-top:6px">
        <span>⭐ 4.${(p.id%9)} · ${p.category?.name ?? 'Burger'}</span>
        ${favBtn}
      </div>
    </div>
  </article>`;
}

// вкладки
document.addEventListener('click', (e)=>{
  const t = e.target.closest('.tab');
  if(!t) return;
  document.querySelectorAll('.tab').forEach(x=>x.classList.remove('is-active'));
  document.querySelectorAll('.tabpane').forEach(x=>x.classList.remove('is-active'));
  t.classList.add('is-active');
  document.querySelector('#tab-' + t.dataset.tab).classList.add('is-active');
});

(async () => {
  // Restaurants tab — просто все рестораны (можно потом фильтровать по избранным)
  const rests = await j('/Restaurants');
  $('#restCount').textContent = rests.length;
  $('#fav-restaurants').innerHTML = rests.slice(0,6).map(restItem).join('');

  // Dishes tab — настоящие избранные
  const favs = await j('/Favourites'); // твой эндпоинт
  $('#dishCount').textContent = favs.length;
  $('#fav-dishes').innerHTML = favs.map(dishCard).join('');

  // toggle favourite
  document.addEventListener('click', async (e)=>{
    const btn = e.target.closest('[data-act="toggle"]');
    if(!btn) return;
    const id = btn.dataset.id;
    const res = await j(`/Favourites/Toggle?productId=${id}`, {method:'POST'});
    if(res.ok){
      // перезагрузим список
      const favs2 = await j('/Favourites');
      $('#dishCount').textContent = favs2.length;
      $('#fav-dishes').innerHTML = favs2.map(dishCard).join('');
    }
  });
})();
