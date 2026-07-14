using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookingBot
{
    internal class InMemoryUserRepository : IUserRepository
    {
        private List<ToDoUser> _usersInMemory = new List<ToDoUser>();

        public void Add(ToDoUser user)
        {
            _usersInMemory.Add(user);
        }

        public ToDoUser? GetUser(Guid userId)
        {
            foreach (var curr in _usersInMemory)
            {
                if (curr.UserId == userId)
                    return curr;
            }
            return null;
        }

        public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
        {
            foreach (var curr in _usersInMemory)
            {
                if (curr.TelegramUserId == telegramUserId)
                    return curr;
            }
            return null;
        }
    }
}
