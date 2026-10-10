using System;

namespace TowerDefense
{
    /// <summary>
    /// Dynamic value with <see cref="OnChanged"/> event.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicValue<T> where T : IEquatable<T>, IComparable<T>
    {
        /// <summary>
        /// Event fired when the <see cref="Value"/> is changed.
        /// <para>The given param is <b>the new value</b>.</para>
        /// </summary>
        public event Action<T> OnChanged;

        /// <summary>
        /// The current value.
        /// </summary>
        public virtual T Value
        {
            get => value;
            set
            {
                if (this.value.Equals(value)) return;

                this.value = value;
                OnChanged?.Invoke(this.value);
            }
        }

        protected T value;

        /// <summary>
        /// Initializes the dynamic value <b>without raising</b> the <see cref="OnChanged"/> event.
        /// </summary>
        /// <param name="value">The current dynamic value.</param>
        public DynamicValue(T value) => this.value = value;
    }
}