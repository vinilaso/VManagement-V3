using VManagement.Commons.Entities.Interfaces;

namespace VManagement.Commons.Entities
{
    public class TrackedField : ITrackedField
    {
        public string Name { get; private set; } = string.Empty;

        public object? Value { get; private set; }

        public object? OriginalValue { get; private set; }

        public bool IsNull => Value == null;

        public bool Changed
        {
            get
            {
                if (Value == null && OriginalValue == null)
                    return false;

                if (Value == null && OriginalValue != null)
                    return true;

                return !Value!.Equals(OriginalValue);
            }
        }

        public TrackedField(string name, object? initialValue)
        {
            Name = name;
            SetValue(initialValue);
        }

        public void SetValue(object? value)
        {
            Value = value;
            OriginalValue = value;
        }

        public void ChangeValue(object? newValue)
        {
            Value = newValue;
        }
    }
}