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
        Task<ToDoUser?> GetUser(Guid userId);

        Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId);

        Task Add(ToDoUser user);
    }
}
