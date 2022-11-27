/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

#ifndef BOING_KIT_SPRING_FLOAT4
#define BOING_KIT_SPRING_FLOAT4

#include "Math.cginc"

// 32 bytes
struct SpringFloat4
{
  // bytes 0-15 (16 bytes)
  float4 value;

  // bytes 16-31 (16 bytes)
  float4 velocity;
};

SpringFloat4 TrackDampingRatio(SpringFloat4 s, float4 targetValue, float angularFrequency, float dampingRatio, float deltaTime)
{
  if (angularFrequency < kEpsilon)
  {
    s.velocity = 0.0f;
    return s;
  }

  float4 delta = targetValue - s.value;

  float f = 1.0f + 2.0f * deltaTime * dampingRatio * angularFrequency;
  float oo = angularFrequency * angularFrequency;
  float hoo = deltaTime * oo;
  float hhoo = deltaTime * hoo;
  float detInv = 1.0f / (f + hhoo);
  float4 detX = f * s.value + deltaTime * s.velocity + hhoo * targetValue;
  float4 detV = s.velocity + hoo * delta;

  s.velocity = detV * detInv;
  s.value = detX * detInv;

  if (length(s.velocity) < kEpsilon && length(delta) < kEpsilon)
  {
    s.velocity = 0.0f;
    s.value = targetValue;
  }

  return s;
}

SpringFloat4 TrackHalfLife(SpringFloat4 s, float4 targetValue, float frequencyHz, float halfLife, float deltaTime)
{
  if (halfLife < kEpsilon)
  {
    s.velocity = 0.0f;
    s.value = targetValue;
    return s;
  }

  float angularFrequency = frequencyHz * kTwoPi;
  float dampingRatio = 0.6931472f / (angularFrequency * halfLife);
  return TrackDampingRatio(s, targetValue, angularFrequency, dampingRatio, deltaTime);
}

SpringFloat4 TrackExponential(SpringFloat4 s, float4 targetValue, float halfLife, float deltaTime)
{
  if (halfLife < kEpsilon)
  {
    s.velocity = 0.0f;
    s.value = targetValue;
    return s;
  }

  float angularFrequency = 0.6931472f / halfLife;
  float dampingRatio = 1.0f;
  return TrackDampingRatio(s, targetValue, angularFrequency, dampingRatio, deltaTime);
}

#endif
