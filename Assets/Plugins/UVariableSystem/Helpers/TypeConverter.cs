using System;
using System.Collections.Generic;

namespace UVariableSystem.Helpers
{
    public static class TypeConverter
    {
        public static readonly Dictionary<Type, UVariableType> TypeDictionary = new Dictionary<Type, UVariableType>
        {
            {typeof(string), UVariableType.String},
            {typeof(int), UVariableType.Int},
            {typeof(float), UVariableType.Float},
            {typeof(bool), UVariableType.Bool},
            {typeof(Nullable), UVariableType.None},
        };

        public static object CastValue(UVariableType type, object value)
        {
            switch (type)
            {
                case UVariableType.Any: { break; }
                case UVariableType.None: { value = null; break; }
                case UVariableType.String: { value = value.ToString(); break; }
                case UVariableType.Int:
                    {
                        int newValue;
                        int.TryParse(value.ToString(), out newValue);
                        value = newValue;
                        break;
                    }
                case UVariableType.Float: 
                    {
                        float newValue;
                        float.TryParse(value.ToString(), out newValue);
                        value = newValue;
                        break;
                    }
                case UVariableType.Bool: 
                    {
                        var stringValue = value.ToString();

                        int intValue;
                        int.TryParse(stringValue, out intValue);

                        if (intValue > 0)
                        {
                            value = true;
                            break;
                        }

                        bool newValue;
                        bool.TryParse(stringValue, out newValue);

                        value = newValue;
                        break; 
                    }
                default: 
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return value;
        }
    }
}
