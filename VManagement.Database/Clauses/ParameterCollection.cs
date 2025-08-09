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

        /// <summary>
        /// Retorna uma representação em string de todos os parâmetros na coleção, mostrando seus nomes e valores.
        /// </summary>
        /// <returns>Uma string formatada com os parâmetros. Ex: "@pID=1, @pNAME=João".</returns>
        public override string ToString()
        {
            if (Count == 0)
            {
                return "Nenhum parâmetro.";
            }

            return string.Join(Environment.NewLine, this.Select(p => $"{p.ParameterName} = {p.Value}"));
        }
    }
}