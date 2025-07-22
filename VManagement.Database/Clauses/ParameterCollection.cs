using Microsoft.Data.SqlClient;

namespace VManagement.Database.Clauses
{
    public class ParameterCollection : List<SqlParameter>
    {
        /// <summary>
        /// Adiciona um novo parâmetro à coleção, garantindo que o nome do parâmetro comece com '@'.
        /// </summary>
        /// <param name="parameterName">O nome do parâmetro.</param>
        /// <param name="parameterValue">O valor do parâmetro.</param>
        public void Add(string parameterName, object? parameterValue)
        {
            if (parameterName.StartsWith('@'))
                Add(new SqlParameter($"{parameterName}", parameterValue));
            else
                Add(new SqlParameter($"@{parameterName}", parameterValue));
        }
    }
}