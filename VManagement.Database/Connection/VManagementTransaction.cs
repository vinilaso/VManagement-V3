using Microsoft.Data.SqlClient;

namespace VManagement.Database.Connection
{
    /// <summary>
    /// Representa uma transação com o banco de dados.
    /// </summary>
    public sealed class VManagementTransaction : IDisposable
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;
        private bool _isCompleted = false;

        /// <summary>
        /// A conexão na qual a transação foi aberta.
        /// </summary>
        internal SqlConnection Connection => _connection;

        /// <summary>
        /// A transação com o banco de dados.
        /// </summary>
        internal SqlTransaction Transaction => _transaction;

        /// <summary>
        /// Inicia uma transação com o banco de dados.
        /// </summary>
        public VManagementTransaction()
        {
            _connection = new SqlConnection(Security.GetConnectionString());
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            TransactionScopeManager.Push(this);
        }

        /// <summary>
        /// Marca a transação como completa.
        /// </summary>
        public void Complete()
        {
            _isCompleted = true;
        }

        /// <summary>
        /// Encerra a transação, realizando o commit ou rollback.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_isCompleted)
                {
                    _transaction.Commit();
                }
                else
                {
                    _transaction.Rollback();
                }
            }
            finally
            {
                _transaction.Dispose();
                _connection.Dispose();

                TransactionScopeManager.Pop();
            }
        }
    }
}
