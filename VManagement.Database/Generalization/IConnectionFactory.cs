namespace VManagement.Database.Generalization
{
    /// <summary>
    /// Define um contrato para uma fábrica responsável por criar conexões com o banco de dados.
    /// </summary>
    /// <remarks>
    /// Esta abstração é um componente central do padrão de Injeção de Dependência no ORM.
    /// Ao depender desta interface em vez de uma implementação concreta, as classes de acesso a dados (DAOs)
    /// podem receber uma fábrica real em produção ou uma fábrica de mocks em testes,
    /// tornando a arquitetura flexível e testável.
    /// </remarks>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Cria e retorna uma nova instância de uma conexão com o banco de dados.
        /// </summary>
        /// <returns>Uma nova instância que implementa <see cref="IVManagementConnection"/>.</returns>
        IVManagementConnection CreateConnection();
    }
}
