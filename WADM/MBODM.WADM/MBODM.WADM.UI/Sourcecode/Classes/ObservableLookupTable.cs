using System.Collections.Generic;

namespace MBODM.WADM.UI
{
    public sealed class ObservableLookupTable<TKey, TValue> : ObservableModel
    {
        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public ObservableLookupTable(IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            foreach (var kvp in keyValuePairs)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }
        }

        public TValue this[TKey key]
        {
            get { return dictionary[key]; }
            set { if (!dictionary[key].Equals(value)) { dictionary[key] = value; OnPropertyChanged("Item[]"); } }
        }
    }
}
