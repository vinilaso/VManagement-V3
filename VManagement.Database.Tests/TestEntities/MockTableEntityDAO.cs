using System.Linq.Expressions;
using VManagement.Commons.Entities;
using VManagement.Commons.Entities.Interfaces;
using VManagement.Database.Clauses;
using VManagement.Database.Generalization;

namespace VManagement.Database.Tests.TestEntities
{
    internal class MockTableEntityDAO<TEntity> : ITableEntityDAO<TEntity> where TEntity : class, ITableEntity, new()
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

        public TSelector? Select<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TSelector> SelectMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TSelector> FetchMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Restriction restriction)
        {
            throw new NotImplementedException();
        }

        public TEntity? Select(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TSelector? Select<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TSelector> SelectMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> FetchMany(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TSelector> FetchMany<TSelector>(Expression<Func<TEntity, TSelector>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
