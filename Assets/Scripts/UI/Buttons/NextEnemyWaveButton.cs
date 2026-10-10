using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Gameplay;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class NextEnemyWaveButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private EnemySpawner spawner;

        private void Reset() => button = GetComponent<Button>();
        private void Awake() => spawner = FindObjectOfType<EnemySpawner>();

        private void OnEnable()
        {
            button.onClick.AddListener(HandleButtonClicked);

            EnemyWave.OnStarted += HandleEnemyWaveStarted;
            EnemyWave.OnFinished += HandleEnemyWaveFinished;
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(HandleButtonClicked);

            EnemyWave.OnStarted -= HandleEnemyWaveStarted;
            EnemyWave.OnFinished -= HandleEnemyWaveFinished;
        }

        private void HandleButtonClicked() => spawner.SpawnNextWave();
        private void HandleEnemyWaveStarted(EnemyWave _) => button.interactable = false;
        private void HandleEnemyWaveFinished(EnemyWave _) => button.interactable = true;
    }
}