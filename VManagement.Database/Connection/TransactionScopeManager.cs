using System.Collections.Immutable;

namespace VManagement.Database.Connection
{
    /// <summary>
    /// Gerencia o contexto da transação ambiente.
    /// </summary>
    internal static class TransactionScopeManager
    {
        // Uma pilha que pode conter transações aninhadas de forma segura.
        private static readonly AsyncLocal<ImmutableStack<VManagementTransaction>> _currentTransactionStack = new();

        /// <summary>
        /// Retorna a transação ativa no topo da pilha, ou null se não houver nenhuma.
        /// </summary>
        public static VManagementTransaction? Current => _currentTransactionStack.Value?.Peek();

        /// <summary>
        /// Coloca uma nova transação no topo da pilha.
        /// </summary>
        internal static void Push(VManagementTransaction transaction)
        {
            var stack = _currentTransactionStack.Value ?? ImmutableStack<VManagementTransaction>.Empty;
            _currentTransactionStack.Value = stack.Push(transaction);
        }

        /// <summary>
        /// Remove a transação do topo da pilha.
        /// </summary>
        internal static void Pop()
        {
            var stack = _currentTransactionStack.Value;
            if (stack != null)
            {
                _currentTransactionStack.Value = stack.Pop();
            }
        }
    }
}
