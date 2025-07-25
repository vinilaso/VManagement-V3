namespace VManagement.Commons.Exceptions
{
    /// <summary>
    /// Representa o erro que ocorre ao tentar acessar um campo que não existe em uma coleção de campos da entidade.
    /// </summary>
    /// <remarks>
    /// Esta exceção é tipicamente lançada ao usar o indexador de uma <see cref="TrackedFieldCollection"/>
    /// com um nome de campo que não corresponde a nenhuma propriedade mapeada na entidade.
    /// </remarks>
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