namespace VManagement.Commons.Entities.Interfaces
{
    public interface ITrackedFieldCollection : ICollection<ITrackedField>
    {
        /// <summary>
        /// Obtém um campo rastreado da coleção pelo seu nome.
        /// </summary>
        /// <param name="fieldName">O nome do campo a ser recuperado.</param>
        ITrackedField this[string fieldName] { get; }
    }
}