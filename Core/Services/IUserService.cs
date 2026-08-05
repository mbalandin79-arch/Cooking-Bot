using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookingBot.Core.Entities;

namespace CookingBot.Core.Services
{
    public interface IUserService
    {
        // Добавляет нового пользователя и возвращает информацию о пользователе
        Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUserName);

        // Возвращает информацию о пользователе по telegramUserId
        Task<ToDoUser?> GetUserAsync(long telegramUserId);

        // Возвращает информацию о пользователе по userId
        Task<ToDoUser?> GetUserByUserIdAsync(Guid userId);

        // Удаляет пользователя по по userId
        Task DeleteUserByUserIdAsync(Guid userId);

        // Удаляет пользователя по telegramUserId
        Task DeleteUserByTelegramUserIdAsync(long telegramUserId);

        // Изменяет статус пользователя с Guest на Member
        Task ChangeStateUserFromGuestToMember(Guid userId);

        // Изменяет статус пользователя с Member на Guest
        Task ChangeStateUserFromMemberToGuest(Guid userId);

        // Изменяет статус пользователя с Member на Advanced
        Task ChangeStateUserFromMemberToAdvanced(Guid userId);

        // Изменяет статус пользователя с Advanced на Member
        Task ChangeStateUserFromAdvancedToMember(Guid userId);

        // Изменяет статус пользователя с Advanced на Moderator
        Task ChangeStateUserFromAdvancedToModerator(Guid userId);

        // Изменяет статус пользователя с Moderator на Advanced
        Task ChangeStateUserFromModeratorToAdvanced(Guid userId);

        // Изменяет статус пользователя с Moderator на Admin
        Task ChangeStateUserFromModeratorToAdmin(Guid userId);

        // Изменяет статус пользователя с Admin на Moderator
        Task ChangeStateUserFromAdminToModerator(Guid userId);

        // Изменяет имя пользователя
        Task ChangeNameUser(Guid userId, string newName);
    }
}
