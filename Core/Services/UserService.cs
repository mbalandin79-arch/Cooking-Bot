using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookingBot.Core.DataAccess;
using CookingBot.Core.Entities;

namespace CookingBot.Core.Services
{
    internal class UserService : IUserService
    {
        private ConcurrentDictionary<long, ToDoUser> _dictUsers = new ConcurrentDictionary<long, ToDoUser>();
        private readonly IUserRepository _userRepository;

        public UserService() { }

        public UserService(IUserRepository userRepository)
        {
            _userRepository = (IUserRepository?)userRepository;
        }

        public ToDoUser? GetUser(long telegramUserId) // поиск Пользователя в БД и возврат всей записи пользователя либо NULL
        {
            return _dictUsers.FirstOrDefault(curr => curr.Key == telegramUserId).Value;
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            _dictUsers[telegramUserId] = new ToDoUser(telegramUserId, telegramUserName);
            var answ = _dictUsers.Values.Where(w => w.TelegramUserId == telegramUserId).First();
            return answ;
        }
    }
}
