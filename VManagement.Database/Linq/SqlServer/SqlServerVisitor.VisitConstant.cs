using System.Linq.Expressions;
using VManagement.Database.Command;

namespace VManagement.Database.Linq.SqlServer
{
    public partial class SqlServerVisitor
    {
        /// <inheritdoc/>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is null)
            {
                _sqlBuilder.Append("NULL");
            }
            else if (node.Value is bool)
            {
                VisitBoolValue(node);
            }
            else
            {
                VisitConstantValue(node);
            }

            return node;
        }

        private void VisitBoolValue(ConstantExpression node)
        {
            var constVal = Expression.Constant((bool)node.Value! ? 1 : 0);
            VisitConstant(constVal);
        }

        private void VisitConstantValue(ConstantExpression node)
        {
            string parameterName = ParameterNameFactory.NewParameter("EXPRESSION_MEMBER");
            _parameters.Add(parameterName, node.Value);

            _sqlBuilder.Append(parameterName);
        }
    }
}
