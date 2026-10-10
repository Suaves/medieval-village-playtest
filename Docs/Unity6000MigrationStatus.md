# Unity 6000 migration status

Target editor: Unity `6000.0.17f1`.

The project version marker was updated to the target editor and
`Assets/Editor/ProjectBootstrap/PackageInstaller.cs` uses the Unity Package
Manager Client API to resolve the retained packages at Unity 6000-compatible
versions. This preserves the upstream runtime source and does not import any
quarantined content.

## Current blocker

On 2026-10-10, Unity `6000.0.17f1` was launched headlessly for a clean import.
It rebuilt `Library/PackageCache` but stalled at `Rebuilding Library because the
metadata folder could not be found!` before it executed the package installer.
The manifest and lock file therefore remain at their 2022-era versions. No
claim of successful package upgrade, compilation, scene creation, NavMesh
validation, tests, or desktop smoke test is made.

The next step is to complete the initial import in an interactive Unity 6000
Editor session (accept its version-mismatch dialog), then invoke
`MedievalVillageDefense.ProjectBootstrap.PackageInstaller.Install` headlessly
or from the Editor. Only after package resolution succeeds should the
primitive-only bootstrap scene be generated and tested.
