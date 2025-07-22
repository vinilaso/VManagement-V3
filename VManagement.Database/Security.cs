using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;
using VManagement.Commons.Files;

[assembly: InternalsVisibleTo("VManagement.Console")]
namespace VManagement.Database
{
    /// <summary>
    /// Oferece operações relacionadas ao gerenciamento do sistema.
    /// </summary>
    public static class Security
    {
        private static string _configurationFilePath = string.Empty;
        private static string _runningExePath = string.Empty;
        private static string _assemblyName = string.Empty;

        /// <summary>
        /// O caminho do arquivo em execução do aplicativo.
        /// </summary>
        internal static string RunningExePath => _runningExePath;

        static Security()
        {
            SetRunningExePath();
        }

        private static void SetRunningExePath()
        {
            _runningExePath = Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Define o caminho do arquivo de configurações utilizado pelo sistema.
        /// </summary>
        /// <param name="path">O caminho do arquivo de configurações.</param>
        /// <exception cref="FileNotFoundException">Lançada quando o caminho informado é inválido.</exception>
        internal static void SetConfigurationStringFilePath(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"The file {path} does not exist or is not accessible.");

            _configurationFilePath = path;
        }

        /// <summary>
        /// Busca a string de conexão com o banco de dados definida pelo arquivo de configurações do sistema.
        /// </summary>
        /// <returns>A string de conexão com o banco de dados.</returns>
        internal static string GetConnectionString()
        {
            try
            {
                using FileStream connectionStringFile = File.OpenRead(_configurationFilePath);
                using StreamReader reader = new(connectionStringFile);

                string fileContent = reader.ReadToEnd();

                if (string.IsNullOrEmpty(fileContent))
                    throw new InvalidDataException($"The content in the file {connectionStringFile} is empty.");

                IniFile configurationFile = new(_configurationFilePath);

                SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = configurationFile.Read("DataSource", "Database"),
                    InitialCatalog = configurationFile.Read("InitialCatalog", "Database"),
                    Password = configurationFile.Read("Password", "Database"),
                    UserID = configurationFile.Read("UserID", "Database"),
                    Pooling = true,
                    TrustServerCertificate = true
                };

                return connectionStringBuilder.ToString();
            }
            catch
            {
                throw;
            }
        }
    }
}
