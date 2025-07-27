using Castle.DynamicProxy;
using VManagement.Core.Entities;
using VManagement.Proxy.Interceptors;

namespace VManagement.Proxy
{
    public class ProxyFactory
    {
        private static ProxyGenerator _generator = new();

        public static TEntity CreateProxy<TEntity>() where TEntity : TableEntity<TEntity>, new()
        {
            IInterceptor[] interceptors = [new ForeignKeyInterceptor()];
            return _generator.CreateClassProxy<TEntity>(interceptors);
        }
    }
}
