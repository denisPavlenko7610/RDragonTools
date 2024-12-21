using System;
using UnityEngine;

namespace RDTools.Runtime
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
            if (!(obj is SerializedVector2))
            {
                return false;
            }

            var s = (SerializedVector2)obj;
            return x == s.x &&
                   y == s.y;
        }


        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        public static bool operator ==(SerializedVector2 a, SerializedVector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(SerializedVector2 a, SerializedVector2 b)
        {
            return a.x != b.x && a.y != b.y;
        }

        public static implicit operator Vector2(SerializedVector2 x)
        {
            return new Vector2(x.x, x.y);
        }

        public static implicit operator SerializedVector2(Vector2 x)
        {
            return new SerializedVector2(x.x, x.y);
        }
    }
}