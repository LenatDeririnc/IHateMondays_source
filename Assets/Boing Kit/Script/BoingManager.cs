/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

#if UNITY_2019_1_OR_NEWER
using UnityEngine.Rendering;
#endif

#if UNITY_2018_1_OR_NEWER
using Unity.Collections;
using Unity.Jobs;
#endif

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace BoingKit
{
  public static class BoingManager
  {
    #region Public

    public enum UpdateMode
    {
      FixedUpdate, 
      EarlyUpdate, 
      LateUpdate, 
    }

    public enum TranslationLockSpace
    {
      Global, 
      Local, 
    }

    // enumerables: use these to iterate through objects already registered
    public static IEnumerable<BoingBehavior> Behaviors { get { return s_behaviorMap.Values; } }
    public static IEnumerable<BoingReactor> Reactors { get { return s_reactorMap.Values; } }
    public static IEnumerable<BoingEffector> Effectors { get { return s_effectorMap.Values; } }
    public static IEnumerable<BoingReactorField> ReactorFields { get { return s_fieldMap.Values; } }
    public static IEnumerable<BoingReactorFieldCPUSampler> ReactorFieldCPUSamlers { get { return s_cpuSamplerMap.Values; } }
    public static IEnumerable<BoingReactorFieldGPUSampler> ReactorFieldGPUSampler { get { return s_gpuSamplerMap.Values; } }

    // callbacks: use these to listen for register/unregister events
    public static BehaviorRegisterDelegate OnBehaviorRegister;
    public static BehaviorUnregisterDelegate OnBehaviorUnregister;
    public static EffectorRegisterDelegate OnEffectorRegister;
    public static EffectorUnregisterDelegate OnEffectorUnregister;
    public static ReactorRegisterDelegate OnReactorRegister;
    public static ReactorUnregisterDelegate OnReactorUnregister;
    public static ReactorFieldRegisterDelegate OnReactorFieldRegister;
    public static ReactorFieldUnregisterDelegate OnReactorFieldUnregister;
    public static ReactorFieldCPUSamplerRegisterDelegate OnReactorFieldCPUSamplerRegister;
    public static ReactorFieldCPUSamplerUnregisterDelegate OnReactorFieldCPUSamplerUnregister;
    public static ReactorFieldGPUSamplerRegisterDelegate OnReactorFieldGPUSamplerRegister;
    public static ReactorFieldGPUSamplerUnregisterDelegate OnFieldGPUSamplerUnregister;
    public static BonesRegisterDelegate OnBonesRegister;
    public static BonesUnregisterDelegate OnBonesUnregister;

    // delegates
    public delegate void BehaviorRegisterDelegate(BoingBehavior behavior);
    public delegate void BehaviorUnregisterDelegate(BoingBehavior behavior);
    public delegate void EffectorRegisterDelegate(BoingEffector effector);
    public delegate void EffectorUnregisterDelegate(BoingEffector effector);
    public delegate void ReactorRegisterDelegate(BoingReactor reactor);
    public delegate void ReactorUnregisterDelegate(BoingReactor reactor);
    public delegate void ReactorFieldRegisterDelegate(BoingReactorField field);
    public delegate void ReactorFieldUnregisterDelegate(BoingReactorField field);
    public delegate void ReactorFieldCPUSamplerRegisterDelegate(BoingReactorFieldCPUSampler sampler);
    public delegate void ReactorFieldCPUSamplerUnregisterDelegate(BoingReactorFieldCPUSampler sampler);
    public delegate void ReactorFieldGPUSamplerRegisterDelegate(BoingReactorFieldGPUSampler sampler);
    public delegate void ReactorFieldGPUSamplerUnregisterDelegate(BoingReactorFieldGPUSampler sampler);
    public delegate void BonesRegisterDelegate(BoingBones bones);
    public delegate void BonesUnregisterDelegate(BoingBones bones);

    #endregion


    #region Timer

    private static float s_deltaTime = 0.0f;
    public static float DeltaTime => s_deltaTime;
    public static float FixedDeltaTime => Time.fixedDeltaTime;

    #endregion


    #region Registry

    private static Dictionary<int, BoingBehavior> s_behaviorMap = new Dictionary<int, BoingBehavior>();
    private static Dictionary<int, BoingEffector> s_effectorMap = new Dictionary<int, BoingEffector>();
    private static Dictionary<int, BoingReactor> s_reactorMap = new Dictionary<int, BoingReactor>();
    private static Dictionary<int, BoingReactorField> s_fieldMap = new Dictionary<int, BoingReactorField>();
    private static Dictionary<int, BoingReactorFieldCPUSampler> s_cpuSamplerMap = new Dictionary<int, BoingReactorFieldCPUSampler>();
    private static Dictionary<int, BoingReactorFieldGPUSampler> s_gpuSamplerMap = new Dictionary<int, BoingReactorFieldGPUSampler>();
    private static Dictionary<int, BoingBones> s_bonesMap = new Dictionary<int, BoingBones>();

    // for reactor fields
    private static readonly int kEffectorParamsIncrement = 16;
    private static List<BoingEffector.Params> s_effectorParamsList = new List<BoingEffector.Params>(kEffectorParamsIncrement);
    private static BoingEffector.Params[] s_aEffectorParams;
    private static ComputeBuffer s_effectorParamsBuffer;
    private static Dictionary<int, int> s_effectorParamsIndexMap = new Dictionary<int, int>();

    internal static int NumBehaviors { get { return s_behaviorMap.Count; } }
    internal static int NumEffectors { get { return s_effectorMap.Count; } }
    internal static int NumReactors { get { return s_reactorMap.Count; } }
    internal static int NumFields { get { return s_fieldMap.Count; } }
    internal static int NumCPUFieldSamplers { get { return s_cpuSamplerMap.Count; } }
    internal static int NumGPUFieldSamplers { get { return s_gpuSamplerMap.Count; } }

    #if UNITY_2018_1_OR_NEWER
    internal static readonly bool UseAsynchronousJobs = true;
    #endif

    internal static GameObject s_managerGo;
    private static void ValidateManager()
    {
      if (s_managerGo != null)
        return;

      s_managerGo = new GameObject("Boing Kit manager (don't delete)");
      s_managerGo.AddComponent<BoingManagerPreUpdatePump>();
      s_managerGo.AddComponent<BoingManagerPostUpdatePump>();
      UnityEngine.Object.DontDestroyOnLoad(s_managerGo);

      // shared collider
      var collider = s_managerGo.AddComponent<SphereCollider>();
      collider.enabled = false;
    }

    internal static SphereCollider SharedSphereCollider
    {
      get
      {
        if (s_managerGo == null)
          return null;

        return s_managerGo.GetComponent<SphereCollider>();
      }
    }

    internal static void Register(BoingBehavior behavior)
    {
      PreRegisterBehavior();
      s_behaviorMap.Add(behavior.GetInstanceID(), behavior);

      if (OnBehaviorRegister != null)
        OnBehaviorRegister(behavior);
    }

    internal static void Unregister(BoingBehavior behavior)
    {
      if (OnBehaviorUnregister != null)
        OnBehaviorUnregister(behavior);

      s_behaviorMap.Remove(behavior.GetInstanceID());
      PostUnregisterBehavior();
    }

    internal static void Register(BoingEffector effector)
    {
      PreRegisterEffectorReactor();
      s_effectorMap.Add(effector.GetInstanceID(), effector);

      if (OnEffectorRegister != null)
        OnEffectorRegister(effector);
    }

    internal static void Unregister(BoingEffector effector)
    {
      if (OnEffectorUnregister != null)
        OnEffectorUnregister(effector);

      s_effectorMap.Remove(effector.GetInstanceID());
      PostUnregisterEffectorReactor();
    }

    internal static void Register(BoingReactor reactor)
    {
      PreRegisterEffectorReactor();
      s_reactorMap.Add(reactor.GetInstanceID(), reactor);

      if (OnReactorRegister != null)
        OnReactorRegister(reactor);
    }

    internal static void Unregister(BoingReactor reactor)
    {
      if (OnReactorUnregister != null)
        OnReactorUnregister(reactor);

      s_reactorMap.Remove(reactor.GetInstanceID());
      PostUnregisterEffectorReactor();
    }

    internal static void Register(BoingReactorField field)
    {
      PreRegisterEffectorReactor();
      s_fieldMap.Add(field.GetInstanceID(), field);

      if (OnReactorFieldRegister != null)
        OnReactorFieldRegister(field);
    }

    internal static void Unregister(BoingReactorField field)
    {
      if (OnReactorFieldUnregister != null)
        OnReactorFieldUnregister(field);

      s_fieldMap.Remove(field.GetInstanceID());
      PostUnregisterEffectorReactor();
    }

    internal static void Register(BoingReactorFieldCPUSampler sampler)
    {
      PreRegisterEffectorReactor();
      s_cpuSamplerMap.Add(sampler.GetInstanceID(), sampler);

      if (OnReactorFieldCPUSamplerRegister != null)
        OnReactorFieldCPUSamplerUnregister(sampler);
    }

    internal static void Unregister(BoingReactorFieldCPUSampler sampler)
    {
      if (OnReactorFieldCPUSamplerUnregister != null)
        OnReactorFieldCPUSamplerUnregister(sampler);

      s_cpuSamplerMap.Remove(sampler.GetInstanceID());
      PostUnregisterEffectorReactor();
    }

    internal static void Register(BoingReactorFieldGPUSampler sampler)
    {
      PreRegisterEffectorReactor();
      s_gpuSamplerMap.Add(sampler.GetInstanceID(), sampler);

      if (OnReactorFieldGPUSamplerRegister != null)
        OnReactorFieldGPUSamplerRegister(sampler);
    }

    internal static void Unregister(BoingReactorFieldGPUSampler sampler)
    {
      if (OnFieldGPUSamplerUnregister != null)
        OnFieldGPUSamplerUnregister(sampler);

      s_gpuSamplerMap.Remove(sampler.GetInstanceID());
      PostUnregisterEffectorReactor();
    }

    internal static void Register(BoingBones bones)
    {
      PreRegisterBones();
      s_bonesMap.Add(bones.GetInstanceID(), bones);

      if (OnBonesRegister != null)
        OnBonesRegister(bones);
    }

    internal static void Unregister(BoingBones bones)
    {
      if (OnBonesUnregister != null)
        OnBonesUnregister(bones);

      s_bonesMap.Remove(bones.GetInstanceID());
      PostUnregisterBones();
    }

    private static void PreRegisterBehavior()
    {
      ValidateManager();
    }

    private static void PostUnregisterBehavior()
    {
      if (s_behaviorMap.Count > 0)
        return;

      #if UNITY_2018_1_OR_NEWER
      BoingWorkAsynchronous.PostUnregisterBehaviorCleanUp();
      #endif
    }

    private static void PreRegisterEffectorReactor()
    {
      ValidateManager();

      if (s_effectorParamsBuffer == null)
      {
        s_effectorParamsList = new List<BoingEffector.Params>(kEffectorParamsIncrement);
        s_effectorParamsBuffer = new ComputeBuffer(s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
      }

      if (s_effectorMap.Count >= s_effectorParamsList.Capacity)
      {
        s_effectorParamsList.Capacity += kEffectorParamsIncrement;
        s_effectorParamsBuffer.Dispose();
        s_effectorParamsBuffer = new ComputeBuffer(s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
      }
    }

    private static void PostUnregisterEffectorReactor()
    {
      if (s_effectorMap.Count > 0 || s_reactorMap.Count > 0 || s_fieldMap.Count > 0 || s_cpuSamplerMap.Count > 0 || s_gpuSamplerMap.Count > 0)
        return;

      s_effectorParamsList = null;
      s_effectorParamsBuffer.Dispose();
      s_effectorParamsBuffer = null;

      #if UNITY_2018_1_OR_NEWER
      BoingWorkAsynchronous.PostUnregisterEffectorReactorCleanUp();
      #endif
    }

    private static void PreRegisterBones()
    {
      ValidateManager();
    }

    private static void PostUnregisterBones()
    {
      
    }

    #endregion // Registry


    #region Update

    internal static void Execute(UpdateMode updateMode)
    {
      if (updateMode == UpdateMode.EarlyUpdate)
        s_deltaTime = Time.deltaTime;

      RefreshEffectorParams();

      // execute bones first, so they operate on unmodified transform hierarchy
      ExecuteBones(updateMode);

      ExecuteBehaviors(updateMode);
      ExecuteReactors(updateMode);
    }

    #endregion


    #region Behavior

    internal static void ExecuteBehaviors(UpdateMode updateMode)
    {
      if (s_behaviorMap.Count == 0)
        return;

      Profiler.BeginSample("BoingManager.ExecuteBehaviors");

      foreach (var itBehavior in s_behaviorMap)
      {
        var behavior = itBehavior.Value;
        if (behavior.InitRebooted)
          continue;
        
        behavior.Reboot();
        behavior.InitRebooted = true;
      }

      #if UNITY_2018_1_OR_NEWER
      if (UseAsynchronousJobs)
      {
        BoingWorkAsynchronous.ExecuteBehaviors(s_behaviorMap, updateMode);
      }
      else
      #endif
      {
        BoingWorkSynchronous.ExecuteBehaviors(s_behaviorMap, updateMode);
      }

      Profiler.EndSample();
    }

    internal static void PullBehaviorResults(UpdateMode updateMode)
    {
      Profiler.BeginSample("BoingManager.PullBehaviorResults");

      foreach (var itBehavior in s_behaviorMap)
      {
        var behavior = itBehavior.Value;
        if (behavior.UpdateMode != updateMode)
          continue;

        itBehavior.Value.PullResults();
      }
  
      Profiler.EndSample();
    }

    internal static void RestoreBehaviors()
    {
      Profiler.BeginSample("BoingManager.UpdateBehaviorsPostRender");

      foreach (var itBehavior in s_behaviorMap)
        itBehavior.Value.Restore();

      Profiler.EndSample();
    }

    #endregion // Behavior


    #region Effector

    internal static void RefreshEffectorParams()
    {
      if (s_effectorParamsList == null)
        return;

      s_effectorParamsIndexMap.Clear();
      s_effectorParamsList.Clear();
      foreach (var itEffector in s_effectorMap)
      {
        var effector = itEffector.Value;
        s_effectorParamsIndexMap.Add(effector.GetInstanceID(), s_effectorParamsList.Count);
        s_effectorParamsList.Add(new BoingEffector.Params(effector));
      }

      if (s_aEffectorParams == null || s_aEffectorParams.Length != s_effectorParamsList.Count)
        s_aEffectorParams = s_effectorParamsList.ToArray();
      else
        s_effectorParamsList.CopyTo(s_aEffectorParams);
    }

    #endregion


    #region Reactor

    internal static void ExecuteReactors(UpdateMode updateMode)
    {
      if (s_effectorMap.Count == 0 && s_reactorMap.Count == 0 && s_fieldMap.Count == 0 && s_cpuSamplerMap.Count == 0)
        return;

      Profiler.BeginSample("BoingManager.ExecuteReactors");

      foreach (var itReactor in s_reactorMap)
      {
        var reactor = itReactor.Value;
        if (reactor.InitRebooted)
          continue;

        reactor.Reboot();
        reactor.InitRebooted = true;
      }

      #if UNITY_2018_1_OR_NEWER
      if (UseAsynchronousJobs)
      {
        BoingWorkAsynchronous.ExecuteReactors(s_effectorMap, s_reactorMap, s_fieldMap, s_cpuSamplerMap, updateMode);
      }
      else
      #endif
      {
        BoingWorkSynchronous.ExecuteReactors(s_aEffectorParams, s_reactorMap, s_fieldMap, s_cpuSamplerMap, updateMode);
      }

      Profiler.EndSample();
    }

    internal static void PullReactorResults(UpdateMode updateMode)
    {
      Profiler.BeginSample("BoingManager.PullReactorResults");

      foreach (var itReactor in s_reactorMap)
      {
        var reactor = itReactor.Value;
        if (reactor.UpdateMode != updateMode)
          continue;

        itReactor.Value.PullResults();
      }

      foreach (var itSampler in s_cpuSamplerMap)
      {
        var sampler = itSampler.Value;
        if (sampler.UpdateMode != updateMode)
          continue;

        itSampler.Value.SampleFromField();
      }

      Profiler.EndSample();
    }

    internal static void RestoreReactors()
    {
      Profiler.BeginSample("BoingManager.PullReactorResults");

      foreach (var itReactor in s_reactorMap)
        itReactor.Value.Restore();

      foreach (var itSampler in s_cpuSamplerMap)
        itSampler.Value.Restore();

      Profiler.EndSample();
    }

    internal static void DispatchReactorFieldCompute()
    {
      if (s_effectorParamsBuffer == null)
        return;
      
      Profiler.BeginSample("Update Fields (GPU)");

      // do we need to call this again right before dispatch in case people add/remove effectors between LateUpdate() and PostRender()?
      //RefreshEffectorParams();

      s_effectorParamsBuffer.SetData(s_aEffectorParams);

      float dt = Time.deltaTime;
      foreach (var itField in s_fieldMap)
      {
        var field = itField.Value;
        switch (field.HardwareMode)
        {
          case BoingReactorField.HardwareModeEnum.GPU:
            field.ExecuteGpu(dt, s_effectorParamsBuffer, s_effectorParamsIndexMap);
            break;
        }
      }

      Profiler.EndSample();
    }

    #endregion // Reactor


    #region Bones

    internal static void ExecuteBones(UpdateMode updateMode)
    {
      if (s_bonesMap.Count == 0)
        return;

      Profiler.BeginSample("BoingManager.ExecuteBones");

      foreach (var itBones in s_bonesMap)
      {
        var bones = itBones.Value;
        if (bones.InitRebooted)
          continue;

        bones.Reboot();
        bones.InitRebooted = true;
      }

      #if UNITY_2018_1_OR_NEWER
      if (UseAsynchronousJobs)
      {
        BoingWorkAsynchronous.ExecuteBones(s_aEffectorParams, s_bonesMap, updateMode);
      }
      else
      #endif
      {
        BoingWorkSynchronous.ExecuteBones(s_aEffectorParams, s_bonesMap, updateMode);
      }

      Profiler.EndSample();
    }

    internal static void PullBonesResults(UpdateMode updateMode)
    {
      if (s_bonesMap.Count == 0)
        return;

      Profiler.BeginSample("BoingManager.PullBonesResults");

      #if UNITY_2018_1_OR_NEWER
      if (UseAsynchronousJobs)
      {
        BoingWorkAsynchronous.PullBonesResults(s_aEffectorParams, s_bonesMap, updateMode);
      }
      else
      #endif
      {
        BoingWorkSynchronous.PullBonesResults(s_aEffectorParams, s_bonesMap, updateMode);
      }

      Profiler.EndSample();
    }

    internal static void RestoreBones()
    {
      Profiler.BeginSample("BoingManager.RestoreBones");

      foreach (var itBones in s_bonesMap)
        itBones.Value.Restore();

      Profiler.EndSample();
    }

    #endregion // Bones
  }
}
