using Fungus;
using UnityEngine;
using UVariableSystem;

namespace FungusCustomCommands
{
    [CommandInfo("Custom", "Set Data", "Set data into static map")]
    public class SetDataHolder : Command
    {
        [SerializeField] protected UVariable holder;

        [Tooltip("Variable to read the value from. Only Boolean, Integer, Float and String are supported.")]
        [VariableProperty(typeof(BooleanVariable),
                          typeof(IntegerVariable), 
                          typeof(FloatVariable), 
                          typeof(StringVariable))]
        [SerializeField] protected Variable variable;

        #region Public members

        public override void OnEnter()
        {
            if (holder == null ||
                variable == null)
            {
                Continue();
                return;
            }

            // Prepend the current save profile (if any)

            System.Type variableType = variable.GetType();

            if (variableType == typeof(BooleanVariable))
            {
                BooleanVariable booleanVariable = variable as BooleanVariable;
                if (booleanVariable != null)
                {
                    // PlayerPrefs does not have bool accessors, so just use int
                    holder.SetValue(booleanVariable.Value);
                }
            }
            else if (variableType == typeof(IntegerVariable))
            {
                IntegerVariable integerVariable = variable as IntegerVariable;
                if (integerVariable != null)
                {
                    holder.SetValue(integerVariable.Value);
                }
            }
            else if (variableType == typeof(FloatVariable))
            {
                FloatVariable floatVariable = variable as FloatVariable;
                if (floatVariable != null)
                {
                    holder.SetValue(floatVariable.Value);
                }
            }
            else if (variableType == typeof(StringVariable))
            {
                StringVariable stringVariable = variable as StringVariable;
                if (stringVariable != null)
                {
                    holder.SetValue(stringVariable.Value);
                }
            }
            
            Continue();
        }
        
        public override string GetSummary()
        {
            if (holder == null)
            {
                return "Error: No holder selected";
            }
            
            if (variable == null)
            {
                return "Error: No variable selected";
            }
            
            return variable.Key + " into '" + holder.name + "'";
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        public override bool HasReference(Variable in_variable)
        {
            return this.variable == in_variable || base.HasReference(in_variable);
        }

        #endregion
        #region Editor caches
#if UNITY_EDITOR
        protected override void RefreshVariableCache()
        {
            base.RefreshVariableCache();

            var f = GetFlowchart();

            f.DetermineSubstituteVariables(holder.name, referencedVariables);
        }
#endif
        #endregion Editor caches

    }
}
