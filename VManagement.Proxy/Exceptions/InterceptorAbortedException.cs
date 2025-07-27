namespace VManagement.Proxy.Exceptions
{
    /// <summary>
    /// Representa o erro que ocorre quando a execução de um interceptor é abortada devido a um estado ou argumento inválido.
    /// </summary>
    /// <remarks>
    /// Esta exceção é utilizada como uma salvaguarda dentro dos interceptores para garantir que eles
    /// falhem de forma rápida e clara ('fail-fast') se uma precondição necessária não for atendida,
    /// prevenindo erros mais complexos e difíceis de depurar.
    /// </remarks>
    public class InterceptorAbortedException : Exception
    {
        /// <summary>
        /// Verifica se um argumento é nulo e lança uma <see cref="InterceptorAbortedException"/> se for.
        /// Serve como um "guarda de nulidade" (null guard clause) para os interceptores.
        /// </summary>
        /// <param name="argument">O objeto a ser verificado.</param>
        /// <param name="paramName">O nome do parâmetro ou variável que está sendo verificado, para ser usado na mensagem de erro.</param>
        /// <exception cref="InterceptorAbortedException">Lançada se o <paramref name="argument"/> for nulo.</exception>
        public static void ThrowIfNull(object? argument, string paramName)
        {
            if (argument is not null)
                return;

            throw new InterceptorAbortedException($"O interceptador foi abortado, pois {paramName} é nulo.");
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="InterceptorAbortedException"/>.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        public InterceptorAbortedException(string message) : base(message)
        {
        }
    }
}
