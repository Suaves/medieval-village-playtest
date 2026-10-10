# First playable milestone — Medieval Village Defense vertical slice

## Definition of done

A Unity 6000.0.17f1 scene builds and runs on desktop and WebGL with only
approved catalog/project-owned assets. The player can place one defensive
structure in allowed zones, start three finite enemy waves, see enemies route
around placed defenses to a village goal, earn/spend currency, take village
health damage on leaks, and reach a restartable game-over screen.

## Exact acceptance checks

1. Build Settings contains only the new project-owned bootstrap/gameplay scene;
   it contains no quarantined upstream reference.
2. Asset ledger identifies every non-Unity asset referenced by the scene as
   approved, including catalog ID and source/license record.
3. One catalog-generated village shell is reproducible from a committed
   blueprint and has a baked NavMesh with a valid spawn-to-goal path.
4. One wrapped defense prefab rejects placements that overlap blocked zones or
   remove every valid route; it fires and damages a wrapped enemy prefab.
5. Three authored waves complete deterministically; currency/health/UI update;
   loss and restart work.
6. Unity Console has no errors in a clean Unity 6000 import; desktop and WebGL
   smoke tests complete.

## Deliberately out of scope

Android touch controls, multiple defense types/upgrades, endless waves, saves,
audio polish, analytics, monetization, and production performance work.
