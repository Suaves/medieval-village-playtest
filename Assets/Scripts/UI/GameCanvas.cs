using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class GameCanvas : MonoBehaviour
    {
        [field: SerializeField] public AudioSource AudioSource { get; private set; }

        // TODO put here access to other UI components if necessary

        private void Reset() => AudioSource = GetComponent<AudioSource>();
    }
}