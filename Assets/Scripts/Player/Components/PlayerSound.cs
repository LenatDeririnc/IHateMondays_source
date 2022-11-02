using UnityEngine;

namespace Characters.Components
{
    public class PlayerSound : MonoBehaviour
    {
        [Range(0, 1)] public float volume = 1;
        public AudioClip Jump;
        public AudioClip Grounding;
        public AudioClip[] walkSteps;
    }
}