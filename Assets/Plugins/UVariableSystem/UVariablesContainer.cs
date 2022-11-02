using System.Collections.Generic;

namespace UVariableSystem
{
    public static class UVariablesContainer
    {
        /// <summary>
        /// Словарь переменных, имеющий вид:
        /// 
        /// { 
        ///     "context_1": 
        ///     { 
        ///         "varname_1": value_1,
        ///         "varname_2": value_2,
        ///         ...
        ///     },
        ///     ...
        /// }
        /// </summary>
        private static Dictionary<string, Dictionary<string, object>> m_VariableContainer;

        public static object Value(string context, string valueName)
        {
            m_VariableContainer ??= new Dictionary<string, Dictionary<string, object>>();

            if (m_VariableContainer.ContainsKey(context))
            {
                if (m_VariableContainer[context].ContainsKey(valueName))
                {
                    return m_VariableContainer[context][valueName];
                }
                
                //{
                //    Debug.Log("unacceptable value name information: " +
                //             $"Context: \"{context}\" doesn't has value name: \"{valueName}\"");
                //}
            }
            
            //{
            //    Debug.Log("unacceptable context name information: " +
            //             $"Variable map doesn't has key of context name \"{context}\"");
            //}

            return null;
        }

        public static void SetValue(string context, string valueName, object value)
        {
            m_VariableContainer ??= new Dictionary<string, Dictionary<string, object>>();

            if (!m_VariableContainer.ContainsKey(context))
                m_VariableContainer[context] = new Dictionary<string, object>();

            m_VariableContainer[context][valueName] = value;
        }
    }
}
