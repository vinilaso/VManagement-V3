namespace VManagement.Commons.Exceptions
{
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