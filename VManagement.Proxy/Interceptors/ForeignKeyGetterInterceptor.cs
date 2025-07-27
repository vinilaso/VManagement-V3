using Castle.DynamicProxy;
using System.Reflection;
using VManagement.Proxy.Utility.Extensions;
using VManagement.Proxy.Exceptions;
using VManagement.Proxy.Utility.Enum;

namespace VManagement.Proxy.Interceptors
{
    /// <summary>
    /// Intercepta chamadas de 'get' a propriedades de navegação para carregar a entidade relacionada sob demanda (Lazy Loading).
    /// </summary>
    /// <remarks>
    /// Este interceptor é o coração do mecanismo de Lazy Loading. Quando o desenvolvedor acessa uma propriedade de navegação
    /// pela primeira vez (ex: `pessoa.SuperiorInstance`), este interceptor é acionado. Ele usa reflexão para encontrar
    /// e invocar o método estático `Find` da entidade relacionada, usando o valor do ID da chave estrangeira correspondente
    /// para buscar o registro no banco de dados. O resultado é então retornado, e a propriedade é efetivamente "carregada".
    /// </remarks>
    public class ForeignKeyGetterInterceptor : IInterceptor
    {
        /// <summary>
        /// O método principal que é executado pelo Castle DynamicProxy sempre que um método virtual em uma entidade proxy é chamado.
        /// </summary>
        /// <param name="invocation">Contém todas as informações sobre a chamada do método que está sendo interceptado (alvo, método, argumentos, etc.).</param>
        public void Intercept(IInvocation invocation)
        {
            if (!invocation.ShouldInterceptForeignKey(InterceptedMethodType.Getter, out Type? foreignKeyType, out string? columnName))
            {
                invocation.Proceed();
                return;
            }

            InterceptorAbortedException.ThrowIfNull(foreignKeyType, nameof(foreignKeyType));
            InterceptorAbortedException.ThrowIfNull(columnName, nameof(columnName));

            MethodInfo? findMethod = foreignKeyType!.GetFindMethod();

            InterceptorAbortedException.ThrowIfNull(findMethod, nameof(findMethod));

            PropertyInfo? columnProp = invocation.TargetType!.GetColumnProperty(columnName!);

            ArgumentNullException.ThrowIfNull(columnProp);

            var value = columnProp.GetValue(invocation.InvocationTarget);

            if (value is null)
                invocation.ReturnValue = null;
            else
                invocation.ReturnValue = findMethod!.Invoke(null, [value]);
        }
    }
}
