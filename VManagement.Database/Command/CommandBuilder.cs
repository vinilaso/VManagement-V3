using System.Reflection;
using System.Runtime.CompilerServices;
using VManagement.Commons.Entities;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Entities.Interfaces;
using VManagement.Commons.Exceptions;
using VManagement.Commons.Utility;
using VManagement.Commons.Utility.Extensions;
using VManagement.Database.Clauses;
using VManagement.Database.Exceptions;
using VManagement.Database.Generalization;

[assembly: InternalsVisibleTo("VManagement.Database.Tests")]
namespace VManagement.Database.Command
{
    /// <summary>
    /// Oferece um mecanismo de construção de comandos SQL para as classes do sistema.
    /// Só é possível utilizar classes que tenham o atributo <see cref="TableEntityAttribute"/> em sua definição.
    /// </summary>
    /// <typeparam name="TTableEntity">A classe que será utilizada para criar os comandos.</typeparam>
    internal class CommandBuilder<TTableEntity> where TTableEntity : ITableEntity, new()
    {
        private readonly TTableEntity? _entity;
        private readonly Restriction? _preRestriction;
        private readonly IVManagementCommand? _command;
        private readonly CommandBuilderOptions _options;

        /// <summary>
        /// O nome da tabela do banco de dados na qual o comando será construído.
        /// </summary>
        internal readonly string TableName;

        /// <summary>
        /// Inicia uma instância de <see cref="CommandBuilder{TTableEntity}"/>.
        /// Caso <typeparamref name="TTableEntity"/> não tenha o atributo <see cref="TableEntityAttribute"/> em sua definição, uma exceção é lançada.
        /// </summary>
        /// <param name="options">As opções que definem o comportamento do construtor de comandos.</param>
        /// <param name="entity">A entidade utilizada para construir os comandos. Pode ser nula para operações que não dependem de uma entidade específica.</param>
        /// <param name="preRestriction">Uma restrição pré-definida a ser adicionada às restrições geradas.</param>
        /// <param name="command">O comando a ser populado com o texto e os parâmetros gerados, caso a opção 'PopulateCommandObject' seja verdadeira.</param>
        /// <exception cref="NotTableEntityException">Lançada caso <typeparamref name="TTableEntity"/> não possua o atributo <see cref="TableEntityAttribute"/>.</exception>
        internal CommandBuilder(CommandBuilderOptions options, TTableEntity? entity = default, Restriction? preRestriction = null, IVManagementCommand? command = null)
        {
            ValidateGenericParam();
            TableName = TableEntityHelper<TTableEntity>.GetTableName();
            _options = options;
            _entity = entity;
            _preRestriction = preRestriction;
            _command = command;
        }

        /// <summary>
        /// Constrói uma cláusula SELECT para a entidade desta instância.
        /// </summary>
        /// <returns>Uma instância de <see cref="CommandBuilderResult"/> contendo o texto do comando e os parâmetros da restrição.</returns>
        internal CommandBuilderResult BuildSelectCommand()
        {
            DelimitedStringBuilder fieldsBuilder = new(", ");
            Restriction restriction = CreateRestriction();

            foreach (PropertyInfo property in TableEntityHelper<TTableEntity>.GetColumnProperties())
                if (property.GetCustomAttribute<EntityColumnNameAttribute>() is EntityColumnNameAttribute attribute)
                    fieldsBuilder.Append(attribute.ColumnName);

            string commandText = $"SELECT {fieldsBuilder} FROM {TableName} {_options.MainTableAlias} {restriction}";

            PopulateCommand(commandText, restriction);

            return new CommandBuilderResult(commandText, restriction);
        }

        /// <summary>
        /// Constrói uma cláusula UPDATE para a entidade desta instância.
        /// </summary>
        /// <returns>Uma instância de <see cref="CommandBuilderResult"/> contendo o texto do comando e os parâmetros da restrição.</returns>
        internal CommandBuilderResult BuildUpdateCommand()
        {
            DelimitedStringBuilder fieldsBuilder = new(", ");
            Restriction restriction = CreateRestriction(withAlias: false);

            foreach (PropertyInfo property in TableEntityHelper<TTableEntity>.GetColumnProperties())
            {
                if (property.GetCustomAttribute<EntityColumnNameAttribute>() is EntityColumnNameAttribute attribute)
                {
                    if (attribute.ColumnName == "ID")
                        continue;

                    if (_entity is not null && !_entity.TrackedFields[attribute.ColumnName].Changed)
                        continue;

                    string
                        columnName = attribute.ColumnName,
                        parameterName = ParameterNameFactory.NewParameter(columnName);

                    fieldsBuilder.Append($"{columnName} = {parameterName}");

                    restriction.Parameters.Add(parameterName, property.GetValue(_entity, null));
                }
            }

            string commandText = $"UPDATE {TableName} SET {fieldsBuilder} {restriction}";

            PopulateCommand(commandText, restriction);

            return new CommandBuilderResult(commandText, restriction);
        }

        /// <summary>
        /// Constrói uma cláusula INSERT para a entidade desta instância.
        /// </summary>
        /// <returns>Uma instância de <see cref="CommandBuilderResult"/> contendo o texto do comando e os parâmetros da restrição.</returns>
        internal CommandBuilderResult BuildInsertCommand()
        {
            DelimitedStringBuilder fieldsBuilder = new(", ");
            DelimitedStringBuilder valuesBuilder = new(", ");
            Restriction restriction = Restriction.Empty;

            foreach (PropertyInfo property in TableEntityHelper<TTableEntity>.GetColumnProperties())
            {
                if (property.GetCustomAttribute<EntityColumnNameAttribute>() is EntityColumnNameAttribute attribute)
                {
                    if (attribute.ColumnName == "ID")
                        continue;

                    string
                        columnName = attribute.ColumnName,
                        parameterName = ParameterNameFactory.NewParameter(attribute.ColumnName);

                    fieldsBuilder.Append(columnName);
                    valuesBuilder.Append(parameterName);

                    restriction.Parameters.Add(parameterName, property.GetValue(_entity, null));
                }
            }

            string commandText = $"INSERT INTO {TableName} ({fieldsBuilder}) OUTPUT INSERTED.ID VALUES ({valuesBuilder})";

            PopulateCommand(commandText, restriction);

            return new CommandBuilderResult(commandText, restriction);
        }

        /// <summary>
        /// Constrói uma cláusula DELETE para a entidade desta instância.
        /// </summary>
        /// <returns>Uma instância de <see cref="CommandBuilderResult"/> contendo o texto do comando e os parâmetros da restrição.</returns>
        internal CommandBuilderResult BuildDeleteCommand()
        {
            Restriction restriction = CreateRestriction(withAlias: false);
            string commandText = $"DELETE FROM {TableName} {restriction}";

            PopulateCommand(commandText, restriction);

            return new CommandBuilderResult(commandText, restriction);
        }

        private Restriction CreateRestriction(bool withAlias = true)
        {
            Restriction restriction = Restriction.Empty;

            if (_options.AutoGenerateRestriction)
            {
                if (_entity is null)
                    throw CommandBuilderAbortedException.InvalidEntity;

                restriction = Restriction.FromId(_entity.Id, withAlias, _options.MainTableAlias);
            }

            if (_options.AppendExistingRestriction && _preRestriction is not null)
            {
                restriction.Append(_preRestriction);
            }                    
            
            return restriction;
        }

        private void PopulateCommand(string commandText, Restriction restriction)
        {
            if (!_options.PopulateCommandObject)
                return;

            if (_command is null)
                throw CommandBuilderAbortedException.InvalidCommand;

            _command.CommandText = commandText;
            _command.AddParameters(restriction.Parameters);
        }

        /// <summary>
        /// Valida se <typeparamref name="TTableEntity"/> informado possui o atributo <see cref="TableEntityAttribute"/> em sua definição.
        /// </summary>
        /// <exception cref="NotTableEntityException"></exception>
        private static void ValidateGenericParam()
        {
            if (!Attribute.IsDefined(typeof(TTableEntity), typeof(TableEntityAttribute)))
                throw new NotTableEntityException(typeof(TTableEntity).FullName.GetValueOrDefault("tipo indefinido"));
        }
    }

    /// <summary>
    /// Representa o resultado da construção de comandos do objeto <see cref="CommandBuilder{TTableEntity}"/>
    /// </summary>
    internal class CommandBuilderResult
    {
        /// <summary>
        /// O texto do comando SQL criado.
        /// </summary>
        internal string CommandText { get; private set; } = string.Empty;

        /// <summary>
        /// A restrição (cláusula WHERE e parâmetros) atribuída ao comando gerado.
        /// </summary>
        internal Restriction Restriction { get; private set; } = new();

        /// <summary>
        /// Inicia uma instância de <see cref="CommandBuilderResult"/>.
        /// </summary>
        /// <param name="commandText">O texto do comando SQL.</param>
        /// <param name="restriction">A restrição associada ao comando.</param>
        internal CommandBuilderResult(string commandText, Restriction? restriction = null)
        {
            CommandText = commandText;
            Restriction = restriction ?? new Restriction();
        }
    }

    /// <summary>
    /// Define as opções de configuração para o comportamento de um <see cref="CommandBuilder{TTableEntity}"/>.
    /// </summary>
    internal class CommandBuilderOptions
    {
        /// <summary>
        /// Obtém ou define um valor que indica se uma restrição WHERE deve ser gerada automaticamente com base no ID da entidade.
        /// O padrão é <see langword="true"/>.
        /// </summary>
        internal bool AutoGenerateRestriction { get; set; } = true;

        /// <summary>
        /// Obtém ou define um valor que indica se o objeto de comando (<see cref="IVManagementCommand"/>) passado para o CommandBuilder deve ser populado com o texto e os parâmetros gerados.
        /// O padrão é <see langword="true"/>.
        /// </summary>
        internal bool PopulateCommandObject { get; set; } = true;

        internal bool AppendExistingRestriction { get; set; } = false;

        /// <summary>
        /// Obtém ou define o alias (apelido) a ser usado para a tabela principal na consulta SQL.
        /// O padrão é "V".
        /// </summary>
        internal string MainTableAlias { get; set; } = "V";
    }
}