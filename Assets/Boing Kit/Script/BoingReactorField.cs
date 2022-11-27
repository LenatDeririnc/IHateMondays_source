/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;

namespace BoingKit
{
  public class BoingReactorField : BoingBase
  {
    public enum HardwareModeEnum
    {
      CPU, 
      GPU, 
    }

    public enum CellMoveModeEnum
    {
      Follow,
      WrapAround,
    }

    public enum FalloffModeEnum
    {
      None, 
      Circle, 
      Square,
    }

    public enum FalloffDimensionsEnum
    {
      XYZ, 
      XY, 
      XZ, 
      YZ, 
    }

    public class ShaderPropertyIdSet
    {
      public int MoveParams;
      public int WrapParams;
      public int Effectors;
      public int EffectorIndices;
      public int ReactorParams;
      public int ComputeFieldParams;
      public int ComputeCells;
      public int RenderFieldParams;
      public int RenderCells;
      public int PositionSampleMultiplier;
      public int RotationSampleMultiplier;
      public int PropagationParams;

      public ShaderPropertyIdSet()
      {
        MoveParams = Shader.PropertyToID("moveParams");
        WrapParams = Shader.PropertyToID("wrapParams");
        Effectors = Shader.PropertyToID("aEffector");
        EffectorIndices = Shader.PropertyToID("aEffectorIndex");
        ReactorParams = Shader.PropertyToID("reactorParams");
        ComputeFieldParams = Shader.PropertyToID("fieldParams");
        ComputeCells = Shader.PropertyToID("aCell");
        RenderFieldParams = Shader.PropertyToID("aBoingFieldParams");
        RenderCells = Shader.PropertyToID("aBoingFieldCell");
        PositionSampleMultiplier = Shader.PropertyToID("positionSampleMultiplier");
        RotationSampleMultiplier = Shader.PropertyToID("rotationSampleMultiplier");
        PropagationParams = Shader.PropertyToID("propagationParams");
      }
    }
    private static ShaderPropertyIdSet s_shaderPropertyId;
    public static ShaderPropertyIdSet ShaderPropertyId
    {
      get
      {
        if (s_shaderPropertyId == null)
          s_shaderPropertyId = new ShaderPropertyIdSet();
        return s_shaderPropertyId;
      }
    }
    public bool UpdateShaderConstants(MaterialPropertyBlock props, float positionSampleMultiplier = 1.0f, float rotationSampleMultiplier = 1.0f)
    {
      if (HardwareMode != HardwareModeEnum.GPU)
        return false;

      if (m_fieldParamsBuffer == null || m_cellsBuffer == null)
        return false;

      props.SetFloat(ShaderPropertyId.PositionSampleMultiplier, positionSampleMultiplier);
      props.SetFloat(ShaderPropertyId.RotationSampleMultiplier, rotationSampleMultiplier);
      props.SetBuffer(ShaderPropertyId.RenderFieldParams, m_fieldParamsBuffer);
      props.SetBuffer(ShaderPropertyId.RenderCells, m_cellsBuffer);

      return true;
    }
    public bool UpdateShaderConstants(Material material, float positionSampleMultiplier = 1.0f, float rotationSampleMultiplier = 1.0f)
    {
      if (HardwareMode != HardwareModeEnum.GPU)
        return false;

      if (m_fieldParamsBuffer == null || m_cellsBuffer == null)
        return false;

      material.SetFloat(ShaderPropertyId.PositionSampleMultiplier, positionSampleMultiplier);
      material.SetFloat(ShaderPropertyId.RotationSampleMultiplier, rotationSampleMultiplier);
      material.SetBuffer(ShaderPropertyId.RenderFieldParams, m_fieldParamsBuffer);
      material.SetBuffer(ShaderPropertyId.RenderCells, m_cellsBuffer);

      return true;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    private struct FieldParams
    {
      public static readonly int Stride = //   112 bytes
          16 * sizeof(float)              // =  64 bytes
        + 12 * sizeof(int);               // +  48 bytes

      // bytes 0-15 (16 bytes)
      public int CellsX;
      public int CellsY;
      public int CellsZ;
      public int NumEffectors;

      // bytes 16-31 (16 bytes)
      public int iCellBaseX;
      public int iCellBaseY;
      public int iCellBaseZ;
      public int m_padding0;

      // bytes 32-47
      public int FalloffMode;
      public int FalloffDimensions;
      public int PropagationDepth;
      public int m_padding1;

      // bytes 48-63 (16 bytes)
      public Vector3 GridCenter;
      private float m_padding3;

      // bytes 64-79 (16 bytes
      public Vector3 UpWs;
      private float m_padding2;

      // bytes 80-95 (16 bytes)
      public Vector3 FieldPosition;
      public float m_padding4;

      // bytes 96-111
      public float FalloffRatio;
      public float CellSize;
      public float DeltaTime;
      private float m_padding5;

      private void SuppressWarnings()
      {
        m_padding0 = 0;
        m_padding1 = 0;
        m_padding2 = 0;
        m_padding4 = 0.0f;
        m_padding5 = 0.0f;
        m_padding0 = m_padding1;
        m_padding1 = (int) m_padding2;
        m_padding2 = m_padding3;
        m_padding3 = m_padding4;
        m_padding4 = m_padding5;
      }
    }
    private FieldParams m_fieldParams;

    public HardwareModeEnum HardwareMode = HardwareModeEnum.GPU;
    private HardwareModeEnum m_hardwareMode;

    public CellMoveModeEnum CellMoveMode = CellMoveModeEnum.WrapAround;
    private CellMoveModeEnum m_cellMoveMode;

    [Range(0.1f, 10.0f)] public float CellSize = 1.0f;
    public int CellsX = 8;
    public int CellsY = 1;
    public int CellsZ = 8;
    private int m_cellsX = -1;
    private int m_cellsY = -1;
    private int m_cellsZ = -1;
    private int m_iCellBaseX = 0;
    private int m_iCellBaseY = 0;
    private int m_iCellBaseZ = 0;

    public FalloffModeEnum FalloffMode = FalloffModeEnum.Square;
    [Range(0.0f, 1.0f)] public float FalloffRatio = 0.7f;

    public FalloffDimensionsEnum FalloffDimensions = FalloffDimensionsEnum.XZ;

    public BoingEffector[] Effectors = new BoingEffector[1];
    private int m_numEffectors = -1;

    private Aabb m_bounds;

    public bool TwoDDistanceCheck      = false;
    public bool TwoDPositionInfluence  = false;
    public bool TwoDRotationInfluence  = false;
    public bool EnablePositionEffect   = true;
    public bool EnableRotationEffect   = true;
    public bool GlobalReactionUpVector = false;

    public BoingWork.Params Params;
    public SharedBoingParams SharedParams;

    public bool EnablePropagation = false;
    [Range(0.0f, 1.0f)] public float PositionPropagation = 1.0f;
    [Range(0.0f, 1.0f)] public float RotationPropagation = 1.0f;
    [Range(1, 3)] public int PropagationDepth = 1;
    public bool AnchorPropagationAtBorder = false;

    static private readonly float kPropagationFactor = 600.0f;

    // CPU resources
    private BoingWork.Params.InstanceData[,,] m_aCpuCell;

    // GPU resources
    private ComputeShader m_shader;
    private ComputeBuffer m_effectorIndexBuffer;
    private ComputeBuffer m_reactorParamsBuffer;
    private ComputeBuffer m_fieldParamsBuffer;
    private ComputeBuffer m_cellsBuffer;
    private int m_gpuResourceSetId = -1;

    public int GpuResourceSetId { get { return m_gpuResourceSetId; } }

    private class ComputeKernelId
    {
      public int InitKernel;
      public int MoveKernel;
      public int WrapXKernel;
      public int WrapYKernel;
      public int WrapZKernel;
      public int ExecuteKernel;
    }
    private static ComputeKernelId s_computeKernelId;

    private bool m_init;
    private Vector3 m_gridCenter;
    private Vector3 m_qPrevGridCenterNorm;

    public BoingReactorField()
    {
      Params.Init();
      m_bounds = Aabb.Empty;
      m_init = false;
    }

    public void Reboot()
    {
      m_gridCenter = transform.position;

      Vector3 qCurrGridCenterNorm = QuantizeNorm(m_gridCenter);
      m_qPrevGridCenterNorm = qCurrGridCenterNorm;

      switch (CellMoveMode)
      {
        case CellMoveModeEnum.Follow:
          m_gridCenter = transform.position;
          m_iCellBaseX = 0;
          m_iCellBaseY = 0;
          m_iCellBaseZ = 0;
          m_iCellBaseZ = 0;
          m_iCellBaseZ = 0;
          break;

        case CellMoveModeEnum.WrapAround:
          m_gridCenter = qCurrGridCenterNorm * CellSize;
          m_iCellBaseX = MathUtil.Modulo((int) m_qPrevGridCenterNorm.x, CellsX);
          m_iCellBaseY = MathUtil.Modulo((int) m_qPrevGridCenterNorm.y, CellsY);
          m_iCellBaseZ = MathUtil.Modulo((int) m_qPrevGridCenterNorm.z, CellsZ);
          break;
      }
    }

    public void OnEnable()
    {
      Reboot();
      BoingManager.Register(this);
    }

    public void Start()
    {
      // need to do this on start again to handle instantiated parented object
      Reboot();

      m_cellMoveMode = CellMoveMode;
    }

    public void OnDisable()
    {
      BoingManager.Unregister(this);

      DisposeCpuResources();
      DisposeGpuResources();
    }

    public void DisposeCpuResources()
    {
      m_aCpuCell = null;
    }

    public void DisposeGpuResources()
    {
      if (m_effectorIndexBuffer != null)
      {
        m_effectorIndexBuffer.Dispose();
        m_effectorIndexBuffer = null;
      }

      if (m_reactorParamsBuffer != null)
      {
        m_reactorParamsBuffer.Dispose();
        m_reactorParamsBuffer = null;
      }

      if (m_fieldParamsBuffer != null)
      {
        m_fieldParamsBuffer.Dispose();
        m_fieldParamsBuffer = null;
      }

      if (m_cellsBuffer != null)
      {
        m_cellsBuffer.Dispose();
        m_cellsBuffer = null;
      }

      if (m_cellsBuffer != null)
      {
        m_cellsBuffer.Dispose();
        m_cellsBuffer = null;
      }
    }

    private static Vector3[] s_aCellOffset = new Vector3[8];
    public bool SampleCpuGrid(Vector3 p, out Vector3 positionOffset, out Vector4 rotationOffset)
    {
      bool inRange = false;
      switch (FalloffDimensions)
      {
        case FalloffDimensionsEnum.XYZ: inRange = m_bounds.Contains(p); break;
        case FalloffDimensionsEnum.XY:  inRange = m_bounds.ContainsX(p) && m_bounds.ContainsY(p); break;
        case FalloffDimensionsEnum.XZ:  inRange = m_bounds.ContainsX(p) && m_bounds.ContainsZ(p); break;
        case FalloffDimensionsEnum.YZ:  inRange = m_bounds.ContainsY(p) && m_bounds.ContainsZ(p); break;
      }
      if (!inRange)
      {
        positionOffset = Vector3.zero;
        rotationOffset = QuaternionUtil.ToVector4(Quaternion.identity);
        return false;
      }

      float halfCellSize = 0.5f * CellSize;

      Vector3 pLs = p - (m_gridCenter + GetCellCenterOffset(0, 0, 0));

      Vector3 qNormLower = QuantizeNorm(pLs + new Vector3(-halfCellSize, -halfCellSize, -halfCellSize));
      Vector3 qLower = qNormLower * CellSize;

      int iLowerRawX = Mathf.Clamp((int) qNormLower.x, 0, CellsX - 1);
      int iLowerRawY = Mathf.Clamp((int) qNormLower.y, 0, CellsY - 1);
      int iLowerRawZ = Mathf.Clamp((int) qNormLower.z, 0, CellsZ - 1);
      int iUpperRawX = Mathf.Min(iLowerRawX + 1, CellsX - 1);
      int iUpperRawY = Mathf.Min(iLowerRawY + 1, CellsY - 1);
      int iUpperRawZ = Mathf.Min(iLowerRawZ + 1, CellsZ - 1);
      int iLowerResX;
      int iLowerResY;
      int iLowerResZ;
      int iUpperResX;
      int iUpperResY;
      int iUpperResZ;
      ResolveCellIndex(iLowerRawX, iLowerRawY, iLowerRawZ, 1, out iLowerResX, out iLowerResY, out iLowerResZ);
      ResolveCellIndex(iUpperRawX, iUpperRawY, iUpperRawZ, 1, out iUpperResX, out iUpperResY, out iUpperResZ);

      bool lerpX = (iLowerResX != iUpperResX);
      bool lerpY = (iLowerResY != iUpperResY);
      bool lerpZ = (iLowerResZ != iUpperResZ);

      Vector3 t = (pLs - qLower) / CellSize;

      Vector3 centerToSample = p - transform.position;
      switch (FalloffDimensions)
      {
        case FalloffDimensionsEnum.XY: centerToSample.z = 0.0f; break;
        case FalloffDimensionsEnum.XZ: centerToSample.y = 0.0f; break;
        case FalloffDimensionsEnum.YZ: centerToSample.x = 0.0f; break;
      }

      int maxCells = Mathf.Max(CellsX, CellsY, CellsZ);
      float influenceMult = 1.0f;
      switch (FalloffMode)
      {
        case FalloffModeEnum.Circle:
          float maxHalfExtent = halfCellSize * maxCells;
          Vector3 radialScaleInv = new Vector3(maxCells / (float) CellsX, maxCells / (float) CellsY, maxCells / (float) CellsZ);
          centerToSample.x *= radialScaleInv.x;
          centerToSample.y *= radialScaleInv.y;
          centerToSample.z *= radialScaleInv.z;
          float radialDist = centerToSample.magnitude;
          float circleFalloffStartDist = Mathf.Max(0.0f, FalloffRatio * maxHalfExtent - halfCellSize);
          float circleFalloffDist = Mathf.Max(MathUtil.Epsilon, (1.0f - FalloffRatio) * maxHalfExtent - halfCellSize);
          influenceMult = 1.0f - Mathf.Clamp01((radialDist - circleFalloffStartDist) / circleFalloffDist);
          break;

        case FalloffModeEnum.Square:
          Vector3 halfExtent = halfCellSize * new Vector3(CellsX, CellsY, CellsZ);
          Vector3 squareFalloffStartDist = FalloffRatio * halfExtent - halfCellSize * Vector3.one;
          squareFalloffStartDist.x = Mathf.Max(0.0f, squareFalloffStartDist.x);
          squareFalloffStartDist.y = Mathf.Max(0.0f, squareFalloffStartDist.y);
          squareFalloffStartDist.z = Mathf.Max(0.0f, squareFalloffStartDist.z);
          Vector3 squareFalloffDist = (1.0f - FalloffRatio) * halfExtent - halfCellSize * Vector3.one;
          squareFalloffDist.x = Mathf.Max(MathUtil.Epsilon, squareFalloffDist.x);
          squareFalloffDist.y = Mathf.Max(MathUtil.Epsilon, squareFalloffDist.y);
          squareFalloffDist.z = Mathf.Max(MathUtil.Epsilon, squareFalloffDist.z);
          Vector3 influenceMultVec =
            new Vector3
            (
              1.0f - Mathf.Clamp01((Mathf.Abs(centerToSample.x) - squareFalloffStartDist.x) / squareFalloffDist.x), 
              1.0f - Mathf.Clamp01((Mathf.Abs(centerToSample.y) - squareFalloffStartDist.y) / squareFalloffDist.y), 
              1.0f - Mathf.Clamp01((Mathf.Abs(centerToSample.z) - squareFalloffStartDist.z) / squareFalloffDist.z)
            );
          switch (FalloffDimensions)
          {
            case FalloffDimensionsEnum.XY: influenceMultVec.x = 1.0f; break;
            case FalloffDimensionsEnum.XZ: influenceMultVec.y = 1.0f; break;
            case FalloffDimensionsEnum.YZ: influenceMultVec.z = 1.0f; break;
          }
          influenceMult = Mathf.Min(influenceMultVec.x, influenceMultVec.y, influenceMultVec.z);
          break;
      }

      s_aCellOffset[0] = m_aCpuCell[iLowerResZ, iLowerResY, iLowerResX].PositionSpring.Value - m_gridCenter - GetCellCenterOffset(iLowerRawX, iLowerRawY, iLowerRawZ); 
      s_aCellOffset[1] = m_aCpuCell[iLowerResZ, iLowerResY, iUpperResX].PositionSpring.Value - m_gridCenter - GetCellCenterOffset(iUpperRawX, iLowerRawY, iLowerRawZ); 
      s_aCellOffset[2] = m_aCpuCell[iLowerResZ, iUpperResY, iLowerResX].PositionSpring.Value - m_gridCenter - GetCellCenterOffset(iLowerRawX, iUpperRawY, iLowerRawZ); 
      s_aCellOffset[3] = m_aCpuCell[iLowerResZ, iUpperResY, iUpperResX].PositionSpring.Value - m_gridCenter - GetCellCenterOffset(iUpperRawX, iUpperRawY, iLowerRawZ); 
      s_aCellOffset[4] = m_aCpuCell[iUpperResZ, iLowerResY, iLowerResX].PositionSpring.Value - m_gridCenter - GetCellCenterOffset(iLowerRawX, iLowerRawY, iUpperRawZ); 
      s_aCellOffset[5] = m_aCpuCell[iUpperResZ, iLowerResY, iUpperResX].PositionSpring.Value - m_gridCenter - GetCellCenterOffset(iUpperRawX, iLowerRawY, iUpperRawZ); 
      s_aCellOffset[6] = m_aCpuCell[iUpperResZ, iUpperResY, iLowerResX].PositionSpring.Value - m_gridCenter - GetCellCenterOffset(iLowerRawX, iUpperRawY, iUpperRawZ); 
      s_aCellOffset[7] = m_aCpuCell[iUpperResZ, iUpperResY, iUpperResX].PositionSpring.Value - m_gridCenter - GetCellCenterOffset(iUpperRawX, iUpperRawY, iUpperRawZ); 
      positionOffset =
        VectorUtil.TriLerp
        (
          ref s_aCellOffset[0],
          ref s_aCellOffset[1],
          ref s_aCellOffset[2],
          ref s_aCellOffset[3],
          ref s_aCellOffset[4],
          ref s_aCellOffset[5],
          ref s_aCellOffset[6],
          ref s_aCellOffset[7],
          lerpX, lerpY, lerpZ,
          t.x, t.y, t.z
        );
      rotationOffset = 
        VectorUtil.TriLerp
        (
          ref m_aCpuCell[iLowerResZ, iLowerResY, iLowerResX].RotationSpring.ValueVec, 
          ref m_aCpuCell[iLowerResZ, iLowerResY, iUpperResX].RotationSpring.ValueVec, 
          ref m_aCpuCell[iLowerResZ, iUpperResY, iLowerResX].RotationSpring.ValueVec, 
          ref m_aCpuCell[iLowerResZ, iUpperResY, iUpperResX].RotationSpring.ValueVec, 
          ref m_aCpuCell[iUpperResZ, iLowerResY, iLowerResX].RotationSpring.ValueVec, 
          ref m_aCpuCell[iUpperResZ, iLowerResY, iUpperResX].RotationSpring.ValueVec, 
          ref m_aCpuCell[iUpperResZ, iUpperResY, iLowerResX].RotationSpring.ValueVec, 
          ref m_aCpuCell[iUpperResZ, iUpperResY, iUpperResX].RotationSpring.ValueVec, 
          lerpX, lerpY, lerpZ, 
          t.x, t.y, t.z
        );

      positionOffset *= influenceMult;
      rotationOffset = QuaternionUtil.ToVector4(QuaternionUtil.Pow(QuaternionUtil.FromVector4(rotationOffset), influenceMult));

      return true;
    }


    #region PrepareExecute

    private void UpdateFieldParamsGpu()
    {
      m_fieldParams.CellsX = CellsX;
      m_fieldParams.CellsY = CellsY;
      m_fieldParams.CellsZ = CellsZ;
      m_fieldParams.NumEffectors = 0;
      if (Effectors != null)
      {
        foreach (var effectorGo in Effectors)
        {
          if (effectorGo == null)
            continue;

          var effector = effectorGo.GetComponent<BoingEffector>();
          if (effector == null)
            continue;

          if (!effector.isActiveAndEnabled)
            continue;

          ++m_fieldParams.NumEffectors;
        }
      }

      m_fieldParams.iCellBaseX = m_iCellBaseX;
      m_fieldParams.iCellBaseY = m_iCellBaseY;
      m_fieldParams.iCellBaseZ = m_iCellBaseZ;

      m_fieldParams.FalloffMode = (int) FalloffMode;
      m_fieldParams.FalloffDimensions = (int) FalloffDimensions;
      m_fieldParams.PropagationDepth = PropagationDepth;

      m_fieldParams.GridCenter = m_gridCenter;
      m_fieldParams.UpWs =
        Params.Bits.IsBitSet((int) BoingWork.ReactorFlags.GlobalReactionUpVector)
        ? Params.RotationReactionUp
        : transform.rotation * VectorUtil.NormalizeSafe(Params.RotationReactionUp, Vector3.up);

      m_fieldParams.FieldPosition = transform.position;

      m_fieldParams.FalloffRatio = FalloffRatio;
      m_fieldParams.CellSize = CellSize;
      m_fieldParams.DeltaTime = Time.deltaTime;

      if (m_fieldParamsBuffer != null)
        m_fieldParamsBuffer.SetData(new FieldParams[] { m_fieldParams });
    }

    private void UpdateFlags()
    {
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.TwoDDistanceCheck, TwoDDistanceCheck);
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.TwoDPositionInfluence, TwoDPositionInfluence);
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.TwoDRotationInfluence, TwoDRotationInfluence);
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.EnablePositionEffect, EnablePositionEffect);
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.EnableRotationEffect, EnableRotationEffect);
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.GlobalReactionUpVector, GlobalReactionUpVector);
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.EnablePropagation, EnablePropagation);
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.AnchorPropagationAtBorder, AnchorPropagationAtBorder);
    }

    public void UpdateBounds()
    {
      m_bounds = new Aabb(m_gridCenter + GetCellCenterOffset(0, 0, 0), m_gridCenter + GetCellCenterOffset(CellsX - 1, CellsY - 1, CellsZ - 1));
      m_bounds.Expand(CellSize);
    }

    public void PrepareExecute()
    {
      Init();

      if (SharedParams != null)
        BoingWork.Params.Copy(ref SharedParams.Params, ref Params);

      UpdateFlags();
      UpdateBounds();

      if (m_hardwareMode != HardwareMode)
      {
        switch (m_hardwareMode)
        {
          case HardwareModeEnum.CPU:
            DisposeCpuResources();
            break;

          case HardwareModeEnum.GPU:
            DisposeGpuResources();
            break;
        }

        m_hardwareMode = HardwareMode;
      }

      switch (m_hardwareMode)
      {
        case HardwareModeEnum.CPU:
          ValidateCpuResources();
          break;

        case HardwareModeEnum.GPU:
          ValidateGpuResources();

          break;
      }

      HandleCellMove();

      switch (m_hardwareMode)
      {
        case HardwareModeEnum.CPU:
          FinishPrepareExecuteCpu();
          break;

        case HardwareModeEnum.GPU:
          FinishPrepareExecuteGpu();
          break;
      }
    }

    private void ValidateCpuResources()
    {
      CellsX = Mathf.Max(1, CellsX);
      CellsY = Mathf.Max(1, CellsY);
      CellsZ = Mathf.Max(1, CellsZ);

      bool cpuCellsNeedReset = 
        m_aCpuCell == null
        || m_cellsX != CellsX
        || m_cellsY != CellsY
        || m_cellsZ != CellsZ;
      if (cpuCellsNeedReset)
      {
        m_aCpuCell = new BoingWork.Params.InstanceData[CellsZ, CellsY, CellsX];
        for (int z = 0; z < CellsZ; ++z)
          for (int y = 0; y < CellsY; ++y)
            for (int x = 0; x < CellsX; ++x)
            {
              int resX, resY, resZ;
              ResolveCellIndex(x, y, z, -1, out resX, out resY, out resZ);
              m_aCpuCell[z, y, x].Reset(m_gridCenter + GetCellCenterOffset(resX, resY, resZ), false);
            }

        m_cellsX = CellsX;
        m_cellsY = CellsY;
        m_cellsZ = CellsZ;
      }
    }

    private bool m_cellBufferNeedsReset = false;
    private void ValidateGpuResources()
    {
      bool incrementResourceSetId = false;

      // compute shader
      bool shaderNeedsReset = (m_shader == null) || (s_computeKernelId == null);
      if (shaderNeedsReset)
      {
        m_shader = Resources.Load<ComputeShader>("Boing Kit/BoingReactorFieldCompute");
        incrementResourceSetId = true;

        if (s_computeKernelId == null)
        {
          s_computeKernelId = new ComputeKernelId();

          s_computeKernelId.InitKernel = m_shader.FindKernel("Init");
          s_computeKernelId.MoveKernel = m_shader.FindKernel("Move");
          s_computeKernelId.WrapXKernel = m_shader.FindKernel("WrapX");
          s_computeKernelId.WrapYKernel = m_shader.FindKernel("WrapY");
          s_computeKernelId.WrapZKernel = m_shader.FindKernel("WrapZ");
          s_computeKernelId.ExecuteKernel = m_shader.FindKernel("Execute");
        }
      }

      // effectors
      bool effectorBufferNeedsReset =
        m_effectorIndexBuffer == null
        || (Effectors != null && m_numEffectors != Effectors.Length);
      if (effectorBufferNeedsReset && Effectors != null)
      {
        if (m_effectorIndexBuffer != null)
          m_effectorIndexBuffer.Dispose();

        m_effectorIndexBuffer = new ComputeBuffer(Effectors.Length, sizeof(int));
        incrementResourceSetId = true;

        m_numEffectors = Effectors.Length;
      }
      if (shaderNeedsReset || effectorBufferNeedsReset)
      {
        m_shader.SetBuffer(s_computeKernelId.ExecuteKernel, ShaderPropertyId.EffectorIndices, m_effectorIndexBuffer);
      }

      // reactor params
      bool reactorParamsBufferNeedsReset =
        m_reactorParamsBuffer == null;
      if (reactorParamsBufferNeedsReset)
      {
        m_reactorParamsBuffer = new ComputeBuffer(1, BoingWork.Params.Stride);
        incrementResourceSetId = true;
      }
      if (shaderNeedsReset || reactorParamsBufferNeedsReset)
      {
        m_shader.SetBuffer(s_computeKernelId.ExecuteKernel, ShaderPropertyId.ReactorParams, m_reactorParamsBuffer);
      }

      // field params
      bool fieldBufferNeedsReset =
        m_fieldParamsBuffer == null;
      if (fieldBufferNeedsReset)
      {
        m_fieldParamsBuffer = new ComputeBuffer(1, FieldParams.Stride);
        incrementResourceSetId = true;
      }
      if (shaderNeedsReset || fieldBufferNeedsReset)
      {
        m_shader.SetBuffer(s_computeKernelId.InitKernel, ShaderPropertyId.ComputeFieldParams, m_fieldParamsBuffer);
        m_shader.SetBuffer(s_computeKernelId.MoveKernel, ShaderPropertyId.ComputeFieldParams, m_fieldParamsBuffer);
        m_shader.SetBuffer(s_computeKernelId.WrapXKernel, ShaderPropertyId.ComputeFieldParams, m_fieldParamsBuffer);
        m_shader.SetBuffer(s_computeKernelId.WrapYKernel, ShaderPropertyId.ComputeFieldParams, m_fieldParamsBuffer);
        m_shader.SetBuffer(s_computeKernelId.WrapZKernel, ShaderPropertyId.ComputeFieldParams, m_fieldParamsBuffer);
        m_shader.SetBuffer(s_computeKernelId.ExecuteKernel, ShaderPropertyId.ComputeFieldParams, m_fieldParamsBuffer);
      }

      // cells
      m_cellBufferNeedsReset =
        m_cellsBuffer == null
        || m_cellsX != CellsX
        || m_cellsY != CellsY
        || m_cellsZ != CellsZ;
      if (m_cellBufferNeedsReset)
      {
        if (m_cellsBuffer != null)
          m_cellsBuffer.Dispose();

        int numCells = CellsX * CellsY * CellsZ;
        m_cellsBuffer = new ComputeBuffer(numCells, BoingWork.Params.InstanceData.Stride);
        var cellBufferInitData = new BoingWork.Params.InstanceData[numCells];
        for (int i = 0; i < numCells; ++i)
        {
          cellBufferInitData[i].PositionSpring.Reset();
          cellBufferInitData[i].RotationSpring.Reset();
        }
        m_cellsBuffer.SetData(cellBufferInitData);

        incrementResourceSetId = true;

        m_cellsX = CellsX;
        m_cellsY = CellsY;
        m_cellsZ = CellsZ;
      }
      if (shaderNeedsReset || m_cellBufferNeedsReset)
      {
        m_shader.SetBuffer(s_computeKernelId.InitKernel, ShaderPropertyId.ComputeCells, m_cellsBuffer);
        m_shader.SetBuffer(s_computeKernelId.MoveKernel, ShaderPropertyId.ComputeCells, m_cellsBuffer);
        m_shader.SetBuffer(s_computeKernelId.WrapXKernel, ShaderPropertyId.ComputeCells, m_cellsBuffer);
        m_shader.SetBuffer(s_computeKernelId.WrapYKernel, ShaderPropertyId.ComputeCells, m_cellsBuffer);
        m_shader.SetBuffer(s_computeKernelId.WrapZKernel, ShaderPropertyId.ComputeCells, m_cellsBuffer);
        m_shader.SetBuffer(s_computeKernelId.ExecuteKernel, ShaderPropertyId.ComputeCells, m_cellsBuffer);
      }

      if (incrementResourceSetId)
      {
        ++m_gpuResourceSetId;
        if (m_gpuResourceSetId < 0)
          m_gpuResourceSetId = -1;
      }
    }
    private void FinishPrepareExecuteCpu()
    {
      Profiler.BeginSample("PrepareExecute");
      Quaternion gridRotation = transform.rotation;
      for (int z = 0; z < CellsZ; ++z)
        for (int y = 0; y < CellsY; ++y)
          for (int x = 0; x < CellsX; ++x)
          {
            int resX, resY, resZ;
            ResolveCellIndex(x, y, z, -1, out resX, out resY, out resZ);
            m_aCpuCell[z, y, x].PrepareExecute(ref Params, m_gridCenter, gridRotation, GetCellCenterOffset(resX, resY, resZ));
          }
      Profiler.EndSample();  
    }

    private void FinishPrepareExecuteGpu()
    {
      if (m_cellBufferNeedsReset)
      {
        UpdateFieldParamsGpu();
        m_shader.Dispatch(s_computeKernelId.InitKernel, CellsX, CellsY, CellsZ);
      }
    }

    public void Init()
    {
      if (m_init)
        return;

      m_hardwareMode = HardwareMode;

      m_init = true;
    }

    public void Sanitize()
    {
      if (PropagationDepth < 0)
        Debug.LogWarning("Propagation iterations must be a positive number.");
      else if (PropagationDepth > 3)
        Debug.LogWarning("For performance reasons, propagation is limited to 3 iterations.");

      PropagationDepth = Mathf.Clamp(PropagationDepth, 1, 3);
    }

    public void HandleCellMove()
    {
      if (m_cellMoveMode != CellMoveMode)
      {
        Reboot();
        m_cellMoveMode = CellMoveMode;
      }

      switch (CellMoveMode)
      {
        case CellMoveModeEnum.Follow:
          {
            Vector3 gridDelta = transform.position - m_gridCenter;
            switch (HardwareMode)
            {
              case HardwareModeEnum.CPU:
                for (int z = 0; z < CellsZ; ++z)
                  for (int y = 0; y < CellsY; ++y)
                    for (int x = 0; x < CellsX; ++x)
                      m_aCpuCell[z, y, x].PositionSpring.Value += gridDelta;
                break;

              case HardwareModeEnum.GPU:
                UpdateFieldParamsGpu();
                m_shader.SetVector(ShaderPropertyId.MoveParams, gridDelta);
                m_shader.Dispatch(s_computeKernelId.MoveKernel, CellsX, CellsY, CellsZ);
                break;
            }

            m_gridCenter = transform.position;
            m_qPrevGridCenterNorm = QuantizeNorm(m_gridCenter);
          }
          break;

        case CellMoveModeEnum.WrapAround:
          {
            m_gridCenter = transform.position;

            Vector3 qCurrGridCenterNorm = QuantizeNorm(m_gridCenter);
            m_gridCenter = qCurrGridCenterNorm * CellSize;

            int normDeltaX = (int) (qCurrGridCenterNorm.x - m_qPrevGridCenterNorm.x);
            int normDeltaY = (int) (qCurrGridCenterNorm.y - m_qPrevGridCenterNorm.y);
            int normDeltaZ = (int) (qCurrGridCenterNorm.z - m_qPrevGridCenterNorm.z);

            m_qPrevGridCenterNorm = qCurrGridCenterNorm;

            if (normDeltaX == 0 && normDeltaY == 0 && normDeltaZ == 0)
              return;

            switch (m_hardwareMode)
            {
              case HardwareModeEnum.CPU:
                WrapCpu(normDeltaX, normDeltaY, normDeltaZ);
                break;

              case HardwareModeEnum.GPU:
                WrapGpu(normDeltaX, normDeltaY, normDeltaZ);
                break;
            }

            m_iCellBaseX = MathUtil.Modulo(m_iCellBaseX + normDeltaX, CellsX);
            m_iCellBaseY = MathUtil.Modulo(m_iCellBaseY + normDeltaY, CellsY);
            m_iCellBaseZ = MathUtil.Modulo(m_iCellBaseZ + normDeltaZ, CellsZ);
          }
          break;
      }
    }

    #endregion


    #region Propagation

    private void InitPropagationCpu(ref BoingWork.Params.InstanceData data)
    {
      data.PositionPropagationWorkData = Vector3.zero;
      data.RotationPropagationWorkData = Vector3.zero;
    }

    private void PropagateSpringCpu(ref BoingWork.Params.InstanceData data, float dt)
    {
      // accumulate weighted neighbor delta from origin
      data.PositionSpring.Velocity += kPropagationFactor * PositionPropagation * data.PositionPropagationWorkData * dt;
      data.RotationSpring.VelocityVec += kPropagationFactor * RotationPropagation * data.RotationPropagationWorkData * dt;
    }

    private void ExtendPropagationBorder(ref BoingWork.Params.InstanceData data, float weight, int adjDeltaX, int adjDeltaY, int adjDeltaZ)
    {
      data.PositionPropagationWorkData += weight * (data.PositionOrigin + new Vector3(adjDeltaX, adjDeltaY, adjDeltaZ) * CellSize);
      data.RotationPropagationWorkData += weight * data.RotationOrigin;
    }

    private void AccumulatePropagationWeightedNeighbor(ref BoingWork.Params.InstanceData data, ref BoingWork.Params.InstanceData neighbor, float weight)
    {
      data.PositionPropagationWorkData += weight * (neighbor.PositionSpring.Value - neighbor.PositionOrigin);
      data.RotationPropagationWorkData += weight * (neighbor.RotationSpring.ValueVec - neighbor.RotationOrigin);
    }

    private void GatherPropagation(ref BoingWork.Params.InstanceData data, float weightSum)
    {
      // average neighbor delta from origin -> delta error v.s. cell delta from origin
      data.PositionPropagationWorkData = (data.PositionPropagationWorkData / weightSum) - (data.PositionSpring.Value - data.PositionOrigin);
      data.RotationPropagationWorkData = (data.RotationPropagationWorkData / weightSum) - (data.RotationSpring.ValueVec - data.RotationOrigin);
    }

    private void AnchorPropagationBorder(ref BoingWork.Params.InstanceData data)
    {
      data.PositionPropagationWorkData = Vector3.zero;
      data.RotationPropagationWorkData = Vector3.zero;
    }

    static float[] s_aSqrtInv = { 0.00000f, 1.00000f, 0.70711f, 0.57735f, 0.50000f, 0.44721f, 0.40825f, 0.37796f, 0.35355f, 0.33333f, 0.31623f, 0.30151f, 0.28868f, 0.27735f, 0.26726f, 0.25820f, 0.25000f, 0.24254f, 0.23570f, 0.22942f, 0.22361f, 0.21822f, 0.21320f, 0.20851f, 0.20412f, 0.20000f, 0.19612f, 0.19245f };
    private void PropagateCpu(float dt)
    {
      Profiler.BeginSample("PropagateCpu");

      var aiAdjDelta = new int[PropagationDepth * 2 + 1];
      for (int i = 0; i < aiAdjDelta.Length; ++i)
        aiAdjDelta[i] = i - PropagationDepth;

      // initialize work data
      {
        Profiler.BeginSample("Initialize");

        for (int z = 0; z < CellsZ; ++z)
          for (int y = 0; y < CellsY; ++y)
            for (int x = 0; x < CellsX; ++x)
              InitPropagationCpu(ref m_aCpuCell[z, y, x]);

        Profiler.EndSample();
      } // end: initialize work data

      // accumulate neighbor deltas
      {
        Profiler.BeginSample("Accumulate");

        for (int z = 0; z < CellsZ; ++z)
          for (int y = 0; y < CellsY; ++y)
            for (int x = 0; x < CellsX; ++x)
            {
              int resX, resY, resZ;
              ResolveCellIndex(x, y, z, -1, out resX, out resY, out resZ);

              float weightSum = 0.0f;
              foreach (int adjDeltaZ in aiAdjDelta)
                foreach (int adjDeltaY in aiAdjDelta)
                  foreach (int adjDeltaX in aiAdjDelta)
                  {
                    // self?
                    if (adjDeltaX == 0 && adjDeltaY == 0 && adjDeltaZ == 0)
                      continue;
                    
                    int deltaSqrSum = adjDeltaX * adjDeltaX + adjDeltaY * adjDeltaY + adjDeltaZ * adjDeltaZ;
                    float weight = s_aSqrtInv[deltaSqrSum];
                    weightSum += weight;

                    // at border?
                    if (   (CellsX > 2 && ((resX == 0 && adjDeltaX < 0) || (resX == CellsX - 1 && adjDeltaX > 0))) 
                        || (CellsY > 2 && ((resY == 0 && adjDeltaY < 0) || (resY == CellsY - 1 && adjDeltaY > 0))) 
                        || (CellsZ > 2 && ((resZ == 0 && adjDeltaZ < 0) || (resZ == CellsZ - 1 && adjDeltaZ > 0))))
                    {
                      continue;
                    }

                    int adjX = MathUtil.Modulo(x + adjDeltaX, CellsX);
                    int adjY = MathUtil.Modulo(y + adjDeltaY, CellsY);
                    int adjZ = MathUtil.Modulo(z + adjDeltaZ, CellsZ);
                    AccumulatePropagationWeightedNeighbor(ref m_aCpuCell[z, y, x], ref m_aCpuCell[adjZ, adjY, adjX], weight);
                  }

              if (weightSum <= 0.0f)
                continue;

              GatherPropagation(ref m_aCpuCell[z, y, x], weightSum);
            }

        Profiler.EndSample();
      } // end: accumulate neighbor deltas

      // anchor border
      if (AnchorPropagationAtBorder)
      {
        Profiler.BeginSample("AnchorBorder");

        for (int z = 0; z < CellsZ; ++z)
          for (int y = 0; y < CellsY; ++y)
            for (int x = 0; x < CellsX; ++x)
            {
              int resX, resY, resZ;
              ResolveCellIndex(x, y, z, -1, out resX, out resY, out resZ);

              bool border = 
                   ((resX == 0 || resX == CellsX - 1) && CellsX > 2)
                || ((resY == 0 || resY == CellsY - 1) && CellsY > 2)
                || ((resZ == 0 || resZ == CellsZ - 1) && CellsZ > 2);

              if (!border)
                continue;

              AnchorPropagationBorder(ref m_aCpuCell[z, y, x]);
            }

        Profiler.EndSample();
      } // end: anchor border

      // propagation
      {
        Profiler.BeginSample("Propagate");

        for (int z = 0; z < CellsZ; ++z)
          for (int y = 0; y < CellsY; ++y)
            for (int x = 0; x < CellsX; ++x)
              PropagateSpringCpu(ref m_aCpuCell[z, y, x], dt);

        Profiler.EndSample();
      } // end: propagation

      Profiler.EndSample();
    }

    #endregion


    #region Wrap

    private void WrapCpu(int deltaX, int deltaY, int deltaZ)
    {
      if (deltaX != 0)
      {
        int itDirX = deltaX > 0 ? -1 : 1;
        for (int z = 0; z < CellsZ; ++z)
          for (int y = 0; y < CellsY; ++y)
            for (int x = deltaX > 0 ? deltaX - 1 : CellsX + deltaX; x >= 0 && x < CellsX; x += itDirX)
            {
              int wrapResX, wrapResY, wrapResZ;
              int resetResX, resetResY, resetResZ;
              ResolveCellIndex(x, y, z, 1, out wrapResX, out wrapResY, out wrapResZ);
              ResolveCellIndex(wrapResX - deltaX, wrapResY - deltaY, wrapResZ - deltaZ, -1, out resetResX, out resetResY, out resetResZ);
              m_aCpuCell[wrapResZ, wrapResY, wrapResX].Reset(m_gridCenter + GetCellCenterOffset(resetResX, resetResY, resetResZ), true);
            }
      }

      if (deltaY != 0)
      {
        int itDirY = deltaY > 0 ? -1 : 1;
        for (int z = 0; z < CellsZ; ++z)
          for (int y = deltaY > 0 ? deltaY - 1 : CellsY + deltaY; y >= 0 && y < CellsY; y += itDirY)
            for (int x = 0; x < CellsX; ++x)
            {
              int wrapResX, wrapResY, wrapResZ;
              int resetResX, resetResY, resetResZ;
              ResolveCellIndex(x, y, z, 1, out wrapResX, out wrapResY, out wrapResZ);
              ResolveCellIndex(wrapResX - deltaX, wrapResY - deltaY, wrapResZ - deltaZ, -1, out resetResX, out resetResY, out resetResZ);
              m_aCpuCell[wrapResZ, wrapResY, wrapResX].Reset(m_gridCenter + GetCellCenterOffset(resetResX, resetResY, resetResZ), true);
            }
      }

      if (deltaZ != 0)
      {
        int itDirZ = deltaZ > 0 ? -1 : 1;
        for (int z = deltaZ > 0 ? deltaZ - 1 : CellsZ + deltaZ; z >= 0 && z < CellsZ; z += itDirZ)
          for (int y = 0; y < CellsY; ++y)
            for (int x = 0; x < CellsX; ++x)
            {
              int wrapResX, wrapResY, wrapResZ;
              int resetResX, resetResY, resetResZ;
              ResolveCellIndex(x, y, z, 1, out wrapResX, out wrapResY, out wrapResZ);
              ResolveCellIndex(wrapResX - deltaX, wrapResY - deltaY, wrapResZ - deltaZ, -1, out resetResX, out resetResY, out resetResZ);
              m_aCpuCell[wrapResZ, wrapResY, wrapResX].Reset(m_gridCenter + GetCellCenterOffset(resetResX, resetResY, resetResZ), true);
            }
      }
    }

    private void WrapGpu(int deltaX, int deltaY, int deltaZ)
    {
      UpdateFieldParamsGpu();

      m_shader.SetInts(ShaderPropertyId.WrapParams, new int[] { deltaX, deltaY, deltaZ });

      if (deltaX != 0)
        m_shader.Dispatch(s_computeKernelId.WrapXKernel, 1, CellsY, CellsZ);
      if (deltaY != 0)
        m_shader.Dispatch(s_computeKernelId.WrapYKernel, CellsX, 1, CellsZ);
      if (deltaZ != 0)
        m_shader.Dispatch(s_computeKernelId.WrapZKernel, CellsX, CellsY, 1);
    }

    #endregion


    #region Execute

    public void ExecuteCpu(float dt)
    {
      PrepareExecute();

      Profiler.BeginSample("BoingReactorField.StepCpu");

      if (Effectors == null || Effectors.Length == 0)
        return;

      if (EnablePropagation)
        PropagateCpu(dt);

      foreach (var effector in Effectors)
      {
        if (effector == null)
          continue;

        var ep = new BoingEffector.Params();
        ep.Fill(effector);

        if (!m_bounds.Intersects(ref ep))
          continue;

        Profiler.BeginSample("AccumulateTarget");
        for (int z = 0; z < CellsZ; ++z)
          for (int y = 0; y < CellsY; ++y)
            for (int x = 0; x < CellsX; ++x)
              m_aCpuCell[z, y, x].AccumulateTarget(ref Params, ref ep, dt);
        Profiler.EndSample();

      }

      Profiler.BeginSample("Execute");
      for (int z = 0; z < CellsZ; ++z)
        for (int y = 0; y < CellsY; ++y)
          for (int x = 0; x < CellsX; ++x)
          {
            m_aCpuCell[z, y, x].EndAccumulateTargets(ref Params);
            m_aCpuCell[z, y, x].Execute(ref Params, dt);
          }
      Profiler.EndSample();

      Profiler.EndSample();
    }

    private BoingWork.Params[] s_aReactorParams = new BoingWork.Params[1];
    public void ExecuteGpu(float dt, ComputeBuffer effectorParamsBuffer, Dictionary<int, int> effectorParamsIndexMap)
    {
      PrepareExecute();

      UpdateFieldParamsGpu();

      m_shader.SetBuffer(s_computeKernelId.ExecuteKernel, ShaderPropertyId.Effectors, effectorParamsBuffer);
      if (m_fieldParams.NumEffectors > 0)
      {
        var aEffectorIndex = new int[m_fieldParams.NumEffectors];
        int iIndex = 0;
        foreach (var effectorGo in Effectors)
        {
          if (effectorGo == null)
            continue;

          var effector = effectorGo.GetComponent<BoingEffector>();
          if (effector == null)
            continue;

          if (!effector.isActiveAndEnabled)
            continue;

          int index;
          if (!effectorParamsIndexMap.TryGetValue(effector.GetInstanceID(), out index))
            continue;

          aEffectorIndex[iIndex++] = index;
        }
        m_effectorIndexBuffer.SetData(aEffectorIndex);
      }

      s_aReactorParams[0] = Params;
      m_reactorParamsBuffer.SetData(s_aReactorParams);
      m_shader.SetVector(ShaderPropertyId.PropagationParams, new Vector4(PositionPropagation, RotationPropagation, kPropagationFactor, 0.0f));

      m_shader.Dispatch(s_computeKernelId.ExecuteKernel, CellsX, CellsY, CellsZ);
    }

    #endregion


    #region Debug

    #if UNITY_EDITOR
    public void OnDrawGizmos()
    {
      if (Effectors == null)
        return;
      
      foreach (var effector in Effectors)
      {
        if (effector == null)
          continue;

        if (!effector.DrawAffectedReactorFieldGizmos)
          continue;

        bool selected = false;
        foreach (var selectedGo in UnityEditor.Selection.gameObjects)
        {
          if (effector.gameObject == selectedGo)
          {
            selected = true;
            break;
          }
        }

        if (!selected)
          continue;

        DrawGizmos(false);
        return;
      }

    }
    #endif

    public void OnDrawGizmosSelected()
    {
      if (!isActiveAndEnabled)
        return;

      DrawGizmos(true);
    }

    private void DrawGizmos(bool drawEffectors)
    {
      Vector3 gridCenter = GetGridCenter();
      switch (CellMoveMode)
      {
        case CellMoveModeEnum.Follow:
          gridCenter = transform.position;
          break;

        case CellMoveModeEnum.WrapAround:
          {
            Vector3 qCurrGridCenterNorm =
              new Vector3
              (
                Mathf.Round(transform.position.x / CellSize), 
                Mathf.Round(transform.position.y / CellSize), 
                Mathf.Round(transform.position.z / CellSize)
              );

            gridCenter = qCurrGridCenterNorm * CellSize;
          }
          break;
      }

      BoingWork.Params.InstanceData[,,] aCellData = null;
      switch (HardwareMode)
      {
        case HardwareModeEnum.CPU:
          aCellData = m_aCpuCell;
          break;

        case HardwareModeEnum.GPU:
          if (m_cellsBuffer == null)
            break;

          aCellData = new BoingWork.Params.InstanceData[CellsZ, CellsY, CellsX];
          m_cellsBuffer.GetData(aCellData);
          break;
      }

      int drawCellEvery = 1;
      if (CellsX * CellsY * CellsZ > 1024)
        drawCellEvery = 2;
      if (CellsX * CellsY * CellsZ > 4096)
        drawCellEvery = 3;
      if (CellsX * CellsY * CellsZ > 8192)
        drawCellEvery = 4;

      for (int z = 0; z < CellsZ; ++z)
        for (int y = 0; y < CellsY; ++y)
          for (int x = 0; x < CellsX; ++x)
          {
            int resX, resY, resZ;
            ResolveCellIndex(x, y, z, -1, out resX, out resY, out resZ);

            Vector3 cellCenter = gridCenter + GetCellCenterOffset(resX, resY, resZ);

            if (aCellData != null 
                && x % drawCellEvery == 0 
                && y % drawCellEvery == 0 
                && z % drawCellEvery == 0)
            {
              var cell = aCellData[z, y, x];

              Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
              Gizmos.matrix = Matrix4x4.TRS(cell.PositionSpring.Value, cell.RotationSpring.ValueQuat, Vector3.one);
              Gizmos.DrawCube(Vector3.zero, Mathf.Min(0.1f, 0.5f * CellSize) * Vector3.one);
              Gizmos.matrix = Matrix4x4.identity;
            }

            Gizmos.color = new Color(1.0f, 0.5f, 0.2f, 1.0f);
            Gizmos.DrawWireCube(cellCenter, CellSize * Vector3.one);
          }

      switch (FalloffMode)
      {
        case FalloffModeEnum.Circle:
          {
            float maxCells = Mathf.Max(CellsX, CellsY, CellsZ);

            Gizmos.color = new Color(1.0f, 1.0f, 0.2f, 0.5f);
            Gizmos.matrix = Matrix4x4.Translate(gridCenter) * Matrix4x4.Scale(new Vector3(CellsX, CellsY, CellsZ) / maxCells);
            Gizmos.DrawWireSphere(Vector3.zero, 0.5f * CellSize * maxCells * FalloffRatio);
            Gizmos.matrix = Matrix4x4.identity;
          }
          break;

        case FalloffModeEnum.Square:
          {
            Vector3 size = CellSize * FalloffRatio * new Vector3(CellsX, CellsY, CellsZ);

            Gizmos.color = new Color(1.0f, 1.0f, 0.2f, 0.5f);
            Gizmos.DrawWireCube(gridCenter, size);
          }
          break;
      }

      if (drawEffectors && Effectors != null)
      {
        foreach (var effector in Effectors)
        {
          if (effector == null)
            continue;

          effector.OnDrawGizmosSelected();
        }
      }
    }

    #endregion


    private Vector3 GetGridCenter()
    {
      switch (CellMoveMode)
      {
        case CellMoveModeEnum.Follow:
          return transform.position;

        case CellMoveModeEnum.WrapAround:
          {
            Vector3 qCurrGridCenterNorm = QuantizeNorm(transform.position);
            return qCurrGridCenterNorm * CellSize;
          }
      }

      return transform.position;
    }

    private Vector3 QuantizeNorm(Vector3 p)
    {
      return
        new Vector3
        (
          Mathf.Round(p.x / CellSize), 
          Mathf.Round(p.y / CellSize), 
          Mathf.Round(p.z / CellSize)
        );
    }

    private Vector3 GetCellCenterOffset(int x, int y, int z)
    {
      return CellSize * (-0.5f * (new Vector3(CellsX, CellsY, CellsZ) - Vector3.one) + new Vector3(x, y, z));
    }

    private void ResolveCellIndex(int x, int y, int z, int baseMult, out int resX, out int resY, out int resZ)
    {
      resX = MathUtil.Modulo(x + baseMult * m_iCellBaseX, CellsX);
      resY = MathUtil.Modulo(y + baseMult * m_iCellBaseY, CellsY);
      resZ = MathUtil.Modulo(z + baseMult * m_iCellBaseZ, CellsZ);
    }
  }
}
