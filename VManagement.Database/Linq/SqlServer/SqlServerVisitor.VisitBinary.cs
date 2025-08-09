using System.Linq.Expressions;
using VManagement.Commons.Utility.Extensions;

namespace VManagement.Database.Linq.SqlServer
{
    public partial class SqlServerVisitor
    {
        /// <inheritdoc/>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            StartCondition();
            Visit(node.Left);

            bool isNullCheck = node.Right.IsConstantNull() || node.Left.IsConstantNull();

            if (node.NodeType.In(ExpressionType.Equal, ExpressionType.NotEqual) && isNullCheck)
            {
                AppendNullCheck(node.NodeType);
            }
            else
            {
                AppendLogicalOperator(node.NodeType);
            }

            Visit(node.Right);

            EndCondition();

            return node;
        }

        private void AppendLogicalOperator(ExpressionType expressionType)
        {
            _sqlBuilder.Append(" " + _logicalOperators[expressionType] + " ");
        }

        private void AppendNullCheck(ExpressionType expressionType)
        {
            string nullCheck = expressionType == ExpressionType.Equal ? " IS " : " IS NOT ";
            _sqlBuilder.Append(nullCheck);
        }
    }
}
