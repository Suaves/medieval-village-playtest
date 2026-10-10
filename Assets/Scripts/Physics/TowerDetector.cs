using UnityEngine;

namespace TowerDefense.Physics
{
    [DisallowMultipleComponent]
    public sealed class TowerDetector : MonoBehaviour
    {
        [SerializeField] private LayerMask collisions;
        [SerializeField] private Transform visualizer;
        [SerializeField, Min(0f)] private float radius = 10f;

        public bool HasTarget { get; private set; }

        public Vector3 TargetPosition => colliders[0].transform.position;

        private const int maxCollision = 1;
        private readonly Collider[] colliders = new Collider[maxCollision];

        private void FixedUpdate()
        {
            UpdateCollisions();
            RotateTowardsCollision();
        }

        private void OnValidate() => ResizeVisualizer();

        public void ToggleVisualizer(bool enabled) => visualizer.gameObject.SetActive(enabled);

        private void ResizeVisualizer()
        {
            if (visualizer == null) return;
            visualizer.localScale = 2F * radius * Vector3.one;
        }

        private void UpdateCollisions()
        {
            var hitCounts = UnityEngine.Physics.OverlapSphereNonAlloc(
                transform.position,
                radius,
                colliders,
                collisions
            );
            HasTarget = hitCounts > 0;
        }

        private void RotateTowardsCollision()
        {
            if (!HasTarget) return;

            var delta = TargetPosition - transform.position;
            delta.y = 0f;
            var direction = delta.normalized;

            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}