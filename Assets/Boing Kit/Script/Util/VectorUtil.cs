/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEngine;

namespace BoingKit
{
  public class VectorUtil
  {
    public static readonly Vector3 Min = new Vector3(float.MinValue, float.MinValue, float.MinValue);
    public static readonly Vector3 Max = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    
    public static Vector3 Rotate2D(Vector3 v, float angle)
    {
      Vector3 results = v;
      float cos = Mathf.Cos(angle);
      float sin = Mathf.Sin(angle);
      results.x = cos * v.x - sin * v.y;
      results.y = sin * v.x + cos * v.y;
      return results;
    }
    
    public static Vector4 NormalizeSafe(Vector4 v, Vector4 fallback)
    {
      return
        v.sqrMagnitude > MathUtil.Epsilon
        ? v.normalized
        : fallback;
    }

    // Returns a vector orthogonal to given vector.
    // If the given vector is a unit vector, the returned vector will also be a unit vector.
    public static Vector3 FindOrthogonal(Vector3 v)
    {
      if (v.x >= MathUtil.Sqrt3Inv)
        return new Vector3(v.y, -v.x, 0.0f);
      else
        return new Vector3(0.0f, v.z, -v.y);
    }

    // Yields two extra vectors that form an orthogonal basis with the given vector.
    // If the given vector is a unit vector, the returned vectors will also be unit vectors.
    public static void FormOrthogonalBasis(Vector3 v, out Vector3 a, out Vector3 b)
    {
      a = FindOrthogonal(v);
      b = Vector3.Cross(a, v);
    }

    // Both vectors must be unit vectors.
    public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
    {
      float dot = Vector3.Dot(a, b);

      if (dot > 0.99999f)
      {
        // singularity: two vectors point in the same direction
        return Vector3.Lerp(a, b, t);
      }
      else if (dot < -0.99999f)
      {
        // singularity: two vectors point in the opposite direction
        Vector3 axis = FindOrthogonal(a);
        return Quaternion.AngleAxis(180.0f * t, axis) * a;
      }

      float rad = MathUtil.AcosSafe(dot);
      return (Mathf.Sin((1.0f - t) * rad) * a + Mathf.Sin(t * rad) * b) / Mathf.Sin(rad);
    }

    public static Vector3 GetClosestPointOnSegment(Vector3 p, Vector3 segA, Vector3 segB)
    {
      Vector3 v = segB - segA;
      if (v.sqrMagnitude < MathUtil.Epsilon)
        return 0.5f * (segA + segB);

      float d = Mathf.Clamp01(Vector3.Dot(p - segA, v.normalized) / v.magnitude);
      return segA + d * v;
    }

    public static Vector3 TriLerp
    (
      ref Vector3 v000, ref Vector3 v001, ref Vector3 v010, ref Vector3 v011, 
      ref Vector3 v100, ref Vector3 v101, ref Vector3 v110, ref Vector3 v111, 
      float tx, float ty, float tz
    )
    {
      Vector3 lerpPosY00 = Vector3.Lerp(v000, v001, tx);
      Vector3 lerpPosY10 = Vector3.Lerp(v010, v011, tx);
      Vector3 lerpPosY01 = Vector3.Lerp(v100, v101, tx);
      Vector3 lerpPosY11 = Vector3.Lerp(v110, v111, tx);
      Vector3 lerpPosZ0 = Vector3.Lerp(lerpPosY00, lerpPosY10, ty);
      Vector3 lerpPosZ1 = Vector3.Lerp(lerpPosY01, lerpPosY11, ty);
      return Vector3.Lerp(lerpPosZ0, lerpPosZ1, tz);
    }

    public static Vector3 TriLerp
    (
      ref Vector3 v000, ref Vector3 v001, ref Vector3 v010, ref Vector3 v011, 
      ref Vector3 v100, ref Vector3 v101, ref Vector3 v110, ref Vector3 v111,
      bool lerpX, bool lerpY, bool lerpZ,
      float tx, float ty, float tz
    )
    {
      Vector3 lerpPosY00 = lerpX ? Vector3.Lerp(v000, v001, tx) : v000;
      Vector3 lerpPosY10 = lerpX ? Vector3.Lerp(v010, v011, tx) : v010;
      Vector3 lerpPosY01 = lerpX ? Vector3.Lerp(v100, v101, tx) : v100;
      Vector3 lerpPosY11 = lerpX ? Vector3.Lerp(v110, v111, tx) : v110;
      Vector3 lerpPosZ0 = lerpY ? Vector3.Lerp(lerpPosY00, lerpPosY10, ty) : lerpPosY00;
      Vector3 lerpPosZ1 = lerpY ? Vector3.Lerp(lerpPosY01, lerpPosY11, ty) : lerpPosY01;
      return lerpZ ? Vector3.Lerp(lerpPosZ0, lerpPosZ1, tz) : lerpPosZ0;
    }

    public static Vector3 TriLerp
    (
      ref Vector3 min, ref Vector3 max, 
      bool lerpX, bool lerpY, bool lerpZ, 
      float tx, float ty, float tz
    )
    {
      Vector3 lerpPosY00 =
        lerpX
        ? Vector3.Lerp(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z), tx)
        : new Vector3(min.x, min.y, min.z);

      Vector3 lerpPosY10 =
        lerpX
        ? Vector3.Lerp(new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z), tx)
        : new Vector3(min.x, max.y, min.z);

      Vector3 lerpPosY01 =
        lerpX
        ? Vector3.Lerp(new Vector3(min.x, min.y, max.z), new Vector3(max.x, min.y, max.z), tx)
        : new Vector3(min.x, min.y, max.z);

      Vector3 lerpPosY11 =
        lerpX
        ? Vector3.Lerp(new Vector3(min.x, max.y, max.z), new Vector3(max.x, max.y, max.z), tx)
        : new Vector3(min.x, max.y, max.z);

      Vector3 lerpPosZ0 =
        lerpY
        ? Vector3.Lerp(lerpPosY00, lerpPosY10, ty)
        : lerpPosY00;

      Vector3 lerpPosZ1 =
        lerpY
        ? Vector3.Lerp(lerpPosY01, lerpPosY11, ty)
        : lerpPosY01;

      return lerpZ ? Vector3.Lerp(lerpPosZ0, lerpPosZ1, tz) : lerpPosZ0;
    }

    public static Vector4 TriLerp
    (
      ref Vector4 v000, ref Vector4 v001, ref Vector4 v010, ref Vector4 v011, 
      ref Vector4 v100, ref Vector4 v101, ref Vector4 v110, ref Vector4 v111,
      bool lerpX, bool lerpY, bool lerpZ,
      float tx, float ty, float tz
    )
    {
      Vector4 lerpPosY00 = lerpX ? Vector4.Lerp(v000, v001, tx) : v000;
      Vector4 lerpPosY10 = lerpX ? Vector4.Lerp(v010, v011, tx) : v010;
      Vector4 lerpPosY01 = lerpX ? Vector4.Lerp(v100, v101, tx) : v100;
      Vector4 lerpPosY11 = lerpX ? Vector4.Lerp(v110, v111, tx) : v110;
      Vector4 lerpPosZ0 = lerpY ? Vector4.Lerp(lerpPosY00, lerpPosY10, ty) : lerpPosY00;
      Vector4 lerpPosZ1 = lerpY ? Vector4.Lerp(lerpPosY01, lerpPosY11, ty) : lerpPosY01;
      return lerpZ ? Vector4.Lerp(lerpPosZ0, lerpPosZ1, tz) : lerpPosZ0;
    }

    public static Vector4 TriLerp
    (
      ref Vector4 min, ref Vector4 max, 
      bool lerpX, bool lerpY, bool lerpZ, 
      float tx, float ty, float tz
    )
    {
      Vector4 lerpPosY00 =
        lerpX
        ? Vector4.Lerp(new Vector4(min.x, min.y, min.z), new Vector4(max.x, min.y, min.z), tx)
        : new Vector4(min.x, min.y, min.z);

      Vector4 lerpPosY10 =
        lerpX
        ? Vector4.Lerp(new Vector4(min.x, max.y, min.z), new Vector4(max.x, max.y, min.z), tx)
        : new Vector4(min.x, max.y, min.z);

      Vector4 lerpPosY01 =
        lerpX
        ? Vector4.Lerp(new Vector4(min.x, min.y, max.z), new Vector4(max.x, min.y, max.z), tx)
        : new Vector4(min.x, min.y, max.z);

      Vector4 lerpPosY11 =
        lerpX
        ? Vector4.Lerp(new Vector4(min.x, max.y, max.z), new Vector4(max.x, max.y, max.z), tx)
        : new Vector4(min.x, max.y, max.z);

      Vector4 lerpPosZ0 =
        lerpY
        ? Vector4.Lerp(lerpPosY00, lerpPosY10, ty)
        : lerpPosY00;

      Vector4 lerpPosZ1 =
        lerpY
        ? Vector4.Lerp(lerpPosY01, lerpPosY11, ty)
        : lerpPosY01;

      return lerpZ ? Vector4.Lerp(lerpPosZ0, lerpPosZ1, tz) : lerpPosZ0;
    }

    public static Vector3 ClampLength(Vector3 v, float minLen, float maxLen)
    {
      float lenSqr = v.sqrMagnitude;
      if (lenSqr < MathUtil.Epsilon)
        return v;

      float len = Mathf.Sqrt(lenSqr);
      return v * (Mathf.Clamp(len, minLen, maxLen) / len);
    }

    public static float MinComponent(Vector3 v)
    {
      return Mathf.Min(v.x, v.y, v.z);
    }

    public static float MaxComponent(Vector3 v)
    {
      return Mathf.Max(v.x, v.y, v.z);
    }

    public static Vector3 ComponentWiseAbs(Vector3 v)
    {
      return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    public static Vector3 ComponentWiseMult(Vector3 a, Vector3 b)
    {
      return Vector3.Scale(a, b);
    }

    public static Vector3 ComponentWiseDiv(Vector3 num, Vector3 den)
    {
      return new Vector3(num.x / den.x, num.y / den.y, num.z / den.z);
    }

    public static Vector3 ComponentWiseDivSafe(Vector3 num, Vector3 den)
    {
      return 
        new Vector3
        (
          num.x * MathUtil.InvSafe(den.x), 
          num.y * MathUtil.InvSafe(den.y), 
          num.z * MathUtil.InvSafe(den.z)
        );
    }

    public static Vector3 ClampBend(Vector3 vector, Vector3 reference, float maxBendAngle)
    {
      float vLenSqr = vector.sqrMagnitude;
      if (vLenSqr < MathUtil.Epsilon)
        return vector;

      float rLenSqr = reference.sqrMagnitude;
      if (rLenSqr < MathUtil.Epsilon)
        return vector;

      Vector3 vUnit = vector / Mathf.Sqrt(vLenSqr);
      Vector3 rUnit = reference / Mathf.Sqrt(rLenSqr);

      Vector3 cross = Vector3.Cross(rUnit, vUnit);
      float dot = Vector3.Dot(rUnit, vUnit);
      Vector3 axis = 
        cross.sqrMagnitude > MathUtil.Epsilon 
          ? cross.normalized 
          : FindOrthogonal(rUnit);
      float angle = Mathf.Acos(Mathf.Clamp01(dot));

      if (angle <= maxBendAngle)
        return vector;

      Quaternion clampedBendRot = QuaternionUtil.AxisAngle(axis, maxBendAngle);
      Vector3 result = clampedBendRot * reference;
      result *= Mathf.Sqrt(vLenSqr) / Mathf.Sqrt(rLenSqr);

      return result;
    }
  }
}
