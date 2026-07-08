using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookingBot
{
    internal class UserService : IUserService
    {
        private ConcurrentDictionary<long, ToDoUser> _dictUsers = new ConcurrentDictionary<long, ToDoUser>();

        public ToDoUser? GetUser(long telegramUserId) // поиск Пользователя в БД и возврат всей записи пользователя либо NULL
        {
            foreach (var curr in _dictUsers)
            {
                if (curr.Value.TelegramUserId == telegramUserId)
                    return curr.Value;
            }
            return null;
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            _dictUsers[telegramUserId] = new ToDoUser(telegramUserId, telegramUserName);
            var answ = _dictUsers.Values.Where(w => w.TelegramUserId == telegramUserId).First();
            return answ;
        }
    }
}
