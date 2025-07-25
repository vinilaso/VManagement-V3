using System;
using VManagement.Core.Entities;

namespace VManagement.Core.Exceptions
{
    /// <summary>
    /// Representa o erro que ocorre ao tentar executar uma ação em uma entidade que não está em um estado válido para tal ação.
    /// </summary>
    /// <remarks>
    /// Esta exceção é uma parte fundamental do ciclo de vida da entidade, garantindo que operações como 'Save' ou 'Delete'
    /// só possam ser executadas quando a entidade está em um estado apropriado (por exemplo, não se pode deletar uma entidade
    /// que já está no estado 'Deleted').
    /// </remarks>
    public class InvalidEntityActionException : Exception
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="InvalidEntityActionException"/> com uma mensagem de erro genérica.
        /// </summary>
        /// <param name="state">O estado em que a entidade se encontrava quando a ação inválida foi tentada.</param>
        public InvalidEntityActionException(EntityState state) : base($"Não é possível executar esta ação com a entidade no estado {state}.")
        {

        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="InvalidEntityActionException"/> com uma mensagem de erro detalhada.
        /// </summary>
        /// <param name="state">O estado em que a entidade se encontrava quando a ação inválida foi tentada.</param>
        /// <param name="action">O nome da ação que foi tentada (ex: "Save", "Delete").</param>
        public InvalidEntityActionException(EntityState state, string action) : base($"Não é possível executar {action} com a entidade no estado {state}.")
        {

        }
    }
}
