using Scenes.PlayerSpawner;
using UnityEngine;

namespace Scenes
{
    public class PlayerSpawnPointComponent : MonoBehaviour
    {
        [SerializeField] PlayerSpawnPointInfo spawnInfo;
        public PlayerSpawnPointInfo SpawnInfo => spawnInfo;

        private void OnDrawGizmos()
        {
            var l_transform = transform;
            var position = l_transform.position;
            var forward = l_transform.forward;
            var right = l_transform.right;
            var playerHeight = new Vector3(0.6f, 1.6f, 0.6f);

            var lineEnd = .75f;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(position, .125f);
            Gizmos.DrawWireCube(position + l_transform.up * playerHeight.y/2, playerHeight);
            Gizmos.DrawRay(position, transform.forward * lineEnd);
            Gizmos.DrawRay(position + forward * lineEnd, (right - forward) * .25f);
            Gizmos.DrawRay(position + forward * lineEnd, (-right - forward) * .25f);
        }
    }
}