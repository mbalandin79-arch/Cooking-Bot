using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookingBot.Core.Entities
{
    public class ToDoUser
    {
        public enum ToDoUserState
        {
            Guest,
            Member,
            Advanced,
            Moderator,
            Admin
        }
        public Guid UserId { get; }
        public string TelegramUserName { get; set; }
        public DateTime RegisteredAt { get; }
        public long TelegramUserId { get; }
        public ToDoUserState State { get; set; }

        public ToDoUser(long telegramUserId, string telegramUserName)
        {
            TelegramUserId = telegramUserId;
            TelegramUserName = telegramUserName;
            RegisteredAt = DateTime.Now;
            UserId = Guid.NewGuid();
            State = ToDoUserState.Guest;
        }
    }
}
