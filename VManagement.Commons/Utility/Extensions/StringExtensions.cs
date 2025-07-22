namespace VManagement.Commons.Utility.Extensions
{
    /// <summary>
    /// Oferece uma série de funcionalidades para objetos anuláveis.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Resgata o valor do <see cref="string?"/>, caso não seja nulo.
        /// Do contrário, retorna o valor fornecido como padrão.
        /// </summary>
        /// <param name="value">O valor a ser aplicado.</param>
        /// <param name="defaultValue">O valor a ser retornado quando value for null.</param>
        public static string GetValueOrDefault(this string? value, string defaultValue = "")
        {
            if (value == null)
                return defaultValue;

            return value;
        }

        /// <summary>
        /// Envolve a string recebida com parêntesis.
        /// </summary>
        /// <param name="value">A string que será envolvida.</param>
        /// <returns>A string envolvida com parêntesis.</returns>
        public static string BetweenParenthesis(this string value)
        {
            return $"({value})";
        }
    }
}
