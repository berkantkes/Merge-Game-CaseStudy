# Asset üretim kaydı

Yöntem: yerleşik imagegen aracı. Her ikon ayrı çağrıda, `transparent_background: true` ile üretildi. Arka plan `false` ile üretildi. Çıktılar yeniden çizilmeden proje Art klasörüne kopyalandı. Unity ikonları 256 px import limitiyle sprite olarak kullanır; kaynak PNG'ler korunur.

İkon prompt şablonu:

> Single mobile merge game inventory icon: {subject}. Cozy seaside workshop, polished hand-painted cartoon 2D, bold readable silhouette, gentle dimensional shading, cream turquoise coral palette, centered isolated object, generous transparent margin, no lettering, no border, no additional objects. Square canvas. Save output for icon {index}.

| Dosya indeksi | subject |
|---|---|
| 00 | orange fruit with leaf |
| 01 | glass of orange juice |
| 02 | plate of sliced tropical fruit |
| 03 | small strawberry cake |
| 04 | wicker picnic basket |
| 05 | folded turquoise cleaning cloth |
| 06 | wooden scrubbing brush |
| 07 | small carpenter hammer |
| 08 | coral metal toolbox |
| 09 | compact wooden workbench |
| 10 | pink seashell |
| 11 | cream candle |
| 12 | turquoise lantern |
| 13 | terracotta flower pot with flowers |
| 14 | striped coral beach umbrella |

Arka plan prompt'u:

> Portrait background illustration for cozy seaside workshop mobile puzzle game. Soft pastel cream sand, tranquil turquoise sea, distant islands, a few palm leaves at top corners. Very quiet pale center and lower central area for overlaying game board. Polished cartoon painted style, no text, no UI, no characters.
