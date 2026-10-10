using UnityEngine;
using TowerDefense.Managers;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public sealed class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private MatchSettings settings;
        [SerializeField] private Canvas canvas;
        [SerializeField] private ScorePanel scorePanel;

        private void Reset() => canvas = GetComponent<Canvas>();
        private void OnEnable() => settings.OnGameOver += HandleGameOver;
        private void OnDisable() => settings.OnGameOver -= HandleGameOver;

        private void HandleGameOver() => Open();

        private void Open()
        {
            scorePanel.Open();
            canvas.enabled = true;
        }
    }
}