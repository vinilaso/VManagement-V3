using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using VManagement.Commons.Entities;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Entities.Interfaces;
using VManagement.Database.Clauses;
using VManagement.Database.Command;
using VManagement.Database.Generalization;

namespace VManagement.Database.DAO
{
    /// <summary>
    /// Fornece uma implementação genérica do padrão Data Access Object (DAO) para operações de CRUD (Create, Read, Update, Delete).
    /// </summary>
    /// <remarks>
    /// Esta classe é responsável por traduzir chamadas de método em comandos SQL, executar esses comandos e mapear os resultados
    /// de volta para instâncias de <typeparamref name="TEntity"/>. Ela depende de uma <see cref="IConnectionFactory"/>
    /// para gerenciar as conexões com o banco de dados, permitindo o desacoplamento da infraestrutura de banco de dados e
    /// facilitando os testes unitários através da injeção de dependência.
    /// </remarks>
    /// <typeparam name="TEntity">O tipo da entidade para a qual este DAO irá gerenciar a persistência.</typeparam>
    public class TableEntityDAO<TEntity> : ITableEntityDAO<TEntity> where TEntity : class, ITableEntity, new()
    {
        private readonly IEnumerable<PropertyInfo> _properties;
        private readonly IConnectionFactory _connectionFactory;

        /// <summary>
        /// Inicia uma nova instância do DAO para um tipo de entidade específico.
        /// </summary>
        /// <param name="connectionFactory">A fábrica responsável por criar e gerenciar conexões com o banco de dados.</param>
        public TableEntityDAO(IConnectionFactory connectionFactory)
        {
            _properties = TableEntityHelper<TEntity>.GetColumnProperties();
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Insere uma nova entidade no banco de dados.
        /// </summary>
        /// <param name="entity">A instância da entidade a ser inserida.</param>
        /// <returns>O ID (chave primária) do novo registro inserido.</returns>
        public long Insert(TEntity entity)
        {
            using IVManagementConnection connection = _connectionFactory.CreateConnection();
            IVManagementCommand command = connection.CreateCommand();

            CommandBuilderOptions options = new()
            {
                AutoGenerateRestriction = true,
                PopulateCommandObject = true,
                AppendExistingRestriction = false
            };

            CommandBuilder<TEntity> commandBuilder = new(options, entity: entity, command: command);
            commandBuilder.BuildInsertCommand();

            return command.ExecuteScalar<long>();
        }

        /// <summary>
        /// Busca a primeira entidade no banco de dados que corresponde à restrição fornecida.
        /// </summary>
        /// <param name="restriction">Os critérios (cláusula WHERE e ORDER BY) para a busca.</param>
        /// <returns>Uma instância de <typeparamref name="TEntity"/> se um registro for encontrado; caso contrário, <see langword="null"/>.</returns>
        public TEntity? Select(Restriction restriction)
        {
            using IVManagementConnection connection = _connectionFactory.CreateConnection();
            IVManagementCommand command = connection.CreateCommand();

            CommandBuilderOptions options = new()
            {
                AppendExistingRestriction = true,
                AutoGenerateRestriction = false,
                PopulateCommandObject = true
            };

            CommandBuilder<TEntity> commandBuilder = new(options, preRestriction: restriction, command: command);
            commandBuilder.BuildSelectCommand();

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
                return TEntityFromDataReader(reader);

            return default;
        }

        /// <summary>
        /// Busca todas as entidades que correspondem à restrição e as retorna em uma coleção carregada em memória.
        /// </summary>
        /// <param name="restriction">Os critérios (cláusula WHERE e ORDER BY) para a busca.</param>
        /// <returns>Uma instância de <see cref="TableEntityCollection{TEntity}"/> contendo todas as entidades encontradas.</returns>
        public TableEntityCollection<TEntity> SelectMany(Restriction restriction)
        {
            TableEntityCollection<TEntity> collection = new();

            using IVManagementConnection connection = _connectionFactory.CreateConnection();
            IVManagementCommand command = connection.CreateCommand();

            CommandBuilderOptions options = new()
            {
                AppendExistingRestriction = true,
                AutoGenerateRestriction = false,
                PopulateCommandObject = true
            };

            CommandBuilder<TEntity> commandBuilder = new(options, preRestriction: restriction, command: command);
            commandBuilder.BuildSelectCommand();

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
                collection.Add(TEntityFromDataReader(reader));

            return collection;
        }

        /// <summary>
        /// Atualiza um registro existente no banco de dados com base nos dados da entidade fornecida.
        /// </summary>
        /// <param name="entity">A entidade contendo os dados a serem atualizados. O ID da entidade é usado para identificar o registro.</param>
        public void Update(TEntity entity)
        {
            using IVManagementConnection connection = _connectionFactory.CreateConnection();
            IVManagementCommand command = connection.CreateCommand();

            CommandBuilderOptions options = new()
            {
                AutoGenerateRestriction = true,
                PopulateCommandObject = true,
                AppendExistingRestriction = false
            };

            CommandBuilder<TEntity> commandBuilder = new(options, entity: entity, command: command);
            commandBuilder.BuildUpdateCommand();

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Exclui um registro do banco de dados com base no ID da entidade fornecida.
        /// </summary>
        /// <param name="entity">A entidade a ser excluída.</param>
        public void Delete(TEntity entity)
        {
            using IVManagementConnection connection = _connectionFactory.CreateConnection();
            IVManagementCommand command = connection.CreateCommand();

            CommandBuilderOptions options = new()
            {
                AutoGenerateRestriction = true,
                PopulateCommandObject = true,
                AppendExistingRestriction = false
            };

            CommandBuilder<TEntity> commandBuilder = new(options, entity: entity, command: command);
            commandBuilder.BuildDeleteCommand();

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Busca entidades que correspondem à restrição usando execução adiada (deferred execution).
        /// Ideal para processar grandes volumes de dados com baixo consumo de memória.
        /// </summary>
        /// <param name="restriction">Os critérios (cláusula WHERE e ORDER BY) para a busca.</param>
        /// <returns>Um <see cref="IEnumerable{TEntity}"/> que produz entidades uma a uma conforme são lidas do banco.</returns>
        public IEnumerable<TEntity> FetchMany(Restriction restriction)
        {
            using IVManagementConnection connection = _connectionFactory.CreateConnection();
            IVManagementCommand command = connection.CreateCommand();

            CommandBuilderOptions options = new()
            {
                AppendExistingRestriction = true,
                PopulateCommandObject = true,
                AutoGenerateRestriction = false
            };

            CommandBuilder<TEntity> commandBuilder = new(options, preRestriction: restriction, command: command);
            commandBuilder.BuildSelectCommand();

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
                yield return TEntityFromDataReader(reader);
        }

        /// <summary>
        /// Verifica se existem tuplas no banco de dados que atendem à restrição informada.
        /// </summary>
        /// <param name="restriction">Os critérios (cláusula WHERE e ORDER BY) para a busca.</param>
        /// <returns><see langword="true"/>, caso ao menos um registro seja encontrado. Caso contrário, <see langword="false"/>.</returns>
        public bool Exists(Restriction restriction)
        {
            using IVManagementConnection connection = _connectionFactory.CreateConnection();
            IVManagementCommand command = connection.CreateCommand();

            CommandBuilderOptions options = new()
            {
                AppendExistingRestriction = true,
                PopulateCommandObject = true,
                AutoGenerateRestriction = false
            };

            CommandBuilder<TEntity> commandBuilder = new(options, preRestriction: restriction, command: command);
            commandBuilder.BuildExistsCommand();

            return command.ExecuteScalar<int>() > 0;
        }

        private TEntity TEntityFromDataReader(SqlDataReader reader)
        {
            TEntity entity = TableEntityFactory.CreateInstanceFor<TEntity>();

            foreach (PropertyInfo propertyInfo in _properties)
            {
                EntityColumnNameAttribute columnAttribute = propertyInfo.GetCustomAttribute<EntityColumnNameAttribute>()!;
                
                object? valueFromDb = GetValueFromReader(propertyInfo.PropertyType, reader, columnAttribute.ColumnName);

                propertyInfo.SetValue(entity, valueFromDb);
            }

            return entity;
        }

        private object? GetValueFromReader(Type propertyType, SqlDataReader reader, string columnName)
        {
            if (reader[columnName] == DBNull.Value)
                return null;

            if (propertyType == typeof(bool?))
                return reader.GetBoolean(columnName);

            return reader[columnName];
        }
    }
}
