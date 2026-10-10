# Medieval Village Defense

Private commercial prototype foundation based on the MIT-licensed gameplay
source of [HyagoOliveira/TowerDefense](https://github.com/HyagoOliveira/TowerDefense).
The original license and attribution are preserved in [LICENSE](LICENSE) and
[NOTICE.md](NOTICE.md).

This is deliberately a **source-only, non-playable baseline**. It contains the
upstream C# systems and Unity project configuration, but no inherited scenes,
prefabs, meshes, materials, textures, fonts, audio, shaders, or visual effects.
Those assets have no complete provenance record and must not be introduced into
a commercial build until individually verified. The reserved
`Assets/ThirdPartyQuarantine/` directory is excluded from production use.

Do not open this project in Unity 6000 yet: its source configuration remains
unchanged at Unity 2022.3.5f1 / URP 14 pending an explicit migration task.

See [Docs/SourceAudit.md](Docs/SourceAudit.md),
[Docs/CatalogIntegration.md](Docs/CatalogIntegration.md), and
[Docs/FirstPlayableMilestone.md](Docs/FirstPlayableMilestone.md).
