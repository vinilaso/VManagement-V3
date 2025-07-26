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
    public interface ITableEntityDAO<TEntity> where TEntity : ITableEntity, new()
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
        /// Busca todas as entidades que correspondem à restrição e as retorna em uma coleção carregada em memória.
        /// </summary>
        /// <param name="restriction">Os critérios (cláusula WHERE e ORDER BY) para a busca.</param>
        /// <returns>Uma instância de <see cref="TableEntityCollection{TEntity}"/> contendo todas as entidades encontradas.</returns>
        TableEntityCollection<TEntity> SelectMany(Restriction restriction);

        /// <summary>
        /// Busca entidades que correspondem à restrição usando execução adiada (deferred execution).
        /// Ideal para processar grandes volumes de dados com baixo consumo de memória.
        /// </summary>
        /// <param name="restriction">Os critérios (cláusula WHERE e ORDER BY) para a busca.</param>
        /// <returns>Um <see cref="IEnumerable{TEntity}"/> que produz entidades uma a uma conforme são lidas do banco.</returns>
        IEnumerable<TEntity> FetchMany(Restriction restriction);

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
