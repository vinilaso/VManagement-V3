using Microsoft.Data.SqlClient;
using VManagement.Database.Clauses;

namespace VManagement.Database.Generalization
{
    public interface IVManagementCommand
    {
        string CommandText { get; set; }
        SqlParameterCollection Parameters { get; }
        void AddParameter(SqlParameter parameter);
        void AddParameters(ParameterCollection parameters);
    }
}
