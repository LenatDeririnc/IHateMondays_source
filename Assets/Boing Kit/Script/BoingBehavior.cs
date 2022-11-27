/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace BoingKit
{
  public class BoingBehavior : BoingBase
  {
    public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

    public bool TwoDDistanceCheck      = false;
    public bool TwoDPositionInfluence  = false;
    public bool TwoDRotationInfluence  = false;
    public bool EnablePositionEffect   = true;
    public bool EnableRotationEffect   = true;
    public bool EnableScaleEffect      = false;
    public bool GlobalReactionUpVector = false;

    public BoingManager.TranslationLockSpace TranslationLockSpace = BoingManager.TranslationLockSpace.Global;
    public bool LockTranslationX = false;
    public bool LockTranslationY = false;
    public bool LockTranslationZ = false;

    public BoingWork.Params Params;
    public SharedBoingParams SharedParams;

    #if UNITY_2018_1_OR_NEWER
    internal bool PositionSpringDirty = false;
    internal bool RotationSpringDirty = false;
    internal bool ScaleSpringDirty    = false;
    #endif

    public Vector3Spring PositionSpring
    {
      get { return Params.Instance.PositionSpring; }
      set
      {
        Params.Instance.PositionSpring = value;

        #if UNITY_2018_1_OR_NEWER
        PositionSpringDirty = true;
        #endif
      }
    }

    public QuaternionSpring RotationSpring
    {
      get { return Params.Instance.RotationSpring; }
      set
      {
        Params.Instance.RotationSpring = value;

        #if UNITY_2018_1_OR_NEWER
        RotationSpringDirty = true;
        #endif
      }
    }

    public Vector3Spring ScaleSpring
    {
      get { return Params.Instance.ScaleSpring; }
      set
      {
        Params.Instance.ScaleSpring = value;

        #if UNITY_2018_1_OR_NEWER
        ScaleSpringDirty = true;
        #endif
      }
    }

    internal bool CachedTransformValid = false;
    internal Vector3 CachedPositionLs;
    internal Vector3 CachedPositionWs;
    internal Vector3 RenderPositionWs;
    internal Quaternion CachedRotationLs;
    internal Quaternion CachedRotationWs;
    internal Quaternion RenderRotationWs;
    internal Vector3 CachedScaleLs;
    internal Vector3 RenderScaleLs;

    internal bool InitRebooted = false;

    public BoingBehavior()
    {
      Params.Init();
    }

    public virtual void Reboot()
    {
      Params.Instance.PositionSpring.Reset(transform.position);
      Params.Instance.RotationSpring.Reset(transform.rotation);
      Params.Instance.ScaleSpring.Reset(transform.localScale);
      CachedPositionLs = transform.localPosition;
      CachedRotationLs = transform.localRotation;
      CachedPositionWs = transform.position;
      CachedRotationWs = transform.rotation;
      CachedScaleLs = transform.localScale;

      CachedTransformValid = true;
    }

    public virtual void OnEnable()
    {
      CachedTransformValid = false;
      InitRebooted = false;
      Register();
    }

    public void Start()
    {
      InitRebooted = false;
    }

    public virtual void OnDisable()
    {
      Unregister();
    }

    protected virtual void Register()
    {
      BoingManager.Register(this);
    }

    protected virtual void Unregister()
    {
      BoingManager.Unregister(this);
    }

    public void UpdateFlags()
    {
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.TwoDDistanceCheck     , TwoDDistanceCheck     );
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.TwoDPositionInfluence , TwoDPositionInfluence );
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.TwoDRotationInfluence , TwoDRotationInfluence );
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.EnablePositionEffect  , EnablePositionEffect  );
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.EnableRotationEffect  , EnableRotationEffect  );
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.EnableScaleEffect     , EnableScaleEffect  );
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.GlobalReactionUpVector, GlobalReactionUpVector);

      Params.Bits.SetBit((int) BoingWork.ReactorFlags.FixedUpdate, (UpdateMode == BoingManager.UpdateMode.FixedUpdate));
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.EarlyUpdate, (UpdateMode == BoingManager.UpdateMode.EarlyUpdate));
      Params.Bits.SetBit((int) BoingWork.ReactorFlags.LateUpdate, (UpdateMode == BoingManager.UpdateMode.LateUpdate));
    }

    public virtual void PrepareExecute()
    {
      PrepareExecute(false);
    }

    protected void PrepareExecute(bool accumulateEffectors)
    {
      if (SharedParams != null)
        BoingWork.Params.Copy(ref SharedParams.Params, ref Params);

      UpdateFlags();

      Params.InstanceID = GetInstanceID();

      #if UNITY_2018_1_OR_NEWER
      Params.Instance.PrepareExecute
      (
        ref Params, 
        CachedPositionWs, 
        CachedRotationWs, 
        transform.localScale, 
        accumulateEffectors
      );
      #else
      Params.Instance.PrepareExecute
      (
        ref Params, 
        transform.position, 
        transform.rotation, 
        transform.localScale, 
        accumulateEffectors
      );
      #endif
    }

    public void Execute(float dt)
    {
      Params.Execute(dt);
    }

    public void PullResults()
    {
      PullResults(ref Params);
    }

    public void GatherOutput(ref BoingWork.Output o)
    {
        #if UNITY_2018_1_OR_NEWER
        if (BoingManager.UseAsynchronousJobs)
        {
          if (PositionSpringDirty)
            PositionSpringDirty = false;
          else
            Params.Instance.PositionSpring = o.PositionSpring;

          if (RotationSpringDirty)
            RotationSpringDirty = false;
          else
            Params.Instance.RotationSpring = o.RotationSpring;

          if (ScaleSpringDirty)
            ScaleSpringDirty = false;
          else
            Params.Instance.ScaleSpring = o.ScaleSpring;
        }
        else
        #endif
        {
          Params.Instance.PositionSpring = o.PositionSpring;
          Params.Instance.RotationSpring = o.RotationSpring;
          Params.Instance.ScaleSpring = o.ScaleSpring;
        }
    }

    private void PullResults(ref BoingWork.Params p)
    {
      CachedPositionLs = transform.localPosition;
      CachedPositionWs = transform.position;
      RenderPositionWs = BoingWork.ComputeTranslationalResults(transform, transform.position, p.Instance.PositionSpring.Value, this);
      transform.position = RenderPositionWs;

      CachedRotationLs = transform.localRotation;
      CachedRotationWs = transform.rotation;
      RenderRotationWs = p.Instance.RotationSpring.ValueQuat;
      transform.rotation = RenderRotationWs;

      CachedScaleLs = transform.localScale;
      RenderScaleLs = p.Instance.ScaleSpring.Value;
      transform.localScale = RenderScaleLs;

      CachedTransformValid = true;
    }

    virtual public void Restore()
    {
      if (!CachedTransformValid)
        return;

      if (Application.isEditor)
      {
        // transforms can be manually modified in editor between post-update and pre-update
        // we respect that by skipping restoration of cached transforms

        if ((transform.position - RenderPositionWs).sqrMagnitude < 1e-4f)
          transform.localPosition = CachedPositionLs;

        if (QuaternionUtil.GetAngle(transform.rotation * Quaternion.Inverse(RenderRotationWs)) < 1e-2f)
          transform.localRotation = CachedRotationLs;

        if ((transform.localScale - RenderScaleLs).sqrMagnitude < 1e-4f)
          transform.localScale = CachedScaleLs;
      }
      else
      {
        transform.localPosition = CachedPositionLs;
        transform.localRotation = CachedRotationLs;
        transform.localScale = CachedScaleLs;
      }
    }
  }
}

