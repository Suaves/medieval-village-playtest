using UnityEngine;

namespace TowerDefense.Gameplay
{
    public sealed class CurrencyCalculator
    {
        private readonly DynamicInteger currency;

        public CurrencyCalculator(DynamicInteger currency) => this.currency = currency;

        public bool CanPurchase(int price) => currency.Value >= price;
        public bool CanPurchase(DefenderTower tower) => CanPurchase(tower.PurchasePrice);
        public bool CanUpgrade(DefenderTower tower) => CanPurchase(tower.UpgradePrice) && tower.CanUpgrade();

        public void TryPurchase(DefenderTower tower)
        {
            if (CanPurchase(tower)) Purchase(tower);
        }

        public void TryUpgrade(DefenderTower tower)
        {
            if (CanUpgrade(tower)) Upgrade(tower);
        }

        private void Purchase(DefenderTower tower)
        {
            var value = currency.Value - tower.PurchasePrice;
            currency.Value = Mathf.Max(0, value);
            // Add Analytics here if a valid project
        }

        private void Upgrade(DefenderTower tower)
        {
            var value = currency.Value - tower.UpgradePrice;
            currency.Value = Mathf.Max(0, value);
            // Add Analytics here if a valid project
        }
    }
}