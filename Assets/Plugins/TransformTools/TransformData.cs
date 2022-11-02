using UnityEngine;

namespace TransformTools
{
    public struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;

        public TransformData(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    public static class TransfromDataExtensions
    {
        public static TransformData ConvertToTransformData(this Transform transform) => 
            new TransformData(transform.position, transform.rotation);
    }
}