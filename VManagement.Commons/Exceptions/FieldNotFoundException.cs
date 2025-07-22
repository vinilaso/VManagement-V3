namespace VManagement.Commons.Exceptions
{
    public class FieldNotFoundException : Exception
    {
        public FieldNotFoundException(string fieldName) : base($"O campo {fieldName} não está presente na entidade.")
        { 
        }

        public FieldNotFoundException(string fieldName, Type type) : base($"O tipo {type.Name} não implementa o campo {fieldName}.")
        {
        }
    }
}
