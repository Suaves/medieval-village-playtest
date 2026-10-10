using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public sealed class RoundViewer : AbstractDynamicViewer
    {
        protected override DynamicInteger GetValue() => settings.Round;

        protected override string GetText(string result) => result;
    }
}