using System;
using UnityEngine;
using TowerDefense.Physics;
using TowerDefense.Gameplay;

namespace TowerDefense.Managers
{
    [CreateAssetMenu(fileName = "MatchSettings", menuName = "Tower Defense/Match Settings", order = 110)]
    public sealed class MatchSettings : ScriptableObject
    {
        [SerializeField] private int initialHealth = 20;
        [SerializeField] private int initialCurrency = 500;
        [SerializeField] private DefenderTower[] towers = new DefenderTower[0];
        [SerializeField] private EnemyWave[] enemyWaves = new EnemyWave[0];

        public event Action OnStarted;
        public event Action OnGameOver;

        public int Towers => towers.Length;
        public int EnemyWaves => enemyWaves.Length;
        public bool IsGameOver { get; private set; }

        public TowerPlacer Placer { get; private set; }
        public CurrencyCalculator Calculator { get; private set; }

        public DynamicInteger Round { get; private set; } = new DynamicInteger();
        public DynamicInteger Score { get; private set; } = new DynamicInteger();
        public DynamicInteger Health { get; private set; } = new DynamicInteger();
        public DynamicInteger Currency { get; private set; } = new DynamicInteger();

        internal void Initialize(TowerPlacer placer) => Placer = placer;
        internal void Enable() => Placer.OnTowerPlaced += HandleTowerPlaced;

        internal void Start()
        {
            ResetValues();
            OnStarted?.Invoke();
        }

        internal void Disable() => Placer.OnTowerPlaced -= HandleTowerPlaced;

        public DefenderTower GetTower(int index) => towers[index];
        public EnemyWave GetEnemyWave(int index) => enemyWaves[index];

        internal void TrySpawnTower(int index)
        {
            if (IsGameOver) return;

            var tower = GetTower(index);
            if (!Calculator.CanPurchase(tower)) return;

            var instance = Instantiate(tower);
            Placer.SetTower(instance);
        }

        internal void TryUpgrade(DefenderTower tower)
        {
            if (IsGameOver) return;

            Calculator.TryUpgrade(tower);
            tower.Upgrade();
        }

        internal void AchieveGoal(Enemy enemy)
        {
            Destroy(enemy.gameObject);
            if (IsGameOver) return;

            var newHealth = Health.Value - enemy.Damage;

            if (newHealth <= 0)
            {
                newHealth = 0;
                GameOver();
            }

            Health.Value = newHealth;
        }

        internal void Defeat(Enemy enemy)
        {
            if (IsGameOver) return;

            Score.Value += enemy.Score;
            Currency.Value += enemy.Currency;
        }

        private void HandleTowerPlaced(DefenderTower tower) => Calculator.TryPurchase(tower);

        private void ResetValues()
        {
            Round.Value = 0;
            Score.Value = 0;
            Health.Value = initialHealth;
            Currency.Value = initialCurrency;

            IsGameOver = false;
            Calculator = new CurrencyCalculator(Currency);
        }

        private void GameOver()
        {
            IsGameOver = true;
            OnGameOver?.Invoke();
        }
    }
}