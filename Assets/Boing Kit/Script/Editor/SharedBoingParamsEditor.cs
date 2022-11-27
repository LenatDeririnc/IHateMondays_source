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
  [CustomEditor(typeof(SharedBoingParams))]
  [CanEditMultipleObjects]
  public class SharedBoingParamsEditor : BoingEditorBase
  {
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

    public virtual void OnEnable()
    {
      var p = serializedObject.FindProperty("Params");

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

    public override void OnInspectorGUI()
    {
      serializedObject.Update();


      Header("Position Effects");
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


      Header("Rotation Effects");
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
      }


      Header("Scale Effects");
      {
        Property(ScaleParameterMode,
          "Parameter Mode",
              "Each mode exposes a different set of parameters.\n\n"
            + "Exponential - Object scale approaches its desired location exponentially at a specific half-life.\n\n"
            + "Oscillation by Half-Life - Object scale approaches its desired location in an oscillating fashion at a specific half-life.\n\n"
            + "Oscillation by Damping Ratio - Object scale approaches its desired location in an oscillating fashion at a specific damping ratio.\n\n"
        );

      if (ScaleParameterMode.enumValueIndex == (int)ParameterMode.Exponential)
      {
        Property(ScaleExponentialHalfLife,
          "   Exponential Half-Life",
              "Exponential half-life duration (in seconds). "
            + "Every time this duration has elapsed, if the desired location maintains stationary, "
            + "the distance between the object and its desired location is halved."
        );
      }
      else if (ScaleParameterMode.enumValueIndex == (int)ParameterMode.OscillationByHalfLife)
      {
        Property(ScaleOscillationFrequency,
          "   Oscillation Frequency",
          "Oscillation frequency (in hertz, i.e. one full back-and-forth oscillation per second)."
        );

        Property(ScaleOscillationHalfLife,
           "   Oscillation Half-Life",
             "Oscillation half-life duration (in seconds). "
           + "Every time this duration has elapsed, if the desired location maintains stationary, "
           + "the scale oscillation magnitude is halved."
        );
      }
      else if (ScaleParameterMode.enumValueIndex == (int)ParameterMode.OscillationByDampingRatio)
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
          + "and the object will not oscillate at all, smoothly approaching its desired location "
          + "(this is essentially the same as exponential mode, except that its not as intuitive as turning the half life duration).\n\n"
          + "Any value between 0.0 and 1.0 will cause the object to overshoot its desired location, "
          + "oscillating back and forth before eventually settling down."
        );
      }

      Property(MoveReactionMultiplier,
        "Move Reaction Multiplier",
            "Extra multiplier applied to influences from effectors.\n"
          + "1.0 means 100%.\n\n"
          + "e.g. If an effector at maximum influence were to push the reactor's desired scale away by a distance of 10.0, "
          + "and MoveReactionMultiplier is set to 0.5, then the desired scale will only be 5.0 away from the reactor."
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


    serializedObject.ApplyModifiedProperties();
    }
  }
}
