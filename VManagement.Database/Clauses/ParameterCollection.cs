using Microsoft.Data.SqlClient;

namespace VManagement.Database.Clauses
{
    /// <summary>
    /// Representa uma coleção de parâmetros <see cref="SqlParameter"/> para um comando de banco de dados.
    /// </summary>
    /// <remarks>
    /// Esta classe herda de <see cref="List{T}"/> de <see cref="SqlParameter"/> e fornece um método 'Add'
    /// sobrecarregado para simplificar a criação e adição de parâmetros, tratando automaticamente a
    /// conversão de valores nulos para <see cref="DBNull.Value"/>.
    /// </remarks>
    public class ParameterCollection : List<SqlParameter>
    {
        /// <summary>
        /// Adiciona um novo parâmetro à coleção, garantindo que o nome do parâmetro comece com '@'.
        /// </summary>
        /// <param name="parameterName">O nome do parâmetro.</param>
        /// <param name="parameterValue">O valor do parâmetro.</param>
        public void Add(string parameterName, object? parameterValue)
        {
            parameterValue ??= DBNull.Value;

            if (parameterName.StartsWith('@'))
                Add(new SqlParameter($"{parameterName}", parameterValue));
            else
                Add(new SqlParameter($"@{parameterName}", parameterValue));
        }
    }
}