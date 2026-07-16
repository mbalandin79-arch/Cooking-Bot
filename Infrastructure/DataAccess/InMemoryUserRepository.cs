using CookingBot.Core.DataAccess;
using CookingBot.Core.Entities;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookingBot.Infrastructure.DataAccess
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
            return _usersInMemory.FirstOrDefault(curr => curr.UserId == userId);
        }

        public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
        {            
            return _usersInMemory.FirstOrDefault(curr => curr.TelegramUserId == telegramUserId);
        }
    }
}
