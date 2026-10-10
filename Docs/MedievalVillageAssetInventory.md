# Medieval Village Environment — Asset Inventory

## Provenance

This scene uses the local-only, licensed `FANTASTIC - Village Pack` content
recorded in `ai-game-designer-unity` at
`Docs/licensed-asset-source-report.json`. The corresponding content lives under
`Assets/LocalLicensedAssets/` in both projects and is ignored by Git. It is not
included in this repository or its pull request.

| Role | Stable catalog asset ID | Source prefab |
| --- | --- | --- |
| Civic landmark (not a native town-hall asset) | `fantastic_house_13` | `P_BLD_house_13.prefab` |
| Houses | `fantastic_house_05`, `fantastic_house_09`, `fantastic_house_14` | `P_BLD_house_05.prefab`, `P_BLD_house_09.prefab`, `P_BLD_house_14.prefab` |
| Fence | `fantastic_fence_v02_01` | `P_PROP_fence_v02_01.prefab` |
| Vegetation | `fantastic_tree_village` | `P_ENV_TREE_village.prefab` |
| Village props | `fantastic_market_v04_01`, `fantastic_well`, `fantastic_barrel_01` | matching `P_PROP_*.prefab` files |

## Explicit catalog gaps

- The catalog has **no road prefab**. The blueprint uses project-owned primitive
  road strips, clearly labelled in the generated scene; it does not invent a
  catalog asset ID.
- The catalog has **no town-hall prefab**. `fantastic_house_13` remains its
  actual catalog ID and is used only as the large civic landmark visual.

## Local installation

Copy the verified `FantasticVillage` dependency set from the licensed catalog
into `Assets/LocalLicensedAssets/FantasticVillage/`, preserving all `.meta`
files. Then invoke `MedievalVillageEnvironmentBuilder.BuildScene` in Unity
6000.0.17f1. The committed blueprint supplies the exact resource paths and
placement data.
