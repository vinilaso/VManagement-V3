using VManagement.Commons.Entities.Interfaces;

namespace VManagement.Commons.Entities
{
    /// <summary>
    /// Representa uma coleção fortemente tipada de entidades de tabela.
    /// </summary>
    /// <remarks>
    /// Esta classe herda de <see cref="List{T}"/> e fornece métodos de conveniência
    /// para trabalhar com coleções de objetos que implementam <see cref="ITableEntity"/>.
    /// </remarks>
    /// <typeparam name="TEntity">O tipo da entidade contida na coleção.</typeparam>
    public class TableEntityCollection<TEntity> : List<TEntity> where TEntity : ITableEntity
    {
        /// <summary>
        /// Extrai os IDs de todas as entidades na coleção e os retorna em uma nova lista.
        /// </summary>
        /// <remarks>
        /// Entidades nulas dentro da coleção são ignoradas.
        /// </remarks>
        /// <returns>Uma <see cref="List{T}"/> de <see cref="long"/> contendo os IDs das entidades.</returns>
        public List<long?> ToIdList()
        {
            return this.Where(item => item is not null).Select(item => item.Id).ToList();
        }
    }
}
