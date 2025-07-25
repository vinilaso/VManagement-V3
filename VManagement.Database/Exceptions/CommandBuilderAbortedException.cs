namespace VManagement.Database.Exceptions
{
    /// <summary>
    /// Representa o erro que ocorre quando a construção de um comando SQL pelo <see cref="Command.CommandBuilder{TTableEntity}"/>
    /// é abortada devido a uma configuração ou estado inválido.
    /// </summary>
    /// <remarks>
    /// Esta exceção é utilizada para fornecer mensagens de erro claras e específicas quando as opções do CommandBuilder
    /// entram em conflito com os parâmetros fornecidos (por exemplo, tentar popular um objeto de comando que não foi passado).
    /// Utilize as propriedades estáticas <see cref="InvalidEntity"/> e <see cref="InvalidCommand"/> para obter instâncias
    /// pré-configuradas para os cenários de erro mais comuns.
    /// </remarks>
    public class CommandBuilderAbortedException(string message) : Exception(message)
    {
        /// <summary>
        /// Obtém uma instância da exceção para o cenário onde uma entidade válida é necessária, mas não foi fornecida.
        /// </summary>
        public static CommandBuilderAbortedException InvalidEntity { get; private set; }

        /// <summary>
        /// Obtém uma instância da exceção para o cenário onde um comando válido é necessário, mas não foi fornecido.
        /// </summary>
        public static CommandBuilderAbortedException InvalidCommand { get; private set; }

        static CommandBuilderAbortedException()
        {
            InvalidEntity = new("Operação cancelada. O builder está configurado para auto gerar restrições, porém não foi informada entidade válida.");
            InvalidCommand = new("Operação cancelada. O builder está configurado para popular objetos de comando, porém não foi informado comando válido.");
        }
    }
}