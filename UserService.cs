using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookingBot
{
    internal class UserService : IUserService
    {
        private ObservableCollection<ToDoUser> oc_Users = new ObservableCollection<ToDoUser>();

        public ToDoUser? GetUser(long telegramUserId) // поиск Пользователя в БД и возврат всей записи пользователя либо NULL
        {
            foreach (var curr in oc_Users)
            {
                if (curr.TelegramUserId == telegramUserId)
                    return curr;
            }
            return null;
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            oc_Users.Add(new ToDoUser(telegramUserId, telegramUserName));
            var answ = oc_Users.Where(w => w.TelegramUserId == telegramUserId).First();
            return answ;
        }
    }
}
