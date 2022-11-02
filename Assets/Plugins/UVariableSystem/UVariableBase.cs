using UVariableSystem.Helpers;

namespace UVariableSystem
{
    public struct UVariableBase
    {
        public UVariableType Type { get; private set; }
        public object Value { get; private set; }

        public void SetValue(object obj)
        {
            Type = TypeConverter.TypeDictionary[obj.GetType()];
            Value = obj;
        }

        public UVariableBase(UVariableType T, object V)
        {
            Type = T;
            Value = TypeConverter.CastValue(T, V);
        }

        public UVariableBase(object V)
        {
            Type = TypeConverter.TypeDictionary[V.GetType()];
            Value = V;
        }

        public override string ToString()
        {
            if (Value == null || Type == UVariableType.None)
                return $"{UVariableType.None.ToString()}: null";

            return $"Type: {Type.ToString()}; Value: \"{Value.ToString()}\"";
        }
    }
}
