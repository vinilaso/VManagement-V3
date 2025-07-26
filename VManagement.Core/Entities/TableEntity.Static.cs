using VManagement.Commons.Entities;
using VManagement.Core.Exceptions;
using VManagement.Database.Clauses;
using VManagement.Database.Generalization;

namespace VManagement.Core.Entities
{
    public abstract partial class TableEntity<TEntity>
    {
        private static ITableEntityDAO<TEntity> _dao;

        /// <summary>
        /// Configura uma instância de DAO para ser utilizada por esta entidade de negócio.
        /// </summary>
        /// <param name="dao">O DAO a ser configurado.</param>
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
        /// Verifica de forma otimizada se pelo menos um registro que atende à restrição existe no banco de dados.
        /// </summary>
        /// <remarks>
        /// Este método é mais performático do que 'FindFirstOrDefault' para cenários onde você só precisa saber
        /// se um registro existe, pois se traduz em uma consulta mais leve (`SELECT 1`).
        /// </remarks>
        /// <param name="restriction">A restrição utilizada para a busca.</param>
        /// <returns><c>true</c> se pelo menos um registro for encontrado; caso contrário, <c>false</c>.</returns>
        public static bool Exists(Restriction restriction)
        {
            return _dao.Exists(restriction);
        }

        /// <summary>
        /// Verifica de forma otimizada se um registro com o ID especificado existe no banco de dados.
        /// </summary>
        /// <param name="id">O ID da entidade a ser verificada.</param>
        /// <returns><c>true</c> se um registro com o ID especificado for encontrado; caso contrário, <c>false</c>.</returns>
        public static bool Exists(long? id)
        {
            return Exists(Restriction.FromId(id));
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
        /// Procura o primeiro registro do banco de dados que possui o ID informado. Caso não encontre, lança uma exceção.
        /// </summary>
        /// <param name="entityId">O ID utilizado para a busca.</param>
        /// <returns>A primeira tupla do banco de dados que possui o ID.</returns>
        /// <exception cref="EntityNotFoundException"></exception>
        public static TEntity Find(long entityId)
        {
            return Find(Restriction.FromId(entityId));
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
        /// Procura o primeiro registro do banco de dados que atende à restrição informada. Caso não encontre, retorna <see langword="null"/>.
        /// </summary>
        /// <param name="entityId">A restrição utilizada para a busca.</param>
        /// <returns>A primeira tupla do banco de dados que atende à restrição. Ou <see langword="null"/>, caso não encontre.</returns>
        public static TEntity? FindFirstOrDefault(long entityId)
        {
            return FindFirstOrDefault(Restriction.FromId(entityId));
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

        /// <summary>
        /// Busca entidades que correspondem à restrição usando execução adiada (deferred execution), retornando os registros sob demanda.
        /// </summary>
        /// <remarks>
        /// Este método é altamente eficiente em termos de memória para processar grandes volumes de dados, pois não carrega
        /// toda a coleção de resultados na memória de uma vez. Em vez disso, ele produz as entidades uma a uma
        /// conforme são requisitadas(por exemplo, dentro de um loop 'foreach').
        /// <para>
        /// IMPORTANTE: A conexão com o banco de dados permanecerá aberta durante toda a iteração.
        /// Portanto, a enumeração deve ser consumida o mais rápido possível para liberar os recursos.
        /// </para>
        /// </remarks>
        /// <param name="restriction">A restrição utilizada para a busca.</param>
        /// <returns>
        /// Um <see cref="IEnumerable{TEntity}"/> que produz as entidades de forma adiada. A consulta ao banco de dados
        /// é executada no início da iteração.
        /// </returns>
        public static IEnumerable<TEntity> FetchMany(Restriction restriction)
        {
            foreach (TEntity entity in _dao.FetchMany(restriction))
                yield return entity;
        }
    }
}
