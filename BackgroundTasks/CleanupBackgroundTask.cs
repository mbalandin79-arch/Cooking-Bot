using CookingBot.Core.Services;
using CookingBot.Helpers;
using CookingBot.TelegramBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookingBot.BackgroundTasks
{
    /// <summary>
    /// Фоновая задача: раз в сутки чистит реестр активности от пользователей,
    /// отсутствующих в базе данных (например, удалённые аккаунты), и записывает
    /// результат в файл лога. Пользователям ничего не отправляется —
    /// это техническая информация.
    /// </summary>
    public class CleanupBackgroundTask : BackgroundTask
    {
        private readonly UserActivityRegistry _activityRegistry;
        private readonly IUserService _userService;

        /// <summary>Создаёт задачу авточистки реестра активности.</summary>
        /// <param name="activityRegistry">Реестр активности пользователей.</param>
        /// <param name="userService">Сервис пользователей (проверка наличия в базе).</param>
        public CleanupBackgroundTask(UserActivityRegistry activityRegistry, IUserService userService)
            : base(TimeSpan.FromDays(1), nameof(CleanupBackgroundTask))
        {
            _activityRegistry = activityRegistry;
            _userService = userService;
        }

        /// <inheritdoc/>
        protected override async Task Execute(CancellationToken ct)
        {
            int removed = 0;

            // Пользователи, у которых есть записи в реестре, но которых
            // уже нет в базе (аккаунт удалён) — их записи становятся мусором.
            foreach (var userId in _activityRegistry.GetUserIds())
            {
                var user = await _userService.GetUserAsync(userId, ct);
                if (user == null)
                {
                    removed += _activityRegistry.RemoveUser(userId);
                }
            }

            FileLogger.LogInformation($"CleanupBackgroundTask: очищено записей удалённых пользователей: {removed}");
        }
    }
}
