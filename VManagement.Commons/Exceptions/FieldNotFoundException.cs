namespace VManagement.Commons.Exceptions
{
    public class FieldNotFoundException : Exception
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FieldNotFoundException"/>.
        /// </summary>
        /// <param name="fieldName">O nome do campo que não foi encontrado.</param>
        public FieldNotFoundException(string fieldName) : base($"O campo {fieldName} não está presente na entidade.")
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FieldNotFoundException"/>.
        /// </summary>
        /// <param name="fieldName">O nome do campo que não foi encontrado.</param>
        /// <param name="type">O tipo da entidade onde o campo não foi encontrado.</param>
        public FieldNotFoundException(string fieldName, Type type) : base($"O tipo {type.Name} não implementa o campo {fieldName}.")
        {
        }
    }
}