/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEditor;

namespace BoingKit
{
  [CustomEditor(typeof(BoingBehavior))]
  [CanEditMultipleObjects]
  public class BoingBehaviorEditor : BoingEditorBase
  {
    private SerializedProperty UpdateMode;

    private SerializedProperty TwoDDistanceCheck;
    private SerializedProperty TwoDPositionInfluence;
    private SerializedProperty TwoDRotationInfluence;
    private SerializedProperty EnablePositionEffect;
    private SerializedProperty EnableRotationEffect;
    private SerializedProperty EnableScaleEffect;
    private SerializedProperty GlobalReactionUpVector;

    private SerializedProperty TranslationLockSpace;
    private SerializedProperty LockTranslatoinX;
    private SerializedProperty LockTranslatoinY;
    private SerializedProperty LockTranslatoinZ;

    private SerializedProperty TwoDPlane;

    private SerializedProperty SharedParams;

    private SerializedProperty PositionParameterMode;
    private SerializedProperty PositionExponentialHalfLife;
    private SerializedProperty PositionOscillationHalfLife;
    private SerializedProperty PositionOscillationFrequency;
    private SerializedProperty PositionOscillationDampingRatio;
    private SerializedProperty MoveReactionMultiplier;
    private SerializedProperty LinearImpulseMultiplier;

    private SerializedProperty RotationParameterMode;
    private SerializedProperty RotationExponentialHalfLife;
    private SerializedProperty RotationOscillationHalfLife;
    private SerializedProperty RotationOscillationFrequency;
    private SerializedProperty RotationOscillationDampingRatio;
    private SerializedProperty RotationReactionMultiplier;
    private SerializedProperty AngularImpulseMultiplier;
    private SerializedProperty RotationReactionUp;

    private SerializedProperty ScaleParameterMode;
    private SerializedProperty ScaleExponentialHalfLife;
    private SerializedProperty ScaleOscillationHalfLife;
    private SerializedProperty ScaleOscillationFrequency;
    private SerializedProperty ScaleOscillationDampingRatio;

    protected bool m_isReactor = false;
    protected bool m_isReactorField = false;
    protected bool m_isBones = false;

    public virtual void OnEnable()
    {
      var p = serializedObject.FindProperty("Params");

      UpdateMode = serializedObject.FindProperty("UpdateMode");

      TwoDDistanceCheck = serializedObject.FindProperty("TwoDDistanceCheck");
      TwoDPositionInfluence = serializedObject.FindProperty("TwoDPositionInfluence");
      TwoDRotationInfluence = serializedObject.FindProperty("TwoDRotationInfluence");
      EnablePositionEffect = serializedObject.FindProperty("EnablePositionEffect");
      EnableRotationEffect = serializedObject.FindProperty("EnableRotationEffect");
      EnableScaleEffect = serializedObject.FindProperty("EnableScaleEffect");
      GlobalReactionUpVector = serializedObject.FindProperty("GlobalReactionUpVector");

      TranslationLockSpace = serializedObject.FindProperty("TranslationLockSpace");
      LockTranslatoinX = serializedObject.FindProperty("LockTranslationX");
      LockTranslatoinY = serializedObject.FindProperty("LockTranslationY");
      LockTranslatoinZ = serializedObject.FindProperty("LockTranslationZ");

      SharedParams = serializedObject.FindProperty("SharedParams");

      TwoDPlane = p.FindPropertyRelative("TwoDPlane");

      PositionParameterMode = p.FindPropertyRelative("PositionParameterMode");
      PositionExponentialHalfLife = p.FindPropertyRelative("PositionExponentialHalfLife");
      PositionOscillationHalfLife = p.FindPropertyRelative("PositionOscillationHalfLife");
      PositionOscillationFrequency = p.FindPropertyRelative("PositionOscillationFrequency");
      PositionOscillationDampingRatio = p.FindPropertyRelative("PositionOscillationDampingRatio");
      MoveReactionMultiplier = p.FindPropertyRelative("MoveReactionMultiplier");
      LinearImpulseMultiplier = p.FindPropertyRelative("LinearImpulseMultiplier");

      RotationParameterMode = p.FindPropertyRelative("RotationParameterMode");
      RotationExponentialHalfLife = p.FindPropertyRelative("RotationExponentialHalfLife");
      RotationOscillationHalfLife = p.FindPropertyRelative("RotationOscillationHalfLife");
      RotationOscillationFrequency = p.FindPropertyRelative("RotationOscillationFrequency");
      RotationOscillationDampingRatio = p.FindPropertyRelative("RotationOscillationDampingRatio");
      RotationReactionMultiplier = p.FindPropertyRelative("RotationReactionMultiplier");
      AngularImpulseMultiplier = p.FindPropertyRelative("AngularImpulseMultiplier");
      RotationReactionUp = p.FindPropertyRelative("RotationReactionUp");

      ScaleParameterMode = p.FindPropertyRelative("ScaleParameterMode");
      ScaleExponentialHalfLife = p.FindPropertyRelative("ScaleExponentialHalfLife");
      ScaleOscillationHalfLife = p.FindPropertyRelative("ScaleOscillationHalfLife");
      ScaleOscillationFrequency = p.FindPropertyRelative("ScaleOscillationFrequency");
      ScaleOscillationDampingRatio = p.FindPropertyRelative("ScaleOscillationDampingRatio");
    }

    protected override void DrawContent()
    {
      base.DrawContent();

      serializedObject.Update();

      bool useSharedParams = (SharedParams.objectReferenceValue != null);

      if (!m_isReactorField)
      {
        Header("Updates");

        if (m_isBones)
        {
          Property(UpdateMode, 
          "Update Mode", 
              "Match this mode with how you update your object's transform.\n\n" 
            + "Update - Use this mode if you update your object's transform in Update(). This uses variable Time.detalTime. Use FixedUpdate if physics simulation becomes unstable.\n\n" 
            + "Fixed Update - Use this mode if you update your object's transform in FixedUpdate(). This uses fixed Time.fixedDeltaTime. " 
            + "Also, use this mode if the game object is affected by Unity physics (i.e. has a rigid body component), which uses fixed updates." 
          );
        }
        else
        {
          Property(UpdateMode, 
            "Update Mode", 
                "Match this mode with how you update your object's transform.\n\n" 
              + "Fixed Update - Use this mode if you update your object's transform in FixedUpdate(). Also, use this mode if the game object is affected by Unity physics (i.e. has a rigid body component), which uses fixed updates. This uses fixed Time.fixedDeltaTime.\n\n" 
              + "Update - Only use this mode if your render logic pulls transforms during Update() and before LateUpdate(), such as Unity's skinned sprite renderer (in certain versions).\n\n" 
              + "Late Update - Use this mode if you update your object's transform in Update(). This uses the variable Time.detalTime."
          );
        }
      }

      Header("Shared Parameters");
      Property(SharedParams, 
        "Use Asset", 
            "Shared boing parameters can be created as Unity assets by right-clicking in the Project menu and select Create > Boing Kit > Shared Boing Params. "
          + "This is useful for avoiding managing duplicate parameters when multiple objects share identical parameters."
      );


      Header("Position Effects");
      {
        Property(EnablePositionEffect, 
          "Enable", 
          "Check to enable position effects."
        );

        if (!useSharedParams)
        {
          Property(PositionParameterMode, 
            "Parameter Mode", 
                "Each mode exposes a different set of parameters.\n\n" 
              + "Exponential - Object position approaches its desired location exponentially at a specific half-life.\n\n" 
              + "Oscillation by Half-Life - Object position approaches its desired location in an oscillating fashion at a specific half-life.\n\n" 
              + "Oscillation by Damping Ratio - Object position approaches its desired location in an oscillating fashion at a specific damping ratio.\n\n"
          );

          if (PositionParameterMode.enumValueIndex == (int) ParameterMode.Exponential)
          {
            Property(PositionExponentialHalfLife, 
              "   Exponential Half-Life", 
                  "Exponential half-life duration (in seconds). " 
                + "Every time this duration has elapsed, if the desired location maintains stationary, " 
                + "the distance between the object and its desired location is halved."
            );
          }
          else if (PositionParameterMode.enumValueIndex == (int) ParameterMode.OscillationByHalfLife)
          {
            Property(PositionOscillationFrequency, 
              "   Oscillation Frequency",
              "Oscillation frequency (in hertz, i.e. one full back-and-forth oscillation per second)."
            );

            Property(PositionOscillationHalfLife,
               "   Oscillation Half-Life",
                 "Oscillation half-life duration (in seconds). "
               + "Every time this duration has elapsed, if the desired location maintains stationary, "
               + "the position oscillation magnitude is halved."
            );
          }
          else if (PositionParameterMode.enumValueIndex == (int) ParameterMode.OscillationByDampingRatio)
          {
            Property(PositionOscillationFrequency,
              "   Oscillation Frequency",
              "Oscillation frequency (in hertz, i.e. one full back-and-forth oscillation per second)."
            );

            Property(PositionOscillationDampingRatio,
              "   Oscillation Damping Ratio", 
                "Damping ratio for oscillating positional approach effect.\n\n" 
              + "A value of 0.0 means no damping at all, " 
              + "and the object will oscillate for an extremely long time before settling at its desired location.\n\n" 
              + "A value of 1.0 means full damping, " 
              + "and the object will not oscillate at all, smoothly approaching its desired location " 
              + "(this is essentially the same as exponential mode, except that its not as intuitive as turning the half life duration).\n\n" 
              + "Any value between 0.0 and 1.0 will cause the object to overshoot its desired location, " 
              + "oscillating back and forth before eventually settling down."
            );
          }

          if (m_isReactor)
          {
            Property(MoveReactionMultiplier, 
              "Move Reaction Multiplier",
                  "Extra multiplier applied to influences from effectors.\n" 
                + "1.0 means 100%.\n\n" 
                + "e.g. If an effector at maximum influence were to push the reactor's desired position away by a distance of 10.0, " 
                + "and MoveReactionMultiplier is set to 0.5, then the desired position will only be 5.0 away from the reactor."
            );

            Property(LinearImpulseMultiplier, 
              "Linear Impulse Multiplier",
                  "Extra multiplier applied to impulse influences from effectors.\n"
                + "1.0 means 100%.\n\n"
                + "e.g. If an effector at maximum influence were to maintain a reactor's minimum movement speed " 
                + "(in the direction of the effector's movement) at 10.0 units per second, " 
                + "and LinearImpulseMultiplier is set to 0.5, " 
                + "then the minimum movement speed will only be maintained at 5.0 units per second."
            );
          }
        }
      }


      Header("Rotation Effects");
      {
        Property(EnableRotationEffect, 
          "Enable", 
          "Check to enable rotation effects."
        );

        if (!useSharedParams)
        {
          Property(RotationParameterMode, 
            "Parameter Mode",
               "Mode in which object's rotation approaches its desired rotation. " 
              + "Each mode exposes a different set of parameters.\n\n" 
              + "Exponential - Object rotation approaches its desired rotation exponentially at a specific half-life.\n\n" 
              + "Oscillation by Half-Life - Object rotation approaches its desired rotation in an oscillating fashion at a specific half-life.\n\n"
              + "Oscillation by Damping Ratio - Object rotation approaches its desired rotation in an oscillating fashion at a specific damping ratio.\n\n"
          );

          if (RotationParameterMode.enumValueIndex == (int) ParameterMode.Exponential)
          {
            Property(RotationExponentialHalfLife, 
              "   Exponential Half-Life", 
                  "Exponential half-life duration (in seconds). " 
                + "Every time this duration has elapsed, if the desired rotation maintains stationary, " 
                + "the angle between the object rotation and its desired rotation is halved."
            );
          }
          else if (RotationParameterMode.enumValueIndex == (int) ParameterMode.OscillationByHalfLife)
          {
            Property(RotationOscillationFrequency, 
              "   Oscillation Frequency", 
              "Oscillation frequency (in hertz, i.e. one full back-and-forth oscillation per second)."
            );

            Property(RotationOscillationHalfLife, 
              "   Oscillation Half-Life", 
                  "Oscillation half-life duration (in seconds). " 
                + "Every time this duration has elapsed, if the desired rotation maintains stationary, " 
                + "the rotation oscillation magnitude is halved."
            );
          }
          else if (RotationParameterMode.enumValueIndex == (int) ParameterMode.OscillationByDampingRatio)
          {
            Property(RotationOscillationFrequency, 
              "   Oscillation Frequency", 
              "Oscillation frequency (in hertz, i.e. one full back-and-forth oscillation per second)."
            );

            Property(RotationOscillationDampingRatio,
              "   Oscillation Damping Ratio", 
                "Damping ratio for oscillating rotational approach effect.\n\n" 
              + "A value of 0.0 means no damping at all, " 
              + "and the object will oscillate for an extremely long time before settling at its desired rotation.\n\n" 
              + "A value of 1.0 means full damping, " 
              + "and the object will not oscillate at all, smoothly approaching its desired rotation " 
              + "(this is essentially the same as exponential mode, except that its not as intuitive as turning the half life duration).\n\n" 
              + "Any value between 0.0 and 1.0 will cause the object to overshoot its desired rotation, " 
              + "oscillating back and forth before eventually settling down."
            );

          }

          if (m_isReactor)
          {
            Property(RotationReactionMultiplier,
              "Rotation Reaction Multiplier",
                  "Extra multiplier applied to influences from effectors.\n\n" 
                + "e.g. If an effector at maximum influence  were to offset the reactor's desired rotation by 20 degrees, "
                + "and RotationReactionMultiplier is set to 0.5, then the desired rotation will only offset by 10 degrees."
            );

            Property(AngularImpulseMultiplier,
              "Angular Impulse Multiplier",
                  "Extra multiplier applied to impulse influences from effectors.\n\n" 
                + "e.g. If an effector at maximum influence were to maintain a reactor's minimum rotation speed " 
                + "(in the direction of the effector's movement) at 20.0 degrees per second, " 
                + "and AngularImpulseMultiplier is set to 0.5, "
                + "then the minimum rotation speed will only be maintained at 10.0 degrees per second."
            );

            Property(RotationReactionUp, 
              "Reaction Up", 
                  "Up vector (defined in object's local space) used for rotational reactions.\n\n" 
                + "If a object's rotation is influenced by an effector, the up vector will be pushed away from the effector " 
                + "(and/or pulled in the direction of effector's movement at the presence of angular impulse influence)."
            );

            Property(GlobalReactionUpVector, 
              "Global Reaction Up", 
              "If checked, Reaction Up is defined in global space. If unchecked, Reaction Up is in local space."
            );
          }
        }
      }


      if (!m_isReactor && 
          !m_isBones)
      {
        Header("Scale Effects");
        {
          Property(EnableScaleEffect, 
            "Enable", 
            "Check to enable Scale effects."
          );

          if (!useSharedParams)
          {
            Property(ScaleParameterMode, 
              "Parameter Mode", 
                  "Each mode exposes a different set of parameters.\n\n" 
                + "Exponential - Object scale approaches its desired location exponentially at a specific half-life.\n\n" 
                + "Oscillation by Half-Life - Object scale approaches its desired location in an oscillating fashion at a specific half-life.\n\n" 
                + "Oscillation by Damping Ratio - Object scale approaches its desired location in an oscillating fashion at a specific damping ratio.\n\n"
            );

            if (ScaleParameterMode.enumValueIndex == (int) ParameterMode.Exponential)
            {
              Property(ScaleExponentialHalfLife, 
                "   Exponential Half-Life", 
                    "Exponential half-life duration (in seconds). " 
                  + "Every time this duration has elapsed, if the desired location maintains stationary, " 
                  + "the distance between the object and its desired location is halved."
              );
            }
            else if (ScaleParameterMode.enumValueIndex == (int) ParameterMode.OscillationByHalfLife)
            {
              Property(ScaleOscillationFrequency, 
                "   Oscillation Frequency",
                "Oscillation frequency (in hertz, i.e. one full back-and-forth oscillation per second)."
              );

              Property(ScaleOscillationHalfLife,
                 "   Oscillation Half-Life",
                   "Oscillation half-life duration (in seconds). "
                 + "Every time this duration has elapsed, if the desired location maintains stationary, "
                 + "the Scale oscillation magnitude is halved."
              );
            }
            else if (ScaleParameterMode.enumValueIndex == (int) ParameterMode.OscillationByDampingRatio)
            {
              Property(ScaleOscillationFrequency,
                "   Oscillation Frequency",
                "Oscillation frequency (in hertz, i.e. one full back-and-forth oscillation per second)."
              );

              Property(ScaleOscillationDampingRatio,
                "   Oscillation Damping Ratio", 
                  "Damping ratio for oscillating scale approach effect.\n\n" 
                + "A value of 0.0 means no damping at all, " 
                + "and the object will oscillate for an extremely long time before settling at its desired location.\n\n" 
                + "A value of 1.0 means full damping, " 
                + "and the object will not oscillate at all, smoothly approaching its desired scale " 
                + "(this is essentially the same as exponential mode, except that its not as intuitive as turning the half life duration).\n\n" 
                + "Any value between 0.0 and 1.0 will cause the object to overshoot its desired location, " 
                + "oscillating back and forth before eventually settling down."
              );
            }

            if (m_isReactor)
            {
              Property(MoveReactionMultiplier, 
                "Move Reaction Multiplier",
                    "Extra multiplier applied to influences from effectors.\n" 
                  + "1.0 means 100%.\n\n" 
                  + "e.g. If an effector at maximum influence were to push the reactor's desired Scale away by a distance of 10.0, " 
                  + "and MoveReactionMultiplier is set to 0.5, then the desired Scale will only be 5.0 away from the reactor."
              );

              Property(LinearImpulseMultiplier, 
                "Linear Impulse Multiplier",
                    "Extra multiplier applied to impulse influences from effectors.\n"
                  + "1.0 means 100%.\n\n"
                  + "e.g. If an effector at maximum influence were to maintain a reactor's minimum movement speed " 
                  + "(in the direction of the effector's movement) at 10.0 units per second, " 
                  + "and LinearImpulseMultiplier is set to 0.5, " 
                  + "then the minimum movement speed will only be maintained at 5.0 units per second."
              );
            }
          }
        }
      }


      if (!m_isReactorField)
      {
        Header("Translational Locks");
        
        Property(TranslationLockSpace, 
          "Space", 
          "Lock translation in global space or object's local space"
        );

        Property(LockTranslatoinX, 
          "Lock X Axis", 
          "When locked, translational effects along the X axis is eliminated."
        );
        
        Property(LockTranslatoinY, 
          "Lock Y Axis", 
          "When locked, translational effects along the Y axis is eliminated."
        );

        Property(LockTranslatoinZ, 
          "Lock Z Axis", 
          "When locked, translational effects along the Z axis is eliminated."
        );
      }

      if (m_isReactor)
      {
        Header("2D Mode (Effector Response)");

        Property(TwoDDistanceCheck, 
          "2D Distance Check", 
          "Enable to only check distance on a 2D plane."
        );

        Property(TwoDPositionInfluence, 
          "2D Position Influence", 
          "Enable to only apply effector position influence on a 2D plane."
        );

        Property(TwoDRotationInfluence, 
          "2D Rotation Influence", 
          "Enable to only apply effector rotation influence on a 2D plane."
        );

        Property(TwoDPlane,
          "2D Plane",
          "2D plane on which distance is checked (if 2D Distance Check is enabled) "
          + "& effector influence is applied (if 2D Influence is enabled)."
        );
      }


      serializedObject.ApplyModifiedProperties();
    }
  }
}
