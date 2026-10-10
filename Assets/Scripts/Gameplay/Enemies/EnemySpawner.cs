using UnityEngine;
using UnityEngine.AI;
using TowerDefense.Managers;

namespace TowerDefense.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private MatchSettings settings;
        [SerializeField] private NavMeshAgent pathAgent;
        [SerializeField] private Transform origin;
        [SerializeField] private Transform destination;

        public int Waves => settings.EnemyWaves;
        public Vector3 Origin => origin.position;
        public Vector3 Destination => destination.position;

        private int waveIndex = -1;

        public void SpawnNextWave()
        {
            if (settings.IsGameOver) return;
            Spawn(GetNextWave());
        }

        public bool IsAbleToCompletePath()
        {
            var path = new NavMeshPath();
            return
                pathAgent.CalculatePath(Destination, path) &&
                path.status == NavMeshPathStatus.PathComplete;
        }

        private EnemyWave GetNextWave()
        {
            waveIndex++;
            var hasFinishAvailableWaves = waveIndex >= Waves;

            if (hasFinishAvailableWaves)
            {
                waveIndex = Waves - 1;
                settings.GetEnemyWave(waveIndex).AdditionalQuantity++;
            }

            return settings.GetEnemyWave(waveIndex);
        }

        private void Spawn(EnemyWave wave)
        {
            var delta = Destination - Origin; delta.y = 0f;
            var direction = delta.normalized;
            var orientation = Quaternion.LookRotation(direction);

            settings.Round.Value++;

            StopAllCoroutines();
            StartCoroutine(wave.SpawnRoutine(Origin, Destination, orientation));
        }
    }
}