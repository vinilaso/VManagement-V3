using System.Linq.Expressions;
using VManagement.Commons.Entities;
using VManagement.Commons.Entities.Interfaces;
using VManagement.Database.Clauses;

namespace VManagement.Database.Generalization
{
    /// <summary>
    /// Define o contrato para um Data Access Object (DAO) genérico, responsável pelas operações de persistência
    /// para um tipo de entidade específico.
    /// </summary>
    /// <remarks>
    /// Esta interface abstrai as operações de CRUD (Create, Read, Update, Delete), permitindo que a camada de negócio
    /// interaja com o banco de dados de forma desacoplada e testável.
    /// </remarks>
    /// <typeparam name="TEntity">O tipo da entidade para a qual este DAO gerenciará a persistência.</typeparam>
    public interface ITableEntityDAO<TEntity> where TEntity : class, ITableEntity, new()
    {
        /// <summary>
        /// Insere uma nova entidade no banco de dados.
        /// </summary>
        /// <param name="entity">A instância da entidade a ser inserida.</param>
        /// <returns>O ID (chave primária) do novo registro inserido.</returns>
        long Insert(TEntity entity);

        /// <summary>
        /// Busca a primeira entidade no banco de dados que corresponde à restrição fornecida.
        /// </summary>
        /// <param name="restriction">Os critérios (cláusula WHERE e ORDER BY) para a busca.</param>
        /// <returns>Uma instância de <typeparamref name="TEntity"/> se um registro for encontrado; caso contrário, <c>null</c>.</returns>
        TEntity? Select(Restriction restriction);

        /// <summary>
        /// Busca a primeira entidade no banco de dados que corresponde ao predicado (filtro) fornecido.
        /// </summary>
        /// <param name="predicate">A expressão que representa a condição de filtro (cláusula WHERE).</param>
        /// <returns>Uma instância de <typeparamref name="TEntity"/> se um registro for encontrado; caso contrário, <c>null</c>.</returns>
        TEntity? Select(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Busca o primeiro registro que corresponde à restrição e o projeta.
        /// </summary>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo) para o qual a entidade será projetada.</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="restriction">Os critérios de filtro (cláusula WHERE) para a busca.</param>
        /// <returns>Uma única instância de <typeparamref name="TSelector"/>; ou <c>null</c> se nenhum registro for encontrado.</returns>
        TSelector? Select<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction);

        /// <summary>
        /// Busca o primeiro registro que corresponde ao predicado e o projeta em um novo tipo.
        /// </summary>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo).</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="predicate">A expressão que representa a condição de filtro (cláusula WHERE).</param>
        /// <returns>Uma única instância de <typeparamref name="TSelector"/>; ou <c>null</c> se nenhum registro for encontrado.</returns>
        TSelector? Select<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Busca todas as entidades que correspondem à restrição e as retorna em uma coleção carregada em memória.
        /// </summary>
        /// <param name="restriction">Os critérios (cláusula WHERE e ORDER BY) para a busca.</param>
        /// <returns>Uma instância de <see cref="TableEntityCollection{TEntity}"/> contendo todas as entidades encontradas.</returns>
        TableEntityCollection<TEntity> SelectMany(Restriction restriction);

        /// <summary>
        /// Projeta todos os registros que correspondem à restrição em uma nova coleção carregada em memória.
        /// </summary>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo).</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="restriction">Os critérios de filtro (cláusula WHERE) para a busca.</param>
        /// <returns>Uma coleção <see cref="IEnumerable{T}"/> contendo todos os objetos projetados.</returns>
        IEnumerable<TSelector> SelectMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction);

        /// <summary>
        /// Busca todos os registros que correspondem ao predicado, os projeta em um novo tipo e os retorna em uma coleção carregada em memória.
        /// </summary>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo).</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="predicate">A expressão que representa a condição de filtro (cláusula WHERE).</param>
        /// <returns>Uma coleção <see cref="IEnumerable{T}"/> contendo todos os objetos projetados.</returns>
        IEnumerable<TSelector> SelectMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Busca entidades que correspondem à restrição usando execução adiada (deferred execution).
        /// Ideal para processar grandes volumes de dados com baixo consumo de memória.
        /// </summary>
        /// <param name="restriction">Os critérios (cláusula WHERE e ORDER BY) para a busca.</param>
        /// <returns>Um <see cref="IEnumerable{TEntity}"/> que produz entidades uma a uma conforme são lidas do banco.</returns>
        IEnumerable<TEntity> FetchMany(Restriction restriction);

        /// <summary>
        /// Busca entidades que correspondem ao predicado usando execução adiada (deferred execution).
        /// Ideal para processar grandes volumes de dados com baixo consumo de memória.
        /// </summary>
        /// <param name="predicate">A expressão que representa a condição de filtro (cláusula WHERE).</param>
        /// <returns>Um <see cref="IEnumerable{TEntity}"/> que produz entidades uma a uma conforme são lidas do banco.</returns>
        IEnumerable<TEntity> FetchMany(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Projeta registros que correspondem à restrição usando execução adiada (deferred execution).
        /// </summary>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo).</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="restriction">Os critérios de filtro (cláusula WHERE) para a busca.</param>
        /// <returns>Um <see cref="IEnumerable{TSelector}"/> que produz os objetos projetados de forma adiada.</returns>
        IEnumerable<TSelector> FetchMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction);

        /// <summary>
        /// Projeta registros que correspondem ao predicado usando execução adiada (deferred execution).
        /// </summary>
        /// <typeparam name="TSelector">O tipo do objeto de resultado (DTO ou anônimo).</typeparam>
        /// <param name="selector">A expressão que define a projeção (quais colunas selecionar).</param>
        /// <param name="predicate">A expressão que representa a condição de filtro (cláusula WHERE).</param>
        /// <returns>Um <see cref="IEnumerable{TSelector}"/> que produz os objetos projetados de forma adiada.</returns>
        IEnumerable<TSelector> FetchMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Atualiza um registro existente no banco de dados com base nos dados da entidade fornecida.
        /// </summary>
        /// <param name="entity">A entidade contendo os dados a serem atualizados. O ID da entidade é usado para identificar o registro.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Exclui um registro do banco de dados com base no ID da entidade fornecida.
        /// </summary>
        /// <param name="entity">A entidade a ser excluída.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Verifica se existem tuplas no banco de dados que atendem à restrição informada.
        /// </summary>
        /// <param name="restriction">Os critérios (cláusula WHERE e ORDER BY) para a busca.</param>
        /// <returns><see langword="true"/>, caso ao menos um registro seja encontrado. Caso contrário, <see langword="false"/>.</returns>
        bool Exists(Restriction restriction);
    }
}
