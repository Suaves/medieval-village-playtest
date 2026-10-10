using UnityEngine;
using TowerDefense.Managers;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public sealed class TowerSpawnButtonGroup : MonoBehaviour
    {
        [SerializeField] private TowerSpawnButton prefab;
        [SerializeField] private MatchSettings settings;

        private TowerSpawnButton[] buttons = new TowerSpawnButton[0];

        private void OnEnable() => settings.OnStarted += HandleMatchStarted;
        private void OnDisable() => settings.OnStarted -= HandleMatchStarted;

        private void HandleMatchStarted() => ResetButtons();

        private void ResetButtons()
        {
            Clear();
            Instantiate();
        }

        private void Instantiate()
        {
            buttons = new TowerSpawnButton[settings.Towers];

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = Instantiate(prefab);
                buttons[i].Initialize(transform, i);
            }
        }

        private void Clear()
        {
            foreach (var button in buttons)
            {
                Destroy(button.gameObject);
            }

            buttons = new TowerSpawnButton[0];
        }
    }
}