using System;
using UnityEngine;

namespace RDTools.Runtime
{
    [Serializable]
    public struct SerializedVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializedVector3(Vector3 position)
        {
            this.x = position.x;
            this.y = position.y;
            this.z = position.z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SerializedVector3))
            {
                return false;
            }

            var s = (SerializedVector3)obj;
            return x == s.x &&
                   y == s.y &&
                   z == s.z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, z);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public static bool operator ==(SerializedVector3 a, SerializedVector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(SerializedVector3 a, SerializedVector3 b)
        {
            return !(a == b);
        }

        public static implicit operator Vector3(SerializedVector3 s)
        {
            return new Vector3(s.x, s.y, s.z);
        }

        public static implicit operator SerializedVector3(Vector3 v)
        {
            return new SerializedVector3(v);
        }
    }
}