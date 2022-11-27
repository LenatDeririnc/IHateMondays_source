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
  // pre-render
  public class BoingManagerPostUpdatePump : MonoBehaviour
  {
    private void Start()
    {
      DontDestroyOnLoad(gameObject);
    }

    private bool TryDestroyDuplicate()
    {
      if (BoingManager.s_managerGo == gameObject)
        return false;

      // so reimporting scripts don't build up duplicate update pumps
      Destroy(gameObject);
      return true;
    }

    private void FixedUpdate()
    {
      if (TryDestroyDuplicate())
        return;

      BoingManager.Execute(BoingManager.UpdateMode.FixedUpdate);
    }

    private void Update()
    {
      if (TryDestroyDuplicate())
        return;

      BoingManager.Execute(BoingManager.UpdateMode.EarlyUpdate);

      BoingManager.PullBehaviorResults(BoingManager.UpdateMode.EarlyUpdate);
      BoingManager.PullReactorResults(BoingManager.UpdateMode.EarlyUpdate);
      BoingManager.PullBonesResults(BoingManager.UpdateMode.EarlyUpdate); // pull bones results last, so bone transforms don't inherit parent transform delta
    }

    private void LateUpdate()
    {
      if (TryDestroyDuplicate())
        return;

      BoingManager.PullBehaviorResults(BoingManager.UpdateMode.FixedUpdate);
      BoingManager.PullReactorResults(BoingManager.UpdateMode.FixedUpdate);
      BoingManager.PullBonesResults(BoingManager.UpdateMode.FixedUpdate); // pull bones results last, so bone transforms don't inherit parent transform delta

      BoingManager.Execute(BoingManager.UpdateMode.LateUpdate);

      BoingManager.PullBehaviorResults(BoingManager.UpdateMode.LateUpdate);
      BoingManager.PullReactorResults(BoingManager.UpdateMode.LateUpdate);
      BoingManager.PullBonesResults(BoingManager.UpdateMode.LateUpdate); // pull bones results last, so bone transforms don't inherit parent transform delta
    }
  }
}
