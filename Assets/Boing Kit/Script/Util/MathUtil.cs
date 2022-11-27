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
  public class MathUtil
  {
    public static readonly float Pi        = Mathf.PI;
    public static readonly float TwoPi     = 2.0f * Mathf.PI;
    public static readonly float HalfPi    = Mathf.PI / 2.0f;
    public static readonly float QuaterPi  = Mathf.PI / 4.0f;
    public static readonly float SixthPi   = Mathf.PI / 6.0f;

    public static readonly float Sqrt2    = Mathf.Sqrt(2.0f);
    public static readonly float Sqrt2Inv = 1.0f / Mathf.Sqrt(2.0f);
    public static readonly float Sqrt3    = Mathf.Sqrt(3.0f);
    public static readonly float Sqrt3Inv = 1.0f / Mathf.Sqrt(3.0f);

    public static readonly float Epsilon = 1.0e-6f;
    public static readonly float Rad2Deg = 180.0f / Mathf.PI;
    public static readonly float Deg2Rad = Mathf.PI / 180.0f;

    public static float AsinSafe(float x)
    {
      return Mathf.Asin(Mathf.Clamp(x, -1.0f, 1.0f));
    }

    public static float AcosSafe(float x)
    {
      return Mathf.Acos(Mathf.Clamp(x, -1.0f, 1.0f));
    }

    public static float InvSafe(float x)
    {
      return 1.0f / Mathf.Max(Epsilon, x);
    }

    public static float PointLineDist(Vector2 point, Vector2 linePos, Vector2 lineDir)
    {
      var delta = point - linePos;
      return (delta - Vector2.Dot(delta, lineDir) * lineDir).magnitude;
    }

    public static float PointSegmentDist(Vector2 point, Vector2 segmentPosA, Vector2 segmentPosB)
    {
      var segmentVec = segmentPosB - segmentPosA;
      float segmentDistInv = 1.0f / segmentVec.magnitude;
      var segmentDir = segmentVec * segmentDistInv;
      var delta = point - segmentPosA;
      float t = Vector2.Dot(delta, segmentDir) * segmentDistInv;
      var closest = segmentPosA + Mathf.Clamp(t, 0.0f, 1.0f) * segmentVec;
      return (closest - point).magnitude;
    }

    public static float Seek(float current, float target, float maxDelta)
    {
      float delta = target - current;
      delta = Mathf.Sign(delta) * Mathf.Min(maxDelta, Mathf.Abs(delta));
      return current + delta;
    }

    public static Vector2 Seek(Vector2 current, Vector2 target, float maxDelta)
    {
      Vector2 delta = target - current;
      float deltaMag = delta.magnitude;
      if (deltaMag < Epsilon)
        return target;

      delta = Mathf.Min(maxDelta, deltaMag) * delta.normalized;
      return current + delta;
    }

    public static float Remainder(float a, float b)
    {
      return a - (a / b) * b;
    }

    public static int Remainder(int a, int b)
    {
      return a - (a / b) * b;
    }

    public static float Modulo(float a, float b)
    {
      return Mathf.Repeat(a, b);
    }

    public static int Modulo(int a, int b)
    {
      int r = a % b;
      return r >= 0 ? r : r + b;
    }
  }
}
