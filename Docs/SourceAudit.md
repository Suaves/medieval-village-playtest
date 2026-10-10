# Source audit — 2026-10-10

## Scope and provenance

- **Upstream:** `HyagoOliveira/TowerDefense`
- **Inspected commit:** `b2c359560865d1fade5c6227b76805f89ee72bd5` (2023-10-27)
- **Original editor:** Unity `2022.3.5f1`
- **License:** MIT, copyright (c) 2023 Hyago Oliveira; preserved unchanged in
  the repository root.
- **Repository baseline:** upstream C# source, package manifests/lock file, and
  project settings only. No upstream content was copied.

## What was inspected

The upstream project has one enabled gameplay scene, `Assets/Scenes/Gameplay.unity`,
and a single `TowerDefense` assembly (35 C# files). Its confirmed systems are:

- `MatchSettings` ScriptableObject state for currency, health, score, towers,
  waves, and game-over;
- NavMeshAgent enemy routing, path validation during tower placement, and
  NavMeshObstacle tower blocking;
- wave spawning, damage, bullets, tower upgrades, UI viewers, and local high
  score storage;
- legacy Input Manager mouse/button placement.

Its declared packages are URP `14.0.8`, AI Navigation `1.1.4`, TextMeshPro
`3.0.6`, Timeline `1.7.5`, and Unity modules. No application tests, test
assemblies, or build/test CI were found; its only GitHub workflow requests a
Unity activation file.

## Asset decision

The upstream `Assets/Content` inventory includes 42 prefabs, 25 materials, one
FBX, textures, audio, and a PSD. It has no license/provenance manifest. The
source project also carries TextMesh Pro assets and a Liberation Sans OFL file;
the font is documented, but the rest of the imported game content is not.

**Decision:** none of the upstream `Assets/Content`, `Assets/Scenes`,
`Assets/Settings`, `Assets/Presets`, or `Assets/TextMesh Pro` content is in
this repository. The original local clone is retained only as a review source.
`Assets/ThirdPartyQuarantine` is empty and must remain non-production until an
asset-level approval record exists.

## Unity 6000.0.17f1 compatibility

**Migration bootstrap validated on 2026-10-10.** The project imported and its
final Unity compilation and Windows player build succeeded in Unity
`6000.0.17f1`; package resolution produced URP `17.0.3`, AI Navigation `2.0.16`,
Visual Studio `2.0.28`, Timeline `1.8.13`, and UGUI `2.0.0`.
The deprecated standalone TextMeshPro package was removed because UGUI 2.0
contains TMP and both packages create immutable GUID conflicts.

The project-owned `Assets/Scenes/Unity6000Bootstrap.unity` is a primitive-only
smoke scene with a baked complete NavMesh route, a carving NavMeshObstacle, and
verified legacy placement-input bindings. A Windows development build and
D3D-backed player smoke completed successfully. This validates the migration
bootstrap, not the upstream gameplay content or the larger first-playable
milestone.

## Constraints

- Keep the root MIT license and NOTICE with distributed source/binaries.
- Do not reproduce upstream level layout, presentation, UI, or content; the
  upstream README states that its economy/layout drew on Endless Siege.
- Do not use the inherited content in any build until a provenance audit passes.
- No gameplay code, Unity asset, package, or project-setting change was made
  for this baseline.
