#ifndef BOING_KIT_SAMPLE_REACTOR_FIELD_SHADER_NODE
#define BOING_KIT_SAMPLE_REACTOR_FIELD_SHADER_NODE

#include "Assets/Boing Kit/Shader/BoingKit.cginc"

void BoingKit_sample_reactor_field_per_vertex_float(float3 posWs, float3 normWs, float3 pivotWs, out float3 vertexPos, out float3 vertexNorm)
{
  BoingReactorFieldResults results = ApplyBoingReactorFieldPerVertex(posWs, normWs, pivotWs);
  vertexPos = results.position;
  vertexNorm = results.normal;
}

void BoingKit_sample_reactor_field_per_obj_float(float3 posWs, float3 normWs, float3 objPosWs, out float3 vertexPos, out float3 vertexNorm)
{
  BoingReactorFieldResults results = ApplyBoingReactorFieldPerObject(posWs, normWs, objPosWs);
  vertexPos = results.position;
  vertexNorm = results.normal;
}

#endif
