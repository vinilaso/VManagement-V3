using Microsoft.Data.SqlClient;
using VManagement.Database.Clauses;
using VManagement.Database.Generalization;

namespace VManagement.Database.Tests.TestEntities
{
    public class MockVManagementCommand : IVManagementCommand
    {
        private readonly SqlCommand _sqlCommand;

        public MockVManagementCommand()
        {
            _sqlCommand = new SqlCommand();
        }

        public string CommandText { get; set; } = string.Empty;

        public SqlParameterCollection Parameters
        {
            get => _sqlCommand.Parameters;
        }

        public void AddParameter(SqlParameter parameter)
        {
            _sqlCommand.Parameters.Add(parameter);
        }

        public void AddParameters(ParameterCollection parameters)
        {
            foreach (SqlParameter parameter in parameters)
                _sqlCommand.Parameters.Add(parameter);
        }
    }
}
