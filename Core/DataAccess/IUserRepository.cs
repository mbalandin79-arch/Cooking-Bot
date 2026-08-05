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
        // Возвращает всю информацию о пользователе по userId
        Task<ToDoUser?> GetUserByUserIdAsync(Guid userId);

        // Возвращает всю информацию о пользователе по telegramUserId
        Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId);

        // Добавляет пользователя
        Task AddAsync(ToDoUser user);

        // Удаляет пользователя по userId
        Task DeleteAsync(Guid userId);

        // Изменяет существующего пользователя
        Task UpdateAsync(ToDoUser user);
    }
}
