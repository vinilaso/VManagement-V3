using System.Linq.Expressions;
using System.Reflection;
using VManagement.Commons.Entities.Attributes;

namespace VManagement.Database.Linq
{
    public class SelectPropertiesExpressionVisitor : ExpressionVisitor
    {
        private readonly List<string> _columnNames = [];
        private readonly List<PropertyInfo> _properties = [];

        public IReadOnlyCollection<string> ColumnNames => _columnNames;
        public IReadOnlyCollection<PropertyInfo> Properties => _properties;

        /// <inheritdoc/>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member is not PropertyInfo propertyInfo)
                return base.VisitMember(node);

            if (propertyInfo.GetCustomAttribute<EntityColumnNameAttribute>() is not EntityColumnNameAttribute attribute)
                return base.VisitMember(node);

            _properties.Add(propertyInfo);
            _columnNames.Add(attribute.ColumnName);

            return base.VisitMember(node);
        }
    }
}
