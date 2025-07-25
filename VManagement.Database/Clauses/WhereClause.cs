using VManagement.Commons.Utility.Extensions;

namespace VManagement.Database.Clauses
{
    /// <summary>
    /// Fornece um mecanismo para a construção de cláusulas where SQL.
    /// </summary>
    public class WhereClause
    {
        private readonly List<string> _restrictions = new();

        /// <summary>
        /// Adiciona uma cláusula where à esta instância, entre parêntesis.
        /// </summary>
        /// <param name="where">A cláusula where.</param>
        public void AddWhere(string where)
        {
            if (!string.IsNullOrEmpty(where))
                _restrictions.Add(where.BetweenParenthesis());
        }

        /// <summary>
        /// Adiciona outro objeto <see cref="WhereClause"/> à este.
        /// </summary>
        /// <param name="other">O objeto que será adicionado.</param>
        public void AddWhere(WhereClause other)
        {
            if (other == null)
                return;

           AddWhere(other.ToString());
        }

        /// <summary>
        /// Verifica se a cláusula where está vazia.
        /// </summary>
        /// <returns>True, caso a cláusula esteja vazia. Caso contrário, False.</returns>
        public bool IsEmpty()
        {
            return _restrictions.Count < 1;
        }

        /// <summary>
        /// Remove todas as cláusulas where adicionadas à esta instância anteriormente.
        /// </summary>
        public void Clear()
        {
            _restrictions.Clear();
        }

        /// <summary>
        /// Retorna a representação desta instância em string.
        /// </summary>
        /// <returns>A representação desta instância em string.</returns>
        public override string ToString()
        {
            return string.Join(" AND ", _restrictions);
        }
    }
}
