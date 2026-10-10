using System;

namespace TowerDefense
{
    /// <summary>
    /// Dynamic integer with change events.
    /// </summary>
    public sealed class DynamicInteger : DynamicValue<int>
    {
        /// <summary>
        /// Event fired when the <see cref="Value"/> increases.
        /// <para>The given param is <b>the increase</b>.</para>
        /// </summary>
        public event Action<int> OnIncreased;

        /// <summary>
        /// Event fired when the <see cref="Value"/> decreases.
        /// <para>The given param is <b>the decrease</b>.</para>
        /// </summary>
        public event Action<int> OnDecreased;

        public override int Value
        {
            get => base.Value;
            set
            {
                var comparison = value.CompareTo(base.value);
                if (comparison < 0) OnDecreased?.Invoke(base.value - value);
                else if (comparison > 0) OnIncreased?.Invoke(value - this.value);

                base.Value = value;
            }
        }

        /// <summary>
        /// Initializes the dynamic integer <b>without raising</b> any event.
        /// </summary>
        /// <param name="value">The current integer value.</param>
        public DynamicInteger(int value = 0) : base(value) { }

        public override string ToString() => value.ToString("D2");

        public static implicit operator int(DynamicInteger d) => d.value;
    }
}