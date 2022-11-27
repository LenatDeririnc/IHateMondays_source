/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
  public enum ParameterMode
  {
    Exponential, 
    OscillationByHalfLife, 
    OscillationByDampingRatio, 
  };

  public enum TwoDPlaneEnum { XY, XZ, YZ };

  public struct Aabb
  {
    public Vector3 Min;
    public Vector3 Max;

    public float MinX { get { return Min.x; } set { Min.x = value; } }
    public float MinY { get { return Min.y; } set { Min.y = value; } }
    public float MinZ { get { return Min.z; } set { Min.z = value; } }
    public float MaxX { get { return Max.x; } set { Max.x = value; } }
    public float MaxY { get { return Max.y; } set { Max.y = value; } }
    public float MaxZ { get { return Max.z; } set { Max.z = value; } }

    public Vector3 Center { get { return 0.5f * (Min + Max); } }
    public Vector3 Size
    {
      get
      {
        Vector3 size = Max - Min;
        size.x = Mathf.Max(0.0f, size.x);
        size.y = Mathf.Max(0.0f, size.y);
        size.z = Mathf.Max(0.0f, size.z);
        return size;
      }
    }

    public static Aabb Empty
    {
      get
      {
        return 
          new Aabb
          (
            new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), 
            new Vector3(float.MinValue, float.MinValue, float.MinValue)
          );
      }
    }

    public static Aabb FromPoint(Vector3 p)
    {
      var aabb = Empty;
      aabb.Include(p);
      return aabb;
    }
    
    public static Aabb FromPoints(Vector3 a, Vector3 b)
    {
      var aabb = Empty;
      aabb.Include(a);
      aabb.Include(b);
      return aabb;
    }

    public Aabb(Vector3 min, Vector3 max)
    {
      Min = min;
      Max = max;
    }

    public void Include(Vector3 p)
    {
      MinX = Mathf.Min(MinX, p.x);
      MinY = Mathf.Min(MinY, p.y);
      MinZ = Mathf.Min(MinZ, p.z);
      MaxX = Mathf.Max(MaxX, p.x);
      MaxY = Mathf.Max(MaxY, p.y);
      MaxZ = Mathf.Max(MaxZ, p.z);
    }

    public bool Contains(Vector3 p)
    {
      return
           MinX <= p.x 
        && MinY <= p.y 
        && MinZ <= p.z 
        && MaxX >= p.x 
        && MaxY >= p.y 
        && MaxZ >= p.z;
    }

    public bool ContainsX(Vector3 p)
    {
      return MinX <= p.x && MaxX >= p.x;
    }

    public bool ContainsY(Vector3 p)
    {
      return MinY <= p.y && MaxY >= p.y;
    }

    public bool ContainsZ(Vector3 p)
    {
      return MinZ <= p.z && MaxZ >= p.z;
    }

    public bool Intersects(Aabb rhs)
    {
      return
           MinX <= rhs.MaxX
        && MinY <= rhs.MaxY
        && MinZ <= rhs.MaxZ
        && MaxX >= rhs.MinX
        && MaxY >= rhs.MinY
        && MaxZ >= rhs.MinZ;
    }

    public bool Intersects(ref BoingEffector.Params effector)
    {
      return
        effector.Bits.IsBitSet((int) BoingWork.EffectorFlags.ContinuousMotion) 
          ? Intersects
            (
              FromPoints
              (
                effector.PrevPosition,
                effector.CurrPosition
              ).Expand(effector.Radius)
            )
          : Intersects
            (
              FromPoint
              (
                effector.CurrPosition
              ).Expand(effector.Radius)
            );
    }

    public Aabb Expand(float amount)
    {
      MinX -= amount;
      MinY -= amount;
      MinZ -= amount;
      MaxX += amount;
      MaxY += amount;
      MaxZ += amount;
      return this;
    }
  }

  [Serializable]
  public struct Bits32
  {
    [SerializeField] private int m_bits;
    public int IntValue { get { return m_bits; } }

    public Bits32(int bits = 0) { m_bits = bits; }

    public void Clear() { m_bits = 0;  }

    public void SetBit(int index, bool value)
    {
      if (value)
        m_bits |= (1 << index);
      else
        m_bits &= ~(1 << index);
    }

    public bool IsBitSet(int index)
    {
      return (m_bits & (1 << index)) != 0;
    }
  }

  public struct BitArray
  {
    private int[] m_aBlock;
    public int[] Blocks { get { return m_aBlock; } }

    static int GetBlockIndex(int index)
    {
      return index / sizeof(int);
    }

    static int GetSubIndex(int index)
    {
      return index % sizeof(int);
    }

    static void SetBit(int index, bool value, int[] blocks)
    {
      int iBlock = GetBlockIndex(index);
      int iSub = GetSubIndex(index);
      if (value)
        blocks[iBlock] |= (1 << iSub);
      else
        blocks[iBlock] &= ~(1 << iSub);
    }

    static bool IsBitSet(int index, int[] blocks)
    {
      return (blocks[GetBlockIndex(index)] & (1 << GetSubIndex(index))) != 0;
    }

    public BitArray(int capacity)
    {
      int numBlocks = (capacity + sizeof(int) - 1) / sizeof(int);
      m_aBlock = new int[numBlocks];
      Clear();
    }

    public void Resize(int capacity)
    {
      int numBlocks = (capacity + sizeof(int) - 1) / sizeof(int);
      if (numBlocks <= m_aBlock.Length)
        return;

      var aNewBlock = new int[numBlocks];
      for (int i = 0, n = m_aBlock.Length; i < n; ++i)
        aNewBlock[i] = m_aBlock[i];
      m_aBlock = aNewBlock;
    }

    public void Clear()
    {
      SetAllBits(false);
    }

    public void SetAllBits(bool value)
    {
      int v = value ? ~0 : 1;
      for (int i = 0, n = m_aBlock.Length; i < n; ++i)
        m_aBlock[i] = v;
    }

    public void SetBit(int index, bool value)
    {
      SetBit(index, value, m_aBlock);
    }

    public bool IsBitSet(int index)
    {
      return IsBitSet(index, m_aBlock);
    }
  }
}
