using UnityEngine;
using System.Collections;
using System;

namespace TowerDefense.Gameplay
{
    [CreateAssetMenu(fileName = "EnemyWave", menuName = "Tower Defense/Enemy Wave", order = 110)]
    public sealed class EnemyWave : ScriptableObject
    {
        [SerializeField] private Enemy prefab;
        [SerializeField, Min(0)] private int quantity = 1;
        [SerializeField, Min(0f)] private float timeBetweenSpawns = 1f;

        public static event Action<EnemyWave> OnStarted;
        public static event Action<EnemyWave> OnFinished;

        public int AdditionalQuantity { get; set; } = 0;

        public IEnumerator SpawnRoutine(
            Vector3 position,
            Vector3 destination,
            Quaternion rotation
        )
        {
            var remainSpawns = quantity + AdditionalQuantity;
            var waitTime = new WaitForSeconds(timeBetweenSpawns);

            OnStarted?.Invoke(this);

            do
            {
                var enemy = Instantiate(prefab, position, rotation);
                enemy.SetDestination(destination);

                yield return waitTime;
                remainSpawns--;

            } while (remainSpawns > 0);

            OnFinished?.Invoke(this);
        }
    }
}