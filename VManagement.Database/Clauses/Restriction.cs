using VManagement.Commons.Utility;
using VManagement.Database.Command;

namespace VManagement.Database.Clauses
{
    public class Restriction
    {
        public static readonly Restriction Empty = new();

        public WhereClause WhereClause { get; set; } = new();
        public OrderByClause OrderByClause { get; set; } = new();
        public ParameterCollection Parameters { get; set; } = new();

        public Restriction()
        {
        }

        public Restriction(string whereClause)
        {
            AddWhere(whereClause);
        }

        public void AddWhere(string whereClause)
        {
            WhereClause.AddWhere(whereClause);
        }

        public void AddWhere(WhereClause whereClause)
        {
            WhereClause.AddWhere(whereClause);
        }

        public void OrderBy(string orderByClause, OrderByClause.SortDirection sortDirection = OrderByClause.SortDirection.Ascending)
        {
            OrderByClause.AddSorting(orderByClause, sortDirection);
        }

        public void OrderBy(OrderByClause orderByClause)
        {
            OrderByClause.AddSorting(orderByClause);
        }

        public void Append(Restriction other)
        {
            if (other == null)
                return;

            AddWhere(other.WhereClause);
            OrderBy(other.OrderByClause);
            other.Parameters.ForEach(Parameters.Add);
        }

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
