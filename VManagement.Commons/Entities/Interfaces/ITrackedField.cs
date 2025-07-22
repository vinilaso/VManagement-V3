namespace VManagement.Commons.Entities.Interfaces
{
    public interface ITrackedField
    {
        string Name { get; }
        object? Value { get; }
        object? OriginalValue { get; }
        bool IsNull { get; }
        bool Changed { get; }

        void ChangeValue(object? newValue);

        string AsString()
        {
            return Convert.ToString(Value) ?? string.Empty;
        }

        short AsInt16()
        {
            return Convert.ToInt16(Value);
        }

        int AsInt32()
        {
            return Convert.ToInt32(Value);
        }

        long AsInt64()
        {
            return Convert.ToInt64(Value);
        }

        DateTime AsDateTime()
        {
            return Convert.ToDateTime(Value);
        }
    }
}