using System;
using UnityEngine;

namespace Plugins.MonoBehHelpers
{
    public class UpdateGetter : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        public void UpdateInvoke()
        {
            if (!enabled)
                return;
            SentUpdate();
        }

        public void FixedUpdateInvoke()
        {
            if (!enabled)
                return;
            SentFixedUpdate();
        }

        protected virtual void SentUpdate()
        {
        }

        protected virtual void SentFixedUpdate()
        {
        }
    }
}