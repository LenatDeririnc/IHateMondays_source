/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEngine;

namespace BoingKit
{
  public class BoingReactorFieldCPUSampler : MonoBehaviour
  {
    public BoingReactorField ReactorField;

    [Tooltip(
        "Match this mode with how you update your object's transform.\n\n" 
      + "Update - Use this mode if you update your object's transform in Update(). This uses variable Time.detalTime. Use FixedUpdate if physics simulation becomes unstable.\n\n" 
      + "Fixed Update - Use this mode if you update your object's transform in FixedUpdate(). This uses fixed Time.fixedDeltaTime. " 
      + "Also, use this mode if the game object is affected by Unity physics (i.e. has a rigid body component), which uses fixed updates."
    )]
    public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

    [Range(0.0f, 10.0f)]
    [Tooltip(
        "Multiplier on positional samples from reactor field.\n" 
      + "1.0 means 100%."
    )]
    public float PositionSampleMultiplier = 1.0f;

    [Range(0.0f, 10.0f)]
    [Tooltip(
        "Multiplier on rotational samples from reactor field.\n" 
      + "1.0 means 100%."
    )]
    public float RotationSampleMultiplier = 1.0f;

    private Vector3 m_objPosition;
    private Quaternion m_objRotation;

    public void OnEnable()
    {
      BoingManager.Register(this);
    }

    public void OnDisable()
    {
      BoingManager.Unregister(this);
    }

    #if UNITY_EDITOR
    private static bool s_warnedComponent = false;
    private static bool s_warnedHardwareMode = false;
    #endif

    public void SampleFromField()
    {
      m_objPosition = transform.position;
      m_objRotation = transform.rotation;

      if (ReactorField == null)
        return;

      var comp = ReactorField.GetComponent<BoingReactorField>();
      if (comp == null)
      {
        #if UNITY_EDITOR
        if (!s_warnedComponent)
        {
          Debug.LogWarning("The assigned ReactorField game object must have a BoingReactorField component for BoingReactorFieldCpuSampler components to sample from.");
          s_warnedComponent = true;
        }
        #endif

        return;
      }

      if (comp.HardwareMode != BoingReactorField.HardwareModeEnum.CPU)
      {
        #if UNITY_EDITOR
        if (!s_warnedHardwareMode)
        {
          Debug.LogWarning("The BoingReactorField component needs to be set to CPU hardware mode for BoingReactorFieldCpuSampler components to sample from.");
          s_warnedHardwareMode = true;
        }
        #endif

        return;
      }

      Vector3 positionOffset;
      Vector4 rotationOffset;
      if (!comp.SampleCpuGrid(transform.position, out positionOffset, out rotationOffset))
        return;

      transform.position = m_objPosition + positionOffset * PositionSampleMultiplier;
      transform.rotation = QuaternionUtil.Pow(QuaternionUtil.FromVector4(rotationOffset), RotationSampleMultiplier) * m_objRotation;
    }

    public void Restore()
    {
      transform.position = m_objPosition;
      transform.rotation = m_objRotation;
    }
  }
}
