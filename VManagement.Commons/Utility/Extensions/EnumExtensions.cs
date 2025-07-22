using System.ComponentModel;
using System.Reflection;

namespace VManagement.Commons.Utility.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T value, bool throwIfNull = false) where T : Enum
        {
            Type type = value.GetType();
            FieldInfo? fieldInfo = type.GetField(value.ToString());
            
            if (fieldInfo == null)
            {
                if (throwIfNull)
                    throw new KeyNotFoundException($"O elemento {value} não possui o atributo {nameof(DescriptionAttribute)} na sua definição.");

                return value.ToString();
            }

            if (fieldInfo.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute attribute)
                return attribute.Description;

            if (throwIfNull)
                throw new KeyNotFoundException($"O elemento {value} não possui o atributo {nameof(DescriptionAttribute)} na sua definição.");

            return value.ToString();
        }
    }
}
