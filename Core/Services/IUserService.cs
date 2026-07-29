using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookingBot.Core.Entities;

namespace CookingBot.Core.Services
{
    internal interface IUserService
    {
        Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName);

        Task<ToDoUser?> GetUser(long telegramUserId);
    }
}
