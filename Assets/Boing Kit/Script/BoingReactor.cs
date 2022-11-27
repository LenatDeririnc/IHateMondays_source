/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

namespace BoingKit
{
  public class BoingReactor : BoingBehavior
  {
    protected override void Register()
    {
      BoingManager.Register(this);
    }

    protected override void Unregister()
    {
      BoingManager.Unregister(this);
    }

    public override void PrepareExecute()
    {
      PrepareExecute(true);
    }
  }
}
