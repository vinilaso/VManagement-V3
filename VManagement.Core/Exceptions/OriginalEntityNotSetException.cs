namespace VManagement.Core.Exceptions
{
    /// <summary>
    /// Representa o erro que ocorre ao tentar acessar o estado original de uma entidade antes que ele tenha sido definido.
    /// </summary>
    /// <remarks>
    /// Esta exceção é uma salvaguarda para o mecanismo de rastreamento de alterações por snapshot.
    /// Ela é lançada para prevenir bugs que poderiam ocorrer se uma operação de comparação de alterações
    /// fosse executada sem que uma cópia "snapshot" do estado original da entidade estivesse disponível.
    /// </remarks>
    public class OriginalEntityNotSetException() : Exception("A instância original da entidade ainda não foi definida.")
    {
    }
}
