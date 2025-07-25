namespace VManagement.Commons.Entities.Interfaces
{
    /// <summary>
    /// Define um contrato para um campo de entidade que rastreia seu estado e alterações.
    /// </summary>
    /// <remarks>
    /// Esta interface é a base do mecanismo de rastreamento de alterações do ORM. Cada propriedade de uma entidade
    /// mapeada para uma coluna do banco de dados é representada por um ITrackedField, permitindo que o sistema
    /// saiba exatamente quais valores foram modificados para gerar comandos de UPDATE eficientes.
    /// </remarks>
    public interface ITrackedField
    {
        /// <summary>
        /// Obtém o nome do campo.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Obtém o valor atual do campo.
        /// </summary>
        object? Value { get; }

        /// <summary>
        /// Obtém o valor original do campo.
        /// </summary>
        object? OriginalValue { get; }

        /// <summary>
        /// Obtém um valor que indica se o campo é nulo.
        /// </summary>
        bool IsNull { get; }

        /// <summary>
        /// Obtém um valor que indica se o campo foi alterado.
        /// </summary>
        bool Changed { get; }

        /// <summary>
        /// Altera o valor atual do campo, marcando-o como 'modificado' se o novo valor for diferente do original.
        /// </summary>
        /// <param name="newValue">O novo valor a ser atribuído ao campo.</param>
        void ChangeValue(object? newValue);

        /// <summary>
        /// Define um valor para o campo, atualizando tanto o valor atual quanto o original.
        /// Isso efetivamente "reseta" o estado de alteração do campo, marcando-o como não modificado.
        /// </summary>
        /// <param name="newValue">O novo valor a ser definido como atual e original.</param>
        void SetValue(object? newValue);

        /// <summary>
        /// Converte o valor do campo para <see cref="string"/>.
        /// </summary>
        /// <returns>O valor como string, ou uma string vazia se for nulo.</returns>
        string AsString()
        {
            return Convert.ToString(Value) ?? string.Empty;
        }

        /// <summary>
        /// Converte o valor do campo para <see cref="short"/> (Int16).
        /// </summary>
        /// <returns>O valor como short.</returns>
        short AsInt16()
        {
            return Convert.ToInt16(Value);
        }

        /// <summary>
        /// Converte o valor do campo para <see cref="int"/> (Int32).
        /// </summary>
        /// <returns>O valor como int.</returns>
        int AsInt32()
        {
            return Convert.ToInt32(Value);
        }

        /// <summary>
        /// Converte o valor do campo para <see cref="long"/> (Int64).
        /// </summary>
        /// <returns>O valor como long.</returns>
        long AsInt64()
        {
            return Convert.ToInt64(Value);
        }

        /// <summary>
        /// Converte o valor do campo para <see cref="DateTime"/>.
        /// </summary>
        /// <returns>O valor como DateTime.</returns>
        DateTime AsDateTime()
        {
            return Convert.ToDateTime(Value);
        }
    }
}