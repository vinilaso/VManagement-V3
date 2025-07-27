using Castle.DynamicProxy;
using System.Reflection;
using VManagement.Proxy.Exceptions;
using VManagement.Proxy.Utility.Enum;
using VManagement.Proxy.Utility.Extensions;

namespace VManagement.Proxy.Interceptors
{
    /// <summary>
    /// Intercepta chamadas de 'set' a propriedades de navegação para sincronizar automaticamente a propriedade de chave estrangeira correspondente.
    /// </summary>
    /// <remarks>
    /// Quando o desenvolvedor define uma propriedade de navegação
    /// (ex: `pessoa.SuperiorInstance = outraPessoa;`), este interceptor é acionado para definir automaticamente a propriedade de ID
    /// correspondente (ex: `pessoa.SuperiorId = outraPessoa.Id;`). Isso remove a necessidade de gerenciar ambos os valores manualmente.
    /// </remarks>
    public class ForeignKeySetterInterceptor : IInterceptor
    {
        /// <summary>
        /// O método principal que é executado pelo Castle DynamicProxy sempre que um método virtual em uma entidade proxy é chamado.
        /// </summary>
        /// <param name="invocation">Contém todas as informações sobre a chamada do método que está sendo interceptado (alvo, método, argumentos, etc.).</param>
        public void Intercept(IInvocation invocation)
        {
            if (!invocation.ShouldInterceptForeignKey(InterceptedMethodType.Setter, out Type? foreignKeyType, out string? columnName))
            {
                invocation.Proceed();
                return;
            }

            InterceptorAbortedException.ThrowIfNull(foreignKeyType, nameof(foreignKeyType));
            InterceptorAbortedException.ThrowIfNull(columnName, nameof(columnName));

            PropertyInfo? columnProp = invocation.TargetType?.GetColumnProperty(columnName!);

            ArgumentNullException.ThrowIfNull(columnProp);

            var argument = invocation.Arguments[0];

            if (argument is null)
            {
                columnProp.SetValue(invocation.InvocationTarget, null);
            }
            else
            {
                var idProp = foreignKeyType!.GetProperty("Id");

                InterceptorAbortedException.ThrowIfNull(idProp, nameof(idProp));

                columnProp.SetValue(invocation.InvocationTarget, idProp!.GetValue(argument));
            }
        }
    }
}
