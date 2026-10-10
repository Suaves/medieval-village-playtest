using System;
using System.IO;
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
    /// <summary>Creates the project-owned, primitive-only migration smoke scene.</summary>
    public static class BootstrapSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Unity6000Bootstrap.unity";
        private const string DesktopBuildPath = "Builds/Windows/MedievalVillageDefenseBootstrap.exe";

        public static void BuildScene()
        {
            Directory.CreateDirectory("Assets/Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var root = new GameObject("Bootstrap");
            CreateCamera();
            CreateLight();
            CreatePrimitive(PrimitiveType.Plane, "Ground", Vector3.zero, new Vector3(3f, 1f, 3f));
            var spawn = CreateMarker("Spawn", new Vector3(-12f, 0.05f, 0f), Color.green);
            var goal = CreateMarker("Goal", new Vector3(12f, 0.05f, 0f), Color.blue);
            var obstacle = CreateObstacle();

            var surface = root.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
            surface.BuildNavMesh();

            var path = new NavMeshPath();
            if (!NavMesh.CalculatePath(spawn.position, goal.position, NavMesh.AllAreas, path) ||
                path.status != NavMeshPathStatus.PathComplete)
            {
                throw new InvalidOperationException("The bootstrap NavMesh has no complete spawn-to-goal route.");
            }

            var smoke = new GameObject("BootstrapRuntimeSmoke").AddComponent<BootstrapRuntimeSmoke>();
            var smokeObject = new SerializedObject(smoke);
            smokeObject.FindProperty("spawn").objectReferenceValue = spawn;
            smokeObject.FindProperty("goal").objectReferenceValue = goal;
            smokeObject.FindProperty("obstacle").objectReferenceValue = obstacle;
            smokeObject.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("[BootstrapSceneBuilder] Created primitive-only bootstrap scene with a complete NavMesh route.");
        }

        public static void BuildWindowsDesktopSmoke()
        {
            if (!File.Exists(ScenePath))
            {
                throw new InvalidOperationException("Create the bootstrap scene before building the desktop smoke player.");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(DesktopBuildPath) ?? "Builds");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = DesktopBuildPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            });

            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException($"Desktop smoke build failed: {report.summary.result}.");
            }

            Debug.Log($"[BootstrapSceneBuilder] Desktop smoke build created at {DesktopBuildPath}.");
        }

        private static Transform CreateMarker(string name, Vector3 position, Color color)
        {
            var marker = CreatePrimitive(PrimitiveType.Sphere, name, position, Vector3.one * 0.5f);
            marker.GetComponent<Collider>().enabled = false;
            marker.GetComponent<Renderer>().sharedMaterial.color = color;
            return marker.transform;
        }

        private static NavMeshObstacle CreateObstacle()
        {
            var obstacle = CreatePrimitive(PrimitiveType.Cube, "CarvingObstacle", new Vector3(0f, 0.5f, 0f), new Vector3(3f, 1f, 2f));
            obstacle.GetComponent<Renderer>().sharedMaterial.color = Color.red;
            var navMeshObstacle = obstacle.AddComponent<NavMeshObstacle>();
            navMeshObstacle.shape = NavMeshObstacleShape.Box;
            navMeshObstacle.size = Vector3.one;
            navMeshObstacle.carving = true;
            navMeshObstacle.carveOnlyStationary = true;
            return navMeshObstacle;
        }

        private static GameObject CreatePrimitive(PrimitiveType type, string name, Vector3 position, Vector3 scale)
        {
            var primitive = GameObject.CreatePrimitive(type);
            primitive.name = name;
            primitive.transform.SetPositionAndRotation(position, Quaternion.identity);
            primitive.transform.localScale = scale;
            return primitive;
        }

        private static void CreateCamera()
        {
            var camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camera.tag = "MainCamera";
            camera.transform.SetPositionAndRotation(new Vector3(0f, 22f, -22f), Quaternion.Euler(48f, 0f, 0f));
            camera.GetComponent<Camera>().backgroundColor = new Color(0.12f, 0.18f, 0.24f);
        }

        private static void CreateLight()
        {
            var light = new GameObject("Directional Light", typeof(Light));
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            light.GetComponent<Light>().type = LightType.Directional;
        }
    }
}
