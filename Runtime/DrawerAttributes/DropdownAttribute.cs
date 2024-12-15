using System.Collections;
using System;
using System.Collections.Generic;

namespace RDTools
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DropdownAttribute : DrawerAttribute
    {
        public string ValuesName { get; }

        public DropdownAttribute(string valuesName)
        {
            ValuesName = valuesName;
        }
    }

    public interface IDropdownList : IEnumerable<KeyValuePair<string, object>>
    {
    }

    public class DropdownList<T> : IDropdownList
    {
        private readonly List<KeyValuePair<string, object>> _values = new();

        public void Add(string displayName, T value)
        {
            _values.Add(new KeyValuePair<string, object>(displayName, value));
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static explicit operator DropdownList<object>(DropdownList<T> target)
        {
            var result = new DropdownList<object>();
            foreach (var kvp in target)
            {
                result.Add(kvp.Key, kvp.Value);
            }
            return result;
        }
    }
}