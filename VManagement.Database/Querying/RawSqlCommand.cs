using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using VManagement.Commons.Entities.Attributes;
using VManagement.Database.Clauses;
using VManagement.Database.Connection;
using VManagement.Database.Generalization;

namespace VManagement.Database.Querying
{
    public class RawSqlCommand
    {
        private string _commandText;
        private ParameterCollection _parameters;

        public string CommandText => _commandText;
        public ParameterCollection Parameters => _parameters;

        public RawSqlCommand(string commandText)
        {
            _commandText = commandText;
            _parameters = [];
        }

        public RawSqlCommand(string commandText, ParameterCollection parameters) : this(commandText)
        {
            _parameters = parameters;
        }

        public RawSqlCommand(string commandText, params SqlParameter[] parameters) : this(commandText)
        {
            _parameters = [..parameters];
        }

        public void ExecuteNonQuery()
        {
            using VManagementConnection connection = new();
            IVManagementCommand command = connection.CreateCommand();

            command.CommandText = _commandText;
            command.Parameters.AddRange([.._parameters]);

            command.ExecuteNonQuery();
        }

        public TResult? ParseFirstTo<TResult>() where TResult : new()
        {
            var resultProperties = typeof(TResult).GetProperties();

            using VManagementConnection connection = new();
            IVManagementCommand command = connection.CreateCommand();

            command.CommandText = _commandText;
            command.Parameters.AddRange([.. _parameters]);

            using var reader = command.ExecuteReader();

            if (reader.Read())
                return TResultFromDataReader<TResult>(reader, resultProperties);

            return default;
        }

        public IEnumerable<TResult> ParseManyTo<TResult>() where TResult : new()
        {
            var resultProperties = typeof(TResult).GetProperties();

            using VManagementConnection connection = new();
            IVManagementCommand command = connection.CreateCommand();

            command.CommandText = _commandText;
            command.Parameters.AddRange([.. _parameters]);

            using var reader = command.ExecuteReader();

            ICollection<TResult> results = [];

            while (reader.Read())
                 results.Add(TResultFromDataReader<TResult>(reader, resultProperties));

            return results;
        }


        private static TResult TResultFromDataReader<TResult>(SqlDataReader reader, IEnumerable<PropertyInfo> properties) where TResult : new()
        {
            TResult entity = new();

            foreach (PropertyInfo propertyInfo in properties)
            {
                object? valueFromDb = GetValueFromReader(propertyInfo.PropertyType, reader, propertyInfo.Name);

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
