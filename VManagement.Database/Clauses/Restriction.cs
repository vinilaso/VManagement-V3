using VManagement.Commons.Utility;
using VManagement.Database.Command;

namespace VManagement.Database.Clauses
{
    public class Restriction
    {
        /// <summary>
        /// Obtém uma instância de restrição vazia.
        /// </summary>
        public static Restriction Empty => new();

        /// <summary>
        /// Obtém ou define a cláusula WHERE desta restrição.
        /// </summary>
        public WhereClause WhereClause { get; set; } = new();

        /// <summary>
        /// Obtém ou define a cláusula ORDER BY desta restrição.
        /// </summary>
        public OrderByClause OrderByClause { get; set; } = new();

        /// <summary>
        /// Obtém ou define a coleção de parâmetros SQL associados a esta restrição.
        /// </summary>
        public ParameterCollection Parameters { get; set; } = new();

        /// <summary>
        /// Inicia uma nova instância vazia da classe <see cref="Restriction"/>.
        /// </summary>
        public Restriction()
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Restriction"/> com uma cláusula WHERE inicial.
        /// </summary>
        /// <param name="whereClause">A string da cláusula WHERE inicial.</param>
        public Restriction(string whereClause)
        {
            AddWhere(whereClause);
        }

        /// <summary>
        /// Adiciona uma condição à cláusula WHERE.
        /// </summary>
        /// <param name="whereClause">A condição a ser adicionada.</param>
        public void AddWhere(string whereClause)
        {
            WhereClause.AddWhere(whereClause);
        }

        /// <summary>
        /// Adiciona todas as condições de outra <see cref="WhereClause"/>.
        /// </summary>
        /// <param name="whereClause">A cláusula WHERE a ser adicionada.</param>
        public void AddWhere(WhereClause whereClause)
        {
            WhereClause.AddWhere(whereClause);
        }

        /// <summary>
        /// Adiciona uma regra de ordenação à cláusula ORDER BY.
        /// </summary>
        /// <param name="orderByClause">O campo pelo qual ordenar.</param>
        /// <param name="sortDirection">A direção da ordenação (ascendente ou descendente).</param>
        public void OrderBy(string orderByClause, OrderByClause.SortDirection sortDirection = OrderByClause.SortDirection.Ascending)
        {
            OrderByClause.AddSorting(orderByClause, sortDirection);
        }

        /// <summary>
        /// Adiciona todas as regras de ordenação de outra <see cref="OrderByClause"/>.
        /// </summary>
        /// <param name="orderByClause">A cláusula ORDER BY a ser adicionada.</param>
        public void OrderBy(OrderByClause orderByClause)
        {
            OrderByClause.AddSorting(orderByClause);
        }

        /// <summary>
        /// Anexa as cláusulas e parâmetros de outra restrição a esta instância.
        /// </summary>
        /// <param name="other">A outra instância de <see cref="Restriction"/> a ser anexada.</param>
        public void Append(Restriction other)
        {
            if (other == null)
                return;

            AddWhere(other.WhereClause);
            OrderBy(other.OrderByClause);
            other.Parameters.ForEach(Parameters.Add);
        }

        /// <summary>
        /// Cria uma restrição padrão baseada no campo ID.
        /// </summary>
        /// <param name="id">O valor do ID para a restrição.</param>
        /// <param name="withAlias">Indica se o nome do campo deve ser prefixado com um alias de tabela.</param>
        /// <param name="alias">O alias da tabela a ser usado se <paramref name="withAlias"/> for verdadeiro.</param>
        /// <returns>Uma nova instância de <see cref="Restriction"/> com a condição "ID = @param".</returns>
        public static Restriction FromId(long? id, bool withAlias = true, string alias = "V")
        {
            string restrictionText = string.Empty;
            string parameterName = ParameterNameFactory.NewParameter("ID");

            if (withAlias)
                restrictionText = $"{alias}.";

            restrictionText += $"ID = {parameterName}";

            Restriction restriction = new(restrictionText);
            restriction.Parameters.Add(parameterName, id);

            return restriction;
        }

        /// <summary>
        /// Retorna a representação em string das cláusulas WHERE e ORDER BY combinadas.
        /// </summary>
        /// <returns>A string SQL formatada das cláusulas.</returns>
        public override string ToString()
        {
            DelimitedStringBuilder builder = new(" ");

            if (!WhereClause.IsEmpty())
            {
                builder.Append("WHERE");
                builder.Append(WhereClause.ToString());
            }

            if (!OrderByClause.IsEmpty())
            {
                builder.Append("ORDER BY");
                builder.Append(OrderByClause.ToString());
            }

            return builder.ToString();
        }
    }
}