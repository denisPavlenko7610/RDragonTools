using System;
using UnityEngine;

namespace RDTools.DataTypes
{
    [Serializable]
    public struct SerializedVector2
    {
        public float x;
        public float y;

        public SerializedVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        
        public override bool Equals(object obj)
        {
            if ((obj is SerializedVector2) == false)
            {
                return false;
            }

            var s = (SerializedVector2)obj;
            return x == s.x && y == s.y;
        }

        public override int GetHashCode()
        {
            var hashCode = 373119288;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public Vector3 ToVector2() => new Vector2(x, y);

        public static bool operator ==(SerializedVector2 a, SerializedVector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(SerializedVector2 a, SerializedVector2 b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public override string ToString() => string.Format($"[{x}, {y}]");

        public static implicit operator Vector2(SerializedVector2 value) => new Vector2(value.x, value.y);
        public static implicit operator SerializedVector2(Vector2 value) => new SerializedVector2(value.x, value.y);
    }
}
