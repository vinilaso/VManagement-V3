using System.Linq.Expressions;
using System.Reflection;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Utility.Extensions;
using VManagement.Database.Command;

namespace VManagement.Database.Linq.SqlServer
{
    public partial class SqlServerVisitor
    {
        /// <inheritdoc/>
        protected override Expression VisitMember(MemberExpression node)
        {
            // Quando a expressão acessa propriedades do tipo base (_baseType) do Visitor.
            if (node.IsBaseTypePropertyGetterNode())
            {
                VisitParameter(node);
            }

            // Quando a expressão acessa variáveis externas
            else if (node.IsClosureNode())
            {
                VisitClosure(node);
            }

            // Quando a expressão acessa propriedades de tipo diferente do tipo base (_baseType) do Visitor.
            else if (node.IsMemberAccess())
            {
                VisitOtherTypeProperty(node);
            }

            return node;
        }

        private void VisitParameter(MemberExpression node)
        {
            var property = (PropertyInfo)node.Member;
            var attribute = property.GetCustomAttribute<EntityColumnNameAttribute>();

            if (attribute is null)
                throw new NotSupportedException($"Só é possível acessar propriedades decoradas com o atributo {nameof(EntityColumnNameAttribute)}.");

            _sqlBuilder.Append(attribute.ColumnName.WithSqlAlias());
        }

        private void VisitClosure(MemberExpression node)
        {
            Func<object> getter = node.ToGetterMethod();
            VisitConstant(Expression.Constant(getter()));
        }

        private void VisitOtherTypeProperty(MemberExpression node)
        {
            Func<object> getter = node.ToGetterMethod();
            VisitConstant(Expression.Constant(getter()));
        }
    }
}
