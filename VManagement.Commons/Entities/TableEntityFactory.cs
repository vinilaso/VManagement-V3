using System.Reflection;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Entities.Interfaces;

namespace VManagement.Commons.Entities
{
    /// <summary>
    /// Fornece métodos estáticos de fábrica para criar e inspecionar instâncias de entidades.
    /// </summary>
    /// <remarks>
    /// Esta classe utiliza reflexão (reflection) para analisar os atributos das entidades,
    /// permitindo a criação dinâmica de instâncias com a coleção de campos rastreáveis (<see cref="ITrackedFieldCollection"/>)
    /// já inicializada. É uma peça fundamental para o funcionamento do padrão Active Record e do DAO.
    /// </remarks>
    public class TableEntityFactory
    {
        /// <summary>
        /// Cria uma nova instância de uma entidade e inicializa sua coleção de campos rastreáveis.
        /// </summary>
        /// <typeparam name="TEntity">O tipo da entidade a ser criada, que deve implementar <see cref="ITableEntity"/>.</typeparam>
        /// <returns>Uma nova instância de <typeparamref name="TEntity"/> com os campos rastreáveis configurados.</returns>
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