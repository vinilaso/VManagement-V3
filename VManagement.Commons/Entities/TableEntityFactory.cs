using System.Reflection;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Entities.Interfaces;

namespace VManagement.Commons.Entities
{
    /// <summary>
    /// Fornece métodos estáticos de fábrica para criar e inspecionar instâncias de entidades.
    /// </summary>
    /// <remarks>
    /// Esta classe utiliza reflexão (reflection) para analisar os atributos das entidades
    /// e extrair metadados, como os nomes das colunas mapeadas.
    /// </remarks>
    public class TableEntityFactory
    {
        /// <summary>
        /// Cria uma nova instância de uma entidade.
        /// </summary>
        /// <remarks>
        /// Este método simplesmente invoca o construtor sem parâmetros da entidade.
        /// A inicialização de coleções internas, como a de campos rastreáveis, é de responsabilidade da própria entidade.
        /// </remarks>
        /// <typeparam name="TEntity">O tipo da entidade a ser criada, que deve implementar <see cref="ITableEntity"/> e ter um construtor sem parâmetros.</typeparam>
        /// <returns>Uma nova instância de <typeparamref name="TEntity"/>.</returns>
        public static TEntity CreateInstanceFor<TEntity>() where TEntity : ITableEntity, new()
        {
            return new();
        }

        /// <summary>
        /// Obtém todas as propriedades de uma entidade que estão marcadas com o atributo <see cref="EntityColumnNameAttribute"/>.
        /// </summary>
        /// <typeparam name="TEntity">O tipo da entidade a ser analisada.</typeparam>
        /// <returns>Uma enumeração das propriedades que representam colunas da tabela.</returns>
        public static IEnumerable<PropertyInfo> GetColumnProperties<TEntity>()
        {
            return typeof(TEntity)
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(EntityColumnNameAttribute)));
        }

        /// <summary>
        /// Obtém os nomes de todas as colunas de uma entidade, com base nas suas propriedades marcadas com o atributo <see cref="EntityColumnNameAttribute"/>.
        /// </summary>
        /// <typeparam name="TEntity">O tipo da entidade a ser analisada.</typeparam>
        /// <returns>Uma enumeração dos nomes das colunas.</returns>
        public static IEnumerable<string> GetColumnNames<TEntity>()
        {
            return GetColumnProperties<TEntity>()
                .Select(prop => prop.GetCustomAttribute<EntityColumnNameAttribute>())
                .Where(attr => attr != null)
                .Select(attr => attr!.ColumnName);
        }
    }
}