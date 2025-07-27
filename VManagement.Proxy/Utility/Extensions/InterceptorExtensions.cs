using Castle.DynamicProxy;
using System.Reflection;
using VManagement.Commons.Utility.Extensions;
using VManagement.Proxy.Attributes;
using VManagement.Proxy.Utility.Enum;

namespace VManagement.Proxy.Utility.Extensions
{
    /// <summary>
    /// Fornece métodos de extensão para a interface <see cref="IInvocation"/> do Castle DynamicProxy.
    /// </summary>
    public static class InterceptorExtensions
    {
        /// <summary>
        /// Determina se a chamada de método interceptada corresponde a uma propriedade de navegação de chave estrangeira.
        /// </summary>
        /// <remarks>
        /// Este método centraliza a lógica de reflexão para verificar se uma propriedade virtual
        /// está decorada com o atributo <see cref="ForeignKeyAttribute{TForeignKey}"/>. Ele verifica o prefixo do método ('get_' ou 'set_')
        /// e inspeciona os atributos da propriedade correspondente.
        /// </remarks>
        /// <param name="invocation">O objeto de invocação fornecido pelo interceptor, contendo informações sobre a chamada do método.</param>
        /// <param name="methodType">O tipo de método a ser verificado (<see cref="InterceptedMethodType.Getter"/> ou <see cref="InterceptedMethodType.Setter"/>).</param>
        /// <param name="foreignKeyType">Parâmetro de saída. Se a interceptação for bem-sucedida, conterá o tipo da entidade estrangeira.</param>
        /// <param name="columnName">Parâmetro de saída. Se a interceptação for bem-sucedida, conterá o nome da coluna de ID da chave estrangeira.</param>
        /// <returns><c>true</c> se a chamada for para um getter ou setter de uma propriedade de chave estrangeira; caso contrário, <c>false</c>.</returns>
        public static bool ShouldInterceptForeignKey(this IInvocation invocation, InterceptedMethodType methodType, out Type? foreignKeyType, out string? columnName)
        {
            foreignKeyType = null;
            columnName = null;

            if (!invocation.Method.Name.StartsWith(methodType.GetMethodPrefix(), StringComparison.OrdinalIgnoreCase))
                return false;

            if (invocation.Method.GetProperty() is not PropertyInfo property)
                return false;

            var customAttributes = property.GetCustomAttributes();

            foreach (var attr in customAttributes)
            {
                var type = attr.GetType();

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ForeignKeyAttribute<>))
                {
                    foreignKeyType = type.GetGenericArguments()[0];
                    columnName = type.GetProperty("ColumnName")?.GetValue(attr) as string;
                    return true;
                }
            }

            return false;
        }
    }
}
