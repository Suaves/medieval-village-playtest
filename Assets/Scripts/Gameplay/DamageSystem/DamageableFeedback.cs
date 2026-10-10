using UnityEngine;
using System.Collections;

namespace TowerDefense.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class DamageableFeedback : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float time = 0.2f;
        [SerializeField] private Color color = Color.red * 0.5F;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private Renderer[] renderers;

        private Color[] originalColors;
        private static readonly WaitForEndOfFrame waitEndOfFrame = new WaitForEndOfFrame();

        private void Reset()
        {
            curve = GetDefaultCurve();
            renderers = GetComponentsInChildren<Renderer>();
        }

        private void Awake() => FindOriginalColors();

        public void Play()
        {
            StopAllCoroutines();
            StartCoroutine(PlayRoutine());
        }

        private void FindOriginalColors()
        {
            originalColors = new Color[renderers.Length];
            for (int i = 0; i < originalColors.Length; i++)
            {
                originalColors[i] = renderers[i].material.color;
            }
        }

        private void ResetToOriginalColors()
        {
            for (int i = 0; i < originalColors.Length; i++)
            {
                renderers[i].material.color = originalColors[i];
            }
        }

        private void LerpColors(float step)
        {
            for (int i = 0; i < originalColors.Length; i++)
            {
                var lerpedColor = Color.Lerp(originalColors[i], color, step);
                renderers[i].material.color = lerpedColor;
            }
        }

        private IEnumerator PlayRoutine()
        {
            var currentTime = 0f;

            do
            {
                var step = curve.Evaluate(currentTime);

                LerpColors(step);

                currentTime += Time.deltaTime;
                yield return waitEndOfFrame;

            } while (currentTime < time);

            ResetToOriginalColors();
        }

        private static AnimationCurve GetDefaultCurve()
        {
            var curve = AnimationCurve.Linear(
                timeStart: 0F,
                valueStart: 0F,
                timeEnd: 0.2F,
                valueEnd: 1F
            );
            curve.postWrapMode = WrapMode.PingPong;

            return curve;
        }
    }
}