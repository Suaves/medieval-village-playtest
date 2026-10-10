using TMPro;
using TowerDefense.Managers;
using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public sealed class ScorePanel : MonoBehaviour
    {
        [SerializeField] private MatchSettings settings;

        [Space]
        [SerializeField] private TMP_Text highScore;
        [SerializeField] private TMP_Text currentScore;
        [SerializeField] private TMP_Text newHighScore;

        public void Open()
        {
            // TODO place Score logic into another class and create unit tests

            const string format = "D2";
            const string prefKey = "score";

            var score = settings.Score.Value;
            var savedHighScore = PlayerPrefs.GetInt(prefKey, defaultValue: 0);
            var isNewHighScore = score > savedHighScore;

            if (isNewHighScore)
            {
                savedHighScore = score;
                PlayerPrefs.SetInt(prefKey, savedHighScore);
                PlayerPrefs.Save();
            }

            newHighScore.enabled = isNewHighScore;
            currentScore.text = score.ToString(format);
            highScore.text = savedHighScore.ToString(format);
        }
    }
}