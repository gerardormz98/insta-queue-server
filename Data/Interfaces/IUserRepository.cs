using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveWaitlistServer.Model;

namespace LiveWaitlistServer.Data.Interfaces
{
    public interface IUserRepository
    {
        public string GetConnectionIdByUserId(string userId);
        public string GetUserIdByConnectionId(string connectionId);
        public void MapUserToConnection(string userId, string connectionId);
    }
}