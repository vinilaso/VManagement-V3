namespace VManagement.Database.Generalization
{
    /// <summary>
    /// Define um contrato para uma conexão com o banco de dados dentro do framework VManagement.
    /// </summary>
    /// <remarks>
    /// Esta abstração permite desacoplar a lógica de negócio das implementações concretas de conexão
    /// (como <c>SqlConnection</c>), facilitando a manutenção e os testes unitários através de mocks.
    /// A interface herda de <see cref="IDisposable"/> para garantir que as conexões sejam liberadas corretamente.
    /// </remarks>
    public interface IVManagementConnection : IDisposable
    {
        /// <summary>
        /// Cria e retorna um novo objeto de comando associado a esta conexão.
        /// </summary>
        /// <returns>Uma nova instância que implementa <see cref="IVManagementCommand"/>.</returns>
        IVManagementCommand CreateCommand();
    }
}
