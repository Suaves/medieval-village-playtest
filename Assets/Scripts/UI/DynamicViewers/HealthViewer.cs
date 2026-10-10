using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public sealed class HealthViewer : AbstractDynamicViewer
    {
        protected override DynamicInteger GetValue() => settings.Health;
    }
}