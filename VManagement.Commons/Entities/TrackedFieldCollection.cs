using VManagement.Commons.Entities.Interfaces;
using VManagement.Commons.Exceptions;

namespace VManagement.Commons.Entities
{
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