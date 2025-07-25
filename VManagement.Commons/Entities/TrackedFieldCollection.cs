using VManagement.Commons.Entities.Interfaces;
using VManagement.Commons.Exceptions;

namespace VManagement.Commons.Entities
{
    /// <summary>
    /// Representa uma coleção de campos rastreáveis (<see cref="ITrackedField"/>) para uma entidade.
    /// </summary>
    /// <remarks>
    /// Esta classe herda de <see cref="List{T}"/> e adiciona funcionalidades específicas para o gerenciamento
    /// de campos rastreáveis, como um indexador para busca por nome e validações para garantir a integridade da coleção.
    /// </remarks>
    public class TrackedFieldCollection : List<ITrackedField>, ITrackedFieldCollection
    {
        /// <summary>
        /// Obtém um campo rastreado da coleção pelo nome da coluna.
        /// </summary>
        /// <param name="fieldName">O nome do campo (coluna) a ser encontrado.</param>
        /// <returns>A instância de <see cref="ITrackedField"/> correspondente.</returns>
        /// <exception cref="FieldNotFoundException">Lançada se nenhum campo com o nome especificado for encontrado.</exception>
        public ITrackedField this[string fieldName]
        {
            get
            {
                if (Find(tf => tf.Name == fieldName) is not ITrackedField result)
                    throw new FieldNotFoundException(fieldName);

                return result;
            }
        }

        /// <summary>
        /// Adiciona um novo campo rastreado à coleção, aplicando validações.
        /// </summary>
        /// <remarks>
        /// Este método oculta o método Add base de <see cref="List{T}"/> para garantir que apenas
        /// campos válidos (com nome não nulo e único) sejam adicionados à coleção.
        /// </remarks>
        /// <param name="trackedField">O campo rastreado a ser adicionado.</param>
        /// <exception cref="InvalidFieldException">Lançada se o nome do campo for nulo/vazio ou se já existir um campo com o mesmo nome na coleção.</exception>
        public new void Add(ITrackedField trackedField)
        {
            if (string.IsNullOrEmpty(trackedField.Name))
                throw new InvalidFieldException(string.Empty, "O nome do campo não pode estar vazio.");

            if (this.Any(tf => tf.Name == trackedField.Name))
                throw new InvalidFieldException(trackedField.Name, "Já existe um campo com este nome na entidade.");

            base.Add(trackedField);
        }
    }
}