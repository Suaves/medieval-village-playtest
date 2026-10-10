using UnityEngine;
using UnityEngine.AI;
using TowerDefense.Managers;

namespace TowerDefense.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Damageable))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class Enemy : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Damageable damageable;
        [SerializeField] private MatchSettings settings;
        [SerializeField, Min(0)] private int score = 1;
        [SerializeField, Min(0)] private int damage = 1;
        [SerializeField, Min(0)] private int currency = 1;

        public float Speed
        {
            get => agent.speed;
            set => agent.speed = value;
        }

        public int Score => score;
        public int Damage => damage;
        public int Currency => currency;

        private void Reset()
        {
            agent = GetComponent<NavMeshAgent>();
            damageable = GetComponent<Damageable>();
        }

        private void OnEnable() => damageable.OnEnergyEnded += HandleEnergyEnded;
        private void OnDisable() => damageable.OnEnergyEnded -= HandleEnergyEnded;

        public void SetDestination(Vector3 destination) => agent.SetDestination(destination);

        private void HandleEnergyEnded() => settings.Defeat(this);
    }
}