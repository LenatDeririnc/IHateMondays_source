using UnityEngine;

namespace UVariableSystem
{

    [CreateAssetMenu(fileName = "UVariable", menuName = "UVariable", order = 0)]
    public class UVariable : ScriptableObject
    {
        [SerializeField] private string m_context = "global";
        [SerializeField] private string m_defaultValue;
        [SerializeField] private UVariableType m_defaultType = UVariableType.Any;

        public void SetValue(object V)
        {
            var variableObject = new UVariableBase(m_defaultType, V);
            UVariablesContainer.SetValue(m_context, name, variableObject);
        }

        public void SetValue(UVariableType T, object value)
        {
            var variableObject = new UVariableBase(T, value);
            UVariablesContainer.SetValue(m_context, name, variableObject);
        }

        public UVariableBase VariableObject()
        {
            var value = UVariablesContainer.Value(m_context, name);

            if (value == null)
            {
                value = new UVariableBase(m_defaultType, m_defaultValue);
                UVariablesContainer.SetValue(m_context, name, value);
            }

            return (UVariableBase)value;
        }

        public object Value()
        {
            return VariableObject().Value;
        }
        public override string ToString()
        {
            return $"Context: \"{m_context}\"; " + VariableObject().ToString();
        }
    }
}
