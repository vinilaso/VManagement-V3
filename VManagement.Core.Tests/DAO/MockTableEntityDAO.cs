using VManagement.Commons.Entities;
using VManagement.Commons.Entities.Interfaces;
using VManagement.Database.Clauses;
using VManagement.Database.Generalization;

namespace VManagement.Core.Tests.DAO
{
    internal class MockTableEntityDAO<TEntity> : ITableEntityDAO<TEntity> where TEntity : ITableEntity, new()
    {
        internal int InsertCallCount { get; set; } = 0;
        internal int UpdateCallCount { get; set; } = 0;
        internal TEntity? LastInsertedEntity { get; private set; }
        internal TEntity? LastUpdatedEntity { get; private set; }
        internal TEntity? LastDeletedEntity { get; private set; }
        internal TEntity? EntityToReturnOnSelect { get; set; }

        internal long InsertedId = 1204L;

        public void Delete(TEntity entity)
        {
            LastDeletedEntity = entity;
        }

        public IEnumerable<TEntity> FetchMany(Restriction restriction)
        {
            throw new NotImplementedException();
        }

        public long Insert(TEntity entity)
        {
            InsertCallCount++;
            LastInsertedEntity = entity;
            return InsertedId;
        }

        public TEntity? Select(Restriction restriction)
        {
            return EntityToReturnOnSelect;
        }

        public TableEntityCollection<TEntity> SelectMany(Restriction restriction)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            UpdateCallCount++;
            LastUpdatedEntity = entity;
        }

        public bool Exists(Restriction restriction)
        {
            throw new NotImplementedException();
        }
    }
}
