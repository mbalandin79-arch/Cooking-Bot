using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CookingBot.Core.Entities;

namespace CookingBot.Core.DataAccess
{
    internal interface IUserRepository
    {
        ToDoUser? GetUser(Guid userId);

        ToDoUser? GetUserByTelegramUserId(long telegramUserId);

        void Add(ToDoUser user);
    }
}
