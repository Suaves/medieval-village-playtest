using UnityEngine;
using System.Collections;

namespace TowerDefense.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class Weapon : MonoBehaviour
    {
        [SerializeField, Min(0)] private int damage;
        [SerializeField, Min(0f)] private float reloadTime;
        [SerializeField, Min(0)] private AudioSource audioSource;

        [Space]
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField, Min(0f)] private float bulletSpeed;

        private DefenderTower tower;

        private void Awake() => tower = GetComponentInParent<DefenderTower>();
        private void Start() => StartAttack();

        public void StartAttack() => StartCoroutine(FireRoutine());
        public void Stop() => StopAllCoroutines();

        private IEnumerator FireRoutine()
        {
            var waitReloadTime = new WaitForSeconds(reloadTime);
            var waitUntilTragetIsFound = new WaitUntil(IsTargetFound);

            while (true)
            {
                yield return waitReloadTime;
                yield return waitUntilTragetIsFound;
                Fire();
            }
        }

        private void Fire()
        {
            //TODO improve using a PoolSystem
            var bullet = Instantiate(
                bulletPrefab,
                bulletSpawnPoint.position,
                rotation: Quaternion.identity
            );
            var deltaPosition = tower.Detector.TargetPosition - bullet.transform.position;
            var bulletDirection = deltaPosition.normalized;

            bullet.Fire(
                bulletSpeed,
                bulletDirection,
                damage
            );
            audioSource.Play();
        }

        private bool IsTargetFound() => tower.Detector.HasTarget;
    }
}