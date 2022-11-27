/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace BoingKit
{
  internal static class BoingWorkSynchronous
  {
    #region Behavior

    internal static void ExecuteBehaviors(Dictionary<int, BoingBehavior> behaviorMap, BoingManager.UpdateMode updateMode)
    {
      float dt = Time.deltaTime;
      foreach (var itBehavior in behaviorMap)
      {
        var behavior = itBehavior.Value;
        if (behavior.UpdateMode != updateMode)
          continue;

        behavior.PrepareExecute();

        switch (behavior.UpdateMode)
        {
          case BoingManager.UpdateMode.EarlyUpdate:
          case BoingManager.UpdateMode.LateUpdate:
            behavior.Execute(dt);
            break;

          case BoingManager.UpdateMode.FixedUpdate:
            behavior.Execute(BoingManager.FixedDeltaTime);
            break;
        }
      }
    }

    #endregion // Behavior


    #region Reactor

    internal static void ExecuteReactors
    (
      BoingEffector.Params[] aEffectorParams, 
      Dictionary<int, BoingReactor> reactorMap, 
      Dictionary<int, BoingReactorField> fieldMap, 
      Dictionary<int, BoingReactorFieldCPUSampler> cpuSamplerMap, 
      BoingManager.UpdateMode updateMode
    )
    {
      float dt = BoingManager.DeltaTime;

      Profiler.BeginSample("Update Reactors");

      foreach (var itReactor in reactorMap)
      {
        var reactor = itReactor.Value;
        if (reactor.UpdateMode != updateMode)
          continue;

        reactor.PrepareExecute();

        if (aEffectorParams != null)
          for (int i = 0; i < aEffectorParams.Length; ++i)
            reactor.Params.AccumulateTarget(ref aEffectorParams[i], dt);

        reactor.Params.EndAccumulateTargets();

        switch (reactor.UpdateMode)
        {
          case BoingManager.UpdateMode.EarlyUpdate:
          case BoingManager.UpdateMode.LateUpdate:
            reactor.Execute(dt);
            break;

          case BoingManager.UpdateMode.FixedUpdate:
            reactor.Execute(BoingManager.FixedDeltaTime);
            break;
        }
      }
      Profiler.EndSample();

      Profiler.BeginSample("Update Fields (CPU)");
      foreach (var itField in fieldMap)
      {
        var field = itField.Value;
        switch (field.HardwareMode)
        {
          case BoingReactorField.HardwareModeEnum.CPU:
            field.ExecuteCpu(dt);
            break;
        }
      }
      Profiler.EndSample();

      Profiler.BeginSample("Update Field Samplers");
      foreach (var itSampler in cpuSamplerMap)
      {
        var sampler = itSampler.Value;
        //sampler.SampleFromField();
      }
      Profiler.EndSample();
    }

    #endregion // Reactor


    #region Bones

    // use fixed time step for bones because they involve collision resolution
    internal static void ExecuteBones
    (
      BoingEffector.Params[] aEffectorParams, 
      Dictionary<int, BoingBones> bonesMap, 
      BoingManager.UpdateMode updateMode
    )
    {
      Profiler.BeginSample("Update Bones (Execute)");

      float dt = BoingManager.DeltaTime;

      foreach (var itBones in bonesMap)
      {
        var bones = itBones.Value;
        if (bones.UpdateMode != updateMode)
          continue;

        bones.PrepareExecute();

        if (aEffectorParams != null)
          for (int i = 0; i < aEffectorParams.Length; ++i)
            bones.AccumulateTarget(ref aEffectorParams[i], dt);

        bones.EndAccumulateTargets();

        switch (bones.UpdateMode)
        {
          case BoingManager.UpdateMode.EarlyUpdate:
          case BoingManager.UpdateMode.LateUpdate:
            bones.Params.Execute(bones, BoingManager.DeltaTime);
            break;

          case BoingManager.UpdateMode.FixedUpdate:
            bones.Params.Execute(bones, BoingManager.FixedDeltaTime);
            break;
        }
      }

      Profiler.EndSample();
    }

    internal static void PullBonesResults
    (
      BoingEffector.Params[] aEffectorParams, 
      Dictionary<int, BoingBones> bonesMap, 
      BoingManager.UpdateMode updateMode
    )
    {
      Profiler.BeginSample("Update Bones (Pull Results)");

      foreach (var itBones in bonesMap)
      {
        var bones = itBones.Value;
        if (bones.UpdateMode != updateMode)
          continue;

        bones.Params.PullResults(bones);
      }

      Profiler.EndSample();
    }

    #endregion // Bones
  }
}
