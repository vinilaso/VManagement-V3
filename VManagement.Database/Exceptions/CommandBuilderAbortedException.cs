namespace VManagement.Database.Exceptions
{
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