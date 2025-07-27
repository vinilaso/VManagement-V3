using System.Reflection;

namespace VManagement.Commons.Utility.Extensions
{
    public static class MethodInfoExtensions
    {
        public static PropertyInfo? GetProperty(this MethodInfo method)
        {
            bool
                takesArg = method.GetParameters().Length == 1,
                hasReturn = method.ReturnType != typeof(void);

            if (takesArg == hasReturn)
                return null;

            if (method.DeclaringType is null)
                return null;

            if (takesArg)
            {
                return method.DeclaringType
                    .GetProperties()
                    .Where(prop => prop.GetSetMethod() == method)
                    .FirstOrDefault();
            }
            else
            {
                return method.DeclaringType
                    .GetProperties()
                    .Where(prop => prop.GetGetMethod() == method)
                    .FirstOrDefault();
            }
        }
    }
}
