using VManagement.Database.Generalization;

namespace VManagement.Database.Connection
{
    internal class ConnectionFactory : IConnectionFactory
    {
        public static ConnectionFactory SqlServerConnectionFactory { get; } = new();

        public IVManagementConnection CreateConnection()
        {
            return new VManagementConnection();
        }
    }
}
