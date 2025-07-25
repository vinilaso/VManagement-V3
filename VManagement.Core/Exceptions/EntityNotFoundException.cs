using VManagement.Database.Clauses;

namespace VManagement.Core.Exceptions
{
    /// <summary>
    /// Representa o erro que ocorre quando uma operação de busca não encontra nenhuma entidade que corresponda aos critérios especificados.
    /// </summary>
    /// <remarks>
    /// Esta exceção é tipicamente lançada por métodos como 'Find', que esperam encontrar exatamente um registro,
    /// para sinalizar que a busca não retornou resultados.
    /// </remarks>
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="EntityNotFoundException"/> com uma mensagem de erro genérica.
        /// </summary>
        public EntityNotFoundException() : base("Não foi encontrada entidade com os parâmetros informados.") 
        { 
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="EntityNotFoundException"/> com uma mensagem de erro detalhada,
        /// incluindo a restrição de busca utilizada.
        /// </summary>
        /// <param name="restriction">A restrição que foi utilizada na busca e não retornou resultados.</param>
        public EntityNotFoundException(Restriction restriction) : base($"Não foi encontrada entidade com a restrição informada. {restriction}")
        {
        }
    }
}
