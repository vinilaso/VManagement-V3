using Microsoft.Data.SqlClient;
using VManagement.Database.Clauses;

namespace VManagement.Database.Generalization
{
    /// <summary>
    /// Define um contrato para um comando de banco de dados, abstraindo as operações de execução e gerenciamento de parâmetros.
    /// </summary>
    /// <remarks>
    /// Esta interface é a chave para desacoplar a lógica de acesso a dados da implementação concreta do ADO.NET,
    /// permitindo a criação de mocks para testes unitários e aumentando a flexibilidade do ORM.
    /// </remarks>
    public interface IVManagementCommand
    {
        /// <summary>
        /// Obtém ou define a instrução SQL ou o nome do procedimento armazenado a ser executado.
        /// </summary>
        string CommandText { get; set; }

        /// <summary>
        /// Obtém a coleção de parâmetros <see cref="SqlParameter"/> utilizados pelo comando.
        /// </summary>
        SqlParameterCollection Parameters { get; }

        /// <summary>
        /// Adiciona um único parâmetro <see cref="SqlParameter"/> à coleção de parâmetros do comando.
        /// </summary>
        /// <param name="parameter">O parâmetro a ser adicionado.</param>
        void AddParameter(SqlParameter parameter);

        /// <summary>
        /// Adiciona múltiplos parâmetros de uma <see cref="ParameterCollection"/> à coleção de parâmetros do comando.
        /// </summary>
        /// <param name="parameters">A coleção de parâmetros a ser adicionada.</param>
        void AddParameters(ParameterCollection parameters);

        /// <summary>
        /// Executa uma instrução SQL em uma conexão e não retorna nenhum conjunto de resultados.
        /// Ideal para operações como INSERT, UPDATE ou DELETE.
        /// </summary>
        void ExecuteNonQuery();

        /// <summary>
        /// Executa o comando e retorna um <see cref="SqlDataReader"/> para ler os resultados.
        /// Ideal para operações SELECT que retornam múltiplas linhas e colunas.
        /// </summary>
        /// <returns>Um objeto <see cref="SqlDataReader"/> para ler os resultados da consulta.</returns>
        SqlDataReader ExecuteReader();

        /// <summary>
        /// Executa o comando e retorna a primeira coluna da primeira linha no conjunto de resultados.
        /// As colunas e linhas adicionais são ignoradas. O resultado é convertido para o tipo genérico especificado.
        /// </summary>
        /// <typeparam name="TGeneric">O tipo para o qual o resultado será convertido.</typeparam>
        /// <returns>O valor da primeira coluna da primeira linha do resultado, convertido para <typeparamref name="TGeneric"/>.</returns>
        TGeneric ExecuteScalar<TGeneric>();
    }
}
