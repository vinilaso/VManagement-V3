using VManagement.Core.Entities;

namespace VManagement.Proxy.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ForeignKeyAttribute<TForeignKey> : Attribute where TForeignKey : TableEntity<TForeignKey>, new()
    {
        public string ColumnName { get; set; } = string.Empty;

        public ForeignKeyAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
