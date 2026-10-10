using UnityEngine;
using TowerDefense.Managers;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class LeftPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private MatchSettings settings;

        private void Reset() => group = GetComponent<CanvasGroup>();
        private void OnEnable() => settings.OnGameOver += HandleGameOver;
        private void OnDisable() => settings.OnGameOver -= HandleGameOver;

        private void HandleGameOver() => group.interactable = false;
    }
}