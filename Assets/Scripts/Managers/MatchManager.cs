using UnityEngine;
using TowerDefense.Physics;

namespace TowerDefense.Managers
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(ExecutionOrder.MANAGERS)]
    public sealed class MatchManager : MonoBehaviour
    {
        [SerializeField] private MatchSettings settings;
        [SerializeField] private TowerPlacer placer;

        private void Reset() => placer = GetComponentInChildren<TowerPlacer>(includeInactive: true);
        private void Awake() => settings.Initialize(placer);
        private void OnEnable() => settings.Enable();
        private void Start() => settings.Start();
        private void OnDisable() => settings.Disable();
    }
}