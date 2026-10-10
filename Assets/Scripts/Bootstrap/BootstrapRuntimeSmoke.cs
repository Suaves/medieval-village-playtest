using UnityEngine;
using UnityEngine.AI;

namespace MedievalVillageDefense.Bootstrap
{
    /// <summary>Logs the minimum runtime evidence for the migration bootstrap scene.</summary>
    public sealed class BootstrapRuntimeSmoke : MonoBehaviour
    {
        [SerializeField] private Transform spawn;
        [SerializeField] private Transform goal;
        [SerializeField] private NavMeshObstacle obstacle;

        private void Start()
        {
            var path = new NavMeshPath();
            var routeComplete = NavMesh.CalculatePath(spawn.position, goal.position, NavMesh.AllAreas, path) &&
                                path.status == NavMeshPathStatus.PathComplete;
            var legacyInputAvailable = ReadLegacyButton("Confirm Tower Placement") &&
                                       ReadLegacyButton("Cancel Tower Placement");
            var obstacleReady = obstacle != null && obstacle.carving;

            Debug.Log($"[BootstrapSmoke] routeComplete={routeComplete}; obstacleCarving={obstacleReady}; legacyInput={legacyInputAvailable}");

            if (!routeComplete || !obstacleReady || !legacyInputAvailable)
            {
                Debug.LogError("[BootstrapSmoke] Bootstrap validation failed.");
            }

            if (Application.isBatchMode)
            {
                Application.Quit(routeComplete && obstacleReady && legacyInputAvailable ? 0 : 1);
            }
        }

        private static bool ReadLegacyButton(string buttonName)
        {
            try
            {
                Input.GetButton(buttonName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
