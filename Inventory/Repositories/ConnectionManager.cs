using Inventory.IRepositories;

namespace Inventory.Repositories
{
    public class ConnectionManager: IConnectionManager
    {
        private static readonly Dictionary<string, HashSet<string>> _userConnections = new();

        public void AddConnection(string userId, string connectionId)
        {
            lock (_userConnections)
            {
                if (!_userConnections.ContainsKey(userId))
                {
                    _userConnections[userId] = new HashSet<string>();
                }
                _userConnections[userId].Add(connectionId);
            }
        }

        public void RemoveConnection(string userId, string connectionId)
        {
            lock (_userConnections)
            {
                if (_userConnections.ContainsKey(userId))
                {
                    _userConnections[userId].Remove(connectionId);
                    if (_userConnections[userId].Count == 0)
                    {
                        _userConnections.Remove(userId);
                    }
                }
            }
        }

        public bool UserHasConnections(string userId)
        {
            lock (_userConnections)
            {
                return _userConnections.ContainsKey(userId) && _userConnections[userId].Count > 0;
            }
        }
    }
}
