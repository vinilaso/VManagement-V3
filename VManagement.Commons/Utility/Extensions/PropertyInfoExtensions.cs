using System.Reflection;
using VManagement.Commons.Entities.Interfaces;

namespace VManagement.Commons.Utility.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static bool IsChanged(this PropertyInfo property, ITableEntity currentInstance)
        {
            ArgumentNullException.ThrowIfNull(property);

            object?
                currentValue = property.GetValue(currentInstance),
                originalValue = property.GetValue(currentInstance.GetOriginalInstance());

            return !currentValue?.Equals(originalValue) ?? false;
        }
    }
}
