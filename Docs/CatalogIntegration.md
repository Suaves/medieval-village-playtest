# AI Game Designer catalog integration approach

## Current compatibility finding

The existing `ai-game-designer-unity` catalog is a Unity `6000.0.17f1` project.
It contains 109 licensed-content prefabs under `Assets/LocalLicensedAssets` and
an AI Game Designer placement workflow that maps stable `assetId` values to
prefabs, creates objects under `AI Game Designer Generated`, and validates the
requested object IDs, asset IDs, and transforms. Its declared packages are
Splines `2.7.2` and the Unity Test Framework.

The catalog and this retained Unity 2022 baseline must **not** be merged or
imported yet. They have different editor/package generations, and the catalog
does not supply tower-defense component bindings.

## Approved future sequence

1. Explicitly migrate this project to Unity 6000 in a separate, reviewed
   change; upgrade URP/package dependencies and resolve all compile/import
   issues before importing catalog content.
2. Use only catalog entries that are present in its licensed-asset library and
   source report. Copy their associated provenance into this repository’s asset
   ledger; do not rely on filename alone.
3. Generate the village shell (terrain dressing, houses, roads, walls, props)
   through the catalog’s stable `assetId` blueprint workflow. Keep generated
   world content under one generated root, separate from gameplay objects.
4. Create project-owned wrapper prefabs for each gameplay role. Preserve the
   visual catalog prefab as a child; add colliders, layers, firing origins,
   health/damage components, and `NavMeshObstacle` only to the wrapper.
5. Bake/validate a NavMesh after the generated settlement is placed. Define
   spawn and goal corridors that cannot be filled by decorative assets.
6. Record the exact catalog revision, asset IDs, approval status, and hashes in
   a committed asset ledger before an item appears in a player build.

## Role mapping for the first slice

- **Village shell:** catalog houses, roads, fences, vegetation, market props;
  static and excluded from tower placement.
- **Defenses:** catalog medieval structures may provide visuals only; each gets
  a project-owned ballista/archer/cannon-equivalent wrapper and placement
  footprint.
- **Enemies/projectiles/UI:** project-owned or separately licensed assets;
  never substituted with unverified upstream content.

This approach keeps deterministic catalog placement useful without coupling the
catalog tool to the inherited tower-defense scene or assets.
