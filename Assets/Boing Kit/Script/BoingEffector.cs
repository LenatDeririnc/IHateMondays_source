/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System.Runtime.InteropServices;
using UnityEngine;

namespace BoingKit
{
  public class BoingEffector : BoingBase
  {
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct Params
    {
      public static readonly int Stride = //   80 bytes
          18 * sizeof(float)              // = 72 bytes
        +  2 * sizeof(int);               // +  8 bytes

      // bytes 0-47 (48 bytes)
      public Vector3 PrevPosition;
      private float m_padding0;
      public Vector3 CurrPosition;
      private float m_padding1;
      public Vector3 LinearVelocityDir;
      private float m_padding2;

      // bytes 48-63 (16 bytes)
      public float Radius;
      public float FullEffectRadius;
      public float MoveDistance;
      public float LinearImpulse;

      // bytes 64-79 (16 bytes)
      public float RotateAngle;
      public float AngularImpulse;
      public Bits32 Bits;
      private int m_padding3;

      public Params(BoingEffector effector)
      {
        Bits = new Bits32();
        Bits.SetBit((int) BoingWork.EffectorFlags.ContinuousMotion, effector.ContinuousMotion);

        float speedEffectRatio =
          effector.MaxImpulseSpeed > MathUtil.Epsilon
          ? Mathf.Min(1.0f, effector.LinearSpeed / effector.MaxImpulseSpeed)
          : 1.0f;

        PrevPosition = effector.m_prevPosition;
        CurrPosition = effector.m_currPosition;
        LinearVelocityDir = VectorUtil.NormalizeSafe(effector.LinearVelocity, Vector3.zero);
        Radius = effector.Radius;
        FullEffectRadius = Radius * effector.FullEffectRadiusRatio;
        MoveDistance = effector.MoveDistance;
        LinearImpulse = speedEffectRatio * effector.LinearImpulse;
        RotateAngle = effector.RotationAngle * MathUtil.Deg2Rad;
        AngularImpulse = speedEffectRatio * effector.AngularImpulse * MathUtil.Deg2Rad;

        m_padding0 = 0.0f;
        m_padding1 = 0.0f;
        m_padding2 = 0.0f;
        m_padding3 = 0;
      }

      public void Fill(BoingEffector effector)
      {
        this = new Params(effector);
      }

      private void SuppressWarnings()
      {
        m_padding0 = 0.0f;
        m_padding1 = 0.0f;
        m_padding2 = 0.0f;
        m_padding3 = 0;
        m_padding0 = m_padding1;
        m_padding1 = m_padding2;
        m_padding2 = m_padding3;
        m_padding3 = (int) m_padding0;
      }
    }

    [Header("Metrics")]

    [Range(0.0f, 20.0f)]
    [Tooltip("Maximum radius of influence.")]
    public float Radius = 3.0f;

    [Range(0.0f, 1.0f)]
    [Tooltip(
        "Fraction of Radius past which influence begins decaying gradually to zero exactly at Radius.\n\n" 
      + "e.g. With a Radius of 10.0 and FullEffectRadiusRatio of 0.5, " 
      + "reactors within distance of 5.0 will be fully influenced, " 
      + "reactors at distance of 7.5 will experience 50% influence, " 
      + "and reactors past distance of 10.0 will not be influenced at all."
    )]
    public float FullEffectRadiusRatio = 0.5f;

    [Header("Dynamics")]

    [Range(0.0f, 100.0f)]
    [Tooltip(
       "Speed of this effector at which impulse effects will be at maximum strength.\n\n" 
     + "e.g. With a MaxImpulseSpeed of 10.0 and an effector traveling at speed of 4.0, " 
     + "impulse effects will be at 40% maximum strength."
    )]
    public float MaxImpulseSpeed = 5.0f;

    [Tooltip(
       "This affects impulse-related effects.\n\n"
     + "If checked, continuous motion will be simulated between frames. "
     + "This means even if an effector \"teleports\" by moving a huge distance between frames, "
     + "the effector will still affect all reactors caught on the effector's path in between frames, "
     + "not just the reactors around the effector's discrete positions at different frames."
    )]
    public bool ContinuousMotion = false;

    [Header("Position Effect")]

    [Range(-10.0f, 10.0f)]
    [Tooltip(
        "Distance to push away reactors at maximum influence.\n\n" 
      + "e.g. With a MoveDistance of 2.0, a Radius of 10.0, a FullEffectRadiusRatio of 0.5, " 
      + "and a reactor at distance of 7.5 away from effector, the reactor will be pushed away " 
      + "to 50% of maximum influence, i.e. 50% of MoveDistance, " 
      + "which is a distance of 1.0 away from the effector."
    )]
    public float MoveDistance = 0.5f;

    [Range(-200.0f, 200.0f)]
    [Tooltip(
        "Under maximum impulse influence "
      + "(within distance of Radius * FullEffectRadiusRatio and with effector moving at speed faster or equal to MaxImpulaseSpeed), " 
      + "a reactor's movement speed will be maintained to be at least as fast as LinearImpulse (unit: distance per second) " 
      + "in the direction of effector's movement direction.\n\n" 
      + "e.g. With a LinearImpulse of 2.0, a Radius of 10.0, a FullEffectRadiusRatio of 0.5, " 
      + "and a reactor at distance of 7.5 away from effector, the reactor's movement speed in the direction of effector's movement direction " 
      + "will be maintained to be at least 50% of LinearImpulse, which is 1.0 per second."
    )]
    public float LinearImpulse = 5.0f;

    [Header("Rotation Effect")]

    [Range(-180.0f, 180.0f)]
    [Tooltip(
        "Angle (in degrees) to rotate reactors at maximum influence. " 
      + "The rotation will point reactors' up vectors (defined individually in the reactor component) away from the effector.\n\n" 
      + "e.g. With a RotationAngle of 20.0, a Radius of 10.0, a FullEffectRadiusRatio of 0.5, " 
      + "and a reactor at distance of 7.5 away from effector, the reactor will be rotated " 
      + "to 50% of maximum influence, i.e. 50% of RotationAngle, which is 10 degrees."
    )]
    public float RotationAngle = 20.0f;

    [Range(-2000.0f, 2000.0f)]
    [Tooltip(
        "Under maximum impulse influence "
      + "(within distance of Radius * FullEffectRadiusRatio and with effector moving at speed faster or equal to MaxImpulaseSpeed), " 
      + "a reactor's rotation speed will be maintained to be at least as fast as AngularImpulse (unit: degrees per second) " 
      + "in the direction of effector's movement direction, " 
      + "i.e. the reactor's up vector will be pulled in the direction of effector's movement direction.\n\n" 
      + "e.g. With a AngularImpulse of 20.0, a Radius of 10.0, a FullEffectRadiusRatio of 0.5, " 
      + "and a reactor at distance of 7.5 away from effector, the reactor's rotation speed in the direction of effector's movement direction " 
      + "will be maintained to be at least 50% of AngularImpulse, which is 10.0 degrees per second."
    )]
    public float AngularImpulse = 400.0f;

    [Header("Debug")]

    [Tooltip("If checked, gizmos of reactor fields affected by this effector will be drawn.")]
    public bool DrawAffectedReactorFieldGizmos = false;

    private Vector3 m_currPosition;
    private Vector3 m_prevPosition;

    private Vector3 m_linearVelocity;
    public Vector3 LinearVelocity { get { return m_linearVelocity; } }
    public float LinearSpeed { get { return m_linearVelocity.magnitude; } }

    public void OnEnable()
    {
      m_currPosition = transform.position;
      m_prevPosition = transform.position;
      m_linearVelocity = Vector3.zero;
      BoingManager.Register(this);
    }

    public void OnDisable()
    {
      BoingManager.Unregister(this);
    }

    public void Update()
    {
      float dt = Time.deltaTime;
      if (dt < MathUtil.Epsilon)
        return;

      m_linearVelocity = (transform.position - m_prevPosition) / dt;
      m_prevPosition = m_currPosition;
      m_currPosition = transform.position;
    }

    public void OnDrawGizmosSelected()
    {
      if (!isActiveAndEnabled)
        return;

      if (FullEffectRadiusRatio < 1.0f)
      {
        Gizmos.color = new Color(1.0f, 0.5f, 0.2f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, Radius);
      }

      Gizmos.color = new Color(1.0f, 0.5f, 0.2f, 1.0f);
      Gizmos.DrawWireSphere(transform.position, Radius * FullEffectRadiusRatio);
    }
  }
}
