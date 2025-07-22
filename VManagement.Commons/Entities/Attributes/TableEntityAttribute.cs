namespace VManagement.Commons.Entities.Attributes
{
    /// <summary>
    /// Especifica que a classe consumidora representará uma tabela do banco de dados.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableEntityAttribute : Attribute
    {
        /// <summary>
        /// O nome da tabela que a classe representa.
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// Inicia uma instância de <see cref="TableEntityAttribute"/> para a tabela passada como parâmetro.
        /// </summary>
        /// <param name="tableName">O nome da tabela que a classe representa.</param>
        public TableEntityAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
