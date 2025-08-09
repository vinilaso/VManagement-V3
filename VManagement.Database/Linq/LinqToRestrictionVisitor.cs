using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Utility.Extensions;
using VManagement.Database.Clauses;
using VManagement.Database.Command;

namespace VManagement.Database.Linq
{
    public class LinqToRestrictionVisitor : ExpressionVisitor
    {
        public Type BaseType { get; private set; }
        public readonly StringBuilder _sqlBuilder = new();
        public readonly ParameterCollection _parameters = [];
        private const string _alias = "V";

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
            [ExpressionType.OrElse] = "OR"
        };

        public Restriction Translate<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            BaseType = typeof(TEntity);

            Visit(expression);

            return new(_sqlBuilder.ToString())
            {
                Parameters = _parameters
            };
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.MemberType is not MemberTypes.Property)
                return base.VisitMember(node);

            if (node.Member.DeclaringType != BaseType)
            {
                var objectMember = Expression.Convert(node, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                return VisitConstant(Expression.Constant(getter()));
            }
            else
            {
                if (node.Member.GetCustomAttribute<EntityColumnNameAttribute>() is EntityColumnNameAttribute attr)
                    _sqlBuilder.Append(attr.ColumnName.WithSqlAlias());
                else
                    throw new NotSupportedException($"Só é possível traduzir para SQL propriedades decoradas com {nameof(EntityColumnNameAttribute)}.");
            }

            return node;
        }

        //protected override expression visitbinary(binaryexpression node)
        //{
        //    _sqlbuilder.append('(');

        //    visit(node.left);

        //    bool isnullcheck = isconstantnull(node.right);

        //    if (node.nodetype.in(expressiontype.equal, expressiontype.notequal) && isnullcheck)
        //    {
        //        string nullcheck = node.nodetype == expressiontype.equal ? " is null" : " is not null";
        //        _sqlbuilder.append(nullcheck);
        //    }
        //    else
        //    {
        //        _sqlbuilder.append(" " + _logicaloperators[node.nodetype] + " ");
        //    }

        //    if (!isnullcheck)
        //    {
        //        visit(node.right);
        //    }

        //    _sqlbuilder.append(')');

        //    return node;
        //}

        //protected override Expression VisitConstant(ConstantExpression node)
        //{
        //    string parameterName = ParameterNameFactory.NewParameter("CONSTANT_EXPRESSION_NODE");
        //    _sqlBuilder.Append(parameterName);

        //    _parameters.Add(parameterName, node.Value);

        //    return node;
        //}

        private static bool IsConstantNull(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
                return ((ConstantExpression)expression).Value is null;

            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var objectMember = Expression.Convert((MemberExpression)expression, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();

                return getter() is null;
            }

            return false;
        }
    }
}
