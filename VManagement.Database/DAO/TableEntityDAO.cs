using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using VManagement.Commons.Entities;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Entities.Interfaces;
using VManagement.Database.Clauses;
using VManagement.Database.Command;
using VManagement.Database.Generalization;
using VManagement.Database.Linq;
using VManagement.Database.Linq.SqlServer;

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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public TSelector? Select<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction)
        {
            SelectPropertiesExpressionVisitor visitor = new();
            visitor.Visit(selector);

            using IVManagementConnection connection = _connectionFactory.CreateConnection();
            IVManagementCommand command = connection.CreateCommand();

            CommandBuilderOptions options = new()
            {
                AppendExistingRestriction = true,
                AutoGenerateRestriction = false,
                PopulateCommandObject = true,
                FetchPredefinedColumns = true
            };

            CommandBuilder<TEntity> commandBuilder = new(options, preRestriction: restriction, command: command, predefinedSelectColumns: visitor.ColumnNames);
            commandBuilder.BuildSelectCommand();

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
                return default;

            var construct = selector.Compile();
            
            return construct(TEntityPOCOFromDataReader(reader, visitor.Properties));
        }

        /// <inheritdoc/>
        public TEntity? Select(Expression<Func<TEntity, bool>> predicate)
        {
            SqlServerVisitor visitor = new();
            Restriction restritction = visitor.Translate(predicate);

            return Select(restritction);
        }

        /// <inheritdoc/>
        public TSelector? Select<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            SqlServerVisitor visitor = new();
            Restriction restritction = visitor.Translate(predicate);

            return Select(selector, restritction);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public IEnumerable<TSelector> SelectMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction)
        {
            List<TSelector> result = [];

            SelectPropertiesExpressionVisitor visitor = new();
            visitor.Visit(selector);

            using IVManagementConnection connection = _connectionFactory.CreateConnection();
            IVManagementCommand command = connection.CreateCommand();

            CommandBuilderOptions options = new()
            {
                AppendExistingRestriction = true,
                AutoGenerateRestriction = false,
                PopulateCommandObject = true,
                FetchPredefinedColumns = true
            };

            CommandBuilder<TEntity> commandBuilder = new(options, preRestriction: restriction, command: command, predefinedSelectColumns: visitor.ColumnNames);
            commandBuilder.BuildSelectCommand();

            using SqlDataReader reader = command.ExecuteReader();
            var construct = selector.Compile();

            while (reader.Read())
                result.Add(construct(TEntityPOCOFromDataReader(reader, visitor.Properties)));
            
            return result;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public IEnumerable<TSelector> FetchMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction)
        {
            SelectPropertiesExpressionVisitor visitor = new();
            visitor.Visit(selector);

            using IVManagementConnection connection = _connectionFactory.CreateConnection();
            IVManagementCommand command = connection.CreateCommand();

            CommandBuilderOptions options = new()
            {
                AppendExistingRestriction = true,
                AutoGenerateRestriction = false,
                PopulateCommandObject = true,
                FetchPredefinedColumns = true
            };

            CommandBuilder<TEntity> commandBuilder = new(options, preRestriction: restriction, command: command, predefinedSelectColumns: visitor.ColumnNames);
            commandBuilder.BuildSelectCommand();

            using SqlDataReader reader = command.ExecuteReader();
            var construct = selector.Compile();

            while (reader.Read())
                yield return construct(TEntityPOCOFromDataReader(reader, visitor.Properties));
        }

        /// <inheritdoc/>
        public IEnumerable<TSelector> SelectMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            SqlServerVisitor visitor = new();
            Restriction restritction = visitor.Translate(predicate);

            return SelectMany(selector, restritction);
        }

        /// <inheritdoc/>
        public IEnumerable<TEntity> FetchMany(Expression<Func<TEntity, bool>> predicate)
        {
            SqlServerVisitor visitor = new();
            Restriction restritction = visitor.Translate(predicate);

            foreach (TEntity entity in FetchMany(restritction))
                yield return entity;
        }

        /// <inheritdoc/>
        public IEnumerable<TSelector> FetchMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            SqlServerVisitor visitor = new();
            Restriction restritction = visitor.Translate(predicate);

            foreach (TSelector projection in FetchMany(selector, restritction))
                yield return projection;
        }

        /// <inheritdoc/>
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

        private static TEntity TEntityPOCOFromDataReader(SqlDataReader reader, IEnumerable<PropertyInfo> properties)
        {
            TEntity entity = new();

            foreach (PropertyInfo propertyInfo in properties)
            {
                EntityColumnNameAttribute columnAttribute = propertyInfo.GetCustomAttribute<EntityColumnNameAttribute>()!;

                object? valueFromDb = GetValueFromReader(propertyInfo.PropertyType, reader, columnAttribute.ColumnName);

                propertyInfo.SetValue(entity, valueFromDb);
            }

            return entity;
        }

        private static object? GetValueFromReader(Type propertyType, SqlDataReader reader, string columnName)
        {
            if (reader[columnName] == DBNull.Value)
                return null;

            if (propertyType == typeof(bool?))
                return reader.GetBoolean(columnName);

            return reader[columnName];
        }
    }
}
