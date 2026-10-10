using TMPro;
using UnityEngine;
using TowerDefense.Managers;
using System.Collections;

namespace TowerDefense.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public abstract class AbstractDynamicViewer : MonoBehaviour
    {
        [SerializeField] protected MatchSettings settings;

        [Space]
        [SerializeField] protected TMP_Text currentValue;
        [SerializeField] protected TMP_Text increasedValue;
        [SerializeField] protected TMP_Text decreasedValue;

        [Space]
        [SerializeField] protected string spriteName;
        [SerializeField] protected string format = "D2";

        private readonly WaitForSeconds waitOneSecond = new WaitForSeconds(1);

        private void Reset() => currentValue = GetComponent<TMP_Text>();

        private void Awake()
        {
            increasedValue.enabled = false;
            decreasedValue.enabled = false;
        }

        private void OnEnable()
        {
            GetValue().OnChanged += HandleValueChanged;
            GetValue().OnIncreased += HandleValueIncreased;
            GetValue().OnDecreased += HandleValueDecreased;
        }

        private void OnDisable()
        {
            GetValue().OnChanged -= HandleValueChanged;
            GetValue().OnIncreased -= HandleValueIncreased;
            GetValue().OnDecreased -= HandleValueDecreased;
        }

        protected abstract DynamicInteger GetValue();

        protected virtual string GetText(string result) =>
            $"<sprite=\"Icons\" name=\"{spriteName}\"> {result}";

        private void HandleValueChanged(int value)
        {
            var result = value.ToString(format);
            var textWithSprite = GetText(result);
            currentValue.text = textWithSprite;
        }

        private void HandleValueIncreased(int increase)
        {
            increasedValue.text = $"+ {increase}";
            StartCoroutine(ShowValueRoutine(increasedValue));
        }

        private void HandleValueDecreased(int decrease)
        {
            decreasedValue.text = $"- {decrease}";
            StartCoroutine(ShowValueRoutine(decreasedValue));
        }

        private IEnumerator ShowValueRoutine(TMP_Text text)
        {
            text.enabled = true;

            //TODO play fade out animation

            yield return waitOneSecond;

            //TODO play fade in animation

            text.enabled = false;
        }
    }
}