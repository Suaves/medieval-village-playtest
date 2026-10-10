using UnityEngine;

namespace TowerDefense.VisualEffects
{
    [DisallowMultipleComponent]
    public sealed class MaterialReplacer : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;

        private Material lasMaterial;
        private Material[] originalMaterials;

        private void Reset() => renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
        private void Awake() => CacheOriginalMaterials();

        public void SetMaterials(Material material)
        {
            if (lasMaterial == material) return;

            lasMaterial = material;
            foreach (var renderer in renderers)
            {
                renderer.material = material;
            }
        }

        public void ResetMaterials()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material = originalMaterials[i];
            }
        }

        private void CacheOriginalMaterials()
        {
            originalMaterials = new Material[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                originalMaterials[i] = renderers[i].material;
            }
        }
    }
}