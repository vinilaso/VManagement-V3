using System.Runtime.InteropServices;
using System.Text;
using VManagement.Commons.Utility.Extensions;

namespace VManagement.Commons.Files
{
    /// <summary>
    /// Representa um arquivo .ini e fornece mecanismos de leitura e escrita das propriedades.
    /// </summary>
    public class IniFile
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// O nome padrão para a seção do arquivo.
        /// </summary>
        public const string DefaultSectionKey = "AppDomain";

        /// <summary>
        /// O caminho para o arquivo.
        /// </summary>
        public string Path { get; } = string.Empty;

        /// <summary>
        /// Inicia uma instância de <see cref="IniFile"/> com o caminho passado como parâmetro.
        /// Caso o arquivo não exista, o cria.
        /// </summary>
        /// <param name="iniPath">O caminho a ser utilizado pela instância.</param>
        public IniFile(string iniPath)
        {
            if (!File.Exists(iniPath))
                File.Create(iniPath).Close();

            Path = new FileInfo(iniPath).FullName;
        }

        /// <summary>
        /// Lê o valor de uma chave em uma determinda seção.
        /// Caso não seja informada uma seção, o valor de <see cref="DefaultSectionKey"/> será utilizado
        /// </summary>
        /// <param name="key">A chave que terá o valor lido.</param>
        /// <param name="section">A seção que contém a chave.</param>
        /// <returns>O valor da chave informada.</returns>
        public string Read(string? key, string? section = null)
        {
            StringBuilder retVal = new(255);
            _ = GetPrivateProfileString(section.GetValueOrDefault(DefaultSectionKey), key.GetValueOrDefault(), string.Empty, retVal, 255, Path);
            return retVal.ToString();
        }

        /// <summary>
        /// Define um valor para a chave informada em uma determinada seção.
        /// Caso não seja informada uma seção, o valor de <see cref="DefaultSectionKey"/> será utilizado
        /// </summary>
        /// <param name="key">A chave que terá o valor lido.</param>
        /// <param name="value">O valor a ser escrito.</param>
        /// <param name="section">A seção que contém a chave.</param>
        public void Write(string? key, string? value, string? section = null)
        {
            WritePrivateProfileString(
                section.GetValueOrDefault(DefaultSectionKey), 
                key.GetValueOrDefault(), 
                value.GetValueOrDefault(), 
                Path
             );
        }

        /// <summary>
        /// Exclui uma chave de uma determinada seção.
        /// Caso não seja informada uma seção, o valor de <see cref="DefaultSectionKey"/> será utilizado
        /// </summary>
        /// <param name="key">A chave que será excluída.</param>
        /// <param name="section">A seção que contém a chave.</param>
        public void DeleteKey(string? key, string? section = null)
        {
            Write(key, null, section);
        }

        /// <summary>
        /// Exclui uma seção.
        /// Caso não seja informada uma seção, o valor de <see cref="DefaultSectionKey"/> será utilizado
        /// </summary>
        /// <param name="section">A seção que será excluída.</param>
        public void DeleteSection(string? section = null)
        {
            Write(null, null, section);
        }

        /// <summary>
        /// Verifica se determinada chave existe dentro de determinada seção.
        /// </summary>
        /// <param name="key">O nome da chave.</param>
        /// <param name="section">A seção que contém a chave.</param>
        /// <returns>
        ///     <see cref="true"/> caso a chave exista dentro da seção. Senão, <see cref="false"/>.
        /// </returns>
        public bool KeyExists(string? key, string? section)
        {
            return Read(key, section).Length > 0;
        }
    }
}
