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
  public class BoingReactorFieldGPUSampler : MonoBehaviour
  {
    public BoingReactorField ReactorField;

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

    private MaterialPropertyBlock m_matProps;
    private int m_fieldResourceSetId = -1;

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

    public void Update()
    {
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

      if (comp.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
      {
        #if UNITY_EDITOR
        if (!s_warnedHardwareMode)
        {
          Debug.LogWarning("The BoingReactorField component needs to be set to GPU hardware mode for BoingReactorFieldCpuSampler components to sample from.");
          s_warnedHardwareMode = true;
        }
        #endif

        return;
      }

      //-----------------------------------------------------------------------

      if (m_fieldResourceSetId != comp.GpuResourceSetId)
      {
        if (m_matProps == null)
          m_matProps = new MaterialPropertyBlock();

        if (comp.UpdateShaderConstants(m_matProps, PositionSampleMultiplier, RotationSampleMultiplier))
        {
          m_fieldResourceSetId = comp.GpuResourceSetId;

          var aRenderer = 
            new Renderer[]
            {
              GetComponent<MeshRenderer>(),
              GetComponent<SkinnedMeshRenderer>(), 
            };

          foreach (var renderer in aRenderer)
          {
            if (renderer == null)
              continue;

            renderer.SetPropertyBlock(m_matProps);
          }
        }
      }
    }
  }
}
