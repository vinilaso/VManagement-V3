namespace VManagement.Commons.Entities.Interfaces
{
    public interface ITableEntity
    {
        /// <summary>
        /// Obtém ou define a chave primária da entidade.
        /// </summary>
        long? Id { get; set; }

        /// <summary>
        /// Obtém ou define a coleção de campos rastreados para monitorar alterações nos valores das propriedades.
        /// </summary>
        ITrackedFieldCollection TrackedFields { get; set; }
    }
}