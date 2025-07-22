using System.Text;

namespace VManagement.Commons.Utility
{
    /// <summary>
    /// Fornece um mecanismo para a construção de strings delimitadas por um delimitador definido.
    /// </summary>
    public class DelimitedStringBuilder
    {
        private StringBuilder _builder;

        /// <summary>
        /// O delimitador utilizado para separar os textos anexados.
        /// </summary>
        public string Delimiter { get; } = string.Empty;

        /// <summary>
        /// Inicia uma instância de <see cref="DelimitedStringBuilder"/> com o delimitador especificado.
        /// </summary>
        /// <param name="delimiter">O delimitador desta instância.</param>
        public DelimitedStringBuilder(string delimiter)
        {
            _builder = new StringBuilder();
            Delimiter = delimiter;
        }

        /// <summary>
        /// Anexa um texto à esta instância. Caso algum texto já tenha sido anexado anteriormente,
        /// o delimitador é inserido.
        /// </summary>
        /// <param name="text">O texto a ser inserido.</param>
        /// <returns>Esta instância.</returns>
        public DelimitedStringBuilder Append(string text)
        {
            if (_builder.Length > 0)
                _builder.Append(Delimiter);

            _builder.Append(text);
            return this;
        }

        /// <summary>
        /// Anexa um texto à esta instância sem anexar o delimitador.
        /// </summary>
        /// <param name="text">O texto a ser inserido.</param>
        /// <returns>Esta instância.</returns>
        public DelimitedStringBuilder AppendRaw(string text)
        {
            _builder.Append(text);
            return this;
        }

        /// <summary>
        /// Remove todo o texto anexado à instância atual.
        /// </summary>
        public void Clear()
        {
            _builder.Clear();
        }

        /// <summary>
        /// Transforma esta instância em uma string.
        /// </summary>
        /// <returns>A string que representa esta instância.</returns>
        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
