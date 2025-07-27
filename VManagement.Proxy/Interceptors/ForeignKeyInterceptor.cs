using Castle.DynamicProxy;
using System.Reflection;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Utility.Extensions;
using VManagement.Database.Clauses;
using VManagement.Proxy.Attributes;

namespace VManagement.Proxy.Interceptors
{
    public class ForeignKeyInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (!ShouldIntercept(invocation, out Type? foreignKeyType, out string? columnName))
            {
                invocation.Proceed();
                return;
            }

            var findMethod = foreignKeyType.BaseType.GetMethod(
                "Find",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(long)],
                modifiers: null
            );

            var columnProp = foreignKeyType.GetProperties().Where(prop =>
            {
                var attribute = prop.GetCustomAttribute<EntityColumnNameAttribute>();

                if (attribute is null)
                    return false;

                return attribute.ColumnName == columnName;
            })
            .FirstOrDefault();

            var value = columnProp.GetValue(invocation.InvocationTarget);

            if (value is null)
            {
                invocation.ReturnValue = null;
            }
            else
            {
                invocation.ReturnValue = findMethod.Invoke(null, [value]);
            }
        }

        private static bool ShouldIntercept(IInvocation invocation, out Type? foreignKeyType, out string? columnName)
        {
            foreignKeyType = null;
            columnName = null;

            if (!(invocation.Method.Name.StartsWith("get_", StringComparison.OrdinalIgnoreCase) || invocation.Method.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase)))
                return false;

            if (invocation.Method.GetProperty() is not PropertyInfo property)
                return false;

            var customAttributes = property.GetCustomAttributes();

            foreach (var attr in customAttributes)
            {
                var type = attr.GetType();

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ForeignKeyAttribute<>))
                {
                    foreignKeyType = type.GetGenericArguments()[0];
                    columnName = type.GetProperty("ColumnName")?.GetValue(attr) as string;
                    return true;
                }
            }

            return false;
        }
    }
}
