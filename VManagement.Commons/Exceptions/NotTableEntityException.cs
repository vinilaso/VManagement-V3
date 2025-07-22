using VManagement.Commons.Entities.Attributes;

namespace VManagement.Commons.Exceptions
{
    /// <summary>
    /// Lançada quando uma classe que não possui o atributo <see cref="TableEntityAttribute"/>
    /// é utilizada para operações em que ele é necessário.
    /// </summary>
    public class NotTableEntityException : Exception
    {
        /// <summary>
        /// Inicia uma instância de <see cref="NotTableEntityException"/> com o nome da classe.
        /// </summary>
        public NotTableEntityException(string className) : base($"A classe {className} não possui o atributo {nameof(TableEntityAttribute)} em sua definição.")
        {
        }

    }
}
