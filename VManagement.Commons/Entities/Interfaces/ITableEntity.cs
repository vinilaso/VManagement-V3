namespace VManagement.Commons.Entities.Interfaces
{
    /// <summary>
    /// Define o contrato fundamental para uma entidade que pode ser mapeada para uma tabela de banco de dados.
    /// </summary>
    /// <remarks>
    /// Toda classe que representa uma tabela no ORM deve implementar esta interface.
    /// Ela garante que a entidade possua uma chave primária (<see cref="Id"/>) e um mecanismo
    /// para rastrear alterações em suas propriedades (<see cref="TrackedFields"/>).
    /// </remarks>
    public interface ITableEntity
    {
        /// <summary>
        /// Obtém ou define a chave primária da entidade.
        /// </summary>
        long? Id { get; set; }

        ITableEntity GetOriginalInstance();
        void AcceptChanges();
    }
}