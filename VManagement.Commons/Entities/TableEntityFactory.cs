using System.Reflection;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Entities.Interfaces;

namespace VManagement.Commons.Entities
{
    public class TableEntityFactory
    {
        public static TEntity CreateInstanceFor<TEntity>() where TEntity : ITableEntity, new()
        {
            TrackedFieldCollection trackedFields = [];

            foreach (string columnName in GetColumnNames<TEntity>())
                trackedFields.Add(new TrackedField(columnName, null));

            return new TEntity
            {
                TrackedFields = trackedFields
            };
        }

        public static IEnumerable<PropertyInfo> GetColumnProperties<TEntity>()
        {
            return typeof(TEntity)
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(EntityColumnNameAttribute)));
        }

        public static IEnumerable<string> GetColumnNames<TEntity>()
        {
            return GetColumnProperties<TEntity>()
                .Select(prop => prop.GetCustomAttribute<EntityColumnNameAttribute>())
                .Where(attr => attr != null)
                .Select(attr => attr!.ColumnName);
        }
    }
}
