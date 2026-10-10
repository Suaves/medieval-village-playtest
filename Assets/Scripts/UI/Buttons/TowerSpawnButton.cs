using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Managers;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class TowerSpawnButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private MatchSettings settings;

        [Space]
        [SerializeField] private TMP_Text towerName;
        [SerializeField] private TMP_Text towerPrice;

        private int towerIndex;

        private void Reset() => button = GetComponent<Button>();

        private void OnEnable()
        {
            button.onClick.AddListener(HandleButtonClicked);
            settings.Currency.OnChanged += HandleCurrencyChanged;
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(HandleButtonClicked);
            settings.Currency.OnChanged -= HandleCurrencyChanged;
        }

        internal void Initialize(Transform parent, int towerIndex)
        {
            this.towerIndex = towerIndex;
            var tower = settings.GetTower(towerIndex);

            transform.SetParent(parent);
            transform.localScale = Vector3.one;

            towerName.text = tower.DysplayName;
            towerPrice.text = tower.PurchasePrice.ToString("D2");
        }

        private void HandleButtonClicked() => settings.TrySpawnTower(towerIndex);

        private void HandleCurrencyChanged(int _)
        {
            var tower = settings.GetTower(towerIndex);
            var isAbleToPurchaseTower = settings.Calculator.CanPurchase(tower);
            button.interactable = isAbleToPurchaseTower;
        }
    }
}