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
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ChangeNameUser(Guid userId, string newName)
        {
            var user = await _userRepository.GetUserByUserIdAsync(userId);
            if (userId != default(Guid) && user != null)
            {
                user.TelegramUserName = newName;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task ChangeStateUserFromAdminToModerator(Guid userId)
        {
            var user = await _userRepository.GetUserByUserIdAsync(userId);
            if (userId != default(Guid) && user != null && user.State == ToDoUser.ToDoUserState.Admin)
            {
                user.State = ToDoUser.ToDoUserState.Moderator;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task ChangeStateUserFromAdvancedToMember(Guid userId)
        {
            var user = await _userRepository.GetUserByUserIdAsync(userId);
            if (userId != default(Guid) && user != null && user.State == ToDoUser.ToDoUserState.Advanced)
            {
                user.State = ToDoUser.ToDoUserState.Member;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task ChangeStateUserFromAdvancedToModerator(Guid userId)
        {
            var user = await _userRepository.GetUserByUserIdAsync(userId);
            if (userId != default(Guid) && user != null && user.State == ToDoUser.ToDoUserState.Advanced)
            {
                user.State = ToDoUser.ToDoUserState.Moderator;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task ChangeStateUserFromGuestToMember(Guid userId)
        {
            var user = await _userRepository.GetUserByUserIdAsync(userId);
            if (userId != default(Guid) && user != null && user.State == ToDoUser.ToDoUserState.Guest)
            {
                user.State = ToDoUser.ToDoUserState.Member;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task ChangeStateUserFromMemberToAdvanced(Guid userId)
        {
            var user = await _userRepository.GetUserByUserIdAsync(userId);
            if (userId != default(Guid) && user != null && user.State == ToDoUser.ToDoUserState.Member)
            {
                user.State = ToDoUser.ToDoUserState.Advanced;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task ChangeStateUserFromMemberToGuest(Guid userId)
        {
            var user = await _userRepository.GetUserByUserIdAsync(userId);
            if (userId != default(Guid) && user != null && user.State == ToDoUser.ToDoUserState.Member)
            {
                user.State = ToDoUser.ToDoUserState.Guest;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task ChangeStateUserFromModeratorToAdmin(Guid userId)
        {
            var user = await _userRepository.GetUserByUserIdAsync(userId);
            if (userId != default(Guid) && user != null && user.State == ToDoUser.ToDoUserState.Moderator)
            {
                user.State = ToDoUser.ToDoUserState.Admin;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task ChangeStateUserFromModeratorToAdvanced(Guid userId)
        {
            var user = await _userRepository.GetUserByUserIdAsync(userId);
            if (userId != default(Guid) && user != null && user.State == ToDoUser.ToDoUserState.Moderator)
            {
                user.State = ToDoUser.ToDoUserState.Advanced;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task DeleteUserByTelegramUserIdAsync(long telegramUserId)
        {
            var user = await _userRepository.GetUserByTelegramUserIdAsync(telegramUserId);
            if (telegramUserId > 0 && user != null)
            {
                await _userRepository.DeleteAsync(user.UserId);
            }
        }

        public async Task DeleteUserByUserIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByUserIdAsync(userId);
            if (userId != default(Guid) && user != null)
            {
                await _userRepository.DeleteAsync(user.UserId);
            }
        }

        public async Task<ToDoUser?> GetUserAsync(long telegramUserId) // поиск Пользователя в БД и возврат всей записи пользователя либо NULL
        {
            return await _userRepository.GetUserByTelegramUserIdAsync(telegramUserId);
        }

        public async Task<ToDoUser?> GetUserByUserIdAsync(Guid userId)
        {
            return await _userRepository.GetUserByUserIdAsync(userId);
        }

        public async Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUserName)
        {
            var existUser = await _userRepository.GetUserByTelegramUserIdAsync(telegramUserId);
            if (existUser == null)
            {
                var user = new ToDoUser(telegramUserId, telegramUserName);
                await _userRepository.AddAsync(user);
                return user;
            }
            return existUser;
        }
    }
}
