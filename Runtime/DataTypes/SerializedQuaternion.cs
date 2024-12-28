using System;
using UnityEngine;

namespace RDTools.DataTypes
{
    [Serializable]
    public struct SerializedQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializedQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public override string ToString() => string.Format($"[{x}, {y}, {z}, {w}]");

        public static implicit operator Quaternion(SerializedQuaternion value) => new Quaternion(value.x, value.y, value.z, value.w);
        public static implicit operator SerializedQuaternion(Quaternion value) => new SerializedQuaternion(value.x, value.y, value.z, value.w);
    }
}
