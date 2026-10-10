using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public sealed class ScoreViewer : AbstractDynamicViewer
    {
        protected override DynamicInteger GetValue() => settings.Score;
    }
}