namespace VManagement.Commons.Utility.Extensions
{
    /// <summary>
    /// Fornece métodos de extensão genéricos e de propósito geral para qualquer tipo <typeparamref name="T"/>.
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// Verifica se um item está contido em uma coleção de elementos.
        /// </summary>
        /// <remarks>
        /// Este método fornece uma sintaxe mais legível e fluente (semelhante ao operador "IN" do SQL)
        /// como uma alternativa ao método <c>Contains()</c> do LINQ.
        /// Exemplo de uso: <c>if (meuEstado.In(Estado.Novo, Estado.Carregado)) { ... }</c>
        /// </remarks>
        /// <typeparam name="T">O tipo do item e dos elementos na coleção.</typeparam>
        /// <param name="item">O item a ser verificado.</param>
        /// <param name="collection">A coleção de elementos na qual a verificação será feita.</param>
        /// <returns><c>true</c> se o item for encontrado na coleção; caso contrário, <c>false</c>.</returns>
        public static bool In<T>(this T item, params T[] collection)
        {
            if (collection.Length == 0)
                return false;

            return collection.Contains(item);
        }
    }
}
