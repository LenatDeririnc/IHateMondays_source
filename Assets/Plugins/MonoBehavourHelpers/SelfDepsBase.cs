using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Plugins.MonoBehSelfDeps
{
    public abstract class SelfDepsBase : MonoBehaviour, ISelfDeps
    {
        public abstract void SetupDeps();
    }
}