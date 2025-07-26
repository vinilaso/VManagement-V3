namespace VManagement.Commons.Entities.Interfaces
{
    /// <summary>
    /// Define o contrato fundamental para uma entidade que pode ser mapeada para uma tabela de banco de dados.
    /// </summary>
    /// <remarks>
    /// Toda classe que representa uma tabela no ORM deve implementar esta interface.
    /// Ela garante que a entidade possua uma chave primária (<see cref="Id"/>) e um mecanismo
    /// para gerenciar o estado original da entidade para o rastreamento de alterações.
    /// </remarks>
    public interface ITableEntity
    {
        /// <summary>
        /// Obtém ou define a chave primária da entidade.
        /// </summary>
        long? Id { get; set; }

        /// <summary>
        /// Obtém uma cópia "snapshot" da entidade, representando seu estado original desde que foi carregada ou salva pela última vez.
        /// </summary>
        /// <remarks>
        /// Este método é usado internamente pelo ORM para comparar o estado atual da entidade com seu estado original,
        /// a fim de determinar quais campos foram modificados e precisam ser atualizados no banco de dados.
        /// </remarks>
        /// <returns>Uma nova instância de <see cref="ITableEntity"/> contendo os valores originais.</returns>
        ITableEntity GetOriginalInstance();

        /// <summary>
        /// Aceita as alterações atuais, atualizando o estado original da entidade para corresponder ao seu estado atual.
        /// </summary>
        /// <remarks>
        /// Após chamar este método, o ORM considerará a entidade como "limpa" (sem alterações),
        /// até que novas modificações sejam feitas.
        /// </remarks>
        void AcceptChanges();
    }
}