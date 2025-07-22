using Microsoft.Data.SqlClient;
using VManagement.Database.Clauses;
using VManagement.Database.Generalization;

namespace VManagement.Database.Connection
{
    /// <summary>
    /// Representa um comando do banco de dados.
    /// </summary>
    public sealed class VManagementCommand : IVManagementCommand
    {
        private readonly SqlCommand _command;

        /// <summary>
        /// Resgata ou define o texto desta instância.
        /// </summary>
        public string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }

        /// <summary>
        /// Resgata os parâmetros SQL desta instância.
        /// </summary>
        public SqlParameterCollection Parameters
        {
            get => _command.Parameters;
        }

        /// <summary>
        /// Adiciona uma parâmetro à este comando.
        /// </summary>
        /// <param name="parameter">O parâmetro à ser adicionado.</param>
        public void AddParameter(SqlParameter parameter)
        {
            _command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Adiciona os parâmetros à este comando.
        /// </summary>
        /// <param name="parameters">Os parâmetros à serem adicionados.</param>
        public void AddParameters(ParameterCollection parameters)
        {
            parameters.ForEach(param => _command.Parameters.Add(param));
        }

        /// <summary>
        /// Inicia uma comando no banco de dados utilizando a conexão recebida.
        /// </summary>
        /// <param name="connection">A conexão utilizada para iniciar o comando.</param>
        internal VManagementCommand(SqlConnection connection)
        {
            _command = connection.CreateCommand();
            
            if (TransactionScopeManager.Current != null)
                _command.Transaction = TransactionScopeManager.Current.Transaction;
        }

        /// <summary>
        /// Executa um comando que não possui resultados.
        /// </summary>
        public void ExecuteNonQuery()
        {
            _command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executa um comando que possui resultados.
        /// </summary>
        /// <returns>Uma instância de <see cref="SqlDataReader"/> com os dados retornados.</returns>
        public SqlDataReader ExecuteReader()
        {
            return _command.ExecuteReader();
        }

        /// <summary>
        /// Executa um comando que possui resultados e retona a primeira linha da primeira coluna, realizando o cast para o tipo informado.
        /// As outras colunas são ignoradas.
        /// </summary>
        /// <typeparam name="TGeneric">O tipo para qual será realizado o cast.</typeparam>
        /// <returns>O primeiro valor retornado no comando.</returns>
        public TGeneric ExecuteScalar<TGeneric>()
        {
            return (TGeneric)_command.ExecuteScalar();
        }
    }
}
