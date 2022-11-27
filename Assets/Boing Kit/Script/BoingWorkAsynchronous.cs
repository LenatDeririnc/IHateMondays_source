/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

#if UNITY_2018_1_OR_NEWER

using Unity.Collections;
using Unity.Jobs;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System;

namespace BoingKit
{
  public static class BoingWorkAsynchronous
  {
    #region Registry

    internal static void PostUnregisterBehaviorCleanUp()
    {
      if (s_behaviorJobNeedsGather)
      {
        s_hBehaviorJob.Complete();
        s_aBehaviorParams.Dispose();
        s_aBehaviorOutput.Dispose();
        s_behaviorJobNeedsGather = false;
      }
    }

    internal static void PostUnregisterEffectorReactorCleanUp()
    {
      if (s_reactorJobNeedsGather)
      {
        s_hReactorJob.Complete();
        s_aEffectors.Dispose();
        s_aReactorExecParams.Dispose();
        s_aReactorExecOutput.Dispose();
        s_reactorJobNeedsGather = false;
      }
    }

    #endregion


    #region Behavior

    private struct BehaviorJob : IJobParallelFor
    {
      public NativeArray<BoingWork.Params> Params;
      public NativeArray<BoingWork.Output> Output;
      public float DeltaTime;
      public float FixedDeltaTime;

      public void Execute(int index)
      {
        var ep = Params[index];

        if (ep.Bits.IsBitSet((int) BoingWork.ReactorFlags.FixedUpdate))
        {
          ep.Execute(FixedDeltaTime);
        }
        else
        {
          ep.Execute(DeltaTime);
        }

        Output[index] = new BoingWork.Output(ep.InstanceID, ref ep.Instance.PositionSpring, ref ep.Instance.RotationSpring, ref ep.Instance.ScaleSpring);
      }
    }

    private static bool s_behaviorJobNeedsGather = false;
    private static JobHandle s_hBehaviorJob;
    private static NativeArray<BoingWork.Params> s_aBehaviorParams;
    private static NativeArray<BoingWork.Output> s_aBehaviorOutput;

    internal static void ExecuteBehaviors
    (
      Dictionary<int, BoingBehavior> behaviorMap, 
      BoingManager.UpdateMode updateMode
    )
    {
      int numBehaviors = 0;

      // kick job
      {
        Profiler.BeginSample("Kick Behavior Job");
        Profiler.BeginSample("Allocate");
        s_aBehaviorParams = new NativeArray<BoingWork.Params>(behaviorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        s_aBehaviorOutput = new NativeArray<BoingWork.Output>(behaviorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        Profiler.EndSample();
        {
          Profiler.BeginSample("Push Data");
          foreach (var itBehavior in behaviorMap)
          {
            var behavior = itBehavior.Value;
            if (behavior.UpdateMode != updateMode)
              continue;

            behavior.PrepareExecute();
            s_aBehaviorParams[numBehaviors++] = behavior.Params;
          }
          Profiler.EndSample();
        }

        if (numBehaviors > 0)
        {
          var job = new BehaviorJob()
          {
            Params = s_aBehaviorParams, 
            Output = s_aBehaviorOutput, 
            DeltaTime = BoingManager.DeltaTime, 
            FixedDeltaTime = BoingManager.FixedDeltaTime, 
          };

          int jobInnerLoopBatchCount = (int) Mathf.Ceil(numBehaviors / ((float) Environment.ProcessorCount));

          s_hBehaviorJob = job.Schedule(numBehaviors, jobInnerLoopBatchCount);
          JobHandle.ScheduleBatchedJobs();
        }
        Profiler.EndSample();

        s_behaviorJobNeedsGather = true;
      }

      // gather job
      {
        if (s_behaviorJobNeedsGather)
        {
          Profiler.BeginSample("Gather Behavior Job");
          if (numBehaviors > 0)
          {
            s_hBehaviorJob.Complete();
            for (int iBehavior = 0; iBehavior < numBehaviors; ++iBehavior)
              s_aBehaviorOutput[iBehavior].GatherOutput(behaviorMap, updateMode);
          }
          s_aBehaviorParams.Dispose();
          s_aBehaviorOutput.Dispose();
          Profiler.EndSample();

          s_behaviorJobNeedsGather = false;
        }
      }
    }

    #endregion // Behavior


    #region Reactor

    private struct ReactorJob : IJobParallelFor
    {
      [ReadOnly] public NativeArray<BoingEffector.Params> Effectors;
      public NativeArray<BoingWork.Params> Params;
      public NativeArray<BoingWork.Output> Output;
      public float DeltaTime;
      public float FixedDeltaTime;

      public void Execute(int index)
      {
        var rep = Params[index];

        for (int i = 0, n = Effectors.Length; i < n; ++i)
        {
          var eep = Effectors[i];
          rep.AccumulateTarget(ref eep, DeltaTime);
        }
        rep.EndAccumulateTargets();

        if (rep.Bits.IsBitSet((int) BoingWork.ReactorFlags.FixedUpdate))
        {
          rep.Execute(FixedDeltaTime);
        }
        else
        {
          rep.Execute(BoingManager.DeltaTime);
        }

        Output[index] = new BoingWork.Output(rep.InstanceID, ref rep.Instance.PositionSpring, ref rep.Instance.RotationSpring, ref rep.Instance.ScaleSpring);
      }
    }

    private static bool s_reactorJobNeedsGather = false;
    private static JobHandle s_hReactorJob;
    private static NativeArray<BoingEffector.Params> s_aEffectors;
    private static NativeArray<BoingWork.Params> s_aReactorExecParams;
    private static NativeArray<BoingWork.Output> s_aReactorExecOutput;

    internal static void ExecuteReactors
    (
      Dictionary<int, BoingEffector> effectorMap, 
      Dictionary<int, BoingReactor> reactorMap, 
      Dictionary<int, BoingReactorField> fieldMap, 
      Dictionary<int, BoingReactorFieldCPUSampler> cpuSamplerMap, 
      BoingManager.UpdateMode updateMode
    )
    {
      int numReactors = 0;

      // kick job
      {
        Profiler.BeginSample("Kick Reactor Job");
        s_aEffectors = new NativeArray<BoingEffector.Params>(effectorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        s_aReactorExecParams = new NativeArray<BoingWork.Params>(reactorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        s_aReactorExecOutput = new NativeArray<BoingWork.Output>(reactorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        {
          Profiler.BeginSample("Push Data");
          foreach (var itReactor in reactorMap)
          {
            var reactor = itReactor.Value;
            if (reactor.UpdateMode != updateMode)
              continue;

            reactor.PrepareExecute();
            s_aReactorExecParams[numReactors++] = reactor.Params;
          }
          if (numReactors > 0)
          {
            int iEffector = 0;
            var eep = new BoingEffector.Params();
            foreach (var itEffector in effectorMap)
            {
              var effector = itEffector.Value;
              eep.Fill(itEffector.Value);
              s_aEffectors[iEffector++] = eep;
            }
          }
          Profiler.EndSample();
        }
        if (numReactors > 0)
        {
          var job = new ReactorJob()
          {
            Effectors = s_aEffectors, 
            Params = s_aReactorExecParams, 
            Output = s_aReactorExecOutput, 
            DeltaTime = BoingManager.DeltaTime,  
            FixedDeltaTime = BoingManager.FixedDeltaTime,
          };
          s_hReactorJob = job.Schedule(numReactors, 32);
          JobHandle.ScheduleBatchedJobs();
        }
        Profiler.EndSample();

        Profiler.BeginSample("Update Fields (CPU)");
        foreach (var itField in fieldMap)
        {
          var field = itField.Value;
          switch (field.HardwareMode)
          {
            case BoingReactorField.HardwareModeEnum.CPU:
              field.ExecuteCpu(BoingManager.DeltaTime);
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

        s_reactorJobNeedsGather = true;
      }

      // gather job
      {
        if (s_reactorJobNeedsGather)
        {
          Profiler.BeginSample("Gather Reactor Job");
          if (numReactors > 0)
          {
            s_hReactorJob.Complete();

            Profiler.BeginSample("Pull Data");
            for (int iReactor = 0; iReactor < numReactors; ++iReactor)
            {
              s_aReactorExecOutput[iReactor].GatherOutput(reactorMap, updateMode);
            }
            Profiler.EndSample();
          }
          s_aEffectors.Dispose();
          s_aReactorExecParams.Dispose();
          s_aReactorExecOutput.Dispose();
          Profiler.EndSample();

          s_reactorJobNeedsGather = false;
        }
      }
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

#endif
