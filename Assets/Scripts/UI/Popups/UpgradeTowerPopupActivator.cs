using UnityEngine;
using TowerDefense.Gameplay;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UpgradeTowerPopupActivator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private UpgradeTowerPopup popup;

        private void Reset() => group = GetComponent<CanvasGroup>();

        private void OnEnable()
        {
            popup.OnClosed += HandlePopupClosed;
            DefenderTower.OnClicked += HandleTowerClicked;
        }

        private void OnDisable()
        {
            popup.OnClosed -= HandlePopupClosed;
            DefenderTower.OnClicked -= HandleTowerClicked;
        }

        private void HandlePopupClosed() => Close();

        private void HandleTowerClicked(DefenderTower tower)
        {
            Open();
            popup.Open(tower);
        }

        private void Open()
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        private void Close()
        {
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }
}