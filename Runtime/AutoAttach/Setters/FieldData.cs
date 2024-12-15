using System.Linq;
using System.Reflection;

namespace RDTools.AutoAttach.Setters
{
    public class FieldData
    {
        public readonly AttachAttribute Attribute;
        
        private readonly FieldInfo[] _fieldInfos;

        public FieldInfo Field => _fieldInfos[^1]; // Uses index from end to get the last field

        public FieldData(AttachAttribute attribute, FieldInfo[] fieldInfos)
        {
            Attribute = attribute;
            _fieldInfos = fieldInfos;
        }

        public object GetContext(object obj)
        {
            return _fieldInfos[..^1].Aggregate(obj, (current, fieldInfo) => fieldInfo.GetValue(current));
        }
    }
}
