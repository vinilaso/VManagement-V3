using System.Reflection;
using VManagement.Proxy.Attributes;

namespace VManagement.Proxy.Utility.Enum
{
    /// <summary>
    /// Enumera os tipos de métodos de propriedade que podem ser interceptados.
    /// </summary>
    public enum InterceptedMethodType
    {
        /// <summary>
        /// Representa o acessor 'get' de uma propriedade.
        /// </summary>
        [MethodTypePrefix("get_")]
        Getter,

        /// <summary>
        /// Representa o acessor 'set' de uma propriedade.
        /// </summary>
        [MethodTypePrefix("set_")]
        Setter
    }

    /// <summary>
    /// Fornece métodos de extensão para o enumerador <see cref="InterceptedMethodType"/>.
    /// </summary>
    public static class InterceptedMethodTypeExtesions
    {
        /// <summary>
        /// Obtém o prefixo de nome de método associado a um membro do enumerador <see cref="InterceptedMethodType"/>.
        /// </summary>
        /// <remarks>
        /// Este método utiliza reflexão para ler o atributo <see cref="MethodTypePrefixAttribute"/>
        /// decorado em cada membro do enumerador.
        /// </remarks>
        /// <param name="type">O valor do enumerador do qual o prefixo será extraído.</param>
        /// <returns>O prefixo de string associado; ou uma string vazia se o atributo não for encontrado.</returns>
        public static string GetMethodPrefix(this InterceptedMethodType type)
        {
            var attr = typeof(InterceptedMethodType)
                .GetMember(type.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<MethodTypePrefixAttribute>();

            if (attr is null)
                return string.Empty;

            return attr.Prefix;
        }
    }
}
