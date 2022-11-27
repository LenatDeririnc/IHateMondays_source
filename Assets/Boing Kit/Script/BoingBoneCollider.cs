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
  public class BoingBoneCollider : MonoBehaviour
  {
    public enum Type
    {
      Sphere, 
      Capsule, 
      Box, 

      // TODO?
      /*
      InverseSphere, 
      InverseCapsule, 
      InverseBox, 
      */
    }

    public Type Shape = Type.Sphere;

    public float Radius = 0.1f;
    public float Height = 0.25f;
    public Vector3 Dimensions = new Vector3(0.1f, 0.1f, 0.1f);

    public Bounds Bounds
    {
      get
      {
        switch (Shape)
        {
        case Type.Sphere:
          {
            float minScale = VectorUtil.MinComponent(transform.localScale);
            return new Bounds(transform.position, 2.0f * minScale * Radius * Vector3.one);
          }

        case Type.Capsule:
          {
            float minScale = VectorUtil.MinComponent(transform.localScale);
            return new Bounds(transform.position, 2.0f * minScale * Radius * Vector3.one + Height * VectorUtil.ComponentWiseAbs(transform.rotation * Vector3.up));
          }

        case Type.Box:
          {
            return new Bounds(transform.position, VectorUtil.ComponentWiseMult(transform.localScale, VectorUtil.ComponentWiseAbs(transform.rotation * Dimensions)));
          }

        // TODO?
        /*
        case Type.InverseSphere:
        case Type.InverseCapsule:
        case Type.InverseBox:
          {
            return new Bounds(Vector3.zero, float.MaxValue * Vector3.one);
          }
        */
        }

        return new Bounds();
      }
    }

    public bool Collide(Vector3 boneCenter, float boneRadius, out Vector3 push)
    {
      switch (Shape)
      {
      case Type.Sphere:
        {
          float minScale = VectorUtil.MinComponent(transform.localScale);
          return Collision.SphereSphere(boneCenter, boneRadius, transform.position, minScale * Radius, out push);
        }

      case Type.Capsule:
        {
          float minScale = VectorUtil.MinComponent(transform.localScale);
          Vector3 head = transform.TransformPoint(0.5f * Height * Vector3.up);
          Vector3 tail = transform.TransformPoint(0.5f * Height * Vector3.down);
          return Collision.SphereCapsule(boneCenter, boneRadius, head, tail, minScale * Radius, out push);
        }

      case Type.Box:
        {
          Vector3 centerLs = transform.InverseTransformPoint(boneCenter);
          Vector3 scaledHalfExtents = 0.5f * VectorUtil.ComponentWiseMult(transform.localScale, Dimensions);
          bool collided = Collision.SphereBox(centerLs, boneRadius, scaledHalfExtents, out push);
          if (!collided)
            return false;

          push = transform.TransformVector(push);
          return true;
        }

        // TODO?
        /*
        case Type.InverseSphere:
          {
            float minScale = VectorUtil.MinComponent(transform.localScale);
            return Collision.SphereSphereInverse(boneCenter, boneRadius, transform.position, minScale * Radius, out push);
          }

        case Type.InverseCapsule:
          {
            float minScale = VectorUtil.MinComponent(transform.localScale);
            Vector3 head = transform.TransformPoint(0.5f * Height * Vector3.up);
            Vector3 tail = transform.TransformPoint(0.5f * Height * Vector3.down);
            return Collision.SphereCapsuleInverse(boneCenter, boneRadius, head, tail, minScale * Radius, out push);
          }
        */
      }

      push = Vector3.zero;
      return false;
    }

    public void OnValidate()
    {
      Radius = Mathf.Max(0.0f, Radius);

      Dimensions.x = Mathf.Max(0.0f, Dimensions.x);
      Dimensions.y = Mathf.Max(0.0f, Dimensions.y);
      Dimensions.z = Mathf.Max(0.0f, Dimensions.z);
    }

    public void OnDrawGizmos()
    {
      DrawGizmos();
    }

    public void DrawGizmos()
    {
      switch (Shape)
      {
      case Type.Sphere:
      //case Type.InverseSphere: // TODO?
        {
          float minScale = VectorUtil.MinComponent(transform.localScale);
          float scaledRadius = minScale * Radius;

          Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

          if (Shape == Type.Sphere)
          {
            Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            Gizmos.DrawSphere(Vector3.zero, scaledRadius);
          }

          Gizmos.color = Color.white;
          Gizmos.DrawWireSphere(Vector3.zero, scaledRadius);

          Gizmos.matrix = Matrix4x4.identity;
        }
        break;

      case Type.Capsule:
      //case Type.InverseCapsule: // TODO?
        {
          float minScale = VectorUtil.MinComponent(transform.localScale);
          float scaledRadius = minScale * Radius;
          float scaledHalfHeight = 0.5f * minScale * Height;

          Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

          if (Shape == Type.Capsule)
          {
            Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            Gizmos.DrawSphere(scaledHalfHeight * Vector3.up, scaledRadius);
            Gizmos.DrawSphere(scaledHalfHeight * Vector3.down, scaledRadius);
          }

          Gizmos.color = Color.white;
          Gizmos.DrawWireSphere(scaledHalfHeight * Vector3.up, scaledRadius);
          Gizmos.DrawWireSphere(scaledHalfHeight * Vector3.down, scaledRadius);
          for (int i = 0; i < 4; ++i)
          {
            float theta = i * MathUtil.HalfPi;
            Vector3 offset = new Vector3(scaledRadius * Mathf.Cos(theta), 0.0f, scaledRadius * Mathf.Sin(theta));
            Gizmos.DrawLine(offset + scaledHalfHeight * Vector3.up, offset + scaledHalfHeight * Vector3.down);
          }

          Gizmos.matrix = Matrix4x4.identity;
        }
        break;
        
      case Type.Box:
      //case Type.InverseBox: // TODO?
        {
          Vector3 scaledDimensions = VectorUtil.ComponentWiseMult(transform.localScale, Dimensions);
          Gizmos.matrix = transform.localToWorldMatrix;

          if (Shape == Type.Box)
          {
            Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            Gizmos.DrawCube(Vector3.zero, scaledDimensions);
          }

          Gizmos.color = Color.white;
          Gizmos.DrawWireCube(Vector3.zero, scaledDimensions);

          Gizmos.matrix = Matrix4x4.identity;
        }
        break;
      }
    }
  }
}
