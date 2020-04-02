using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace OrchardCore.Notifications
{
    public sealed class Notification : DynamicObject, IReadOnlyDictionary<string, object>, IEvent
    {
        private readonly IDictionary<string, object> _propertyBag = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public Notification(IDictionary<string, object> dictionary = default)
        {
            if (dictionary != null)
            {
                foreach (var keyValuePair in dictionary)
                {
                    _propertyBag.TryAdd(keyValuePair.Key, keyValuePair.Value);
                }
            }

            _propertyBag.TryAdd(nameof(Tags), new List<string>());
            _propertyBag.TryAdd(nameof(TimeStamp), DateTimeOffset.UtcNow);
        }

        object IReadOnlyDictionary<string, object>.this[string key] => _propertyBag[key];

        public ICollection<string> Tags => (ICollection<string>)_propertyBag[nameof(Tags)];

        public DateTimeOffset TimeStamp => (DateTimeOffset)_propertyBag[nameof(TimeStamp)];

        IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => _propertyBag.Keys;

        IEnumerable<object> IReadOnlyDictionary<string, object>.Values => _propertyBag.Values;

        int IReadOnlyCollection<KeyValuePair<string, object>>.Count => _propertyBag.Count;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _propertyBag.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _propertyBag[binder.Name] = value;
            return true;
        }

        bool IReadOnlyDictionary<string, object>.ContainsKey(string key) => _propertyBag.ContainsKey(key);

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => _propertyBag.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _propertyBag.GetEnumerator();

        bool IReadOnlyDictionary<string, object>.TryGetValue(string key, out object value) => _propertyBag.TryGetValue(key, out value);
    }
}
