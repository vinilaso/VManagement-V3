using System.Text.RegularExpressions;
using VManagement.Commons.Utility.Enums;

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

        /// <summary>
        /// Analisa uma string de formato e determina o número de argumentos únicos necessários.
        /// </summary>
        /// <param name="formatString">A string a ser analisada, como por exemplo "{0} {1} {0} {2}".</param>
        /// <returns>O número de argumentos únicos necessários.</returns>
        public static int GetRequiredParameterCount(this string formatString)
        {
            Regex regex = new(@"\{(\d+)\}");
            MatchCollection matches = regex.Matches(formatString);

            if (matches.Count == 0) 
                return 0;

            int maxIndex = matches.Cast<Match>()
                                  .Select(m => int.Parse(m.Groups[1].Value))
                                  .Max();

            return maxIndex + 1;
        }

        public static string WithSqlAlias(this string value, string alias = "V")
        {
            if (!alias.EndsWith('.'))
                alias += '.';

            return alias + value;
        }

        public static bool Equals(this string? value, string other, Collations collation)
        {
            throw new NotImplementedException("Este método é utilizado apenas para tradução de expressões LINQ para SQL. Para comparação de strings, utilize String.Equals().");
        }

        public static bool Contains(this string? value, string other, Collations collation)
        {
            throw new NotImplementedException("Este método é utilizado apenas para tradução de expressões LINQ para SQL. Utilize String.Contains().");
        }

        public static bool StartsWith(this string? value, string other, Collations collation)
        {
            throw new NotImplementedException("Este método é utilizado apenas para tradução de expressões LINQ para SQL. Utilize String.StartsWith().");
        }

        public static bool EndsWith(this string? value, string other, Collations collation)
        {
            throw new NotImplementedException("Este método é utilizado apenas para tradução de expressões LINQ para SQL. Utilize String.EndsWith().");
        }
    }
}
