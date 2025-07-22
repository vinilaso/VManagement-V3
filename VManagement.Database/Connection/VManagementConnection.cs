using Microsoft.Data.SqlClient;

namespace VManagement.Database.Connection
{
    /// <summary>
    /// Representa uma conexão com o banco de dados.
    /// </summary>
    internal sealed class VManagementConnection : IDisposable
    {
        private readonly SqlConnection _connection;
        private readonly bool _ownsConnection;

        /// <summary>
        /// Inicia uma instância de <see cref="VManagementConnection"/>, 
        /// levando em consideração a transação aberta no contexto.
        /// </summary>
        internal VManagementConnection()
        {
            if (TransactionScopeManager.Current != null)
            {
                _connection = TransactionScopeManager.Current.Connection;
                _ownsConnection = false;
            }
            else
            {
                _connection = new SqlConnection(Security.GetConnectionString());
                _connection.Open();
                _ownsConnection = true;
            }
        }

        /// <summary>
        /// Cria um comando com a conexão atual.
        /// </summary>
        /// <returns>Uma instância de <see cref="VManagementCommand"/> com a conexão atual.</returns>
        internal VManagementCommand CreateCommand()
        {
            return new VManagementCommand(_connection);
        }

        /// <summary>
        /// Finaliza a conexão.
        /// </summary>
        public void Dispose()
        {
            if (_ownsConnection)
            {
                _connection.Dispose();
            }
        }
    }
}
