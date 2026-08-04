using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookingBot.Core.Entities;

namespace CookingBot.Core.DataAccess
{
    public interface IUserRepository
    {
        // вернуть всю информацию о пользователе по userId
        Task<ToDoUser?> GetUserAsync(Guid userId);

        // вернуть всю информацию о пользователе по telegramUserId
        Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId);

        // добавление пользователя
        Task AddAsync(ToDoUser user);

        // Удаляет пользователя по userId
        Task DeleteAsync(Guid userId);

        // Возвращает пользователя по id
        Task<ToDoUser?> GetAsync(Guid userId);

        // Изменяет существующего пользователя
        Task UpdateAsync(ToDoUser user);
    }
}
