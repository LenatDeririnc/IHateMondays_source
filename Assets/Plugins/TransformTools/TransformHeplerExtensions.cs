using UnityEngine;

namespace TransformTools
{
    public static class TransformHeplerExtensions
    {
        public static float[] PositionToFloatArray(this Transform transform)
        {
            var position = transform.position;
            return new[] { position.x, position.y, position.z };
        }

        public static float[] RotationToFloatArray(this Transform transform)
        {
            var rotation = transform.rotation.eulerAngles;
            return new[] { rotation.x, rotation.y, rotation.z };
        }

        public static Vector3 FloatArrayToPosition(this float[] array)
        {
            var vector3 = new Vector3(array[0], array[1], array[2]);
            return vector3;
        }

        public static Quaternion FloatArrayToRotation(this float[] array)
        {
            var vector3 = new Vector3(array[0], array[1], array[2]);
            Quaternion quaternion = Quaternion.Euler(vector3);
            return quaternion;
        }
    }
}