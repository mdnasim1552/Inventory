namespace Inventory.IRepositories
{
    public interface IConnectionManager
    {
        void AddConnection(string userId, string connectionId);
        void RemoveConnection(string userId, string connectionId);
        bool UserHasConnections(string userId);
    }
}
