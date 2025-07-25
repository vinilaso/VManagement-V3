using VManagement.Commons.Entities;
using VManagement.Core.Exceptions;
using VManagement.Database.Clauses;
using VManagement.Database.Generalization;

namespace VManagement.Core.Entities
{
    public abstract partial class TableEntity<TEntity>
    {
        private static ITableEntityDAO<TEntity> _dao;

        public static void ConfigureDAO(ITableEntityDAO<TEntity> dao)
        {
            _dao = dao;
        }

        /// <summary>
        /// Cria uma nova instância de <typeparamref name="TEntity"/>, que representa uma nova tupla no banco de dados.
        /// Ela é criada com o estado <see cref="EntityState.New"/>.
        /// </summary>
        /// <returns>A nova instância de <typeparamref name="TEntity"/>.</returns>
        public static TEntity New()
        {
            TEntity entity = TableEntityFactory.CreateInstanceFor<TEntity>();
            entity.OnAfterCreated();
            return entity;
        }

        /// <summary>
        /// Procura o primeiro registro do banco de dados que atende à restrição informada. Caso não encontre, lança uma exceção.
        /// </summary>
        /// <param name="restriction">A restrição utilizada para a busca.</param>
        /// <returns>A primeira tupla do banco de dados que atende à restrição.</returns>
        /// <exception cref="EntityNotFoundException"></exception>
        public static TEntity Find(Restriction restriction)
        {
            if (_dao.Select(restriction) is not TEntity entity)
                throw new EntityNotFoundException(restriction);

            entity.OnAfterGet();

            return entity;
        }

        /// <summary>
        /// Procura o primeiro registro do banco de dados que atende à restrição informada. Caso não encontre, retorna <see langword="null"/>.
        /// </summary>
        /// <param name="restriction">A restrição utilizada para a busca.</param>
        /// <returns>A primeira tupla do banco de dados que atende à restrição. Ou <see langword="null"/>, caso não encontre.</returns>
        public static TEntity? FindFirstOrDefault(Restriction restriction)
        {
            TEntity? entity = _dao.Select(restriction);

            entity?.OnAfterGet();

            return entity;
        }

        /// <summary>
        /// Procura todos os registros do banco de dados que atendem à restrição informada.
        /// </summary>
        /// <param name="restriction">A restrição utilizada para a busca.</param>
        /// <returns>Uma instância de <see cref="TableEntityCollection{TEntity}"/> contendo todas as tuplas que atendem à restrição.</returns>
        public static TableEntityCollection<TEntity> FindMany(Restriction restriction)
        {
            TableEntityCollection<TEntity> results = _dao.SelectMany(restriction);

            results.ForEach(entity => entity.OnAfterGet());

            return results;
        }

        /// <summary>
        /// Procura todos os registros da tabela, sem aplicar restrição.
        /// </summary>
        /// <remarks>
        /// Deve ser utilizado apenas por tabelas que contém poucos registros, pois carrega todo o conteúdo dela em memória.
        /// </remarks>
        /// <returns>Uma instância de <see cref="TableEntityCollection{TEntity}"/> contendo todas as tuplas da tabela.</returns>
        public static TableEntityCollection<TEntity> FindAll()
        {
            return FindMany(Restriction.Empty);
        }

        public static IEnumerable<TEntity> FetchMany(Restriction restriction)
        {
            foreach (TEntity entity in _dao.FetchMany(restriction))
                yield return entity;
        }
    }
}
