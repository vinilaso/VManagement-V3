using Microsoft.Data.SqlClient;

namespace VManagement.Database.Clauses
{
    public class ParameterCollection : List<SqlParameter>
    {
        public void Add(string parameterName, object? parameterValue)
        {
            if (parameterName.StartsWith('@'))
                Add(new SqlParameter($"{parameterName}", parameterValue));
            else
                Add(new SqlParameter($"@{parameterName}", parameterValue));
        }
    }
}
