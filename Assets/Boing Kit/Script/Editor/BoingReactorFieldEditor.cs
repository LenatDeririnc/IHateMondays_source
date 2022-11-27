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
using UnityEngine;

namespace BoingKit
{
  [CustomEditor(typeof(BoingReactorField))]
  [CanEditMultipleObjects]
  public class BoingReactorFieldEditor : BoingReactorEditor
  {
    private SerializedProperty HardwareMode;

    private SerializedProperty CellSize;
    private SerializedProperty CellMoveMode;
    private SerializedProperty CellsX;
    private SerializedProperty CellsY;
    private SerializedProperty CellsZ;

    private SerializedProperty FalloffMode;
    private SerializedProperty FalloffRatio;
    private SerializedProperty FalloffDimensions;

    private SerializedProperty Effectors;

    private SerializedProperty EnablePropagation;
    private SerializedProperty PositionPropagation;
    private SerializedProperty RotationPropagation;
    private SerializedProperty PropagationDepth;
    private SerializedProperty AnchorPropagationAtBorder;

    public BoingReactorFieldEditor()
    {
      m_isReactorField = true;
    }

    public override void OnEnable()
    {
      HardwareMode = serializedObject.FindProperty("HardwareMode");

      CellMoveMode = serializedObject.FindProperty("CellMoveMode");
      CellsX = serializedObject.FindProperty("CellsX");
      CellsY = serializedObject.FindProperty("CellsY");
      CellsZ = serializedObject.FindProperty("CellsZ");
      CellSize = serializedObject.FindProperty("CellSize");

      FalloffMode = serializedObject.FindProperty("FalloffMode");
      FalloffRatio = serializedObject.FindProperty("FalloffRatio");
      FalloffDimensions = serializedObject.FindProperty("FalloffDimensions");

      Effectors = serializedObject.FindProperty("Effectors");

      EnablePropagation = serializedObject.FindProperty("EnablePropagation");
      PositionPropagation = serializedObject.FindProperty("PositionPropagation");
      RotationPropagation = serializedObject.FindProperty("RotationPropagation");
      PropagationDepth = serializedObject.FindProperty("PropagationDepth");
      AnchorPropagationAtBorder = serializedObject.FindProperty("AnchorPropagationAtBorder");

      base.OnEnable();
    }

    protected override void DrawContent()
    {
      serializedObject.Update();


      Header("Reactor Field");

      Property(HardwareMode, 
        "Hardware Mode", 
            "Processing hardware used to update the reactor field.\n\n" 
          + "CPU - Update happens on the CPU. Use this mode in conjunction with BoingReactorFieldSampler components.\n\n" 
          + "GPU - Update happens on the GPU. Use this mode in conjunction with shaders."
      );

      Property(CellMoveMode, 
        "Cell Move Mode", 
            "How cells react when as the reactor field center moves around.\n\n" 
          + "Follow - Cells move with the reactor field. " 
          +   "Use this mode when a visually shifting field is desired as the field center moves around.\n\n" 
          + "Wrap Around - Cells stay in place and wrap around when the reactor field moves far enough. " 
          +   "Use this mode if a visually stationary field is desired as the field center moves around."
      );

      EditorGUILayout.BeginHorizontal();
      {
        Vector3 cells =
          EditorGUILayout.Vector3Field
          (
            new GUIContent()
            {
              text = "  Cell Count",
              tooltip = "Number of cells in each dimension."
            }, 
            new Vector3(CellsX.intValue, CellsY.intValue, CellsZ.intValue)
          );

        CellsX.intValue = (int) Mathf.Clamp(cells.x, 1, 128);
        CellsY.intValue = (int) Mathf.Clamp(cells.y, 1, 128);
        CellsZ.intValue = (int) Mathf.Clamp(cells.z, 1, 128);
      }
      EditorGUILayout.EndHorizontal();

      Property(CellSize,
        "Cell Size", 
        "Size of each reactor field cell."
      );

      Property(FalloffMode, 
        "Fall-off Mode", 
            "How the reactor field influence falls off to zero towards the edges of the field.\n\n" 
          + "None - Constant influence throughout the entire field.\n\n" 
          + "Square - Influence falls of towards the edges of the field along its local axes from the center of the field.\n\n" 
          + "Circle - Influence falls of towards the edges of the field based on the distance from the center of the field."
      );

      Property(FalloffRatio, 
        "Fall-off Ratio", 
        "Fraction of half reactor field dimensions where the influence fall-off starts."
      );

      Property(FalloffDimensions, 
        "Fall-off Dimensions", 
            "Which dimensions to apply influence fall-off when sampled by samplers. "
          + "The dimension left out will act as if samplers are clamped within the bounds on that dimension.\n\n" 
          + "e.g. If fall-off dimensions are set to XZ, and the field's maximum Y boundary is 0.5, " 
          + " a sampler at (0.0, 1.0, 0.0) will sample the field as if it's at (0.0, 0.5, 0.0)."
      );

      Header("Effects Propagation");

      Property(EnablePropagation, 
        "Enable", 
        "Check to enable propagation of boing effects throughout cells.");

      Property(PositionPropagation, 
        "Position Propagation",
        "Ratio of positional boing effects to propagate from each cell to its nearby cells.");

      Property(RotationPropagation, 
        "Rotation Propagation",
        "Ratio of Rotational boing effects to propagate from each cell to its nearby cells.");

      Property(PropagationDepth, 
        "Depth", 
            "How many cells apart to propagate from each cell to nearby cells during each update cycle. " 
          + "Increasing this number increases the speed of propagation, but also requires more computations.\n\n" 
          + "To increase propagation speed, it is recommended to first try increasing cell sizes, " 
          + "before increasing propagation depths.");

      Property(AnchorPropagationAtBorder, 
        "Anchored Border", 
        "Enable to fix border cells in place.");

      Space();

      Array(Effectors, "Effectors");


      serializedObject.ApplyModifiedProperties();


      base.DrawContent();


      serializedObject.Update();


      serializedObject.ApplyModifiedProperties();
    }
  }

}
