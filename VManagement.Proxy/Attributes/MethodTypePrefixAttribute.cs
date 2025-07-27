using VManagement.Proxy.Utility.Enum;

namespace VManagement.Proxy.Attributes
{
    /// <summary>
    /// Associa um prefixo de nome de método a um membro de um enumerador.
    /// </summary>
    /// <remarks>
    /// Este atributo é usado como um metadado para o enum <see cref="InterceptedMethodType"/>,
    /// permitindo que a lógica do interceptor obtenha dinamicamente o prefixo de string correto
    /// para verificar o tipo de método que está sendo invocado.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MethodTypePrefixAttribute : Attribute
    {
        /// <summary>
        /// Obtém ou define o prefixo de string associado ao membro do enumerador.
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// Inicia uma nova instância do atributo <see cref="MethodTypePrefixAttribute"/>.
        /// </summary>
        /// <param name="prefix">O prefixo a ser associado (ex: "get_").</param>
        public MethodTypePrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }
}
