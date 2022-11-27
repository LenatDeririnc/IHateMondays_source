using Fungus;
using UnityEngine;
using UVariableSystem;

namespace FungusCustomCommands
{
    [CommandInfo("Custom", "Get Data", "Get data from static map")]
    public class GetDataHolder : Command
    {
        [SerializeField] protected UVariable holder;

        [Tooltip("Variable to store the value in.")]
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

            System.Type variableType = variable.GetType();

            if (variableType == typeof(BooleanVariable))
            {
                BooleanVariable booleanVariable = variable as BooleanVariable;
                var value = holder.Value();
                if (booleanVariable != null && value != null)
                {
                    // PlayerPrefs does not have bool accessors, so just use int
                    booleanVariable.Value = ((bool) holder.Value());
                }
            }
            else if (variableType == typeof(IntegerVariable))
            {
                IntegerVariable integerVariable = variable as IntegerVariable;
                var value = holder.Value();
                if (integerVariable != null && value != null)
                {
                    integerVariable.Value = (int) holder.Value();
                }
            }
            else if (variableType == typeof(FloatVariable))
            {
                FloatVariable floatVariable = variable as FloatVariable;
                var value = holder.Value();
                if (floatVariable != null && value != null)
                {
                    floatVariable.Value = (float) holder.Value();
                }
            }
            else if (variableType == typeof(StringVariable))
            {
                StringVariable stringVariable = variable as StringVariable;
                var value = holder.Value();
                if (stringVariable != null && value != null)
                {
                    stringVariable.Value = (string) holder.Value();
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

            return "'" + holder.name + "' into " + variable.Key;
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        public override bool HasReference(Variable in_variable)
        {
            return this.variable == in_variable ||
                base.HasReference(in_variable);
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