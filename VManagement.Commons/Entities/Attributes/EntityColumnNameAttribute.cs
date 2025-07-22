namespace VManagement.Commons.Entities.Attributes
{
    /// <summary>
    /// Especifica qual coluna da tabela do banco de dados a propriedade representa.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityColumnNameAttribute : Attribute
    {
        /// <summary>
        /// O nome da coluna que a propriedade representa.
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// Inicia uma instância de <see cref="EntityColumnNameAttribute"/> para a coluna passada como parâmetro.
        /// </summary>
        /// <param name="columnName">O nome da coluna que a propriedade representa.</param>
        public EntityColumnNameAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
