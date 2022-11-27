/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

#ifndef BOING_KIT
#define BOING_KIT

#if (defined(SHADER_API_D3D11) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE) || defined(SHADER_API_XBOXONE) || defined(SHADER_API_PSSL) || defined(SHADER_API_SWITCH) || defined(SHADER_API_VULKAN) || (defined(SHADER_API_METAL) && defined(UNITY_COMPILER_HLSLCC)))
#define BOING_KIT_SUPPORTED
#endif


#define kBoingKitEpsilon     (0.00001f)
#define kBoingKitEpsilonComp (0.99999f)


#if defined(BOING_KIT_SUPPORTED)

#include "Internal/BoingKitCore.cginc"

// util
//-----------------------------------------------------------------------------

inline float4 TriLerp
(
  float4 v000, float4 v001, float4 v010, float4 v011,
  float4 v100, float4 v101, float4 v110, float4 v111,
  float3 t
)
{
  return
    lerp
    (
      lerp
      (
        lerp(v000, v001, t.x), 
        lerp(v010, v011, t.x), 
        t.y
      ), 
      lerp
      (
        lerp(v100, v101, t.x), 
        lerp(v110, v111, t.x), 
        t.y
      ), 
      t.z
    );
}

inline bool BoundsContain(float3 boundsMin, float3 boundsMax, float3 p)
{
  return
       boundsMin.x <= p.x 
    && boundsMin.y <= p.y 
    && boundsMin.z <= p.z 
    && boundsMax.x >= p.x 
    && boundsMax.y >= p.y 
    && boundsMax.z >= p.z;
}

inline int3 QuantizeNorm(float3 p, float cellSize)
{
  return int3(round(p / cellSize));
}

inline float3 GetCellCenterOffset(int3 index, int3 numCells, float cellSize)
{
  return cellSize * (-0.5f * (numCells - 1) + index);
}

inline int3 ResolveCellIndex(int3 index, int baseMult, int3 iCellBase, int3 numCells)
{
  float3 m = fmod(index + baseMult * iCellBase, numCells);
  return int3(m + step(m, -0.5f) * numCells);
}

inline int FlattenCellIndex(int3 index, int3 numCells)
{
  return
      index.z * numCells.y * numCells.x
    + index.y * numCells.x
    + index.x;
}

struct TriLerpParams
{
  int3 iRaw000;
  int3 iRaw001;
  int3 iRaw010;
  int3 iRaw011;
  int3 iRaw100;
  int3 iRaw101;
  int3 iRaw110;
  int3 iRaw111;
  float3 t;
  float influenceMult;
};

inline bool SampleInFieldRange(float3 posWs, FieldParams fieldParams)
{
  float cellSize = fieldParams.floatData.y;

  float4 boundsMin = 
    float4
    (
      fieldParams.gridCenter.xyz 
       + GetCellCenterOffset(int3(0, 0, 0), fieldParams.nums.xyz, cellSize) 
       - cellSize, 
      0.0f
    );
  float4 boundsMax = 
    float4
    (
      fieldParams.gridCenter.xyz 
       + GetCellCenterOffset(fieldParams.nums.xyz - 1, fieldParams.nums.xyz, cellSize) 
       + cellSize, 
      0.0f
    );

  bool inRange = false;
  switch (fieldParams.intData.y)
  {
    case kFalloffXYZ: inRange = BoundsContain(boundsMin.xyz, boundsMax.xyz, posWs); break;
    case kFalloffXY:  inRange = BoundsContain(boundsMin.xyw, boundsMax.xyw, float3(posWs.xy, 0.0f)); break;
    case kFalloffXZ:  inRange = BoundsContain(boundsMin.xzw, boundsMax.xzw, float3(posWs.xz, 0.0f)); break;
    case kFalloffYZ:  inRange = BoundsContain(boundsMin.yzw, boundsMax.yzw, float3(posWs.yz, 0.0f)); break;
  }
  
  return inRange;
}

inline TriLerpParams GetTriLerpParams(float3 posWs, FieldParams fieldParams)
{
  int maxCells = max(max(fieldParams.nums.x, fieldParams.nums.y), fieldParams.nums.z);
  float falloffRatio = fieldParams.floatData.x;
  float cellSize = fieldParams.floatData.y;
  float halfCellSize = 0.5f * cellSize;

  float3 pLs = posWs - (fieldParams.gridCenter.xyz + GetCellCenterOffset(int3(0, 0, 0), fieldParams.nums.xyz, cellSize));

  int3 qNormLower = QuantizeNorm(pLs - halfCellSize, cellSize);
  float3 qLower = qNormLower * cellSize;

  int3 iLowerRaw = clamp(qNormLower, int3(0, 0, 0), fieldParams.nums.xyz - 1);
  int3 iUpperRaw = min(iLowerRaw + 1, fieldParams.nums.xyz - 1);
  int3 iLowerRes = ResolveCellIndex(iLowerRaw, 1, fieldParams.cellData.xyz, fieldParams.nums.xyz);
  int3 iUpperRes = ResolveCellIndex(iUpperRaw, 1, fieldParams.cellData.xyz, fieldParams.nums.xyz);

  float3 t = (pLs - qLower) / cellSize;

  float3 centerToSample = posWs - fieldParams.fieldCenter.xyz;
  switch (fieldParams.intData.y)
  {
    case kFalloffXY: centerToSample.z = 0.0f; break;
    case kFalloffXZ: centerToSample.y = 0.0f; break;
    case kFalloffYZ: centerToSample.x = 0.0f; break;
  };

  float influenceMult = 1.0f;
  switch (fieldParams.intData.x)
  {
    case kFalloffCircle:
    {
      float maxHalfExtent = halfCellSize * maxCells;
      float3 radialScaleInv = maxCells / float3(fieldParams.nums.xyz);
      centerToSample *= radialScaleInv;
      float radialDist = length(centerToSample);
      float circleFalloffStartDist = max(0.0f, falloffRatio * maxHalfExtent - halfCellSize);
      float circleFalloffDist = max(kBoingKitEpsilon, (1.0f - falloffRatio) * maxHalfExtent - halfCellSize);
      influenceMult = 1.0f - saturate((radialDist - circleFalloffStartDist) / circleFalloffDist);
    }
    break;

    case kFalloffSquare:
    {
      float3 halfExtent = halfCellSize * fieldParams.nums.xyz;
      float3 squareFalloffStartDist = max(0.0f, falloffRatio * halfExtent - halfCellSize);
      float3 squareFalloffDist = max(kBoingKitEpsilon, (1.0f - falloffRatio) * halfExtent - halfCellSize);
      float3 cardinalDist = abs(centerToSample);
      float3 influenceMultVec = 1.0f - saturate ((cardinalDist - squareFalloffStartDist) / squareFalloffDist);
      switch (fieldParams.intData.y)
      {
        case kFalloffXY: influenceMultVec.z = 1.0f; break;
        case kFalloffXZ: influenceMultVec.y = 1.0f; break;
        case kFalloffYZ: influenceMultVec.x = 1.0f; break;
      }
      influenceMult = min(min(influenceMultVec.x, influenceMultVec.y), influenceMultVec.z);
    }
    break;
  }

  TriLerpParams params;
  params.iRaw000 = int3(iLowerRaw.x, iLowerRaw.y, iLowerRaw.z);
  params.iRaw001 = int3(iUpperRaw.x, iLowerRaw.y, iLowerRaw.z);
  params.iRaw010 = int3(iLowerRaw.x, iUpperRaw.y, iLowerRaw.z);
  params.iRaw011 = int3(iUpperRaw.x, iUpperRaw.y, iLowerRaw.z);
  params.iRaw100 = int3(iLowerRaw.x, iLowerRaw.y, iUpperRaw.z);
  params.iRaw101 = int3(iUpperRaw.x, iLowerRaw.y, iUpperRaw.z);
  params.iRaw110 = int3(iLowerRaw.x, iUpperRaw.y, iUpperRaw.z);
  params.iRaw111 = int3(iUpperRaw.x, iUpperRaw.y, iUpperRaw.z);
  params.t = t;
  params.influenceMult = influenceMult;

  return params;
}

//-----------------------------------------------------------------------------
// end: util

#endif // #if defined(BOING_KIT_SUPPORTED)


// Boing Kit functions
//-----------------------------------------------------------------------------


struct BoingReactorFieldSampleResults
{
  float3 positionOffset;
  float  padding;
  float4 rotationOffset; // quaternion
};

#if defined(BOING_KIT_SUPPORTED)
float positionSampleMultiplier;
float rotationSampleMultiplier;
StructuredBuffer<FieldParams> aBoingFieldParams;
StructuredBuffer<InstanceData> aBoingFieldCell;
#endif // #if defined(BOING_KIT_SUPPORTED)

BoingReactorFieldSampleResults SampleBoingReactorField(float3 posWs)
{
  BoingReactorFieldSampleResults results;
  results.positionOffset = 0.0f;
  results.rotationOffset = 0.0f;

  #if defined(BOING_KIT_SUPPORTED)

  FieldParams fieldParams = aBoingFieldParams[0];
  int3 numCells = fieldParams.nums.xyz;
  float4 gridCenter = float4(fieldParams.gridCenter.xyz, 0.0f);
  float cellSize = fieldParams.floatData.y;

  if (!SampleInFieldRange(posWs, fieldParams))
  {
    results.rotationOffset = float4(0.0f, 0.0f, 0.0f, 1.0f); // quat_identity
    return results;
  }

  TriLerpParams tlp = GetTriLerpParams(posWs, fieldParams);

  int iResFlat000 = FlattenCellIndex(ResolveCellIndex(tlp.iRaw000, 1, fieldParams.cellData.xyz, numCells), numCells);
  int iResFlat001 = FlattenCellIndex(ResolveCellIndex(tlp.iRaw001, 1, fieldParams.cellData.xyz, numCells), numCells);
  int iResFlat010 = FlattenCellIndex(ResolveCellIndex(tlp.iRaw010, 1, fieldParams.cellData.xyz, numCells), numCells);
  int iResFlat011 = FlattenCellIndex(ResolveCellIndex(tlp.iRaw011, 1, fieldParams.cellData.xyz, numCells), numCells);
  int iResFlat100 = FlattenCellIndex(ResolveCellIndex(tlp.iRaw100, 1, fieldParams.cellData.xyz, numCells), numCells);
  int iResFlat101 = FlattenCellIndex(ResolveCellIndex(tlp.iRaw101, 1, fieldParams.cellData.xyz, numCells), numCells);
  int iResFlat110 = FlattenCellIndex(ResolveCellIndex(tlp.iRaw110, 1, fieldParams.cellData.xyz, numCells), numCells);
  int iResFlat111 = FlattenCellIndex(ResolveCellIndex(tlp.iRaw111, 1, fieldParams.cellData.xyz, numCells), numCells);

  results.positionOffset =
    TriLerp
    (
      aBoingFieldCell[iResFlat000].positionSpring.value - gridCenter - float4(GetCellCenterOffset(tlp.iRaw000, numCells, cellSize), 0.0f), 
      aBoingFieldCell[iResFlat001].positionSpring.value - gridCenter - float4(GetCellCenterOffset(tlp.iRaw001, numCells, cellSize), 0.0f), 
      aBoingFieldCell[iResFlat010].positionSpring.value - gridCenter - float4(GetCellCenterOffset(tlp.iRaw010, numCells, cellSize), 0.0f), 
      aBoingFieldCell[iResFlat011].positionSpring.value - gridCenter - float4(GetCellCenterOffset(tlp.iRaw011, numCells, cellSize), 0.0f), 
      aBoingFieldCell[iResFlat100].positionSpring.value - gridCenter - float4(GetCellCenterOffset(tlp.iRaw100, numCells, cellSize), 0.0f), 
      aBoingFieldCell[iResFlat101].positionSpring.value - gridCenter - float4(GetCellCenterOffset(tlp.iRaw101, numCells, cellSize), 0.0f), 
      aBoingFieldCell[iResFlat110].positionSpring.value - gridCenter - float4(GetCellCenterOffset(tlp.iRaw110, numCells, cellSize), 0.0f), 
      aBoingFieldCell[iResFlat111].positionSpring.value - gridCenter - float4(GetCellCenterOffset(tlp.iRaw111, numCells, cellSize), 0.0f), 
      tlp.t
    ).xyz;
  results.rotationOffset =
    TriLerp
    (
      aBoingFieldCell[iResFlat000].rotationSpring.value, 
      aBoingFieldCell[iResFlat001].rotationSpring.value, 
      aBoingFieldCell[iResFlat010].rotationSpring.value, 
      aBoingFieldCell[iResFlat011].rotationSpring.value, 
      aBoingFieldCell[iResFlat100].rotationSpring.value, 
      aBoingFieldCell[iResFlat101].rotationSpring.value, 
      aBoingFieldCell[iResFlat110].rotationSpring.value, 
      aBoingFieldCell[iResFlat111].rotationSpring.value, 
      tlp.t
    );

  results.positionOffset *= tlp.influenceMult * positionSampleMultiplier;

  // normalize_safe
  {
    float rotOffsetLen = length(results.rotationOffset);
    results.rotationOffset = 
      rotOffsetLen > kBoingKitEpsilon 
        ? results.rotationOffset / rotOffsetLen
        : float4(0.0f, 0.0f, 0.0f, 1.0f);
  }

  // inline quat_pow so we don't need to include Quaternion.cginc
  {
    float4 q = results.rotationOffset;
    float r = length(q.xyz);
    if (r > kEpsilon)
    {
      float t = (tlp.influenceMult * rotationSampleMultiplier) * atan2(r, q.w);
      results.rotationOffset = float4(sin(t) * q.xyz / r, cos(t));
    }
  }

  #endif // #if defined(BOING_KIT_SUPPORTED)

  return results;
}

struct BoingReactorFieldResults
{
  // all in world space
  float3 position;
  float3 normal;
};

// use this in custom vertex shaders
BoingReactorFieldResults ApplyBoingReactorFieldPerVertex(float3 position, float3 normal, float3 rotationPivot)
{
  // everything is in world space
  BoingReactorFieldSampleResults sampleResults = SampleBoingReactorField(position);

  // inline quat_rot so we don't need to include Quaternion.cginc
  {
    float4 q = sampleResults.rotationOffset;

    float3 r = position - rotationPivot;
    position = 
      dot(q.xyz, r) * q.xyz 
      + q.w * q.w * r 
      + 2.0 * q.w * cross(q.xyz, r) 
      - cross(cross(q.xyz, r), q.xyz) 
      + rotationPivot;

    normal = 
      dot(q.xyz, normal) * q.xyz 
      + q.w * q.w * normal 
      + 2.0 * q.w * cross(q.xyz, normal) 
      - cross(cross(q.xyz, normal), q.xyz);
  }

  position += sampleResults.positionOffset;

  BoingReactorFieldResults results;
  results.position = position;
  results.normal = normal;

  return results;
}


// use this in custom vertex shaders
BoingReactorFieldResults ApplyBoingReactorFieldPerObject(float3 position, float3 normal, float3 objOrigin)
{
  // everything is in world space
  BoingReactorFieldSampleResults sampleResults = SampleBoingReactorField(objOrigin);

  // inline quat_rot so we don't need to include Quaternion.cginc
  {
    float4 q = sampleResults.rotationOffset;

    float3 r = position - objOrigin;
    position = 
      dot(q.xyz, r) * q.xyz 
      + q.w * q.w * r 
      + 2.0 * q.w * cross(q.xyz, r) 
      - cross(cross(q.xyz, r), q.xyz) 
      + objOrigin;

    normal = 
      dot(q.xyz, normal) * q.xyz 
      + q.w * q.w * normal 
      + 2.0 * q.w * cross(q.xyz, normal) 
      - cross(cross(q.xyz, normal), q.xyz);
  }

  position += sampleResults.positionOffset;

  BoingReactorFieldResults results;
  results.position = position;
  results.normal = normal;

  return results;
}


// use this at the very beginning of vertex shaders set up with Unity's instancing macros
#define APPLY_BOING_REACTOR_FIELD_PER_VERTEX(v)                                                 \
  {                                                                                             \
    UNITY_SETUP_INSTANCE_ID(v);                                                                 \
    float3 posWs = mul(unity_ObjectToWorld, v.vertex).xyz;                                      \
    float3 normWs = mul(unity_ObjectToWorld, v.normal);                                         \
    float3 pivotWs = mul(unity_ObjectToWorld, float4(0.0f, 0.0f, 0.0f, 1.0f)).xyz;              \
    BoingReactorFieldResults results = ApplyBoingReactorFieldPerVertex(posWs, normWs, pivotWs); \
    v.vertex.xyz = mul(unity_WorldToObject, float4(results.position, 1.0f));                    \
    v.normal = mul(unity_WorldToObject, results.normal);                                        \
  }

//-----------------------------------------------------------------------------
// end: Boing Kit functions


#endif // #ifndef BOING_KIT
