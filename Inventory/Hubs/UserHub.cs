using Inventory.IRepositories;
using Inventory.UnitOfWork;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Inventory.Hubs
{
    public class UserHub:Hub
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IUnitOfWork _unitOfWork; 

        public UserHub(IConnectionManager connectionManager, IUnitOfWork unitOfWork)
        {
            _connectionManager = connectionManager;
            _unitOfWork = unitOfWork;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("UserID")?.Value;

            if (userId != null)
            {
                bool isFirstConnection = !_connectionManager.UserHasConnections(userId);
                // Add connection
                _connectionManager.AddConnection(userId, Context.ConnectionId);

                // If this is the user's first connection, mark them as active
                if (isFirstConnection)
                {
                    var user = await _unitOfWork.Credential.SingleOrDefaultAsync(u => u.Id ==Convert.ToInt32( userId));
                    if (user != null)
                    {
                        user.IsActive = true;
                         _unitOfWork.Credential.Update(user);
                        bool userUpdateStatus = await _unitOfWork.SaveAsync();
                        await Clients.All.SendAsync("ReceiveStatusUpdate", user.Id, user.IsActive);
                    }
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.FindFirst("UserID")?.Value;

            if (userId != null)
            {
                // Remove connection
                _connectionManager.RemoveConnection(userId, Context.ConnectionId);

                // If no more connections, mark the user as inactive
                if (!_connectionManager.UserHasConnections(userId))
                {
                    var user = await _unitOfWork.Credential.SingleOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));
                    if (user != null)
                    {
                        user.IsActive = false;
                        _unitOfWork.Credential.Update(user);
                        bool userUpdateStatus = await _unitOfWork.SaveAsync();
                        await Clients.All.SendAsync("ReceiveStatusUpdate", user.Id, user.IsActive);
                    }                   
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
        public async Task NotifyStatusChange(string userId, bool isActive)
        {
            // Notify all clients that a user's status has changed
            await Clients.All.SendAsync("ReceiveStatusUpdate", userId, isActive);
        } 
    }
}
