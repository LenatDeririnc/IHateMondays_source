/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

#ifndef BOING_KIT_VECTOR
#define BOING_KIT_VECTOR

#include "Math.cginc"

inline float3 unit_x() { return float3(1.0f, 0.0f, 0.0f); }
inline float3 unit_y() { return float3(0.0f, 1.0f, 0.0f); }
inline float3 unit_z() { return float3(0.0f, 0.0f, 1.0f); }

inline float3 normalize_safe(float3 v, float3 fallback)
{
  float vv = dot(v, v);
  return vv > kEpsilon ? v / sqrt(vv) : fallback;
}

inline float3 normalize_safe(float3 v)
{
  return normalize_safe(v, unit_z());
}

inline float4 normalize_safe(float4 v, float4 fallback)
{
  float vv = dot(v, v);
  return vv > kEpsilon ? v / sqrt(vv) : fallback;
}

inline float4 normalize_safe(float4 v)
{
  return normalize_safe(v, float4(unit_z(), 0.0f));
}

inline float3 project_vec(float3 v, float3 onto)
{
  onto = normalize(onto);
  return dot(v, onto) * onto;
}

inline float3 project_plane(float3 v, float3 n)
{
  return v - project_vec(v, n);
}

inline float3 find_ortho(float3 v)
{
  if (v.x >= kSqrt3Inv)
    return float3(v.y, -v.x, 0.0);
  else
    return float3(0.0, v.z, -v.y);
}

inline float3x3 orthonormalize(float3 axis0, float3 axis1, float3 axis2)
{
  axis0 = normalize_safe(axis0, unit_x());
  axis1 = normalize_safe(axis1 - project_vec(axis1, axis0), unit_y());
  axis2 = normalize_safe(axis2 - project_vec(axis2, axis0) - project_vec(axis2, axis1), unit_z());
  return float3x3(axis0, axis1, axis2);
}

inline float3 closest_point_on_segment(float3 p, float3 segA, float3 segB)
{
  float3 v = segB - segA;
  float vv = dot(v, v);
  if (vv < kEpsilon)
    return 0.5f * (segA + segB);

  float vMag = sqrt(vv);
  float3 vNorm = v / vMag;
  float d = saturate(dot(p - segA, vNorm) / vMag);
  return segA + d * v;
}

inline float3 slerp(float3 a, float3 b, float t)
{
  float d = dot(normalize(a), normalize(b));
  if (d > kEpsilonComp)
  {
    return lerp(a, b, t);
  }

  float r = acos(clamp(d, -1.0f, 1.0f));
  return (sin((1.0 - t) * r) * a + sin(t * r) * b) / sin(r);
}

inline float3 nlerp(float3 a, float b, float t)
{
  return normalize(lerp(a, b, t));
}

inline float3x3 mat_basis(float3 xAxis, float3 yAxis, float3 zAxis)
{
  return transpose(float3x3(xAxis, yAxis, zAxis));
}

inline float3x3 mat_look_at(float3 dir, float3 up)
{
  float3 zAxis = normalize_safe(dir, unit_z());
  float3 xAxis = normalize_safe(cross(up, zAxis), unit_x());
  float3 yAxis = cross(zAxis, xAxis);
  return mat_basis(xAxis, yAxis, zAxis);
}

#endif
