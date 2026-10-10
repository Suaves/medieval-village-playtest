using System;
using UnityEngine;

namespace TowerDefense.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DamageableFeedback))]
    [DefaultExecutionOrder(ExecutionOrder.COMPONENTS)]
    public sealed class Damageable : MonoBehaviour
    {
        [SerializeField] private int initialEnergy = 10;
        [SerializeField] private DamageableFeedback feedback;

        public event Action OnEnergyEnded;

        public DynamicInteger Energy { get; private set; } = new DynamicInteger();

        private void Reset() => feedback = GetComponent<DamageableFeedback>();
        private void Awake() => Energy.Value = initialEnergy;

        public void TakeDamage(int damage)
        {
            var hasEnergy = Energy.Value > 0;
            if (!hasEnergy) return;

            Energy.Value -= damage;
            feedback.Play();

            var noEnergy = Energy.Value <= 0;
            if (noEnergy)
            {
                OnEnergyEnded?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}