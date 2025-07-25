namespace VManagement.Commons.Entities.Interfaces
{
    /// <summary>
    /// Define um contrato para uma coleção de campos rastreáveis (<see cref="ITrackedField"/>).
    /// </summary>
    /// <remarks>
    /// Esta interface estende a funcionalidade de um <see cref="ICollection{T}"/> padrão,
    /// adicionando um indexador que permite o acesso direto a um campo rastreado através do seu nome,
    /// simplificando a manipulação e a verificação de alterações nas entidades.
    /// </remarks>
    public interface ITrackedFieldCollection : ICollection<ITrackedField>
    {
        /// <summary>
        /// Obtém um campo rastreado da coleção pelo seu nome.
        /// </summary>
        /// <param name="fieldName">O nome do campo a ser recuperado.</param>
        ITrackedField this[string fieldName] { get; }
    }
}