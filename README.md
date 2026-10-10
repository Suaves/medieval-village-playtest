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

The Unity 6000.0.17f1 migration bootstrap is complete. The project uses URP 17
and includes a project-owned primitive smoke scene; see
[Docs/Unity6000MigrationStatus.md](Docs/Unity6000MigrationStatus.md) for the
exact validation evidence and remaining scope.

See [Docs/SourceAudit.md](Docs/SourceAudit.md),
[Docs/CatalogIntegration.md](Docs/CatalogIntegration.md), and
[Docs/FirstPlayableMilestone.md](Docs/FirstPlayableMilestone.md).
