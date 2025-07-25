namespace VManagement.Commons.Exceptions
{
    /// <summary>
    /// Representa o erro que ocorre quando um campo que está sendo adicionado a uma entidade é considerado inválido.
    /// </summary>
    /// <remarks>
    /// Esta exceção é tipicamente lançada pela <see cref="Entities.TrackedFieldCollection"/> ao tentar adicionar um campo
    /// com um nome nulo, vazio ou duplicado.
    /// </remarks>
    public class InvalidFieldException : Exception
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="InvalidFieldException"/>.
        /// </summary>
        /// <param name="fieldName">O nome do campo que é inválido.</param>
        /// <param name="motivo">A razão pela qual o campo é considerado inválido.</param>
        public InvalidFieldException(string fieldName, string motivo = "Motivo não especificado") : base($"O campo {fieldName} é inválido. {motivo}.")
        {
        }
    }
}