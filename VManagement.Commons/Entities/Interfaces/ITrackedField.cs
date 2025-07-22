namespace VManagement.Commons.Entities.Interfaces
{
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
        // Obtém um valor que indica se o campo foi alterado.
        /// </summary>
        bool Changed { get; }

        /// <summary>
        /// Altera o valor atual do campo.
        /// </summary>
        /// <param name="newValue">O novo valor a ser definido.</param>
        void ChangeValue(object? newValue);

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