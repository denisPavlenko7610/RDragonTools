using System;
using UnityEngine;

namespace RDTools.Runtime
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

        public override bool Equals(object obj)
        {
            if (!(obj is SerializedQuaternion))
            {
                return false;
            }

            var s = (SerializedQuaternion)obj;
            return x == s.x &&
                   y == s.y &&
                   z == s.z &&
                   w == s.w;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, z, w);
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }

        public static bool operator ==(SerializedQuaternion a, SerializedQuaternion b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
        }

        public static bool operator !=(SerializedQuaternion a, SerializedQuaternion b)
        {
            return !(a == b);
        }

        public static implicit operator Quaternion(SerializedQuaternion s)
        {
            return new Quaternion(s.x, s.y, s.z, s.w);
        }

        public static implicit operator SerializedQuaternion(Quaternion q)
        {
            return new SerializedQuaternion(q.x, q.y, q.z, q.w);
        }
    }
}