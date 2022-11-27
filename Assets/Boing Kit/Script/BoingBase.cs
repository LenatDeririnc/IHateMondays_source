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
  public class BoingBase : MonoBehaviour //, ISerializationCallbackReceiver
  {
    [SerializeField] private Version m_currentVersion;
    [SerializeField] private Version m_previousVersion;
    [SerializeField] private Version m_initialVersion = BoingKit.Version;
    public Version CurrentVersion => m_currentVersion;
    public Version PreviousVersion => m_previousVersion;
    public Version InitialVersion => m_initialVersion;

    protected virtual void OnUpgrade(Version oldVersion, Version newVersion)
    {
      m_previousVersion = m_currentVersion;

      // before version tracking was introduced
      if (m_currentVersion.Revision < 33)
      {
        m_initialVersion = Version.Invalid;
        m_previousVersion = Version.Invalid;
      }

      m_currentVersion = newVersion;
    }

    // NOTE: somehow this doesn't work as expected
    //       Unity always calls this function with current revision == 0 for the first time, even if 0 is never assigned in the code base
    /*
    public void OnAfterDeserialize()
    {
      if (m_currentVersion == BoingKit.Version)
        return;

      OnUpgrade(m_currentVersion, BoingKit.Version);
    }

    public void OnBeforeSerialize() { }
    */
  }
}
