#if UNITY_EDITOR
using UnityEditor;

namespace RDTools.Editor
{
    internal class SavedBool
    {
        private readonly string _name;
        private bool _value;

        public bool Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    EditorPrefs.SetBool(_name, value);
                }
            }
        }

        public SavedBool(string name, bool defaultValue = false)
        {
            _name = name;
            _value = EditorPrefs.GetBool(name, defaultValue);
        }
    }
}
#endif