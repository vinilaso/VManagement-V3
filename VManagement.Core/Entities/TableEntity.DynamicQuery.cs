using System.Linq.Expressions;
using VManagement.Core.Exceptions;
using VManagement.Database.Clauses;

namespace VManagement.Core.Entities
{
    public abstract partial class TableEntity<TEntity>
    {
        /// <summary>
        /// Projeta e retorna o primeiro registro que corresponde à restrição. Lança uma exceção se nenhum registro for encontrado.
        /// </summary>
        /// <remarks>
        /// Este método executa uma consulta otimizada, buscando apenas as colunas necessárias para popular o tipo de resultado <typeparamref name="TSelector"/>.
        /// </remarks>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo) para o qual a entidade será projetada.</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="restriction">Os critérios de filtro (cláusula WHERE) para a busca.</param>
        /// <returns>Uma única instância de <typeparamref name="TSelector"/> populada com os dados.</returns>
        /// <exception cref="EntityNotFoundException">Lançada se nenhum registro que atenda à restrição for encontrado.</exception>
        public static TSelector Find<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction)
        {
            if (_dao.Select(selector, restriction) is not TSelector result)
                throw new EntityNotFoundException(restriction);

            return result;
        }

        /// <summary>
        /// Projeta e retorna o primeiro registro que corresponde ao predicado. Lança uma exceção se nenhum registro for encontrado.
        /// </summary>
        /// <remarks>
        /// Este método executa uma consulta otimizada, buscando apenas as colunas necessárias para popular o tipo de resultado <typeparamref name="TSelector"/>.
        /// </remarks>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo) para o qual a entidade será projetada.</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="predicate">A expressão que representa a condição de filtro (cláusula WHERE).</param>
        /// <returns>Uma única instância de <typeparamref name="TSelector"/> populada com os dados.</returns>
        /// <exception cref="EntityNotFoundException">Lançada se nenhum registro que atenda ao predicado for encontrado.</exception>
        public static TSelector Find<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            if (_dao.Select(selector, predicate) is not TSelector result)
                throw new EntityNotFoundException(predicate);

            return result;
        }

        /// <summary>
        /// Projeta e retorna o primeiro registro que corresponde à restrição. Retorna nulo se nenhum registro for encontrado.
        /// </summary>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo) para o qual a entidade será projetada.</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="restriction">Os critérios de filtro (cláusula WHERE) para a busca.</param>
        /// <returns>Uma única instância de <typeparamref name="TSelector"/>; ou <c>null</c> se nenhum registro for encontrado.</returns>
        public static TSelector? FindFirstOrDefault<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction) 
        {
            return _dao.Select(selector, restriction);
        }

        /// <summary>
        /// Projeta e retorna o primeiro registro que corresponde ao predicado. Retorna nulo se nenhum registro for encontrado.
        /// </summary>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo) para o qual a entidade será projetada.</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="predicate">A expressão que representa a condição de filtro (cláusula WHERE).</param>
        /// <returns>Uma única instância de <typeparamref name="TSelector"/>; ou <c>null</c> se nenhum registro for encontrado.</returns>
        public static TSelector? FindFirstOrDefault<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            return _dao.Select(selector, predicate);
        }

        /// <summary>
        /// Projeta todos os registros que correspondem à restrição em uma nova coleção carregada em memória.
        /// </summary>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo) para o qual a entidade será projetada.</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="restriction">Os critérios de filtro (cláusula WHERE) para a busca.</param>
        /// <returns>Uma coleção <see cref="IEnumerable{T}"/> contendo todos os objetos projetados.</returns>
        public static IEnumerable<TSelector> FindMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction)
        {
            return _dao.SelectMany(selector, restriction);
        }

        /// <summary>
        /// Projeta todos os registros que correspondem ao predicado em uma nova coleção carregada em memória.
        /// </summary>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo) para o qual a entidade será projetada.</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="predicate">A expressão que representa a condição de filtro (cláusula WHERE).</param>
        /// <returns>Uma coleção <see cref="IEnumerable{T}"/> contendo todos os objetos projetados.</returns>
        public static IEnumerable<TSelector> FindMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            return _dao.SelectMany(selector, predicate);
        }

        /// <summary>
        /// Projeta todos os registros da tabela em uma nova coleção carregada em memória.
        /// </summary>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo) para o qual a entidade será projetada.</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <returns>Uma coleção <see cref="IEnumerable{T}"/> contendo todos os objetos projetados.</returns>
        public static IEnumerable<TSelector> FindAll<TSelector>(Expression<Func<TEntity, TSelector>> selector) 
        {
            return _dao.SelectMany(selector, Restriction.Empty);
        }

        /// <summary>
        /// Projeta registros que correspondem à restrição usando execução adiada (deferred execution), retornando-os sob demanda.
        /// </summary>
        /// <remarks>
        /// Este é o método mais performático para ler dados projetados, ideal para grandes volumes de dados.
        /// A conexão com o banco de dados permanecerá aberta durante toda a iteração.
        /// </remarks>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo) para o qual a entidade será projetada.</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="restriction">Os critérios de filtro (cláusula WHERE) para a busca.</param>
        /// <returns>Um <see cref="IEnumerable{TSelector}"/> que produz os objetos projetados de forma adiada.</returns>
        public static IEnumerable<TSelector> FetchMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction) 
        {
            foreach (TSelector projection in _dao.FetchMany(selector, restriction))
                yield return projection;
        }

        /// <summary>
        /// Projeta registros que correspondem ao predicado usando execução adiada (deferred execution), retornando-os sob demanda.
        /// </summary>
        /// <remarks>
        /// Este é o método mais performático para ler dados projetados, ideal para grandes volumes de dados.
        /// A conexão com o banco de dados permanecerá aberta durante toda a iteração.
        /// </remarks>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo) para o qual a entidade será projetada.</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="predicate">A expressão que representa a condição de filtro (cláusula WHERE).</param>
        /// <returns>Um <see cref="IEnumerable{TSelector}"/> que produz os objetos projetados de forma adiada.</returns>
        public static IEnumerable<TSelector> FetchMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            foreach (TSelector projection in _dao.FetchMany(selector, predicate))
                yield return projection;
        }
    }
}