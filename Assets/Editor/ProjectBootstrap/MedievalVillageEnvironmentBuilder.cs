using System;
using System.IO;
using System.Linq;
using MedievalVillageDefense.Bootstrap;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace MedievalVillageDefense.ProjectBootstrap
{
    /// <summary>Builds the local-licensed village from its committed, deterministic blueprint.</summary>
    public static class MedievalVillageEnvironmentBuilder
    {
        private const string BlueprintPath = "Assets/Blueprints/MedievalVillageEnvironment.json";
        private const string ScenePath = "Assets/Scenes/MedievalVillageEnvironment.unity";
        private const string ScreenshotDirectory = "Docs/UnityScreenshots";
        private const string WebGlBuildPath = "Builds/WebGL";

        [Serializable] private sealed class Blueprint { public string schemaVersion; public Asset[] environment; public Road[] roads; public Zone[] towerPlacementZones; }
        [Serializable] private sealed class Asset { public string objectId; public string assetId; public string resourcePath; public Position position; public float yaw; public float scale; }
        [Serializable] private sealed class Road { public string objectId; public Position position; public Position scale; }
        [Serializable] private sealed class Zone { public string objectId; public Position position; public Position scale; }
        [Serializable] private sealed class Position { public float x; public float y; public float z; public Vector3 ToVector3() { return new Vector3(x, y, z); } }

        [MenuItem("Medieval Village/Build Environment")]
        public static void BuildScene()
        {
            var blueprint = LoadBlueprint();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var environmentRoot = new GameObject("Environment").transform;
            var gameplayRoot = new GameObject("Gameplay").transform;
            var navigationRoot = new GameObject("Navigation").transform;
            CreateCamera("Village Camera - Angled", new Vector3(34f, 42f, -43f), new Vector3(50f, -36f, 0f));
            CreateLight();
            CreateGround(environmentRoot, navigationRoot);
            CreateRoads(blueprint, environmentRoot);
            CreateLicensedEnvironment(blueprint, environmentRoot);
            var spawn = CreateMarker("EnemySpawn", new Vector3(0f, 0.1f, -28f), Color.red, gameplayRoot);
            var goal = CreateMarker("VillageGoal", new Vector3(0f, 0.1f, 29f), Color.green, gameplayRoot);
            var zones = CreateTowerPlacementZones(blueprint, navigationRoot, gameplayRoot);

            // The ground is always a bake source. Licensed scenery contributes
            // normal mesh geometry, while its runtime carving obstacles protect
            // agents from the visual structures after the scene starts.
            var surface = navigationRoot.gameObject.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
            surface.BuildNavMesh();
            ValidateNavMesh(spawn.position, goal.position, zones);

            var smoke = new GameObject("VillageEnvironmentSmoke").AddComponent<BootstrapRuntimeSmoke>();
            var smokeObject = new SerializedObject(smoke);
            smokeObject.FindProperty("spawn").objectReferenceValue = spawn;
            smokeObject.FindProperty("goal").objectReferenceValue = goal;
            smokeObject.FindProperty("obstacle").objectReferenceValue = environmentRoot.GetComponentInChildren<NavMeshObstacle>();
            smokeObject.ApplyModifiedPropertiesWithoutUndo();

            new GameObject("VillagePlaytest").AddComponent<VillagePlaytestController>();

            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath) ?? "Assets/Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("[MedievalVillageEnvironmentBuilder] Created deterministic village scene and baked a connected NavMesh.");
        }

        [MenuItem("Medieval Village/Capture Screenshots")]
        public static void CaptureScreenshots()
        {
            if (!File.Exists(ScenePath)) throw new InvalidOperationException("Build the village scene before capturing screenshots.");
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Directory.CreateDirectory(ScreenshotDirectory);
            RenderScreenshot("medieval-village-angled.png", new Vector3(34f, 42f, -43f), new Vector3(50f, -36f, 0f));
            RenderScreenshot("medieval-village-top-down.png", new Vector3(0f, 65f, -1f), new Vector3(90f, 0f, 0f));
            AssetDatabase.Refresh();
            Debug.Log("[MedievalVillageEnvironmentBuilder] Captured Unity-rendered angled and top-down screenshots.");
        }

        [MenuItem("Medieval Village/Build WebGL Playtest")]
        public static void BuildWebGlPlaytest()
        {
            if (!File.Exists(ScenePath)) throw new InvalidOperationException("Build the village scene before creating the WebGL playtest.");
            Directory.CreateDirectory(WebGlBuildPath);
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = WebGlBuildPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException($"WebGL playtest build failed: {report.summary.result}.");
            Debug.Log($"[MedievalVillageEnvironmentBuilder] WebGL playtest built at {WebGlBuildPath} ({report.summary.totalSize} bytes).");
        }

        private static Blueprint LoadBlueprint()
        {
            var blueprint = JsonUtility.FromJson<Blueprint>(File.ReadAllText(BlueprintPath));
            if (blueprint == null || blueprint.schemaVersion != "1.0" || blueprint.environment == null || blueprint.roads == null || blueprint.towerPlacementZones == null)
                throw new InvalidOperationException("Village blueprint is missing or has an unsupported schema.");
            return blueprint;
        }

        private static void CreateLicensedEnvironment(Blueprint blueprint, Transform root)
        {
            foreach (var entry in blueprint.environment)
            {
                var prefab = Resources.Load<GameObject>(entry.resourcePath);
                if (prefab == null) throw new InvalidOperationException($"Missing local licensed prefab for '{entry.assetId}' at Resources/{entry.resourcePath}.");
                var instance = PrefabUtility.InstantiatePrefab(prefab, root) as GameObject;
                instance.name = $"{entry.objectId} [{entry.assetId}]";
                instance.transform.SetPositionAndRotation(entry.position.ToVector3(), Quaternion.Euler(0f, entry.yaw, 0f));
                instance.transform.localScale = Vector3.one * entry.scale;
                var obstacle = instance.AddComponent<NavMeshObstacle>();
                obstacle.carving = true;
                obstacle.carveOnlyStationary = true;
            }
        }

        private static void CreateRoads(Blueprint blueprint, Transform root)
        {
            var material = CreateMaterial(new Color(0.23f, 0.15f, 0.08f));
            foreach (var road in blueprint.roads)
            {
                var strip = GameObject.CreatePrimitive(PrimitiveType.Cube);
                strip.name = $"{road.objectId} [project-owned primitive road]";
                strip.transform.SetParent(root);
                strip.transform.SetPositionAndRotation(road.position.ToVector3(), Quaternion.identity);
                strip.transform.localScale = new Vector3(road.scale.x, 0.05f, road.scale.z);
                strip.GetComponent<Renderer>().sharedMaterial = material;
            }
        }

        private static Transform[] CreateTowerPlacementZones(Blueprint blueprint, Transform navigationRoot, Transform gameplayRoot)
        {
            var material = CreateMaterial(new Color(0.11f, 0.32f, 0.53f));
            return blueprint.towerPlacementZones.Select(zone =>
            {
                var marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                marker.name = zone.objectId;
                marker.transform.SetParent(navigationRoot);
                marker.transform.position = zone.position.ToVector3();
                marker.transform.localScale = new Vector3(zone.scale.x, zone.scale.y, zone.scale.z);
                marker.GetComponent<Renderer>().sharedMaterial = material;
                var modifier = marker.AddComponent<NavMeshModifier>();
                modifier.overrideArea = true;
                modifier.area = NavMesh.GetAreaFromName("Not Walkable");
                marker.AddComponent<VillagePlacementZone>();
                return marker.transform;
            }).ToArray();
        }

        private static void ValidateNavMesh(Vector3 spawn, Vector3 goal, Transform[] zones)
        {
            var path = new NavMeshPath();
            var hasSpawn = NavMesh.SamplePosition(spawn, out var spawnHit, 3f, NavMesh.AllAreas);
            var hasGoal = NavMesh.SamplePosition(goal, out var goalHit, 3f, NavMesh.AllAreas);
            var hasPath = hasSpawn && hasGoal && NavMesh.CalculatePath(spawnHit.position, goalHit.position, NavMesh.AllAreas, path);
            if (!hasPath || path.status != NavMeshPathStatus.PathComplete)
            {
                var triangles = NavMesh.CalculateTriangulation().indices.Length / 3;
                throw new InvalidOperationException($"Village NavMesh has no complete enemy-spawn to village-goal route (spawn={hasSpawn}, goal={hasGoal}, triangles={triangles}, status={path.status}).");
            }
            foreach (var zone in zones)
            {
                if (NavMesh.SamplePosition(zone.position, out _, 0.1f, NavMesh.AllAreas))
                    throw new InvalidOperationException($"Tower placement zone '{zone.name}' was not excluded from the baked NavMesh.");
            }
        }

        private static void CreateGround(Transform environmentRoot, Transform navigationRoot)
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Village Ground [project-owned primitive]";
            ground.transform.SetParent(navigationRoot);
            // A Unity plane is 10m across before scaling.  Eight keeps the
            // committed spawn at z=-28 and goal at z=29 safely inside its edges.
            ground.transform.localScale = new Vector3(8f, 1f, 8f);
            ground.GetComponent<Renderer>().sharedMaterial = CreateMaterial(new Color(0.22f, 0.38f, 0.14f));
        }

        private static Transform CreateMarker(string name, Vector3 position, Color color, Transform root)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = name;
            marker.transform.SetParent(root);
            marker.transform.position = position;
            marker.transform.localScale = Vector3.one * 0.8f;
            marker.GetComponent<Collider>().enabled = false;
            marker.GetComponent<Renderer>().sharedMaterial = CreateMaterial(color);
            return marker.transform;
        }

        private static Material CreateMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader) { color = color };
            return material;
        }

        private static void CreateCamera(string name, Vector3 position, Vector3 rotation)
        {
            var camera = new GameObject(name, typeof(Camera), typeof(AudioListener));
            camera.tag = "MainCamera";
            camera.transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
            camera.GetComponent<Camera>().backgroundColor = new Color(0.36f, 0.57f, 0.78f);
        }

        private static void CreateLight()
        {
            var light = new GameObject("Directional Light", typeof(Light));
            light.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
            var component = light.GetComponent<Light>();
            component.type = LightType.Directional;
            component.intensity = 1.25f;
        }

        private static void RenderScreenshot(string fileName, Vector3 position, Vector3 rotation)
        {
            var camera = Camera.main;
            camera.transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
            camera.targetTexture = new RenderTexture(1600, 900, 24);
            camera.Render();
            RenderTexture.active = camera.targetTexture;
            var texture = new Texture2D(1600, 900, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0);
            texture.Apply();
            File.WriteAllBytes(Path.Combine(ScreenshotDirectory, fileName), texture.EncodeToPNG());
            RenderTexture.active = null;
            UnityEngine.Object.DestroyImmediate(texture);
            UnityEngine.Object.DestroyImmediate(camera.targetTexture);
            camera.targetTexture = null;
        }
    }
}
