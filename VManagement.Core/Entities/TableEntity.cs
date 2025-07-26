using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Entities.Interfaces;
using VManagement.Core.Exceptions;

namespace VManagement.Core.Entities
{
    /// <summary>
    /// Representa os estados do ciclo de vida de uma entidade de negócio.
    /// </summary>
    public enum EntityState
    {
        /// <summary>
        /// Indica que a entidade foi apenas instanciada.
        /// </summary>
        Initialized,

        /// <summary>
        /// Indica que a instância representa um novo registro no banco de dados.
        /// </summary>
        New,

        /// <summary>
        /// Indica que a instância é um registro carregado do banco de dados.
        /// </summary>
        Loaded,

        /// <summary>
        /// Indica que a instância representa um registro que não existe mais no banco.
        /// </summary>
        Deleted
    }

    /// <summary>
    /// Fornece uma classe base abstrata para entidades que representam tabelas de banco de dados,
    /// implementando o padrão Active Record e um ciclo de vida de entidade com eventos (hooks).
    /// </summary>
    /// <remarks>
    /// Esta classe utiliza o "Curiously Recurring Template Pattern" (CRTP), onde a classe derivada
    /// deve passar a si mesma como o parâmetro de tipo <typeparamref name="TEntity"/>.
    /// Isso permite que a classe base execute operações de persistência de forma estaticamente tipada
    /// através de um DAO (Data Access Object) associado.
    /// 
    /// O ciclo de vida de uma entidade é gerenciado pela propriedade <see cref="State"/>.
    /// </remarks>
    /// <typeparam name="TEntity">O tipo concreto da entidade que está herdando desta classe (ex: 'public class User : TableEntity&lt;User&gt;').</typeparam>
    public abstract partial class TableEntity<TEntity> : ITableEntity where TEntity : TableEntity<TEntity>, new()
    {
        private long? _id;
        private ITableEntity? _originalInstance;

        /// <summary>
        /// Representa o estado da entidade durante seu ciclo de vida.
        /// </summary>
        public EntityState State { get; private set; } = EntityState.Initialized;

        /// <inheritdoc/>
        [EntityColumnName("ID")]
        public long? Id { get; set; }

        /// <inheritdoc/>
        public ITableEntity GetOriginalInstance()
        {
            if (_originalInstance == null)
                throw new OriginalEntityNotSetException();

            return _originalInstance;
        }

        /// <inheritdoc/>
        public void AcceptChanges()
        {
            SetOriginalInstance((TEntity)MemberwiseClone());
        }

        /// <summary>
        /// Define a instância "snapshot" que representa o estado original da entidade.
        /// </summary>
        /// <remarks>
        /// Este método é usado internamente pelo ORM após carregar uma entidade do banco de dados
        /// para armazenar seu estado inicial.
        /// </remarks>
        /// <param name="entity">A instância que servirá como o estado original.</param>
        public void SetOriginalInstance(ITableEntity entity)
        {
            _originalInstance = entity;
        }
    }
}
