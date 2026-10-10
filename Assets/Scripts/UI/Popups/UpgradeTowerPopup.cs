using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Gameplay;
using TowerDefense.Managers;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public sealed class UpgradeTowerPopup : MonoBehaviour
    {
        [SerializeField] private MatchSettings settings;

        [Space]
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text upgradePrice;
        [SerializeField] private TMP_Text currentWeaponsCounter;
        [SerializeField] private TMP_Text upgradedWeaponsCounter;

        [Space]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button upgradeButton;

        [Space]
        [SerializeField] private GameObject upgradablePanel;
        [SerializeField] private GameObject notUpgradablePanel;

        public event Action OnClosed;

        private DefenderTower tower;

        private void OnEnable()
        {
            closeButton.onClick.AddListener(HandleCloseButtonClicked);
            upgradeButton.onClick.AddListener(HandleUpgradeButtonClicked);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(HandleCloseButtonClicked);
            upgradeButton.onClick.RemoveListener(HandleUpgradeButtonClicked);
        }

        public void Open(DefenderTower tower)
        {
            this.tower = tower;
            ShowTowerAttributes();
            settings.Currency.OnChanged += HandleCurrencyChanched;
        }

        public void Close()
        {
            OnClosed?.Invoke();
            settings.Currency.OnChanged -= HandleCurrencyChanched;
        }

        private void HandleCurrencyChanched(int _) => UpdateUpgradeInteractivity();

        private void HandleCloseButtonClicked() => Close();

        private void HandleUpgradeButtonClicked()
        {
            settings.TryUpgrade(tower);
            ShowTowerAttributes();
        }

        private void ShowTowerAttributes()
        {
            const string formatter = "D2";

            title.text = tower.DysplayName;
            upgradePrice.text = tower.UpgradePrice.ToString(formatter);
            currentWeaponsCounter.text = tower.CurrentWeaponsCount.ToString(formatter);
            upgradedWeaponsCounter.text = tower.UpgradedWeaponsCount.ToString(formatter);

            UpdateUpgradeInteractivity();

            var canUpgradeTower = tower.CanUpgrade();
            upgradablePanel.SetActive(canUpgradeTower);
            notUpgradablePanel.SetActive(!canUpgradeTower);
        }

        private void UpdateUpgradeInteractivity() =>
            upgradeButton.interactable = settings.Calculator.CanUpgrade(tower);
    }
}