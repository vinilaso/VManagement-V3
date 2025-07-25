using System.ComponentModel;
using System.Reflection;

namespace VManagement.Commons.Utility.Extensions
{
    /// <summary>
    /// Fornece métodos de extensão para tipos <see cref="Enum"/>.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Obtém o texto de descrição de um membro de um enumerador, com base em seu atributo <see cref="DescriptionAttribute"/>.
        /// </summary>
        /// <remarks>
        /// Se o membro do enumerador não possuir o atributo <see cref="DescriptionAttribute"/>, o comportamento
        /// padrão é retornar o nome do próprio membro como uma string.
        /// </remarks>
        /// <typeparam name="T">O tipo do enumerador, que deve ser um <see cref="Enum"/>.</typeparam>
        /// <param name="value">O valor do enumerador do qual a descrição será extraída.</param>
        /// <param name="throwIfNull">Se <c>true</c>, lança uma <see cref="KeyNotFoundException"/> caso o atributo de descrição não seja encontrado. Se <c>false</c>, retorna o nome do membro do enumerador.</param>
        /// <returns>A string contida no <see cref="DescriptionAttribute"/>, ou o nome do membro do enumerador se o atributo não existir e <paramref name="throwIfNull"/> for <c>false</c>.</returns>
        /// <exception cref="KeyNotFoundException">Lançada se <paramref name="throwIfNull"/> for <c>true</c> e o atributo <see cref="DescriptionAttribute"/> não for encontrado.</exception>
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
