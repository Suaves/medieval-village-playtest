# Medieval Village Environment — Validation

This report is populated by the Unity 6000.0.17f1 build process for this PR.

## Acceptance checks

- Deterministic source: `Assets/Blueprints/MedievalVillageEnvironment.json`.
- Separation: visual instances are generated below `Environment`; spawn, goal,
  and marker-only placement zones are generated below `Gameplay`.
- Route: the builder fails unless `NavMesh.CalculatePath` returns a complete
  enemy-spawn-to-village-goal route.
- Placement zones: the builder fails if `NavMesh.SamplePosition` finds baked
  NavMesh at a placement-zone center.
- Visual evidence: Unity-rendered top-down and angled PNGs are stored in
  `Docs/UnityScreenshots/`.

## Verified evidence

- Unity 6000.0.17f1 generated `Assets/Scenes/MedievalVillageEnvironment.unity`
  with the builder log marker: `Created deterministic village scene and baked a
  connected NavMesh.`
- The generator samples both committed endpoints before calculating the route;
  the final generated scene passed its complete spawn-to-goal route check and
  its three blocked placement-zone checks.
- Unity WebGL build: **Success**. The player build log reports a 64,959,154
  byte WebGL output at `Builds/WebGL` and `Build Finished, Result: Success`.

## Phone playtest scope

The generated `VillagePlaytestController` provides the intentionally minimal
interactive test loop: touch/mouse placement of one ballista on a blue zone,
NavMesh-driven raiders, range damage, gold reward, village health, wave start,
and restart. It uses the legacy pointer input API so browser touch events and
desktop mouse events follow the same tested path. No catalog visuals are
invented or substituted.
