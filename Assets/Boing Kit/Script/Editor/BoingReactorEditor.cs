/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEditor;

namespace BoingKit
{
  [CustomEditor(typeof(BoingReactor))]
  [CanEditMultipleObjects]
  public class BoingReactorEditor : BoingBehaviorEditor
  {
    public BoingReactorEditor()
    {
      m_isReactor = true;
    }
  }

}
