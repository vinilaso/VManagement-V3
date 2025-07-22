using VManagement.Commons.Entities.Interfaces;

namespace VManagement.Commons.Entities
{
    public class TrackedField : ITrackedField
    {
        /// <summary>
        /// Obtém o nome do campo, que corresponde ao nome de uma coluna no banco de dados.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Obtém o valor atual do campo.
        /// </summary>
        public object? Value { get; private set; }

        /// <summary>
        /// Obtém o valor original do campo, como era quando foi inicialmente carregado.
        /// </summary>
        public object? OriginalValue { get; private set; }

        /// <summary>
        /// Obtém um valor que indica se o valor atual do campo é nulo.
        /// </summary>
        public bool IsNull => Value == null;

        /// <summary>
        /// Obtém um valor que indica se o valor atual foi alterado em relação ao valor original.
        /// </summary>
        public bool Changed
        {
            get
            {
                if (Value == null && OriginalValue == null)
                    return false;

                if (Value == null && OriginalValue != null)
                    return true;

                return !Value!.Equals(OriginalValue);
            }
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TrackedField"/>.
        /// </summary>
        /// <param name="name">O nome do campo.</param>
        /// <param name="initialValue">O valor inicial do campo.</param>
        public TrackedField(string name, object? initialValue)
        {
            Name = name;
            SetValue(initialValue);
        }

        /// <summary>
        /// Define o valor inicial do campo, configurando tanto o valor atual quanto o original.
        /// </summary>
        /// <param name="value">O valor inicial a ser definido.</param>
        public void SetValue(object? value)
        {
            Value = value;
            OriginalValue = value;
        }

        /// <summary>
        /// Altera o valor atual do campo, mantendo o valor original para comparação.
        /// </summary>
        /// <param name="newValue">O novo valor a ser atribuído.</param>
        public void ChangeValue(object? newValue)
        {
            Value = newValue;
        }
    }
}