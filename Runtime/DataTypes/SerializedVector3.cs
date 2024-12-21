namespace RDTools.Runtime
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SerializedVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializedVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}