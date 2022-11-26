using System;
using UnityEngine;

namespace Tools
{
    public class TriggerRenderer : MonoBehaviour
    {
        public enum TriggerType
        {
            Box,
            BoxOffseted,
            Sphere
        }
        
        [SerializeField] private Color _color = Color.red;
        [SerializeField] private TriggerType _triggerType = TriggerType.Box;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = _color;
            Matrix4x4 rotationMatrix;
            switch (_triggerType) {
                case TriggerType.Box:
                    rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                    Gizmos.matrix = rotationMatrix;
                    Gizmos.DrawWireCube(Vector3.zero,Vector3.one);
                    break;
                case TriggerType.BoxOffseted:
                    var boxCollider = GetComponent<BoxCollider>();
                    if (boxCollider == null)
                        break;
                    rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation,  Vector3.Scale(transform.lossyScale, boxCollider.size));
                    Gizmos.matrix = rotationMatrix;
                    Gizmos.DrawWireCube(boxCollider.center, Vector3.one);
                    break;
                case TriggerType.Sphere:
                    var sphereCollider = GetComponent<SphereCollider>();
                    if (sphereCollider == null)
                        break;
                    var max = transform.lossyScale.x;
                    if (transform.lossyScale.y > max)
                        max = transform.lossyScale.y;
                    if (transform.lossyScale.z > max)
                        max = transform.lossyScale.z;
                    rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(max, max, max));
                    Gizmos.matrix = rotationMatrix;
                    Gizmos.DrawWireSphere(Vector3.zero, sphereCollider.radius);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
        }
    }
}