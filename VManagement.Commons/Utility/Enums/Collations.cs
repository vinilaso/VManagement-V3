using System.Reflection;

namespace VManagement.Commons.Utility.Enums
{
    public enum Collations
    {
        /// <summary>
        /// Latim: Case Sensitive, Accent Sensitive
        /// </summary>
        [CollationValue("Latin1_General_CS_AS")]
        LATIN_CS_AS,

        /// <summary>
        /// Latin: Case Insensitive, Accent Sensitive
        /// </summary>
        [CollationValue("Latin1_General_CI_AS")]
        LATIN_CI_AS,

        /// <summary>
        /// Latim: Case Sensitive, Accent Insensitive 
        /// </summary>
        [CollationValue("Latin1_General_CS_AI")]
        LATIN_CS_AI,

        /// <summary>
        /// Latin: Case Insensitive, Accent Insensitive
        /// </summary>
        [CollationValue("Latin1_General_CI_AI")]
        LATIN_CI_AI
    }

    public class CollationValueAttribute(string collationValue) : Attribute
    {
        public string CollationValue { get; set; } = collationValue;
    }

    public static class CollationsExtensions
    {
        public static string GetCollationValue(this Collations collation)
        {
            var attr = typeof(Collations)
                .GetMember(collation.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<CollationValueAttribute>();

            if (attr is null)
                return string.Empty;

            return attr.CollationValue;
        }
    }
}
