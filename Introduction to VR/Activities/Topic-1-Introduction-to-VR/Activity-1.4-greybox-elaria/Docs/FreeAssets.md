# Free 3D, texture, and audio assets

You are not expected to model, texture, or compose anything yourself in this course. Use free assets, keep track of where each one came from, and credit the author in your Game Design Document. Always check the license line on the download page: CC0 means no attribution required; CC-BY means credit the author; "personal use only" means you may not ship it.

## 3D models and modular kits

| Site | Best for | License notes |
|---|---|---|
| [Kenney](https://kenney.nl/assets) | Low-poly nature, dungeon, castle, and prop kits that import cleanly and run fast on Quest | CC0 |
| [Quaternius](https://quaternius.com/) | Stylized characters, animated NPCs, fantasy props, trees, rocks | CC0 |
| [Poly Pizza](https://poly.pizza/) | Searchable low-poly models, downloads as glTF/FBX | Mostly CC0 / CC-BY, shown per model |
| [Sketchfab](https://sketchfab.com/features/free-3d-models) | Large library incl. scanned ruins and statues; filter by *Downloadable* and license | Varies per model |
| [Unity Asset Store — free filter](https://assetstore.unity.com/?price=0-0) | Complete environment packs, particle packs, sample terrains | Unity Asset Store EULA |
| [OpenGameArt](https://opengameart.org/) | Community game art, tilesets, props | Varies (CC0, CC-BY, GPL) |
| [TurboSquid free](https://www.turbosquid.com/Search/3D-Models/free) | Higher-poly props — check polycount before using on Quest | Per model |
| [Free3D](https://free3d.com/) | Assorted models, many in OBJ | Per model |
| [CGTrader free](https://www.cgtrader.com/free-3d-models) | Architecture and props | Per model |
| [Mixamo](https://www.mixamo.com/) | Rigged humanoid characters and animations (needs a free Adobe account) | Adobe terms, free to use in projects |

## Textures, materials, and skies

| Site | Best for | License |
|---|---|---|
| [Poly Haven](https://polyhaven.com/) | PBR textures (stone, moss, snow, bark), HDRI skies | CC0 |
| [ambientCG](https://ambientcg.com/) | PBR materials and terrain textures | CC0 |
| [Kenney Textures](https://kenney.nl/assets?q=texture) | Stylized texture packs | CC0 |

## Audio: music, ambience, effects

| Site | Best for | License |
|---|---|---|
| [Freesound](https://freesound.org/) | Field recordings, wind, water, footsteps, UI clicks | Per sound (many CC0) |
| [Kenney Audio](https://kenney.nl/assets?q=audio) | UI sounds, impacts, game jingles | CC0 |
| [OpenGameArt — music](https://opengameart.org/art-search-advanced?field_art_type_tid%5B%5D=12) | Looping background tracks | Varies |
| [Free Music Archive](https://freemusicarchive.org/) | Longer ambient tracks | Per track (check CC type) |
| [Pixabay Sound Effects](https://pixabay.com/sound-effects/) | Effects and ambiences, no attribution | Pixabay license |
| [BBC Sound Effects](https://sound-effects.bbcrewind.co.uk/) | 30 000+ archive recordings | RemArc license (personal, educational, research) |

## Import tips for Quest 3

- Prefer FBX or glTF. For OBJ, check that normals came through.
- Aim for under 10 000 triangles for a hero prop and under 1 000 for scatter props (trees, rocks). Check with the *Stats* overlay.
- Set *Scale Factor* in the model import settings so 1 unit = 1 meter; a door should be about 2.1 m tall.
- Combine many small textures into one atlas where you can; fewer materials means fewer draw calls.
- Mark environment props *Static* so lightmapping, batching, and occlusion culling can use them.

Created by Isac Artzi
