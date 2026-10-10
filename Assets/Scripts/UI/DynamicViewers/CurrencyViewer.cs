using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public sealed class CurrencyViewer : AbstractDynamicViewer
    {
        protected override DynamicInteger GetValue() => settings.Currency;
    }
}