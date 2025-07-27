using System.Reflection;
using VManagement.Commons.Entities.Attributes;

namespace VManagement.Proxy.Utility.Extensions
{
    /// <summary>
    /// Fornece métodos de extensão internos baseados em reflexão para o tipo <see cref="Type"/>.
    /// </summary>
    /// <remarks>
    /// Estes métodos de ajuda encapsulam a lógica de reflexão complexa usada pelos interceptores
    /// para encontrar dinamicamente métodos e propriedades nas classes de entidade.
    /// </remarks>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Obtém a referência ao método estático 'FindFirstOrDefault(long entityId)' da classe base <see cref="TableEntity{TEntity}"/>.
        /// </summary>
        /// <remarks>
        /// Este método é usado pelo interceptor de Lazy Loading para poder invocar dinamicamente o método de busca
        /// da entidade relacionada sem conhecer seu tipo concreto em tempo de compilação.
        /// </remarks>
        /// <param name="type">O tipo da entidade (ex: typeof(Pessoas)) do qual o método de busca será obtido.</param>
        /// <returns>Um objeto <see cref="MethodInfo"/> representando o método 'FindFirstOrDefault(long)'; ou <c>null</c> se não for encontrado.</returns>
        internal static MethodInfo? GetFindMethod(this Type type)
        {
            if (type.BaseType is null)
                return null;

            return type.BaseType.GetMethod(
                        "FindFirstOrDefault",
                        BindingFlags.Public | BindingFlags.Static,
                        binder: null,
                        types: [typeof(long)],
                        modifiers: null
                   );
        }

        /// <summary>
        /// Obtém a propriedade de um tipo de entidade que está decorada com um <see cref="EntityColumnNameAttribute"/> correspondente ao nome da coluna fornecido.
        /// </summary>
        /// <param name="type">O tipo da entidade a ser inspecionada.</param>
        /// <param name="columnName">O nome da coluna do banco de dados a ser procurada no atributo.</param>
        /// <returns>Um objeto <see cref="PropertyInfo"/> representando a propriedade correspondente; ou <c>null</c> se nenhuma propriedade for encontrada.</returns>
        internal static PropertyInfo? GetColumnProperty(this Type type, string columnName)
        {
            return type.GetProperties().FirstOrDefault(prop =>
            {
                var attribute = prop.GetCustomAttribute<EntityColumnNameAttribute>();
                if (attribute is null)
                    return false;

                return attribute.ColumnName == columnName;
            });

        }
    }
}
