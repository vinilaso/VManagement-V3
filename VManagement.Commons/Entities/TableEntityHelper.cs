using System.Reflection;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Entities.Interfaces;

namespace VManagement.Commons.Entities
{
    /// <summary>
    /// Fornece métodos auxiliares estáticos para extrair metadados de classes de entidade usando reflexão.
    /// </summary>
    /// <remarks>
    /// Esta classe encapsula a lógica de reflexão para ler os atributos de uma entidade,
    /// como o nome da tabela (<see cref="TableEntityAttribute"/>) e os nomes das colunas (<see cref="EntityColumnNameAttribute"/>),
    /// de forma centralizada e reutilizável.
    /// </remarks>
    /// <typeparam name="TTableEntity">O tipo da entidade da qual os metadados serão extraídos.</typeparam>
    public class TableEntityHelper<TTableEntity> where TTableEntity : ITableEntity
    {
        private static IEnumerable<PropertyInfo>? _columnProperties = null;
        private static string? _tableName = null;

        /// <summary>
        /// Resgata as colunas da tabela do banco de dados que <typeparamref name="TTableEntity"/> representa.
        /// </summary>
        /// <returns>Uma lista de <see cref="PropertyInfo"/> que possuem <see cref="EntityColumnNameAttribute"/> em sua definição.</returns>
        public static IEnumerable<PropertyInfo> GetColumnProperties()
        {
            _columnProperties ??= typeof(TTableEntity).GetProperties().Where(p => Attribute.IsDefined(p, typeof(EntityColumnNameAttribute)));
            return _columnProperties;
        }

        /// <summary>
        /// Resgata o nome da tabela do banco de dados que <typeparamref name="TTableEntity"/> representa.
        /// </summary>
        /// <returns>O nome da tabela.</returns>
        public static string GetTableName()
        {
            _tableName ??= typeof(TTableEntity).GetCustomAttribute<TableEntityAttribute>()?.TableName ?? string.Empty;
            return _tableName;
        }
    }
}
