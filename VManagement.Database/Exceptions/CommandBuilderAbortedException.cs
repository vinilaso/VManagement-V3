namespace VManagement.Database.Exceptions
{
    public class CommandBuilderAbortedException(string message) : Exception(message)
    {
        public static CommandBuilderAbortedException InvalidEntity { get; private set; }
        public static CommandBuilderAbortedException InvalidCommand { get; private set; }

        static CommandBuilderAbortedException()
        {
            InvalidEntity = new("Operação cancelada. O builder está configurado para auto gerar restrições, porém não foi informada entidade válida.");
            InvalidCommand = new("Operação cancelada. O builder está configurado para popular objetos de comando, porém não foi informado comando válido.");
        }
    }
}
