namespace VManagement.Commons.Exceptions
{
    public class InvalidFieldException : Exception
    {
        public InvalidFieldException(string fieldName, string motivo = "Motivo não especificado") : base($"O campo {fieldName} é inválido. {motivo}.")
        {
        }
    }
}
