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
  // post-render
  public class BoingManagerPreUpdatePump : MonoBehaviour
  {
    private int m_lastPumpedFrame = -1;

    private void FixedUpdate()
    {
      TryPump();
    }

    private void Update()
    {
      TryPump();
    }

    private void TryPump()
    {
      if (m_lastPumpedFrame >= Time.frameCount)
        return;

      if (m_lastPumpedFrame >= 0)
        DoPump();

      m_lastPumpedFrame = Time.frameCount;
    }

    private void DoPump()
    {
      BoingManager.RestoreBehaviors();
      BoingManager.RestoreReactors();
      BoingManager.RestoreBones();

      // do this post-render so we have one frame to finish compute dispatches
      BoingManager.DispatchReactorFieldCompute();
    }
  }
}
