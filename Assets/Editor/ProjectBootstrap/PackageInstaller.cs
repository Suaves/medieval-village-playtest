using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace MedievalVillageDefense.ProjectBootstrap
{
    /// <summary>
    /// Resolves the Unity 6000-compatible versions of the packages retained from
    /// the upstream source. Invoke explicitly from a headless Editor process.
    /// </summary>
    public static class PackageInstaller
    {
        private static readonly string[] PackagesToAdd =
        {
            "com.unity.2d.sprite",
            "com.unity.ai.navigation",
            "com.unity.ide.visualstudio",
            "com.unity.render-pipelines.universal",
            "com.unity.timeline"
        };

        private const double TimeoutSeconds = 600;

        private static AddAndRemoveRequest request;
        private static double deadline;

        public static void Install()
        {
            request = Client.AddAndRemove(PackagesToAdd, new string[0]);
            deadline = EditorApplication.timeSinceStartup + TimeoutSeconds;
            EditorApplication.update += Poll;
        }

        private static void Poll()
        {
            if (!request.IsCompleted)
            {
                if (EditorApplication.timeSinceStartup > deadline)
                {
                    EditorApplication.update -= Poll;
                    Debug.LogError("[PackageInstaller] Timed out waiting for package resolution.");
                    EditorApplication.Exit(2);
                }

                return;
            }

            EditorApplication.update -= Poll;

            if (request.Status == StatusCode.Success)
            {
                var resolvedPackages = request.Result.Select(package => $"{package.name}@{package.version}");
                Debug.Log($"[PackageInstaller] Resolved: {string.Join(", ", resolvedPackages)}");
                EditorApplication.Exit(0);
                return;
            }

            Debug.LogError($"[PackageInstaller] {request.Error?.message}");
            EditorApplication.Exit(1);
        }
    }
}
