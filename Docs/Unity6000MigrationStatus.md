# Unity 6000 migration status

Target editor: Unity `6000.0.17f1`.

## Completed evidence — 2026-10-10

- An interactive Unity `6000.0.17f1` import completed after 292.466 seconds
  (2,309 imported assets). The initial resolver took 289.04 seconds, not a
  deadlock: it unpacked the Unity 6 URP package graph.
- `PackageInstaller.Install` completed successfully. The manifest and lock file
  now resolve AI Navigation `2.0.16`, Visual Studio `2.0.28`, URP `17.0.3`,
  and Timeline `1.8.13`. Unity 6 includes TextMeshPro through UGUI `2.0.0`, so
  the obsolete standalone TextMeshPro `3.0.6` dependency was removed to avoid
  immutable GUID conflicts.
- The final Unity compilation and Windows player build succeeded. During the
  package transition Unity automatically rebuilt two transient stale graphs
  (a missing Visual Studio source file and Timeline `ObjectId`), then compiled
  successfully. Three pre-existing `FindObjectOfType` obsolescence warnings
  remain in `TowerPlacer` and `NextEnemyWaveButton`.
- `Assets/Scenes/Unity6000Bootstrap.unity` was generated from project-owned
  primitives only. It is the sole enabled Build Settings scene and contains a
  baked complete NavMesh route from spawn to goal plus a carving
  `NavMeshObstacle`.
- The Windows development player build succeeded in 163.064 seconds at
  `Builds/Windows/MedievalVillageDefenseBootstrap.exe`.
- The D3D-backed Windows player smoke log recorded
  `routeComplete=True; obstacleCarving=True; legacyInput=True`. The legacy
  input checks cover the existing Confirm Tower Placement and Cancel Tower
  Placement bindings.

## Test coverage and limitations

No authored test assemblies or test source files exist in this project. An
EditMode Unity Test Framework invocation completed with no test result file,
consistent with that empty test inventory. WebGL and gameplay-wave validation
were not run; they remain outside this primitive bootstrap migration scope.

## Import recovery note

The first interactive pass revealed duplicate immutable TextMeshPro GUIDs after
UGUI 2.0 imported its integrated TMP implementation. Removing the legacy TMP
package and restarting the target editor resolved the issue. No Library folder
deletion, catalog import, upstream visual import, or AI Game Designer project
interaction was performed.
