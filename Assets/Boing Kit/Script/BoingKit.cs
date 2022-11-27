/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System;

namespace BoingKit
{
  public static class BoingKit
  {
    public static readonly Version Version = new Version(1, 2, 37);
  }

  public struct Version : IEquatable<Version>
  {
    public static readonly Version Invalid = new Version(-1, -1, -1);
    public static readonly Version FirstTracked = new Version(1, 2, 33);
    public static readonly Version LastUntracked = new Version(1, 2, 32);

    public int MajorVersion { get; }
    public int MinorVersion { get; }
    public int Revision { get; }
    public override string ToString() { return MajorVersion + "." + MinorVersion + "." + Revision; }
    public bool IsValid() { return MajorVersion >= 0 && MinorVersion >= 0 && Revision >= 0; }

    public Version(int majorVersion = -1, int minorVersion = -1, int revision = -1)
    {
      MajorVersion = majorVersion;
      MinorVersion = minorVersion;
      Revision = revision;
    }

    public static bool operator ==(Version lhs, Version rhs)
    {
      if (!lhs.IsValid())
        return false;

      if (!rhs.IsValid())
        return false;

      return 
           lhs.MajorVersion == rhs.MajorVersion 
        && lhs.MinorVersion == rhs.MinorVersion 
        && lhs.Revision == rhs.Revision;
    }

    public static bool operator !=(Version lhs, Version rhs) => !(lhs == rhs);

    public override bool Equals(object obj)
    {
      return obj is Version && Equals((Version)obj);
    }

    public bool Equals(Version other)
    {
      return MajorVersion == other.MajorVersion &&
             MinorVersion == other.MinorVersion &&
             Revision == other.Revision;
    }

    public override int GetHashCode()
    {
      var hashCode = 366299368;
      hashCode = hashCode * -1521134295 + MajorVersion.GetHashCode();
      hashCode = hashCode * -1521134295 + MinorVersion.GetHashCode();
      hashCode = hashCode * -1521134295 + Revision.GetHashCode();
      return hashCode;
    }
  }
}
