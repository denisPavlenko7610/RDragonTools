namespace RDTools.Runtime
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SerializedVector2
    {
        public float x;
        public float y;

        public SerializedVector2(Vector2 vector)
        {
            x = vector.x;
            y = vector.y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }
    }
}