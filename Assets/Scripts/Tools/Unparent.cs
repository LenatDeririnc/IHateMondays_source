using UnityEngine;

namespace Tools
{
    public class Unparent : MonoBehaviour
    {
        private void Awake()
        {
            transform.parent = null;
        }
    }
}