using System;
using Unity.Collections.LowLevel.Unsafe;

namespace ToolkitByJonathan
{
    [Serializable]
    public class SerializableKeyValuePair<TKey, TValue>
    {
        public TKey Key = default;
        public TValue Value = default;

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        public override string ToString()
        {
            return $"SerializableKeyValuePair(Key: {Key.ToString()} ({typeof(TKey).ToString()}) | Value: {Value} ({typeof(TValue).ToString()})";
        }
    }
}