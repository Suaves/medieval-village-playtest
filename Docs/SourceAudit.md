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

**Not compatible as-is; migration deferred.** The baseline deliberately retains
the upstream Unity 2022.3.5f1 project version and URP 14 package graph. Unity
6000.0.17f1 requires an explicit project/package upgrade and re-validation of
URP materials, AI Navigation/NavMesh data, TextMesh Pro, serialized project
settings, legacy input, and target builds. No such migration or import has
been performed.

The C# gameplay architecture is likely portable after that migration: it uses
standard MonoBehaviour, ScriptableObject, NavMeshAgent/Obstacle, UGUI, and
legacy `Input` APIs. This is an inference from code inspection, not a Unity 6
compile result. The future migration gate is: clean Unity 6000 import, compile
with no errors, then an isolated scene smoke test before any gameplay change.

## Constraints

- Keep the root MIT license and NOTICE with distributed source/binaries.
- Do not reproduce upstream level layout, presentation, UI, or content; the
  upstream README states that its economy/layout drew on Endless Siege.
- Do not use the inherited content in any build until a provenance audit passes.
- No gameplay code, Unity asset, package, or project-setting change was made
  for this baseline.
