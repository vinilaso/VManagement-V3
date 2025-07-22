namespace VManagement.Commons.Entities.Interfaces
{
    public interface ITableEntity
    {
        long? Id { get; set; }
        ITrackedFieldCollection TrackedFields { get; set; }
    }
}
