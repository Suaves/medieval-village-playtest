using UnityEngine;
using TowerDefense.Managers;

namespace TowerDefense.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider))]
    public sealed class EnemyGoalDetector : MonoBehaviour
    {
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private MatchSettings settings;
        [SerializeField] private LayerMask collisions;

        private const int maxCollision = 1;
        private readonly Collider[] colliders = new Collider[maxCollision];

        private void Reset()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        private void FixedUpdate() => CheckCollisions();

        private void CheckCollisions()
        {
            var bounds = boxCollider.bounds;
            var hitCounts = UnityEngine.Physics.OverlapBoxNonAlloc(
                bounds.center,
                bounds.extents,
                colliders,
                transform.rotation,
                collisions
            );
            var hasCollisions = hitCounts > 0;
            if (!hasCollisions) return;

            var hasEnemy = colliders[0].TryGetComponent(out Enemy enemy);
            if (hasEnemy) settings.AchieveGoal(enemy);
        }
    }
}