using UnityEngine;

namespace TowerDefense.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class Damager : MonoBehaviour
    {
        [SerializeField] private Collider collider;
        [SerializeField, Min(0)] private int damage = 1;
        [SerializeField] private LayerMask layers;

        public int Damage
        {
            get => damage;
            set => damage = Mathf.Max(0, value);
        }

        private const int maxColliders = 10;
        private readonly Collider[] colliders = new Collider[maxColliders];

        private void Reset() => collider = GetComponent<Collider>();
        private void FixedUpdate() => FindDamageables();

        private void FindDamageables()
        {
            var damageables = FindCollidingDamageables();
            foreach (var damageable in damageables)
            {
                damageable.TakeDamage(damage);
                Destroy(gameObject);
            }
        }

        private Damageable[] FindCollidingDamageables()
        {
            var bounds = collider.bounds;
            var hitsCount = UnityEngine.Physics.OverlapBoxNonAlloc(
                bounds.center,
                bounds.extents,
                colliders,
                Quaternion.identity,
                layers
            );

            var damagebles = new Damageable[hitsCount];
            for (int i = 0; i < damagebles.Length; i++)
            {
                damagebles[i] = colliders[i].GetComponent<Damageable>();
            }

            return damagebles;
        }
    }
}