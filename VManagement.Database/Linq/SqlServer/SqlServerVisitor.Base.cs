using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using VManagement.Commons.Utility.Extensions;
using VManagement.Database.Clauses;

namespace VManagement.Database.Linq.SqlServer
{
    public partial class SqlServerVisitor : ExpressionVisitor
    {
        private Type? _baseType = null;
        private readonly StringBuilder _sqlBuilder = new();
        private readonly ParameterCollection _parameters = [];

        private static readonly Dictionary<ExpressionType, string> _logicalOperators = new()
        {
            [ExpressionType.Equal] = "=",
            [ExpressionType.NotEqual] = "<>",
            [ExpressionType.GreaterThan] = ">",
            [ExpressionType.LessThan] = "<",
            [ExpressionType.GreaterThanOrEqual] = ">=",
            [ExpressionType.LessThanOrEqual] = "<=",
            [ExpressionType.Modulo] = "%",
            [ExpressionType.Add] = "+",
            [ExpressionType.Subtract] = "-",
            [ExpressionType.Multiply] = "*",
            [ExpressionType.Divide] = "/",
            [ExpressionType.AndAlso] = "AND",
            [ExpressionType.And] = "AND",
            [ExpressionType.OrElse] = "OR",
            [ExpressionType.Or] = "OR"
        };

        public Restriction Translate<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            _baseType = typeof(TEntity);

            Visit(expression);

            return new(_sqlBuilder.ToString())
            {
                Parameters = _parameters,
            };
        }

        private void StartCondition()
        {
            _sqlBuilder.Append('(');
        }

        private void EndCondition()
        {
            _sqlBuilder.Append(')');
        }
    }

    static class ExpressionExtensions
    {
        private readonly static ExpressionType[] _mathOperators =
        [
            ExpressionType.Modulo,
            ExpressionType.Add,
            ExpressionType.Subtract,
            ExpressionType.Multiply,
            ExpressionType.Divide,
        ];

        internal static bool IsBaseTypePropertyGetterNode(this MemberExpression node)
        {
            if (node.Expression is null)
                return false;

            return node.Expression.NodeType == ExpressionType.Parameter;
        }

        internal static bool IsMemberAccess(this MemberExpression node)
        {
            return node.NodeType == ExpressionType.MemberAccess;
        }

        internal static bool IsClosureNode(this MemberExpression node)
        {
            if (node.Expression is null)
                return false;

            return
                node.Expression.NodeType == ExpressionType.Constant &&
                node.Expression.Type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false);
        }

        internal static bool IsConstantNull(this Expression expression)
        {
            if (expression.NodeType.In([.._mathOperators, ExpressionType.MemberAccess]))
                return false;

            if (expression is MemberExpression memberExpression && memberExpression.IsClosureNode())
                return false;

            return expression.GetExpressionValue() is null;
        }

        internal static object? GetExpressionValue(this Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
                return ((ConstantExpression)expression).Value;

            if (expression.NodeType == ExpressionType.Parameter)
                return ((MemberExpression)expression).ToGetterMethod().Invoke();

            if (expression.NodeType == ExpressionType.Convert)
                return ((UnaryExpression)expression).GetUnaryExpressionValue();

            return null;
        }

        internal static object? GetUnaryExpressionValue(this UnaryExpression expression)
        {
            if (expression.Operand is ConstantExpression constantOperandf)
                return constantOperandf.Value;

            var unaryExpression = (UnaryExpression)expression.Operand;

            if (unaryExpression.Operand is ConstantExpression constantOperand)
                return constantOperand.Value;

            if (unaryExpression.Operand is MemberExpression memberExpression && memberExpression.IsClosureNode())
                return memberExpression.ToGetterMethod().Invoke();

            return null;
        }

        internal static Func<object> ToGetterMethod(this MemberExpression node)
        {
            var objectMember = Expression.Convert(node, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            return getterLambda.Compile();
        }
    }
}
