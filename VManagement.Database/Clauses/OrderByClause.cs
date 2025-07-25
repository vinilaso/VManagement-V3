using System.ComponentModel;
using VManagement.Commons.Utility.Extensions;

namespace VManagement.Database.Clauses
{
    /// <summary>
    /// Representa a cláusula ORDER BY de um comando do banco de dados.
    /// </summary>
    public class OrderByClause
    {
        /// <summary>
        /// Especifica qual método de ordenação será utilizado.
        /// </summary>
        public enum SortDirection
        {
            /// <summary>
            /// Ordena do menor para o maior.
            /// </summary>
            [Description("ASC")]
            Ascending,

            /// <summary>
            /// Ordena do maior para o menor.
            /// </summary>
            [Description("DESC")]
            Descending,

            /// <summary>
            /// Ordena de forma aleatória.
            /// </summary>
            [Description("NEWID()")]
            Random
        }

        private readonly List<string> _sortings = new();

        /// <summary>
        /// Adiciona uma cláusula order by à esta instância.
        /// </summary>
        /// <param name="field">O campo em que a ordenação se baseará.</param>
        /// <param name="direction">A direção da ordenação. (ASC, DESC, RANDOM)</param>
        public void AddSorting(string field, SortDirection direction = SortDirection.Ascending)
        {
            _sortings.Add($"{field} {direction.GetDescription()}");
        }

        /// <summary>
        /// Adiciona uma cláusula ORDER BY RANDOM à esta instância.
        /// </summary>
        public void OrderByRandom()
        {
            _sortings.Add(SortDirection.Random.GetDescription());
        }

        /// <summary>
        /// Adiciona uma cláusula order by à esta instância.
        /// </summary>
        /// <param name="other">A cláusula a ser adicionada.</param>
        public void AddSorting(OrderByClause other)
        {
            if (other is null)
                return;

            other._sortings.ForEach(_sortings.Add);
        }

        /// <summary>
        /// Verifica se foram adicionadas ordenações à esta instância.
        /// </summary>
        /// <returns>True, caso hajam ordenações. Caso contrário, False.</returns>
        public bool IsEmpty()
        {
            return _sortings.Count < 1;
        }

        /// <summary>
        /// Retorna a representação em string desta instância.
        /// </summary>
        /// <returns>A representação em string desta instância.</returns>
        public override string ToString()
        {
            return string.Join(", ", _sortings);
        }
    }
}
