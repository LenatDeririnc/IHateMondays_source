/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System.Runtime.InteropServices;

using UnityEngine;

namespace BoingKit
{
  public class Codec
  {
    // Vector2 between 0.0 and 1.0
    // https://stackoverflow.com/questions/17638800/storing-two-float-values-in-a-single-float-variable
    //-----------------------------------------------------------------------------

    public static float PackSaturated(float a, float b)
    {
      const int precision = 4096;
      a = Mathf.Floor(a * (precision - 1));
      b = Mathf.Floor(b * (precision - 1));
      return a * precision + b;
    }

    public static float PackSaturated(Vector2 v)
    {
      return PackSaturated(v.x, v.y);
    }

    public static Vector2 UnpackSaturated(float f)
    {
      const int precision = 4096;
      return new Vector2(Mathf.Floor(f / precision), Mathf.Repeat(f, precision)) / (precision - 1);
    }

    //-----------------------------------------------------------------------------
    // end: Vector2 between 0.0 and 1.0


    // normals
    // https://knarkowicz.wordpress.com/2014/04/16/octahedron-normal-vector-encoding/
    //-----------------------------------------------------------------------------

    public static Vector2 OctWrap(Vector2 v)
    {
      return
        (Vector2.one - new Vector2(Mathf.Abs(v.y), Mathf.Abs(v.x))) 
        * new Vector2(Mathf.Sign(v.x), Mathf.Sign(v.y));
    }

    public static float PackNormal(Vector3 n)
    {
      n /= (Mathf.Abs(n.x) + Mathf.Abs(n.y) + Mathf.Abs(n.z));
      Vector2 n2 = n.z >= 0.0f ? new Vector2(n.x, n.y) : OctWrap(new Vector2(n.x, n.y));
      n2 = n2 * 0.5f + 0.5f * Vector2.one;
      return PackSaturated(n2);
    }

    public static Vector3 UnpackNormal(float f)
    {
      Vector2 v = UnpackSaturated(f);
      v = v * 2.0f - Vector2.one;
      Vector3 n = new Vector3(v.x, v.y, 1.0f - Mathf.Abs(v.x) - Mathf.Abs(v.y));
      float t = Mathf.Clamp01(-n.z);
      n.x += n.x >= 0.0f ? -t : t;
      n.y += n.y >= 0.0f ? -t : t;
      return n.normalized;
    }

    //-----------------------------------------------------------------------------
    // end: normals


    // colors
    //-----------------------------------------------------------------------------

    public static uint PackRgb(Color color)
    {
      return 
          (((uint) (color.b * 255)) << 16) 
        | (((uint) (color.g * 255)) <<  8) 
        | (((uint) (color.r * 255)) <<  0);
    }

    public static Color UnpackRgb(uint i)
    {
      return
        new Color
        (
          ((i & 0x000000FF) >>  0) / 255.0f, 
          ((i & 0x0000FF00) >>  8) / 255.0f, 
          ((i & 0x00FF0000) >> 16) / 255.0f
        );
    }

    public static uint PackRgba(Color color)
    {
      return 
          (((uint) (color.a * 255)) << 24) 
        | (((uint) (color.b * 255)) << 16) 
        | (((uint) (color.g * 255)) <<  8) 
        | (((uint) (color.r * 255)) <<  0);
    }

    public static Color UnpackRgba(uint i)
    {
      return
        new Color
        (
          ((i & 0x000000FF) >>  0) / 255.0f, 
          ((i & 0x0000FF00) >>  8) / 255.0f, 
          ((i & 0x00FF0000) >> 16) / 255.0f, 
          ((i & 0xFF000000) >> 24) / 255.0f
        );
    }

    //-----------------------------------------------------------------------------
    // end: colors


    // bits
    //-----------------------------------------------------------------------------

    public static uint Pack8888(uint x, uint y, uint z, uint w)
    {
      return 
          ((x & 0xFF) << 24) 
        | ((y & 0xFF) << 16) 
        | ((z & 0xFF) <<  8) 
        | ((w & 0xFF) <<  0);
    }

    public static void Unpack8888(uint i, out uint x, out uint y, out uint z, out uint w)
    {
      x = (i >> 24) & 0xFF;
      y = (i >> 16) & 0xFF;
      z = (i >>  8) & 0xFF;
      w = (i >>  0) & 0xFF;
    }

    //-----------------------------------------------------------------------------
    // end: bits


    // hash
    //-----------------------------------------------------------------------------

    public static readonly int FnvDefaultBasis = unchecked((int) 2166136261);
    public static readonly int FnvPrime = 16777619;

    [StructLayout(LayoutKind.Explicit)]
    private struct IntFloat
    {
      [FieldOffset(0)]
      public int IntValue;
      [FieldOffset(0)]
      public float FloatValue;
    }
    private static int IntReinterpret(float f)
    {
      return (new IntFloat { FloatValue = f }).IntValue;
    }

    public static int HashConcat(int hash, int i)
    {
      return (hash ^ i) * FnvPrime;
    }

    public static int HashConcat(int hash, long i)
    {
      hash = HashConcat(hash, (int) (i & 0xFFFFFFFF));
      hash = HashConcat(hash, (int) (i >> 32));
      return hash;
    }

    public static int HashConcat(int hash, float f)
    {
      return HashConcat(hash, IntReinterpret(f));
    }

    public static int HashConcat(int hash, bool b)
    {
      return HashConcat(hash, b ? 1 : 0);
    }

    public static int HashConcat(int hash, params int [] ints)
    {
      foreach (int i in ints)
        hash = HashConcat(hash, i);
      return hash;
    }

    public static int HashConcat(int hash, params float [] floats)
    {
      foreach (float f in floats)
        hash = HashConcat(hash, f);
      return hash;
    }

    public static int HashConcat(int hash, Vector2 v)
    {
      return HashConcat(hash, v.x, v.y);
    }

    public static int HashConcat(int hash, Vector3 v)
    {
      return HashConcat(hash, v.x, v.y, v.z);
    }

    public static int HashConcat(int hash, Vector4 v)
    {
      return HashConcat(hash, v.x, v.y, v.z, v.w);
    }

    public static int HashConcat(int hash, Quaternion q)
    {
      return HashConcat(hash, q.x, q.y, q.z, q.w);
    }

    public static int HashConcat(int hash, Color c)
    {
      return HashConcat(hash, c.r, c.g, c.b, c.a);
    }

    public static int HashConcat(int hash, Transform t)
    {
      return HashConcat(hash, t.GetHashCode());
    }

    public static int Hash(int i)
    {
      return HashConcat(FnvDefaultBasis, i);
    }

    public static int Hash(long i)
    {
      return HashConcat(FnvDefaultBasis, i);
    }

    public static int Hash(float f)
    {
      return HashConcat(FnvDefaultBasis, f);
    }

    public static int Hash(bool b)
    {
      return HashConcat(FnvDefaultBasis, b);
    }

    public static int Hash(params int[] ints)
    {
      return HashConcat(FnvDefaultBasis, ints);
    }

    public static int Hash(params float[] floats)
    {
      return HashConcat(FnvDefaultBasis, floats);
    }

    public static int Hash(Vector2 v)
    {
      return HashConcat(FnvDefaultBasis, v);
    }

    public static int Hash(Vector3 v)
    {
      return HashConcat(FnvDefaultBasis, v);
    }

    public static int Hash(Vector4 v)
    {
      return HashConcat(FnvDefaultBasis, v);
    }

    public static int Hash(Quaternion q)
    {
      return HashConcat(FnvDefaultBasis, q);
    }

    public static int Hash(Color c)
    {
      return HashConcat(FnvDefaultBasis, c);
    }

    private static int HashTransformHierarchyRecurvsive(int hash, Transform t)
    {
      hash = HashConcat(hash, t);
      hash = HashConcat(hash, t.childCount);
      for (int i = 0; i < t.childCount; ++i)
      {
        hash = HashTransformHierarchyRecurvsive(hash, t.GetChild(i));
      }
      return hash;
    }

    public static int HashTransformHierarchy(Transform t)
    {
      return HashTransformHierarchyRecurvsive(FnvDefaultBasis, t);
    }

    //-----------------------------------------------------------------------------
    // end: hash
  }
}

