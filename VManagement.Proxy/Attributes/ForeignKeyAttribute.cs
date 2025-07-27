using VManagement.Core.Entities;

namespace VManagement.Proxy.Attributes
{
    /// <summary>
    /// Especifica que uma propriedade de navegação representa um relacionamento de chave estrangeira.
    /// </summary>
    /// <remarks>
    /// Este atributo é a chave para habilitar o Lazy Loading. Ele deve ser aplicado a uma propriedade de navegação 'virtual'
    /// que representa a entidade relacionada. O interceptor do ORM usará as informações deste atributo para
    /// carregar a entidade relacionada sob demanda quando a propriedade for acessada pela primeira vez.
    /// </remarks>
    /// <example>
    /// <code>
    /// [ForeignKey&lt;Categoria&gt;("ID_CATEGORIA")]
    /// public virtual Categoria Categoria { get; set; }
    /// </code>
    /// </example>
    /// <typeparam name="TForeignKey">O tipo da entidade estrangeira com a qual esta propriedade se relaciona.</typeparam>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ForeignKeyAttribute<TForeignKey> : Attribute where TForeignKey : TableEntity<TForeignKey>, new()
    {
        /// <summary>
        /// Obtém ou define o nome da coluna na tabela atual que armazena o valor do ID da chave estrangeira.
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// Inicia uma nova instância do atributo <see cref="ForeignKeyAttribute{TForeignKey}"/>.
        /// </summary>
        /// <param name="columnName">O nome da coluna na tabela desta entidade que contém o ID da chave estrangeira.</param>
        public ForeignKeyAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
