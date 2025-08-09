using System.Linq.Expressions;
using System.Xml.Linq;
using VManagement.Commons.Utility.Enums;
using VManagement.Commons.Utility.Extensions;

namespace VManagement.Database.Linq.SqlServer
{
    public partial class SqlServerVisitor
    {
        /// <inheritdoc/>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(string))
            {
                VisitStringMethods(node);
            }
            else if (node.Method.DeclaringType == typeof(StringExtensions))
            {
                VisitStringExtensionsMethods(node);
            }

            return node;
        }

        private void ThrowMethodNotSupportedException(string methodName, Type declaringType, string additionalText = "")
        {
            throw new NotSupportedException($"O método {methodName} da classe {declaringType.FullName} não possui suporte para tradução SQL. {additionalText}");
        }

        #region >> System.String methods

        private void VisitStringMethods(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case nameof(string.ToUpper):
                    VisitStringToUpper(node);
                    break;

                case nameof(string.ToLower):
                    VisitStringToLower(node);
                    break;

                case nameof(string.IsNullOrEmpty):
                    VisitStringIsNullOrEmpty(node);
                    break;

                case nameof(string.Equals):
                    ThrowMethodNotSupportedException(node.Method.Name, typeof(string), $"Utilize o método {nameof(StringExtensions.Equals)} da classe {nameof(StringExtensions)}.");
                    break;

                case nameof(string.Contains):
                    ThrowMethodNotSupportedException(node.Method.Name, typeof(string), $"Utilize o método {nameof(StringExtensions.Contains)} da classe {nameof(StringExtensions)}.");
                    break;

                default:
                    ThrowMethodNotSupportedException(node.Method.Name, typeof(string));
                    break;
            }
        }

        private void VisitStringToUpper(MethodCallExpression node)
        {
            _sqlBuilder.Append("UPPER(");
            Visit(node.Object);
            _sqlBuilder.Append(')');
        }

        private void VisitStringToLower(MethodCallExpression node)
        {
            _sqlBuilder.Append("LOWER(");
            Visit(node.Object);
            _sqlBuilder.Append(')');
        }

        private void VisitStringIsNullOrEmpty(MethodCallExpression node)
        {
            Expression argument = node.Arguments[0];

            BinaryExpression nullCheck = Expression.Equal(argument, Expression.Constant(null));
            BinaryExpression emptyCheck = Expression.Equal(argument, Expression.Constant(string.Empty));
            BinaryExpression finalExpression = Expression.OrElse(nullCheck, emptyCheck);

            Visit(finalExpression);
        }

        #endregion

        #region >> VManagement.Commons.Utility.Extensions.StringExtensions methods

        private void VisitStringExtensionsMethods(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case nameof(StringExtensions.Equals):
                    VisitStringExtensionsCollateEquals(node);
                    break;

                case nameof(StringExtensions.Contains):
                    VisitStringLikeMethod(node, prefix: '%', suffix: '%');
                    break;

                case nameof(StringExtensions.StartsWith):
                    VisitStringLikeMethod(node, suffix: '%');
                    break;

                case nameof(StringExtensions.EndsWith):
                    VisitStringLikeMethod(node, prefix: '%');
                    break;

                default:
                    ThrowMethodNotSupportedException(node.Method.Name, typeof(StringExtensions));
                    break;
            }
        }

        private void VisitStringExtensionsCollateEquals(MethodCallExpression node)
        {
            StartCondition();

            Expression
                firstString = node.Arguments[0],
                secondString = node.Arguments[1];

            Visit(firstString);

            AppendLogicalOperator(ExpressionType.Equal);

            Visit(secondString);

            AppendCollation(node.Arguments[2]);

            EndCondition();
        }

        private void VisitStringLikeMethod(MethodCallExpression node, char prefix = '\0', char suffix = '\0')
        {
            StartCondition();

            Visit(node.Arguments[0]);

            _sqlBuilder.Append(" LIKE ");

            if (node.Arguments[1].GetExpressionValue() is not string secondArgument)
                throw new NotSupportedException();

            VisitConstant(Expression.Constant(prefix + secondArgument + suffix));
            
            AppendCollation(node.Arguments[2]);

            EndCondition();
        }

        private void AppendCollation(Expression collationArgument)
        {
            Collations collation = GetCollationFromArguments(collationArgument);
            _sqlBuilder.Append(" COLLATE ");
            _sqlBuilder.Append(collation.GetCollationValue());
        }

        private static Collations GetCollationFromArguments(Expression argument)
        {
#pragma warning disable CS8605

            if (argument is ConstantExpression constant)
                return (Collations)constant.Value;

#pragma warning restore CS8605

            if (argument is MemberExpression member)
            {
                Func<object> getter = member.ToGetterMethod();
                return (Collations)getter();
            }

            throw new NotSupportedException($"O tipo {argument.GetType()} não é suportado.");
        }

        #endregion
    }
}
